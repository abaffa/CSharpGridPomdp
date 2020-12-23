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
    public class ATR : PluginAbstract
    {

        private int period = 14;
        private int skipdays = 0;

        private bool useExponential = false;

        double[] tr; // True Range
        double[] atr; // Average True Range

        double[] retATR;

        public ATR()
        {
            initialSetup();
        }

        public ATR(Series series)
        {
            initialSetup();
            this.series = series;
        }

        private void initialSetup()
        {
            isConfigurable = true;
            isSimilarToSeries = false;
            chartType = chartTypes.CONTINUOUS;

            pluginName = "ATR";
            pluginDescription = "Average True Range";
        }

        public double[] Tr(Series series)
        {

            double[] retTr = new double[series.getClosePrice().Length];

            tr = new double[series.getClosePrice().Length];
            tr[0] = 0;

            for (int i = 1; i < retTr.Length; i++)
            {

                double tr_ = 0;

                // Calculate TR
                double max1 = Math.Abs(series.getHigherPrice()[i]
                        - series.getLowerPrice()[i]);
                double max2 = Math.Abs(series.getHigherPrice()[i]
                        - series.getClosePrice()[i - 1]);
                double max3 = Math.Abs(series.getClosePrice()[i - 1]
                        - series.getLowerPrice()[i]);

                tr_ = Math.Max(Math.Max(max1, max2), max3);
                tr[i] = tr_;

                //			Console.WriteLine("tr = " + tr[i]);
            }
            retTr = tr;

            return retTr;
        }

        public double[] Atr(int n, double[] tr_)
        {

            double[] retAtr = new double[tr_.Length];

            atr = new double[tr_.Length];
            atr[0] = 0;

            for (int i = 1; i < retAtr.Length; i++)
            {

                //			if (i == 1)
                //				atr[i] = tr_[i];
                //
                //			else {

                // Calculate ATR
                if (!useExponential)
                {
                    atr[i] = (atr[i - 1] * (((double)n - (double)1) / (double)n))
                            + (tr_[i] * ((double)1 / (double)n));
                }
                else
                {
                    // /Variance using exponential moving average//
                    atr[i] = (atr[i - 1] * ((double)1 - ((double)2 / ((double)n + (double)1))))
                            + (tr_[i] * ((double)2 / ((double)n + (double)1)));
                }
                //			}

                //			Console.WriteLine("atr = " + atr[i]);

            }

            retAtr = atr;

            return retAtr;
        }


        public override List<String> GetResultDescription()
        {

            List<String> results = new List<String>();

            results.Add("ATR");

            return results;
        }

        public override List<double[]> GetResultValues()
        {

            List<double[]> results = new List<double[]>();

            double[] retTr = Tr(series);
            tr = retTr;

            retATR = Atr(period, retTr);
            atr = retATR;

            results.Add(retATR);

            return results;
        }

        public String[] calculateBuySellMethod1()
        {

            String[] ret = new String[series.getClosePrice().Length];
            //
            // bool buy = false;
            //
            // double compra = 0;
            // double totalCompra = 0;
            // double totalVenda = 0;
            //
            // double percTotal = 0;
            //
            // String lastVenda = "";
            // String lastCompra = "";
            // String lastMessage = "";
            //
            // double[] macd = retADX;
            // double[] signal = retSignal;
            // double[] histogram = retHistogram;
            //
            // for (int i = 0; i < series.getClosePrice().Length; i++) {
            //
            // if (i > 0)
            // ret[i] = ret[i - 1];
            // else
            // ret[i] = "-";
            //
            // if (macd[i] > signal[i] && !buy) {
            // compra = series.getClosePrice()[i];
            //
            // totalCompra = totalCompra + compra;
            //
            // String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
            // + compra;
            //
            // lastMessage = sCompra;
            // lastCompra = sCompra;
            // Console.WriteLine(sCompra);
            //
            // buy = true;
            // ret[i] = "C";
            // } else if (macd[i] < signal[i] && buy) {
            // double venda = series.getClosePrice()[i];
            // double perc = (venda / compra) - 1;
            // percTotal = percTotal + perc;
            //
            // totalVenda = totalVenda + venda;
            //
            // String sVenda = "venda" + "\t" + series.getDate()[i] + "\t"
            // + venda + "\t"
            // + StatisticLib.percFormat(perc, 2);
            //
            // lastMessage = sVenda;
            // lastVenda = sVenda;
            // Console.WriteLine(sVenda);
            //
            // buy = false;
            // ret[i] = "V";
            // }
            //
            // }
            //
            // Console.WriteLine("MACD1\tResultado\t"
            // + StatisticLib.percFormat(percTotal, 2));
            //
            // if (percTotal > 0)
            // Console.WriteLine("MACD1\tResultado\tLUCRO\n");
            // else
            // Console.WriteLine("MACD1\tResultado\tPREJUIZO\n");
            //
            // // TotalTotal = TotalTotal + percTotal;
            //
            return ret;
        }

        public String[] calculateBuySellMethod2()
        {

            String[] ret = new String[series.getClosePrice().Length];
            //
            // bool buy = false;
            //
            // double compra = 0;
            // double totalCompra = 0;
            // double totalVenda = 0;
            //
            // double percTotal = 0;
            //
            // String lastVenda = "";
            // String lastCompra = "";
            // String lastMessage = "";
            //
            // double[] macd = retADX;
            // double[] signal = retSignal;
            // double[] histogram = retHistogram;
            //
            // for (int i = 0; i < series.getClosePrice().Length; i++) {
            //
            // if (i > 0)
            // ret[i] = ret[i - 1];
            // else
            // ret[i] = "-";
            //
            // if (macd[i] > 0 && !buy) {
            // compra = series.getClosePrice()[i];
            //
            // totalCompra = totalCompra + compra;
            //
            // String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
            // + series.getClosePrice()[i];
            //
            // lastMessage = sCompra;
            // lastCompra = sCompra;
            // Console.WriteLine(sCompra);
            //
            // buy = true;
            // ret[i] = "C";
            // } else if (macd[i] < 0 && buy) {
            // double perc = (series.getClosePrice()[i] / compra) - 1;
            // percTotal = percTotal + perc;
            //
            // totalVenda = totalVenda + series.getClosePrice()[i];
            //
            // String sVenda = "venda" + "\t" + series.getDate()[i] + "\t"
            // + series.getClosePrice()[i] + "\t"
            // + StatisticLib.percFormat(perc, 2);
            //
            // lastMessage = sVenda;
            // lastVenda = sVenda;
            // Console.WriteLine(sVenda);
            //
            // buy = false;
            // ret[i] = "V";
            // }
            //
            // }
            //
            // Console.WriteLine("MACD2\tResultado\t"
            // + StatisticLib.percFormat(percTotal, 2));
            //
            // if (percTotal > 0)
            // Console.WriteLine("MACD2\tResultado\tLUCRO\n");
            // else
            // Console.WriteLine("MACD2\tResultado\tPREJUIZO\n");
            //
            // // TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public String[] calculateBuySellMethod3()
        {

            String[] ret = new String[series.getClosePrice().Length];
            //
            // bool buy = false;
            //
            // double compra = 0;
            // double totalCompra = 0;
            // double totalVenda = 0;
            //
            // double percTotal = 0;
            //
            // String lastVenda = "";
            // String lastCompra = "";
            // String lastMessage = "";
            //
            // double[] macd = retADX;
            // double[] signal = retSignal;
            // double[] histogram = retHistogram;
            //
            // for (int i = 0; i < series.getClosePrice().Length; i++) {
            //
            // if (i > 0)
            // ret[i] = ret[i - 1];
            // else
            // ret[i] = "-";
            //
            // if (histogram[i] > 0 && !buy) {
            // compra = series.getClosePrice()[i];
            //
            // totalCompra = totalCompra + compra;
            //
            // String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
            // + series.getClosePrice()[i];
            //
            // lastMessage = sCompra;
            // lastCompra = sCompra;
            // Console.WriteLine(sCompra);
            //
            // buy = true;
            // ret[i] = "C";
            // } else if (histogram[i] < 0 && buy) {
            // double perc = (series.getClosePrice()[i] / compra) - 1;
            // percTotal = percTotal + perc;
            //
            // totalVenda = totalVenda + series.getClosePrice()[i];
            //
            // String sVenda = "venda" + "\t" + series.getDate()[i] + "\t"
            // + series.getClosePrice()[i] + "\t"
            // + StatisticLib.percFormat(perc, 2);
            //
            // lastMessage = sVenda;
            // lastVenda = sVenda;
            // Console.WriteLine(sVenda);
            //
            // buy = false;
            // ret[i] = "V";
            // }
            //
            // }
            //
            // Console.WriteLine("MACD3\tResultado\t"
            // + StatisticLib.percFormat(percTotal, 2));
            //
            // if (percTotal > 0)
            // Console.WriteLine("MACD3\tResultado\tLUCRO\n");
            // else
            // Console.WriteLine("MACD3\tResultado\tPREJUIZO\n");
            //
            // // TotalTotal = TotalTotal + percTotal;
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