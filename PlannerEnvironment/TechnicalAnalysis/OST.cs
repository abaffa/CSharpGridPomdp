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
    public class OST : PluginAbstract
    {

        public int ostPeriod = 14;

        public int smaPeriod = 3;
        public int skipdays = 0;

        double[] retKOST;
        double[] retDOST;

        public OST()
        {
            initialSetup();
        }

        public OST(Series series)
        {
            initialSetup();
            this.series = series;
        }

        private void initialSetup()
        {
            isConfigurable = true;
            isSimilarToSeries = false;
            chartType = chartTypes.CONTINUOUS;

            pluginName = "OST";
            pluginDescription = "Stochastic Oscillator";
        }

        public double[] sma(int n, Series series, int skipdays)
        {

            return sma(n, series.getClosePrice(), skipdays);
        }

        public double[] sma(int period, double[] series, int skipdays)
        {

            // Console.WriteLine("SMA(" + period + ") for " + series.Length
            // + " skipd:" + skipdays);

            double[] retValue = new double[series.Length];

            for (int counter = 1; counter <= series.Length; counter++)
            {

                double value = 0.0;
                int per = period;
                if (counter < period)
                    per = counter;

                for (int i = counter - per; i < counter; i++)
                {
                    // debugPrint("i: "+i);

                    value += series[i];
                }
                value /= (double)per;
                retValue[counter - 1] = value;

                // Console.WriteLine(counter + " # " + series[counter - 1] + " - "
                // + retValue[counter - 1]);

            }

            return retValue;
        }
        public double[][] ost(int n1, int n2, Series series, int skipdays)
        {

            double[][] retPerc = new double[2][];
            retPerc[0] = new double[series.getClosePrice().Length];
            retPerc[1] = new double[series.getClosePrice().Length];

            for (int x = 0; x < series.getClosePrice().Length; x++)
            {
                int init = 0;

                if (x >= n1)
                    init = x - n1;

                double min = 0;
                double max = 0;

                for (int z = init; z < (x + 1); z++)
                {

                    if (min > series.getLowerPrice()[z])
                        min = series.getLowerPrice()[z];
                    else if (min == 0)
                        min = series.getLowerPrice()[z];

                    if (max < series.getHigherPrice()[z])
                        max = series.getHigherPrice()[z];
                    else if (max == 0)
                        max = series.getHigherPrice()[z];
                }

                retPerc[0][x] = (series.getClosePrice()[x] - min) / (max - min);

            }

            retPerc[1] = sma(n2, retPerc[0], skipdays);

            return retPerc;
        }

        public override List<String> GetResultDescription()
        {

            List<String> results = new List<String>();

            results.Add("%K (Fast)");

            results.Add("%D (Slow)");

            return results;
        }

        public override List<double[]> GetResultValues()
        {

            List<double[]> results = new List<double[]>();

            double[][] ret = ost(ostPeriod, smaPeriod, series, skipdays);
            retKOST = ret[0];
            results.Add(retKOST);

            retDOST = ret[1];
            results.Add(retDOST);

            return results;
        }

        public String[] calculateBuySellMethod1()
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

            double[] K_perc = retKOST;
            double[] D_perc = retDOST;

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (K_perc[i] > D_perc[i] && !buy)
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
                else if (K_perc[i] < D_perc[i] && buy)
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

            Console.WriteLine("OST1\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("OST1\tResultado\tLUCRO\n");
            else
                Console.WriteLine("OST1\tResultado\tPREJUIZO\n");

            // TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public String[] calculateBuySellMethod2()
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

            double[] K_perc = retKOST;


            double[] D_perc = retDOST;

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (K_perc[i] > 0.70 && !buy)
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
                else if (K_perc[i] < 0.30 && buy)
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

            Console.WriteLine("OST2\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("OST2\tResultado\tLUCRO\n");
            else
                Console.WriteLine("OST2\tResultado\tPREJUIZO\n");

            // TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public String[] calculateBuySellMethod3()
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



            double[] K_perc = retKOST;
            double[] D_perc = retDOST;

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (D_perc[i] > 0.70 && !buy)
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
                else if (D_perc[i] < 0.30 && buy)
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

            Console.WriteLine("OST3\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("OST3\tResultado\tLUCRO\n");
            else
                Console.WriteLine("OST3\tResultado\tPREJUIZO\n");

            // TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public override List<String> GetBuySellDescription()
        {
            List<String> results = new List<String>();

            results.Add("%K x %D");

            results.Add("%K x Frontier");

            results.Add("%D x Frontier");

            return results;
        }

        public override List<String[]> GetBuySellValues()
        {
            List<String[]> results = new List<String[]>();

            results.Add(calculateBuySellMethod1());

            results.Add(calculateBuySellMethod2());

            results.Add(calculateBuySellMethod3());

            return results;
        }


    }
}