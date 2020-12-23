using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomdpPBVI2
{
    public class PlannerUtils
    {

        // ///////////////////////////
        /**
         * find B Index
         */
        // //////////////////////////
        public static int findBIndex(Dictionary<double[], int> polB, double[] b)
        {

            int ret = -1;

            for (int i = 0; i < polB.Keys.Count; i++)
            {
                double[] key = polB.Keys.ElementAt(i);
                bool eq = true;

                for (int x = 0; x < key.Length; x++)
                {
                    // if (round(b[x], Settings.dec) != round(key[x], Settings.dec)
                    if (Math.Abs(Utils.Round(b[x], Settings.decPol)
                            - Utils.Round(key[x], Settings.decPol)) > Settings.diffErrPol)
                    {
                        eq = false;
                        break;
                    }
                }

                if (eq)
                {
                    ret = i;
                    break;
                }

            }

            return ret;
        }

        public static int findNearestBIndex(Dictionary<double[], int> polB,
                double[] b)
        {

            int ret = -1;

            double[] bestDiff = null;

            for (int i = 0; i < polB.Keys.Count; i++)
            {
                double[] key = polB.Keys.ElementAt(i);

                double[] actualDiff = new double[b.Length];

                bool less = true;

                for (int x = 0; x < key.Length; x++)
                {

                    actualDiff[x] = Math.Abs(b[x] - key[x]);

                    if (bestDiff != null)
                    {
                        if (actualDiff[x] > bestDiff[x])
                        {
                            less = false;
                            break;
                        }
                    }
                }

                if (less)
                {
                    ret = i;
                    bestDiff = new Double[actualDiff.Length];
                    Array.Copy(actualDiff, bestDiff, actualDiff.Length);
                }

            }

            return ret;
        }

        // ///////////////////////////
        /**
         * Policy
         */
        // //////////////////////////
        public static void writePolFile(Model model,
                Dictionary<double[], int> polB)
        {
            String linePolOut = "";
            String lineBOut = "";

            for (int i = 0; i < polB.Keys.Count; i++)
            {
                double[] key = polB.Keys.ElementAt(i);

                String bbb = "";
                for (int x = 0; x < key.Length; x++)
                    bbb = bbb + " " + key[x].ToString();

                int bestAIndex = (int)polB[key];
                String lineB = i + "\t" + bbb + "\t" + bestAIndex;
                String linePol = i + " " + bestAIndex + " ";

                for (int oi = 0; oi < model.getO().Length; oi++)
                {
                    double[] newB = newBelief(model, key, bestAIndex, oi);
                    int bi = findBIndex(polB, newB);

                    if (bi == -1)
                        bi = findNearestBIndex(polB, newB);

                    linePol = linePol + " " + bi;
                }
                linePolOut = linePolOut + linePol + "\n";
                lineBOut = lineBOut + lineB + "\n";
            }
            Utils.writeFile(Settings.projectFolder + Settings.polFileName,
                    linePolOut);
            Utils.writeFile(Settings.projectFolder + Settings.beliefFileName,
                    lineBOut);
        }

        public static void writeAlphaFile(Dictionary<double[], int> polAlpha)
        {
            String lineAlphaOut = "";


            foreach (double[] key in polAlpha.Keys)
            {

                int bestAIndex = (int)polAlpha[key];
                lineAlphaOut = lineAlphaOut + bestAIndex + "\n";

                String bbb = "";
                for (int x = 0; x < key.Length; x++)
                    bbb = bbb + " " + key[x].ToString();

                lineAlphaOut = lineAlphaOut + bbb.Trim() + "\n\n";
            }
            Utils.writeFile(Settings.projectFolder + Settings.alphaFileName,
                    lineAlphaOut);
        }

        // ///////////////////////////
        /**
         * New Belief
         */
        // ///////////////////////////
        public static double ba_s(Model model, double[] b, String a, int sli)
        {

            double ret = 0;

            for (int si = 0; si < model.getS().Length; si++)
                ret = ret + (model.getTa(a)[si][sli] * b[si]);

            return ret;
        }

        public static double ba_o(Model model, double[] b, String a, int oi)
        {

            double ret = 0;

            for (int si = 0; si < model.getS().Length; si++)
                ret = ret + (model.getPa(a)[si][oi] * ba_s(model, b, a, si));

            return ret;
        }

        public static double[] newBelief(Model model, double[] b, int ai, int oi)
        {

            double[] ret = new double[b.Length];
            String a = model.getA()[ai];

            double _ba_o = ba_o(model, b, a, oi);

            for (int si = 0; si < model.getS().Length; si++)
                ret[si] = ((model.getPa(a)[si][oi] * ba_s(model, b, a, si)) / _ba_o);

            return ret;
        }

        // ///////////////////////////
        /**
         * Random Functions
         */
        // //////////////////////////
        public static int retValidRandomSLI(Model model, int si, int ai)
        {
            int sli = 0;
            double prob = 0;
            while (prob == 0)
            {

                String a = model.getA()[ai];
                sli = Randomize.Integer(model.getS().Length);
                prob = model.getTa(a)[si][sli];
            }

            return sli;
        }

        public static int retValidRandomOI(Model model, int si, int ai)
        {
            int oi = 0;
            double prob = 0;
            while (prob == 0)
            {
                String a = model.getA()[ai];
                oi = Randomize.Integer(model.getO().Length);
                prob = model.getPa(a)[si][oi];
            }

            return oi;
        }

        // ///////////////////////////
        /**
         * Reload Functions
         */
        // //////////////////////////
        public static void reloadAlpha(Dictionary<double[], int> polAlpha)
        {
            polAlpha.Clear();

            if (File.Exists(Settings.projectFolder + Settings.alphaFileName))
            {
                try
                {
                    using (StreamReader input = new StreamReader(Settings.projectFolder + Settings.alphaFileName))
                    {

                        String line = null;
                        int iLine = 0;
                        int a = -1;

                        while ((line = input.ReadLine()) != null)
                        {

                            if (iLine == 0)
                            {
                                a = int.Parse(line.Trim());
                                iLine = 1;
                            }
                            else if (iLine == 1)
                            {

                                if (a > -1)
                                {
                                    String[] aAlpha = line.Trim().Split(' ');
                                    double[] alpha = new double[aAlpha.Length];
                                    for (int i = 0; i < alpha.Length; i++)
                                    {
                                        alpha[i] = double.Parse(aAlpha[i]);
                                    }

                                    polAlpha.Add(alpha, a);
                                }

                                iLine = 2;
                            }
                            else if (iLine > 1)
                            {

                                a = -1;
                                iLine = 0;
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

        public static void reloadPol(Dictionary<double[], int> polB)
        {

            polB.Clear();


            if (File.Exists(Settings.projectFolder + Settings.beliefFileName))
            {
                try
                {
                    StreamReader input = new StreamReader(Settings.projectFolder + Settings.beliefFileName);

                    String line = null;

                    while ((line = input.ReadLine()) != null)
                    {

                        String[] aLine = line.Split('\t');
                        String[] aBelief = aLine[1].Trim().Split(' ');

                        double[] b = new double[aBelief.Length];
                        int a = int.Parse(aLine[2]);

                        for (int i = 0; i < b.Length; i++)
                        {
                            b[i] = double.Parse(aBelief[i]);
                        }

                        polB.Add(b, a);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
        }
    }

}
