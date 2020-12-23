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
    public class SMA : PluginAbstract
    {

        bool useLongOnly = false;

        int shortPeriod = 10;
        int shortSkipdays = 0;

        int longPeriod = 40;
        int longSkipdays = 0;

        double[] retShortSMA;
        double[] retLongSMA;



        public SMA()
        {
            initialSetup();
        }

        public SMA(Series series)
        {
            initialSetup();
            this.series = series;
        }

        private void initialSetup()
        {
            isConfigurable = true;
            isSimilarToSeries = true;
            chartType = chartTypes.CONTINUOUS;

            pluginName = "SMA";
            pluginDescription = "Simple Moving Average";
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

        public override List<String> GetResultDescription()
        {

            List<String> results = new List<String>();

            if (!useLongOnly)
            {
                results.Add("Short SMA");
            }

            results.Add("Long SMA");

            return results;
        }

        public override List<double[]> GetResultValues()
        {

            List<double[]> results = new List<double[]>();

            if (!useLongOnly)
            {
                retShortSMA = sma(shortPeriod, series, shortSkipdays);
                results.Add(retShortSMA);
            }

            retLongSMA = sma(longPeriod, series, longSkipdays);
            results.Add(retLongSMA);

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

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (retShortSMA[i] >= retLongSMA[i] && !buy)
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
                else if (retShortSMA[i] <= retLongSMA[i] && buy)
                {
                    double venda = series.getClosePrice()[i];

                    double perc = (venda / compra) - 1;

                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series.getClosePrice()[i];

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
            Console.WriteLine("SMA1\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("SMA1\tResultado\tLUCRO\n");
            else
                Console.WriteLine("SMA1\tResultado\tPREJUIZO\n");

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

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (series.getClosePrice()[i] >= retLongSMA[i] && !buy)
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
                else if (series.getClosePrice()[i] <= retLongSMA[i] && buy)
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
            Console.WriteLine("SMA2\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("SMA2\tResultado\tLUCRO\n");
            else
                Console.WriteLine("SMA2\tResultado\tPREJUIZO\n");

            // TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public String[] calculateBuySellMethod3()
        {

            String[] ret = new String[series.getClosePrice().Length];

            String[] ret1 = calculateBuySellMethod1();
            String[] ret2 = calculateBuySellMethod2();

            bool buy = false;

            double compra = 0;
            double totalCompra = 0;
            double totalVenda = 0;

            double percTotal = 0;


            String lastVenda = "";

            String lastCompra = "";

            String lastMessage = "";

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (ret1[i] == "C" && ret2[i] == "C" && !buy)
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
                else if (ret1[i] == "V" && ret2[i] == "V" && buy)
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
            Console.WriteLine("SMA3\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("SMA3\tResultado\tLUCRO\n");
            else
                Console.WriteLine("SMA3\tResultado\tPREJUIZO\n");

            // TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public override List<String> GetBuySellDescription()
        {
            List<String> results = new List<String>();

            if (!useLongOnly)
                results.Add("Short x Long");

            results.Add("Serie x Long");

            if (!useLongOnly)
                results.Add("(Short x Long) x (Serie x Long)");

            return results;
        }

        public override List<String[]> GetBuySellValues()
        {
            List<String[]> results = new List<String[]>();

            if (!useLongOnly)
                results.Add(calculateBuySellMethod1());

            results.Add(calculateBuySellMethod2());

            if (!useLongOnly)
                results.Add(calculateBuySellMethod3());

            return results;
        }

    }

}