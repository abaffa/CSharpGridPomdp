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

    public class MACD : PluginAbstract
    {

        int shortPeriod = 12;
        int longPeriod = 26;
        int smoothingPeriod = 9;

        int skipdays = 0;

        double[] retMacd;
        double[] retSignal;
        double[] retHistogram;



        public MACD()
        {
            initialSetup();
        }

        public MACD(Series series)
        {
            initialSetup();
            this.series = series;
        }

        private void initialSetup()
        {
            isConfigurable = true;
            isSimilarToSeries = false;
            chartType = chartTypes.CONTINUOUS;

            pluginName = "MACD";
            pluginDescription = "Moving Average Convergence-Divergence";
        }

        public double[] ema(int n, Series series, int skipdays)
        {

            return ema(n, series.getClosePrice(), skipdays);
        }

        public double[] ema(int n, double[] series, int skipdays)
        {

            // Console.WriteLine("EMA(" + n + ") for " + series.Length + " skipd:"
            // + skipdays);

            double[] retValue = new double[series.Length];

            double exponent = 2 / (double)(n + 1);

            retValue[0] = series[0];// * (1- exponent);

            for (int counter = 1; counter < series.Length; counter++)
            {

                double value = 0;
                // old
                // value = ((series[counter] - retValue[counter - 1]) * exponent)
                // + retValue[counter - 1];

                // new
                value = ((series[counter] * exponent) + (retValue[counter - 1] * (1 - exponent)));

                //			Console.WriteLine("\t" + series[counter] + "\t" + value);

                // Console.WriteLine("Value:"+value);

                retValue[counter] = value;
            }

            return retValue;
        }


        public double[][] macd(int p1, int p2, int signal, Series series,
                int skipdays)
        {
            double[][] retMACD = new double[3][];
            retMACD[0] = new double[series.getClosePrice().Length];
            retMACD[1] = new double[series.getClosePrice().Length];
            retMACD[2] = new double[series.getClosePrice().Length];

            double[] ema1 = ema(p1, series, skipdays);
            double[] ema2 = ema(p2, series, skipdays);

            for (int i = 0; i < retMACD[0].Length; i++)
            {
                retMACD[0][i] = ema1[i] - ema2[i];
                // Console.WriteLine("#" + i + "#" + ema1[i] + " - " + ema2[i]
                // + " = " + retMACD[0][i]);
            }

            retMACD[1] = ema(signal, retMACD[0], skipdays);

            for (int i = 0; i < retMACD[0].Length; i++)
            {
                retMACD[2][i] = retMACD[0][i] - retMACD[1][i];
                // Console.WriteLine("#" + i + "#" + retMACD[0][i] + " - "
                // + retMACD[1][i] + " = " + retMACD[2][i]);

            }

            return retMACD;
        }

        public override List<String> GetResultDescription()
        {

            List<String> results = new List<String>();

            // results.Add("MACD.OSC");
            // results.Add("MACD.EMAOSC");
            // results.Add("MACD.DIFF");

            results.Add("MACD");
            results.Add("Signal");
            results.Add("Histogram");

            return results;
        }

        public override List<double[]> GetResultValues()
        {

            List<double[]> results = new List<double[]>();

            double[][] values = macd(shortPeriod, longPeriod, smoothingPeriod,
                    series, skipdays);

            retMacd = values[0];
            retSignal = values[1];
            retHistogram = values[2];

            results.Add(retMacd);
            results.Add(retSignal);
            results.Add(retHistogram);

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

            double[] macd = retMacd;
            double[] signal = retSignal;

            double[] histogram = retHistogram;

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (macd[i] > signal[i] && !buy)
                {
                    compra = series.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
                            + compra;

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (macd[i] < signal[i] && buy)
                {
                    double venda = series.getClosePrice()[i];
                    double perc = (venda / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + venda;

                    String sVenda = "venda" + "\t" + series.getDate()[i] + "\t"
                            + venda + "\t"
                            + StatisticLib.percFormat(perc, 2);

                    lastMessage = sVenda;
                    lastVenda = sVenda;
                    Console.WriteLine(sVenda);

                    buy = false;
                    ret[i] = "V";
                }

            }

            Console.WriteLine("MACD1\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("MACD1\tResultado\tLUCRO\n");
            else
                Console.WriteLine("MACD1\tResultado\tPREJUIZO\n");

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

            double[] macd = retMacd;

            double[] signal = retSignal;

            double[] histogram = retHistogram;

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (macd[i] > 0 && !buy)
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
                else if (macd[i] < 0 && buy)
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

            Console.WriteLine("MACD2\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("MACD2\tResultado\tLUCRO\n");
            else
                Console.WriteLine("MACD2\tResultado\tPREJUIZO\n");

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


            double[] macd = retMacd;

            double[] signal = retSignal;
            double[] histogram = retHistogram;

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (histogram[i] > 0 && !buy)
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
                else if (histogram[i] < 0 && buy)
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

            Console.WriteLine("MACD3\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("MACD3\tResultado\tLUCRO\n");
            else
                Console.WriteLine("MACD3\tResultado\tPREJUIZO\n");

            // TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public override List<String> GetBuySellDescription()
        {
            List<String> results = new List<String>();

            results.Add("MACD X Signal");

            results.Add("MACD");

            results.Add("Histogram");

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