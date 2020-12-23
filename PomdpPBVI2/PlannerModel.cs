using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomdpPBVI2
{
    public class PlannerModel
    {

        Model model = new Model();


        public Model getModel()
        {
            return model;
        }

        public bool checkModel()
        {
            return model.check();
        }

        public String getLastErrStatus()
        {
            return model.getLastErrStatus();
        }

        /**
         * Initial Belief
         */
        public double[] initB()
        {
            double[] binit = new double[model.getS().Length];

            if (model.getBInit() == null)
            {
                for (int x = 0; x < binit.Length; x++)
                    binit[x] = (double)1 / (double)binit.Length;

            }
            else
                binit = model.getBInit();

            return binit;
        }

        public List<double[]> initBList()
        {
            List<double[]> B_Actual = new List<double[]>();

            B_Actual.Add(initB());

            return B_Actual;
        }

        /**
         * Model
         */
        public void loadModel()
        {

            model.Clear();

            if (File.Exists(Settings.projectFolder + Settings.pomdpFileName))
            {
                try
                {
                    // use buffering, reading one line at a time
                    // FileReader always assumes default encoding is OK!
                    using (StreamReader input = new StreamReader(Settings.projectFolder + Settings.pomdpFileName))
                    {
                        String line = null; // not declared within while loop
                                            /*
                                             * readLine is a bit quirky : it returns the content of a
                                             * line MINUS the newline. it returns null only for the END
                                             * of the stream. it returns an empty String if two newlines
                                             * appear in a row.
                                             */
                        bool rew = false;
                        while ((line = input.ReadLine()) != null)
                        {

                            if (line.IndexOf("discount:") > -1)
                            {
                                String line2 = line.Substring("discount:".Length)
                                        .Trim();

                                Settings.gamma = double.Parse(line2);
                            }

                            if (line.IndexOf("states:") > -1)
                            {
                                String line2 = line.Substring("states:".Length)
                                        .Trim();
                                String[] aLine = line2.Split(' ');
                                model.setS(aLine);
                            }

                            if (line.IndexOf("actions:") > -1)
                            {
                                String line2 = line.Substring("actions:".Length)
                                        .Trim();
                                String[] aLine = line2.Split(' ');
                                model.setA(aLine);
                            }

                            if (line.IndexOf("observations:") > -1)
                            {
                                String line2 = line.Substring(
                                        "observations:".Length).Trim();
                                String[] aLine = line2.Split(' ');
                                model.setO(aLine);
                            }

                            if (line.IndexOf("start:") > -1)
                            {
                                String line2 = line.Substring("start:".Length)
                                        .Trim();
                                String[] aLine = line2.Split(' ');

                                double[] binit = new double[model.getS().Length];
                                for (int x = 0; x < binit.Length; x++)
                                    binit[x] = double.Parse(aLine[x]);

                                model.setBInit(binit);
                            }

                            if (line.IndexOf("T:") > -1)
                            {
                                loadModelT(input, line.Substring("T:".Length)
                                        .Trim());
                            }

                            if (line.IndexOf("O:") > -1)
                            {
                                loadModelO(input, line.Substring("O:".Length)
                                        .Trim());
                            }

                            if (line.IndexOf("R:") > -1)
                            {
                                String line2 = line.Substring("R:".Length).Trim();
                                String[] aLine = line2.Split(':');
                                String[] aLine2 = aLine[3].Trim().Split(' ');

                                String action = aLine[0].Trim();
                                // String s = aLine[1].Trim();
                                String sl = aLine[2].Trim();
                                // String o = aLine2[0].Trim();
                                double reward = double.Parse(aLine2[1]);

                                int sli = model.indexS(sl);

                                if (!rew)
                                {
                                    loadModelR();
                                    rew = true;
                                }

                                if (action.Equals("*"))
                                {

                                    for (int ai = 0; ai < model.getA().Length; ai++)
                                    {
                                        String a = model.getA()[ai];
                                        model.getR(a)[sli] = reward;
                                        // model.setR(a, sli, reward);
                                    }
                                }
                                else
                                {
                                    model.getR(action)[sli] = reward;
                                    // model.setR(a, sli, reward);
                                }

                            }

                        }
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                }

            }
        }

        public void loadModelTiger()
        {

            model.Clear();
            String[] S = { "tigerLeft", "tigerRight" };
            model.setS(S);

            String[] A = { "openLeft", "openRight", "listen" };
            model.setA(A);

            String[] O = { "tigerLeft", "tigerRight" };
            model.setO(O);

            // //////////////////////////////////////////////////////

            double[][] ta_openLeft = new double[S.Length][];
            for (int i = 0; i < S.Length; i++)
                ta_openLeft[i] = new double[S.Length];
            ta_openLeft[model.indexS("tigerLeft")][model.indexS("tigerLeft")] = 0.5;
            ta_openLeft[model.indexS("tigerLeft")][model.indexS("tigerRight")] = 0.5;
            ta_openLeft[model.indexS("tigerRight")][model.indexS("tigerLeft")] = 0.5;
            ta_openLeft[model.indexS("tigerRight")][model.indexS("tigerRight")] = 0.5;
            model.setTa("openLeft", ta_openLeft);

            double[][] ta_openRight = new double[S.Length][];
            for (int i = 0; i < S.Length; i++)
                ta_openRight[i] = new double[S.Length];
            ta_openRight[model.indexS("tigerLeft")][model.indexS("tigerLeft")] = 0.5;
            ta_openRight[model.indexS("tigerLeft")][model.indexS("tigerRight")] = 0.5;
            ta_openRight[model.indexS("tigerRight")][model.indexS("tigerLeft")] = 0.5;
            ta_openRight[model.indexS("tigerRight")][model.indexS("tigerRight")] = 0.5;
            model.setTa("openRight", ta_openRight);

            double[][] ta_listen = new double[S.Length][];
            for (int i = 0; i < S.Length; i++)
                ta_listen[i] = new double[S.Length];
            ta_listen[model.indexS("tigerLeft")][model.indexS("tigerLeft")] = 1;
            ta_listen[model.indexS("tigerLeft")][model.indexS("tigerRight")] = 0;
            ta_listen[model.indexS("tigerRight")][model.indexS("tigerLeft")] = 0;
            ta_listen[model.indexS("tigerRight")][model.indexS("tigerRight")] = 1;
            model.setTa("listen", ta_listen);

            // //////////////////////////////////////////////////////

            double[][] pa_openLeft = new double[S.Length][];
            for (int i = 0; i < S.Length; i++)
                pa_openLeft[i] = new double[O.Length];
            pa_openLeft[model.indexS("tigerLeft")][model.indexO("tigerLeft")] = 0.5;
            pa_openLeft[model.indexS("tigerLeft")][model.indexO("tigerRight")] = 0.5;
            pa_openLeft[model.indexS("tigerRight")][model.indexO("tigerLeft")] = 0.5;
            pa_openLeft[model.indexS("tigerRight")][model.indexO("tigerRight")] = 0.5;
            model.setPa("openLeft", pa_openLeft);

            double[][] pa_openRight = new double[S.Length][];
            for (int i = 0; i < S.Length; i++)
                pa_openRight[i] = new double[O.Length];
            pa_openRight[model.indexS("tigerLeft")][model.indexO("tigerLeft")] = 0.5;
            pa_openRight[model.indexS("tigerLeft")][model.indexO("tigerRight")] = 0.5;
            pa_openRight[model.indexS("tigerRight")][model.indexO("tigerLeft")] = 0.5;
            pa_openRight[model.indexS("tigerRight")][model.indexO("tigerRight")] = 0.5;
            model.setPa("openRight", pa_openRight);

            double[][] pa_listen = new double[S.Length][];
            for (int i = 0; i < S.Length; i++)
                pa_listen[i] = new double[O.Length];
            pa_listen[model.indexS("tigerLeft")][model.indexO("tigerLeft")] = 0.85;
            pa_listen[model.indexS("tigerLeft")][model.indexO("tigerRight")] = 0.15;
            pa_listen[model.indexS("tigerRight")][model.indexO("tigerLeft")] = 0.15;
            pa_listen[model.indexS("tigerRight")][model.indexO("tigerRight")] = 0.85;
            model.setPa("listen", pa_listen);

            // //////////////////////////////////////////////////////

            double[] r_openLeft = new double[S.Length];
            r_openLeft[model.indexS("tigerLeft")] = -100;
            r_openLeft[model.indexS("tigerRight")] = 10;
            model.setR("openLeft", r_openLeft);

            double[] r_openRight = new double[S.Length];
            r_openRight[model.indexS("tigerLeft")] = 10;
            r_openRight[model.indexS("tigerRight")] = -100;
            model.setR("openRight", r_openRight);

            double[] r_listen = new double[S.Length];
            r_listen[model.indexS("tigerLeft")] = -1;
            r_listen[model.indexS("tigerRight")] = -1;
            model.setR("listen", r_listen);

            // //////////////////////////////////////////////////////

            double[][] c_openLeft = new double[S.Length][];
            for (int i = 0; i < S.Length; i++)
                c_openLeft[i] = new double[S.Length];
            c_openLeft[model.indexS("tigerLeft")][model.indexS("tigerLeft")] = 0;
            c_openLeft[model.indexS("tigerLeft")][model.indexS("tigerRight")] = 0;
            c_openLeft[model.indexS("tigerRight")][model.indexS("tigerLeft")] = 0;
            c_openLeft[model.indexS("tigerRight")][model.indexS("tigerRight")] = 0;
            model.setC("openLeft", ta_openLeft);

            double[][] c_openRight = new double[S.Length][];
            for (int i = 0; i < S.Length; i++)
                c_openRight[i] = new double[S.Length];
            c_openRight[model.indexS("tigerLeft")][model.indexS("tigerLeft")] = 0;
            c_openRight[model.indexS("tigerLeft")][model.indexS("tigerRight")] = 0;
            c_openRight[model.indexS("tigerRight")][model.indexS("tigerLeft")] = 0;
            c_openRight[model.indexS("tigerRight")][model.indexS("tigerRight")] = 0;
            model.setC("openRight", ta_openRight);

            double[][] c_listen = new double[S.Length][];
            for (int i = 0; i < S.Length; i++)
                c_openRight[i] = new double[S.Length];
            c_listen[model.indexS("tigerLeft")][model.indexS("tigerLeft")] = 0;
            c_listen[model.indexS("tigerLeft")][model.indexS("tigerRight")] = 0;
            c_listen[model.indexS("tigerRight")][model.indexS("tigerLeft")] = 0;
            c_listen[model.indexS("tigerRight")][model.indexS("tigerRight")] = 0;
            model.setC("listen", c_listen);

            // ////////////////////////////////////////////////////

        }

        public void loadModelR()
        {

            for (int ai = 0; ai < model.getA().Length; ai++)
            {
                String a = model.getA()[ai];
                model.setR(a, new double[model.getS().Length]);
            }
        }

        public void loadModelT(StreamReader input, String action)

        {
            String line = null;

            double[][] ta_listen = new double[model.getS().Length][];
            int iLine = 0;
            while ((line = input.ReadLine()) != null)
            {

                if (line.Trim().Length < 1)
                {
                    break;
                }
                else
                {
                    ta_listen[iLine] = new double[model.getS().Length];
                    String[] aLine = line.Split('\t');

                    for (int y = 0; y < aLine.Length; y++)
                        ta_listen[iLine][y] = double.Parse(aLine[y]);

                    iLine++;
                }
            }

            if (action.Equals("*"))
            {
                for (int ai = 0; ai < model.getA().Length; ai++)
                {
                    String a = model.getA()[ai];
                    model.setTa(a, ta_listen);
                }
            }
            else
                model.setTa(action, ta_listen);

        }

        public void loadModelO(StreamReader input, String action)

        {
            String line = null;

            double[][] pa_listen = new double[model.getS().Length][];
            int iLine = 0;
            while ((line = input.ReadLine()) != null)
            {

                if (line.Trim().Length < 1)
                {
                    break;
                }
                else
                {
                    pa_listen[iLine] = new double[model.getO().Length];
                    String[] aLine = line.Split('\t');

                    for (int y = 0; y < aLine.Length; y++)
                        pa_listen[iLine][y] = double.Parse(aLine[y]);

                    iLine++;
                }
            }

            if (action.Equals("*"))
            {
                for (int ai = 0; ai < model.getA().Length; ai++)
                {
                    String a = model.getA()[ai];
                    model.setPa(a, pa_listen);
                }
            }
            else
                model.setPa(action, pa_listen);
        }
    }
}