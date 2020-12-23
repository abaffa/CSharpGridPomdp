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

    public class ROC : PluginAbstract
    {

        public int period = 10;

        double[] retROC;



        public ROC()
        {
            initialSetup();
        }

        public ROC(Series series)
        {
            initialSetup();
            this.series = series;
        }

        private void initialSetup()
        {
            isConfigurable = true;
            isSimilarToSeries = false;
            chartType = chartTypes.CONTINUOUS;

            pluginName = "ROC";
            pluginDescription = "Rate of Change";
        }


        public double[] roc(int n, Series series)
        {

            double[] retRoc = new double[series.getClosePrice().Length];

            for (int x = 0; x < series.getClosePrice().Length; x++)
            {
                int init = 0;

                if (x >= n)
                    init = x - n;

                retRoc[x] = (series.getClosePrice()[x] - series.getClosePrice()[init])
                        / series.getClosePrice()[init];
            }
            return retRoc;
        }

        public override List<String> GetResultDescription()
        {

            List<String> results = new List<String>();

            results.Add("Rate of Change");

            return results;
        }

        public override List<double[]> GetResultValues()
        {

            List<double[]> results = new List<double[]>();

            retROC = roc(period, series);
            results.Add(retROC);

            return results;
        }

        public String[] calculateBuySellMethod()
        {

            String[] ret = new String[series.getClosePrice().Length];

            bool buy = false;

            double compra = 0;
            double totalCompra = 0;
            double totalVenda = 0;

            double percTotal = 0;


            String lastVenda = "";

            String lastCompra = "";

            String lastMessage = "";

            double[] roc = retROC;

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (roc[i] > 0 && !buy)
                {
                    compra = series.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
                            + series.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (roc[i] < 0 && buy)
                {
                    double perc = (series.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i] + "\t"
                            + series.getClosePrice()[i] + "\t"
                            + StatisticLib.percFormat(perc, 2);

                    lastMessage = sVenda;
                    lastVenda = sVenda;
                    Console.WriteLine(sVenda);

                    buy = false;
                    ret[i] = "V";
                }
            }
            Console.WriteLine("ROC\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("ROC\tResultado\tLUCRO\n");
            else
                Console.WriteLine("ROC\tResultado\tPREJUIZO\n");

            // TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public override List<String> GetBuySellDescription()
        {
            List<String> results = new List<String>();

            results.Add("ROC");

            return results;
        }

        public override List<String[]> GetBuySellValues()
        {
            List<String[]> results = new List<String[]>();

            results.Add(calculateBuySellMethod());

            return results;
        }


    }

}