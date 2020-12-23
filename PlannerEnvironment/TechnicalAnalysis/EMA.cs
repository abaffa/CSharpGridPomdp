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

    public class EMA : PluginAbstract
    {

        bool useLongOnly = false;

        public int shortPeriod = 10;
        public int shortSkipdays = 0;

        public int longPeriod = 40;
        public int longSkipdays = 0;

        double[] retShortEMA;
        double[] retLongEMA;

        public EMA()
        {
            initialSetup();
        }

        public EMA(Series series)
        {
            initialSetup();
            this.series = series;
        }

        private void initialSetup()
        {
            isConfigurable = true;
            isSimilarToSeries = true;
            chartType = chartTypes.CONTINUOUS;

            pluginName = "EMA";
            pluginDescription = "Exponential Moving Average";
        }

        public double[] ema(int n, Series series, int skipdays)
        {

            return ema(n, series.getClosePrice(), skipdays);
        }

        //old
        //	public double[] ema(int n, double[] series, int skipdays) {
        //
        //		// Console.WriteLine("EMA(" + n + ") for " + series.Length + " skipd:"
        //		// + skipdays);
        //
        //		double[] retValue = new double[series.Length];
        //
        //		double exponent = 2 / (double) (n + 1);
        //
        //		retValue[0] = series[0];// * (1- exponent);
        //
        //		for (int counter = 1; counter < series.Length; counter++) {
        //
        //			double value = 0;
        //
        //			value = ((series[counter] - retValue[counter - 1]) * exponent)
        //					+ retValue[counter - 1];
        //			// Console.WriteLine("Value:"+value);
        //
        //			retValue[counter] = value;
        //		}
        //
        //		return retValue;
        //	}

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

        public override List<String> GetResultDescription()
        {

            List<String> results = new List<String>();

            if (!useLongOnly)
            {
                results.Add("Short EMA");
            }

            results.Add("Long EMA");

            return results;
        }

        public override List<double[]> GetResultValues()
        {

            List<double[]> results = new List<double[]>();

            if (!useLongOnly)
            {
                retShortEMA = ema(shortPeriod, series, shortSkipdays);
                results.Add(retShortEMA);
            }

            retLongEMA = ema(longPeriod, series, longSkipdays);
            results.Add(retLongEMA);

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

                if (retShortEMA[i] >= retLongEMA[i] && !buy)
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
                else if (retShortEMA[i] <= retLongEMA[i] && buy)
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
            Console.WriteLine("EMA1\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("EMA1\tResultado\tLUCRO\n");
            else
                Console.WriteLine("EMA1\tResultado\tPREJUIZO\n");

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

                if (series.getClosePrice()[i] >= retLongEMA[i] && !buy)
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
                else if (series.getClosePrice()[i] <= retLongEMA[i] && buy)
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
            Console.WriteLine("EMA2\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("EMA2\tResultado\tLUCRO\n");
            else
                Console.WriteLine("EMA2\tResultado\tPREJUIZO\n");

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
            Console.WriteLine("EMA3\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("EMA3\tResultado\tLUCRO\n");
            else
                Console.WriteLine("EMA3\tResultado\tPREJUIZO\n");

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