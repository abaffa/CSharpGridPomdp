using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baffa.Helpers
{
    public class Statistics
    {
        private static int denFormatConst = 8;

        public static String status(String sMethod, String sName, int iWindow,
                int iHorizon, double[] dblRealSerieValues,
                double[] dblForecastSerieValues)
        {

            return status(sMethod, sName, iWindow, iHorizon, dblRealSerieValues, dblForecastSerieValues, false);
        }

        public static String status(String sMethod, String sName, int iWindow,
                int iHorizon, double[] dblRealSerieValues,
                double[] dblForecastSerieValues,
                Boolean listSerieValues)
        {

            StringBuilder textResult = new StringBuilder();
            StringBuilder str = new StringBuilder(250000);
            StringBuilder strValores = new StringBuilder(250000);

            // ////////////////////////////////////////////

            // Mean absolute error (a.k.a. Absolute deviation)
            double maeSum = 0;
            double errCount = 0;

            // Mean squared error
            // Sum((Actual value - forecast)^2))/n
            double mseSum = 0;

            // Root mean squared error
            // Sqrt(MSE)
            // double rmseSum = 0;

            // Mean absolute percentage error
            // Sum((Actual value - forecast)/ Actual value ))/n
            double mapeSum = 0;

            // Variáveis para calcular o valor U-Theil da previsão
            double uTheilNum = 0;
            double uTheilDen = 0;

            double seriesNormalizer = normalize(bigger(dblRealSerieValues));

            // Inclui no StringBuffer os valores reais, previstos e a diferença
            // entre
            // os valores
            strValores.Append("***** SERIES DATA *****" + "\n");

            int actualElement = 0;

            for (int i = iWindow - 1; i < dblRealSerieValues.Length; i++)
            {
                strValores.Append("Element: " + (actualElement));
                strValores.Append(" - Real: " + dblRealSerieValues[i]);
                strValores.Append(" - Predicted: " + dblForecastSerieValues[i]);
                strValores.Append(" - Absolute Error: "
                        + (dblForecastSerieValues[i] - dblRealSerieValues[i])
                        + "\n");

                // Acumula valores para o cálculo U-Theil
                uTheilNum = uTheilNum
                        + Math.Pow(dblRealSerieValues[i]
                                - dblForecastSerieValues[i], 2);
                uTheilDen = uTheilDen
                        + Math.Pow(dblRealSerieValues[i]
                                - dblRealSerieValues[i - 1], 2);

                // Acumula valores para o cálculo da diferença média
                maeSum += Math.Abs((dblRealSerieValues[i] / seriesNormalizer)
                        - (dblForecastSerieValues[i] / seriesNormalizer));

                mseSum += Math.Pow(Math
                        .Abs((dblRealSerieValues[i] / seriesNormalizer)
                                - (dblForecastSerieValues[i] / seriesNormalizer)),
                        2);

                mapeSum += fixNumber(Math
                        .Abs(((dblRealSerieValues[i] / seriesNormalizer) - (dblForecastSerieValues[i] / seriesNormalizer))
                                / (dblRealSerieValues[i] / seriesNormalizer)));

                errCount++;
                actualElement++;

            }
            strValores.Append("***** END SERIES DATA *****" + "\n");

            // Início do Resumo da previsão

            str.Append("***** FORECAST STATUS *****" + "\n");
            str.Append("* Method: " + sMethod + "\n");
            str.Append("* Name: " + sName + "\n");

            str.Append("* Window Size: " + iWindow + "\n");
            str.Append("* Forecast Horizon: " + iHorizon + "\n");

            str.Append("* Predicted Series Size: " + dblRealSerieValues.Length
                    + " elements" + "\n");
            str.Append("*" + "\n");

            str.Append("* U - Theil\t\t: " + formatNumber(Math.Sqrt(uTheilNum)
                    / Math.Sqrt(uTheilDen), denFormatConst) + "\n");
            str.Append("* Mean Abs Err\t: " + formatNumber((maeSum / errCount), denFormatConst) + "\n");
            str.Append("* MAPE\t\t: " + formatNumber((mapeSum / errCount), denFormatConst) + "\n");
            str.Append("* MSE\t\t: " + formatNumber((mseSum / errCount), denFormatConst) + "\n");
            str.Append("* RMSE\t\t: " + formatNumber(Math.Sqrt((mseSum / errCount)), denFormatConst) + "\n");
            str.Append("*" + "\n");
            str.Append("***** END FORECAST STATUS *****" + "\n");

            // Fim do Resumo da previsão
            textResult.Append(str.ToString());
            if (listSerieValues)
                textResult.Append("\n" + strValores.ToString());

            return textResult.ToString();
        }

        private static double bigger(double[] v)
        {
            double ret = 0;

            for (int i = 0; i < v.Length; i++)
            {

                if (v[i] > ret)
                    ret = v[i];

            }

            return ret;
        }

        private static String formatNumber(double v, int d)
        {
            String mask = "0.";
            for (int i = 0; i < d; i++)
                mask = mask + "0";

            return string.Format(CultureInfo.InvariantCulture, "{0:" + mask + "}", v);
        }

        //	private static double round(double v, int d){
        //	
        //		 return Math.round((v*(double)Math.Pow(10, d)))/(double)Math.Pow(10, d);
        //	}


        private static double normalize(double v)
        {
            double ret = 1;

            if (v >= 1000000000)
                ret = 10000000000.0;
            else if (v >= 100000000)
                ret = 1000000000;
            else if (v >= 10000000)
                ret = 100000000;
            else if (v >= 1000000)
                ret = 10000000;
            else if (v >= 100000)
                ret = 1000000;
            else if (v >= 10000)
                ret = 100000;
            else if (v < 10000 && v >= 1000)
                ret = 10000;
            else if (v < 1000 && v >= 100)
                ret = 1000;
            else if (v < 100 && v >= 10)
                ret = 100;
            else if (v < 10 && v >= 1)
                ret = 10;
            else if (v < 1 && v >= 0.1)
                ret = 1;
            else if (v < 0.1 && v >= 0.01)
                ret = 0.1;

            return ret;
        }

        public static double fixNumber(double d)
        {
            double ret = 0;

            if (Double.IsInfinity(d) || Double.IsNaN(d))
                ret = 0;
            else
                ret = d;

            return ret;

        }

    }
}