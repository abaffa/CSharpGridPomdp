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

    public class CCI : PluginAbstract
    {

        public int period = 20;
        public double factor = 0.015;
        public int skipdays = 0;

        double[] retCCI;



        public CCI()
        {
            initialSetup();
        }

        public CCI(Series series)
        {
            initialSetup();
            this.series = series;
        }

        private void initialSetup()
        {
            isConfigurable = true;
            isSimilarToSeries = false;
            chartType = chartTypes.CONTINUOUS;

            pluginName = "CCI";
            pluginDescription = "Commodity Channel Index";
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

        public double[] cci(int n, double factor, Series series, int skipdays)
        {

            double[] retCCI = new double[series.getClosePrice().Length];

            double[] tr = new double[series.getClosePrice().Length];

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {
                tr[i] = (series.getHigherPrice()[i] + series.getLowerPrice()[i] + series
                        .getClosePrice()[i]) / 3;
            }

            double[] vSMA = sma(n, tr, skipdays);

            for (int x = 0; x < series.getClosePrice().Length; x++)
            {
                int init = 0;

                if (x >= 20)
                    init = x - 20;

                double[] m = new double[(x + 1) - init];

                for (int z = init; z < (x + 1); z++)
                {
                    m[z - init] = tr[z];
                }
                double v2 = m[0];
                if (x > 0)
                    v2 = StatisticLib.stddeviation(m);

                retCCI[x] = (1 / factor) * ((tr[x] - vSMA[x]) / v2);
            }
            return retCCI;
        }

        public override List<String> GetResultDescription()
        {

            List<String> results = new List<String>();

            results.Add("CCI");

            return results;
        }

        public override List<double[]> GetResultValues()
        {

            List<double[]> results = new List<double[]>();

            retCCI = cci(period, factor, series, skipdays);
            results.Add(retCCI);

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

            double[] cci = retCCI;

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (cci[i] > 0 && !buy)
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
                else if (cci[i] < 0 && buy)
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
            Console.WriteLine("CCI1\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("CCI1\tResultado\tLUCRO\n");
            else
                Console.WriteLine("CCI1\tResultado\tPREJUIZO\n");

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

            double[] cci = retCCI;

            for (int i = 0; i < series.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (cci[i] >= 100 && !buy)
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
                else if (cci[i] <= -100 && buy)
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
            Console.WriteLine("CCI2\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("CCI2\tResultado\tLUCRO\n");
            else
                Console.WriteLine("CCI2\tResultado\tPREJUIZO\n");

            // TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public override List<String> GetBuySellDescription()
        {
            List<String> results = new List<String>();

            results.Add("CCI x CENTER");
            results.Add("CCI x FRONTIER ");

            return results;
        }

        public override List<String[]> GetBuySellValues()
        {
            List<String[]> results = new List<String[]>();

            results.Add(calculateBuySellMethod1());
            results.Add(calculateBuySellMethod2());

            return results;
        }

    }

}