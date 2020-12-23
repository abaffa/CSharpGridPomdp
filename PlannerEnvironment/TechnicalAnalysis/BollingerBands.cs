using Baffa.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraderWhatever.Business;

namespace PlannerEnvironment.TechnicalAnalysis
{
    public class BollingerBands : PluginAbstract
    {

        private int period = 20;
        private int stdDevNumber = 2;
        private int skipdays = 0;

        double[] middleBand;
        double[] upperBand;
        double[] lowerBand;

        double[] retMiddleBand;
        double[] retUpperBand;
        double[] retLowerBand;

       

        public BollingerBands() {
            initialSetup();
        }

        public BollingerBands(Series series) {
            initialSetup();
            this.series = series;
        }

        private void initialSetup() {
            isConfigurable = true;
            isSimilarToSeries = true;
            chartType = chartTypes.CONTINUOUS;

            pluginName = "Bollinger Bands";
            pluginDescription = "Bollinger Bands";
        }


        public double[] sma(int n, Series series, int skipdays) {

            return sma(n, series.getClosePrice(), skipdays);
        }

        public double[] sma(int period, double[] series, int skipdays) {

            // Console.WriteLine("SMA(" + period + ") for " + series.Length
            // + " skipd:" + skipdays);

            double[] retValue = new double[series.Length];

            for (int counter = 1; counter <= series.Length; counter++) {

                double value = 0.0;
                int per = period;
                if (counter < period)
                    per = counter;

                for (int i = counter - per; i < counter; i++) {
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


        public double[] lowerB(int n, int stdDevNum, double[] middleB, Series series, int skipdays) {

            return lowerB(n, stdDevNum, middleB, series.getClosePrice(), skipdays);
        }

        public double[] lowerB(int period, int stdDevNum, double[] middleB, double[] series, int skipdays) {

            // Console.WriteLine("SMA(" + period + ") for " + series.Length
            // + " skipd:" + skipdays);

            double[] retValue = new double[series.Length];

            for (int counter = 1; counter <= series.Length; counter++) {

                double value = 0.0;
                int per = period;
                if (counter < period)
                    per = counter;

                for (int i = counter - per; i < counter; i++) {
                    // debugPrint("i: "+i);

                    value += Math.Pow((series[i] - middleB[i]), 2);
                }
                value /= (double)per;

                retValue[counter - 1] = middleB[counter - 1] - (stdDevNum * Math.Sqrt(value));

                // Console.WriteLine(counter + " # " + series[counter - 1] + " - "
                // + retValue[counter - 1]);

            }

            return retValue;
        }

        public double[] upperB(int n, int stdDevNum, double[] middleB, Series series, int skipdays) {

            return upperB(n, stdDevNum, middleB, series.getClosePrice(), skipdays);
        }

        public double[] upperB(int period, int stdDevNum, double[] middleB, double[] series, int skipdays) {

            // Console.WriteLine("SMA(" + period + ") for " + series.Length
            // + " skipd:" + skipdays);

            double[] retValue = new double[series.Length];

            for (int counter = 1; counter <= series.Length; counter++) {

                double value = 0.0;
                int per = period;
                if (counter < period)
                    per = counter;

                for (int i = counter - per; i < counter; i++) {
                    // debugPrint("i: "+i);

                    value += Math.Pow((series[i] - middleB[i]), 2);
                }
                value /= (double)per;

                retValue[counter - 1] = middleB[counter - 1] + (stdDevNum * Math.Sqrt(value));

                // Console.WriteLine(counter + " # " + series[counter - 1] + " - "
                // + retValue[counter - 1]);

            }

            return retValue;
        }





        public override List<String> GetResultDescription() {

            List<String> results = new List<String>();

            results.Add("Middle Band");
            results.Add("Lower Band");
            results.Add("Upper Band");

            return results;
        }

        public override List<double[]> GetResultValues() {

            List<double[]> results = new List<double[]>();

            middleBand = sma(period, series, skipdays);

            lowerBand = lowerB(period, stdDevNumber, middleBand, series, skipdays);
            upperBand = upperB(period, stdDevNumber, middleBand, series, skipdays);


            retMiddleBand = middleBand;
            retLowerBand = lowerBand;
            retUpperBand = upperBand;

            results.Add(retMiddleBand);
            results.Add(retLowerBand);
            results.Add(retUpperBand);

            return results;
        }

        public String[] calculateBuySellMethod1() {

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

        public String[] calculateBuySellMethod2() {

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

        public String[] calculateBuySellMethod3() {

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

        public override List<String> GetBuySellDescription() {
            List<String> results = new List<String>();

            results.Add("MACD X Signal");

            results.Add("MACD");

            results.Add("Histogram");

            return results;
        }

        public override List<String[]> GetBuySellValues() {
            List<String[]> results = new List<String[]>();

            results.Add(calculateBuySellMethod1());

            results.Add(calculateBuySellMethod2());

            results.Add(calculateBuySellMethod3());

            return results;
        }

      

        public static void main(String[] args) {

            // String codNeg = "^BVSP";
            //		
            // String datInicio = "2008-04-11";
            // String datFim = "2008-09-03";
            // java.text.SimpleDateFormat df = new java.text.SimpleDateFormat(
            // "yyyy-MM-dd");
            // try {
            // java.util.Calendar cdatInicio = Calendar.getInstance();
            // cdatInicio.setTime(df.parse(datInicio));
            //		
            // java.util.Calendar cdatFim = Calendar.getInstance();
            // cdatFim.setTime(df.parse(datFim));
            //		
            // Series series = new Series(codNeg, cdatInicio, cdatFim, null);
            //		
            // ADX s = new ADX(series);
            // s.showConfig();
            // List d = s.getResultValues();
            // double[] x = (double[]) d.get(0);
            // for (int i = 0; i < x.Length; i++)
            // Console.WriteLine(x[i]);
            //		
            // } catch (Exception e) {
            // e.printStackTrace();
            // }
        }

        // ////////////////////////////////////////////
        // /CONFIG LAYOUT
        // ////////////////////////////////////////////

      

    }
}