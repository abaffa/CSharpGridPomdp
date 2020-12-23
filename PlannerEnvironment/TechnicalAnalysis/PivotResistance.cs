using Baffa.MathModels;
using Baffa.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraderWhatever.Business;

namespace PlannerEnvironment.TechnicalAnalysis
{

    public class PivotResistance : PluginAbstract
    {

        public PivotResistance()
        {
            initialSetup();
        }

        public PivotResistance(Series series)
        {
            initialSetup();
            this.series = series;
        }

        public void initialSetup()
        {
            isConfigurable = false;
            isSimilarToSeries = true;
            chartType = chartTypes.DOTTY;

            pluginName = "Pivots and Resistances";
            pluginDescription = "...";
        }

        public override List<String> GetResultDescription()
        {

            List<String> results = new List<String>();

            results.Add("Pivot");
            results.Add("Resistance");

            return results;
        }

        public override List<double[]> GetResultValues()
        {

            List<double[]> results = new List<double[]>();

            double[] r3 = new double[series.getLowerPrice().Length];
            double[] p3 = new double[series.getLowerPrice().Length];

            /*
            bool comprado = false;

            double acum = 0;

            double lcc = 0;

            double pacum = 0;
            */

            for (int i = 0; i < series.getLowerPrice().Length; i++)
            {

                double CC = series.getClosePrice()[i];
                double H = series.getHigherPrice()[i];
                double L = series.getLowerPrice()[i];
                double PP = (H + L + CC) / 3;

                if (i == 0)
                {
                    r3[i] = H + 2 * (PP - L);
                    p3[i] = L - 2 * (H - PP);
                }
                else
                {
                    if (CC > r3[i - 1] || CC < p3[i - 1])
                        r3[i] = H + 2 * (PP - L);
                    else
                        r3[i] = r3[i - 1];

                    if (CC > r3[i - 1] || CC < p3[i - 1])
                        p3[i] = L - 2 * (H - PP);
                    else
                        p3[i] = p3[i - 1];

                }
            }

            results.Add(p3);
            results.Add(r3);

            return results;
        }

        public override List<String> GetBuySellDescription()
        {
            List<String> results = new List<String>();

            results.Add("Pivot x Resistance");

            return results;
        }

        public override List<String[]> GetBuySellValues()
        {
            List<String[]> results = new List<String[]>();


            String[] ret = new String[series.getClosePrice().Length];

            double[] r3 = new double[series.getLowerPrice().Length];
            double[] s3 = new double[series.getLowerPrice().Length];

            bool comprado = false;
            double acum = 0;
            double lcc = 0;
            double pacum = 0;

            for (int i = 0; i < series.getLowerPrice().Length; i++)
            {

                double CC = series.getClosePrice()[i];
                double H = series.getHigherPrice()[i];
                double L = series.getLowerPrice()[i];
                double PP = (H + L + CC) / 3;

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (i == 0)
                {
                    r3[i] = H + 2 * (PP - L);
                    s3[i] = L - 2 * (H - PP);
                }
                else
                {
                    if (CC > r3[i - 1] || CC < s3[i - 1])
                        r3[i] = H + 2 * (PP - L);
                    else
                        r3[i] = r3[i - 1];

                    if (CC > r3[i - 1] || CC < s3[i - 1])
                        s3[i] = L - 2 * (H - PP);
                    else
                        s3[i] = s3[i - 1];

                    if (CC >= r3[i - 1] && !comprado)
                    {
                        lcc = CC;
                        acum = acum - lcc;
                        comprado = true;

                        ret[i] = "C";
                        Console.WriteLine("C" + "\t" + series.getDate()[i] + "\t"
                                + (-CC));

                    }
                    else if (CC <= s3[i - 1] && comprado)
                    {
                        acum = acum + CC;
                        pacum = pacum + (((CC / lcc) - 1) * 100);
                        comprado = false;

                        ret[i] = "V";
                        Console.WriteLine("V" + "\t" + series.getDate()[i] + "\t"
                                + CC);

                    }

                }
            }
            if (comprado)
                acum = acum + lcc;

            Console.WriteLine("Lucro =" + acum);
            Console.WriteLine("LucrP =" + pacum + "%");

            results.Add(ret);

            return results;
        }
    }

}