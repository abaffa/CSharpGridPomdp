using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraderWhatever.Business;

namespace Baffa.MathModels
{
    public class FinancialLibrary
    {

      /**
	 * returns the simple moving average.
	 * 
	 * @param period
	 * @param candles
	 * @param skipdays -
	 *            does not work - future implementation
	 * @return the sma.
	 */
        public static double[] sma(int period, Series series, int skipdays)
        {

            //		System.out.println("SMA(" + period + ") for "
            //				+ series.getClosePrice().Length + " skipd:" + skipdays);

            double[] retValue = new double[series.getClosePrice().Length];

            for (int counter = 1; counter <= series.getClosePrice().Length; counter++)
            {

                double value = 0.0;
                int per = period;
                if (counter < period)
                    per = counter;

                for (int i = counter - per; i < counter; i++)
                {
                    // debugPrint("i: "+i);

                    value += series.getClosePrice()[i];
                }
                value /= (double)per;
                retValue[counter - 1] = value;

                //			System.out.println(counter + " # "
                //					+ series.getClosePrice()[counter - 1] + " - "
                //					+ retValue[counter - 1]);

            }

            return retValue;
        }

        public static double[] sma(int period, double[] series, int skipdays)
        {

            //		System.out.println("SMA(" + period + ") for " + series.Length
            //				+ " skipd:" + skipdays);

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

                //			System.out.println(counter + " # " + series[counter - 1] + " - "
                //					+ retValue[counter - 1]);

            }

            return retValue;
        }

        /**
         * returns the exponential moving average <br/> see
         * http://www.quotelinks.com/technical/ema.html
         * 
         * @param n
         * @param candles
         * @param skipdays
         * @return the exponential moving average
         */

        public static double[] ema(int n, Series series, int skipdays)
        {

            return ema(n, series.getClosePrice(), skipdays);
        }

        public static double[] ema(int n, double[] series, int skipdays)
        {

            // System.out.println("EMA(" + n + ") for " + series.Length + " skipd:"
            // + skipdays);

            double[] retValue = new double[series.Length];

            double exponent = 2 / (double)(n + 1);

            retValue[0] = series[0];// * (1- exponent);

            for (int counter = 1; counter < series.Length; counter++)
            {

                double value = 0;

                value = ((series[counter] - retValue[counter - 1]) * exponent)
                        + retValue[counter - 1];
                // System.out.println("Value:"+value);

                retValue[counter] = value;
            }

            return retValue;
        }

        /**
         * calculates the MACD, returns as [0] the MACD and as [1] the trigger line
         * and as [2] the oscillator
         * 
         * @param p1 =
         *            12
         * @param p2 =
         *            26
         * @param signal
         *            (SmoothingPeriod) = 9
         * @param v
         * @param skipdays
         * @return the MACD and the trigger.
         * 
         * retMACD[0] -> MACD.OSC
         * 
         * retMACD[1] -> MACD.EMAOSC
         * 
         * retMACD[2] ->- MACD.DIFF
         * 
         * MACD.OSC = EMA period1 (C) - EMA period2 (C)
         * 
         * MACD.EMAOSC = EMA SmoothingPeriod (MACD.OSC)
         * 
         * MACD.DIFF = MACD.OSC – MACD.EMAOSC
         * 
         * 
         * 
         * EMA = Exponential Moving Average
         * 
         * C = Closing price
         * 
         * Period1 = Usually defined as 12 days
         * 
         * MACD.EMAOSC = Moving Average over the MACD.OSC, usually takes the period
         * of 9 days
         * 
         * Period2 = Usually defined as 26 days
         * 
         * SmoothingPeriod = Usually defined as 9 days
         * 
         * 
         */
        public static double[][] macd(int p1, int p2, int signal, Series series,
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
                // System.out.println("#" + i + "#" + ema1[i] + " - " + ema2[i]
                // + " = " + retMACD[0][i]);
            }

            retMACD[1] = ema(signal, retMACD[0], skipdays);

            for (int i = 0; i < retMACD[0].Length; i++)
            {
                retMACD[2][i] = retMACD[0][i] - retMACD[1][i];
                // System.out.println("#" + i + "#" + retMACD[0][i] + " - "
                // + retMACD[1][i] + " = " + retMACD[2][i]);

            }

            return retMACD;
        }

        public static double[] mao(Series series, int skipdays)
        {

            double[] retMAO = new double[series.getClosePrice().Length];

            double[] ema1 = ema(3, series, skipdays);
            double[] ema2 = ema(13, series, skipdays);

            for (int i = 0; i < retMAO.Length; i++)
            {
                retMAO[i] = ema1[i] - ema2[i];
            }

            retMAO = sma(3, retMAO, skipdays);

            return retMAO;
        }

        /**
         * returns the commodity change index
         * 
         * @param n
         *            the period
         * @param candles
         *            the input candles
         * @param skipdays
         *            skipdays as usual
         * @return the value of commodity change index
         */
        public static double[] cci(int n, double factor, Series series, int skipdays)
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

        /**
         * returns the RSI
         * 
         * @param n
         * @param candles
         * @param skipdays
         * @return the RSI
         */

        public static double[] rsi(int period, Series series, int skipdays)
        {

            //		
            // System.out.println("RSI(" + period + ") for "
            // + series.getClosePrice().Length + " skipd:" + skipdays);

            double[] retValue = new double[series.getClosePrice().Length];

            retValue[0] = 0;
            for (int counter = 1; counter < series.getClosePrice().Length; counter++)
            {

                // double value = 0.0;
                int per = period;
                if (counter < period)
                    per = counter;

                double U = 0.0;
                double D = 0.0;

                for (int i = counter - per; i < counter; i++)
                {
                    // debugPrint("i: "+i);
                    double change = series.getClosePrice()[i + 1]
                            - series.getClosePrice()[i];

                    if (change > 0)
                    {
                        U += change;
                    }
                    else
                    {
                        D += Math.Abs(change);
                    }

                }

                try
                {

                    retValue[counter] = 100 - (100 / (1 + (U / D)));
                }
                catch (Exception e)
                {
                }
                //
                // System.out.println(counter + " # "
                // + series.getClosePrice()[counter - 1] + " - "
                // + retValue[counter - 1]);

            }

            return retValue;
        }

        /**
         * returns the parabolic SAR - double check with a reference implementation !
         * 
         * @param initialValue
         * @param candles
         * @param skipdays
         * @param n
         * 
         * @return the parabolic SAR
         */
        public static double[] sar(double initialValue, double af, double max,
                Series series, int skipdays)
        {

            double[] retSAR = new double[series.getClosePrice().Length];
            bool uptrend = true;

            double fator = initialValue;
            double lowerPrice = 0;
            double upperPrice = series.getClosePrice()[0];
            double EP = upperPrice;

            retSAR[0] = fator * EP;

            for (int z = 1; z < series.getClosePrice().Length; z++)
            {

                fator = fator + af;

                if (fator > max)
                    fator = max;

                if (uptrend)
                {

                    if (lowerPrice > 0)
                    {
                        lowerPrice = 0;
                        fator = initialValue;
                    }

                    if (upperPrice < series.getClosePrice()[z])
                        upperPrice = series.getClosePrice()[z];
                    else if (upperPrice == 0)
                        upperPrice = series.getClosePrice()[z];

                    EP = upperPrice;

                }
                else if (!uptrend)
                {

                    if (upperPrice > 0)
                    {
                        upperPrice = 0;
                        fator = initialValue;
                    }

                    if (lowerPrice > series.getClosePrice()[z])
                        lowerPrice = series.getClosePrice()[z];
                    else if (lowerPrice == 0)
                        lowerPrice = series.getClosePrice()[z];

                    EP = lowerPrice;
                }

                retSAR[z] = retSAR[z - 1] + (fator * (EP - retSAR[z - 1]));

                // debugPrint("Break - trend is at the start is in an up:" +
                // uptrend);
            }
            return retSAR;
        }

        public static double[] momentum(int n, Series series)
        {

            double[] retMom = new double[series.getClosePrice().Length];

            for (int x = 0; x < series.getClosePrice().Length; x++)
            {
                int init = 0;

                if (x >= n)
                    init = x - n;

                retMom[x] = series.getClosePrice()[x]
                        - series.getClosePrice()[init];
            }
            return retMom;
        }

        public static double[] roc(int n, Series series)
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

        public static double[][] ost(int n1, int n2, Series series, int skipdays)
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

                    if (max > series.getHigherPrice()[z])
                        max = series.getHigherPrice()[z];
                    else if (max == 0)
                        max = series.getHigherPrice()[z];
                }

                retPerc[0][x] = (series.getClosePrice()[x] - min) / (max - min);

            }

            retPerc[1] = sma(n2, retPerc[0], skipdays);

            return retPerc;
        }

    }
}
