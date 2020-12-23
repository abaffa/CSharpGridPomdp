using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraderWhatever.Business;

namespace Baffa.MathModels
{
    public class EvaluateMethods
    {


        

        public static double TotalTotal = 0;

        public static String[] sma1(Series series, Series series2)
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

            double[] shortSMA = FinancialLibrary.sma(10, series, 0);
            double[] longSMA = FinancialLibrary.sma(40, series, 0);

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (shortSMA[i] >= longSMA[i] && !buy)
                {
                    compra = series2.getClosePrice()[i];
                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + compra;

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (shortSMA[i] <= longSMA[i] && buy)
                {
                    double venda = series2.getClosePrice()[i];

                    double perc = (venda / compra) - 1;

                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] sma2(Series series, Series series2)
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

            // double[] shortSMA = FinancialLibrary.sma(10, series, 0);
            double[] longSMA = FinancialLibrary.sma(40, series, 0);

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (series.getClosePrice()[i] >= longSMA[i] && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + compra;

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (series.getClosePrice()[i] <= longSMA[i] && buy)
                {
                    double venda = series2.getClosePrice()[i];
                    double perc = (venda / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + venda;

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] sma3(Series series, Series series2)
        {

            String[] ret = new String[series.getClosePrice().Length];

            String[] ret1 = sma1(series, series2);
            String[] ret2 = sma2(series, series2);

            bool buy = false;

            double compra = 0;
            double totalCompra = 0;
            double totalVenda = 0;

            double percTotal = 0;

            


        String lastVenda = "";
            


        String lastCompra = "";
            


        String lastMessage = "";

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (ret1[i] == "C" && ret2[i] == "C" && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + compra;

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (ret1[i] == "V" && ret2[i] == "V" && buy)
                {
                    double venda = series2.getClosePrice()[i];
                    double perc = (venda / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + venda;

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] ema1(Series series, Series series2)
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

            double[] shortSMA = FinancialLibrary.ema(10, series, 0);
            double[] longSMA = FinancialLibrary.ema(40, series, 0);

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (shortSMA[i] >= longSMA[i] && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + compra;

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (shortSMA[i] <= longSMA[i] && buy)
                {
                    double venda = series2.getClosePrice()[i];
                    double perc = (venda / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + venda;

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] ema2(Series series, Series series2)
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

            // double[] shortSMA = FinancialLibrary.ema(10, series, 0);
            double[] longSMA = FinancialLibrary.ema(40, series, 0);

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (series.getClosePrice()[i] >= longSMA[i] && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + compra;

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (series.getClosePrice()[i] <= longSMA[i] && buy)
                {
                    double venda = series2.getClosePrice()[i];
                    double perc = (venda / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + venda;

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] ema3(Series series, Series series2)
        {

            String[] ret = new String[series.getClosePrice().Length];

            String[] ret1 = ema1(series, series2);
            String[] ret2 = ema2(series, series2);

            bool buy = false;

            double compra = 0;
            double totalCompra = 0;
            double totalVenda = 0;

            double percTotal = 0;

            


        String lastVenda = "";
            


        String lastCompra = "";
            


        String lastMessage = "";

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (ret1[i] == "C" && ret2[i] == "C" && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + compra;

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (ret1[i] == "V" && ret2[i] == "V" && buy)
                {
                    double venda = series2.getClosePrice()[i];
                    double perc = (venda / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + venda;

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] mao(Series series, Series series2)
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

            double[] mao = FinancialLibrary.mao(series, 0);

            ret[0] = "-";
            for (int i = 1; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (mao[i] >= 0 && mao[i - 1] < 0 && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + compra;

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (mao[i] < 0 && mao[i - 1] >= 0 && buy)
                {
                    double venda = series2.getClosePrice()[i];
                    double perc = (venda / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + venda;

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + venda + "\t"
                            + StatisticLib.percFormat(perc, 2);

                    lastMessage = sVenda;
                    lastVenda = sVenda;
                    Console.WriteLine(sVenda);

                    buy = false;
                    ret[i] = "V";
                }
            }
            Console.WriteLine("MAO\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("MAO\tResultado\tLUCRO\n");
		else
			Console.WriteLine("MAO\tResultado\tPREJUIZO\n");

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] macd1(Series series, Series series2)
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

            double[][] cMacd = FinancialLibrary.macd(12, 26, 9, series, 0);
            double[] macd = cMacd[0];
            double[] signal = cMacd[1];
            


        double[] histogram = cMacd[2];

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (macd[i] > signal[i] && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + compra;

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (macd[i] < signal[i] && buy)
                {
                    double venda = series2.getClosePrice()[i];
                    double perc = (venda / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + venda;

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] macd2(Series series, Series series2)
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

            double[][] cMacd = FinancialLibrary.macd(12, 26, 9, series, 0);
            double[] macd = cMacd[0];
            


        double[] signal = cMacd[1];
            


        double[] histogram = cMacd[2];

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (macd[i] > 0 && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (macd[i] < 0 && buy)
                {
                    double perc = (series2.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i] + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] macd3(Series series, Series series2)
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

            double[][] cMacd = FinancialLibrary.macd(12, 26, 9, series, 0);
            


        double[] macd = cMacd[0];
            


        double[] signal = cMacd[1];
            double[] histogram = cMacd[2];

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (histogram[i] > 0 && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (histogram[i] < 0 && buy)
                {
                    double perc = (series2.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i] + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] cci1(Series series, Series series2)
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

            double[] cci = FinancialLibrary.cci(20, 0.015, series, 0);

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (cci[i] > 0 && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (cci[i] < 0 && buy)
                {
                    double perc = (series2.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i] + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] cci2(Series series, Series series2)
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

            double[] cci = FinancialLibrary.cci(20, 0.015, series, 0);

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (cci[i] >= 100 && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (cci[i] <= -100 && buy)
                {
                    double perc = (series2.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i] + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] momentum(Series series, Series series2)
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

            double[] mom = FinancialLibrary.momentum(10, series);

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (mom[i] > 0 && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (mom[i] < 0 && buy)
                {
                    double perc = (series2.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i] + "\t"
                            + StatisticLib.percFormat(perc, 2);

                    lastMessage = sVenda;
                    lastVenda = sVenda;
                    Console.WriteLine(sVenda);

                    buy = false;
                    ret[i] = "V";
                }
            }
            Console.WriteLine("MOM\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("MOM\tResultado\tLUCRO\n");
		else
			Console.WriteLine("MOM\tResultado\tPREJUIZO\n");

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] roc(Series series, Series series2)
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

            double[] roc = FinancialLibrary.roc(10, series);

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (roc[i] > 0 && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (roc[i] < 0 && buy)
                {
                    double perc = (series2.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i] + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] rsi1(Series series, Series series2)
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

            // double[] shortRsi = org.baffa.utils.FinancialLibrary.rsi(5, series,
            // 0);
            // double[] longRsi = org.baffa.utils.FinancialLibrary.rsi(14, series,
            // 0);
            double[] shortRsi = FinancialLibrary.rsi(14, series, 0);
            double[] longRsi = FinancialLibrary.rsi(27, series, 0);

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (shortRsi[i] > longRsi[i] && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (shortRsi[i] < longRsi[i] && buy)
                {
                    double perc = (series2.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i] + "\t"
                            + StatisticLib.percFormat(perc, 2);

                    lastMessage = sVenda;
                    lastVenda = sVenda;
                    Console.WriteLine(sVenda);

                    buy = false;
                    ret[i] = "V";
                }

            }

            Console.WriteLine("RSI1\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("RSI1\tResultado\tLUCRO\n");
		else
			Console.WriteLine("RSI1\tResultado\tPREJUIZO\n");

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] rsi2(Series series, Series series2)
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

            // double[] shortRsi = org.baffa.utils.FinancialLibrary.rsi(5, series,
            // 0);
            // double[] longRsi = org.baffa.utils.FinancialLibrary.rsi(14, series,
            // 0);
            double[] shortRsi = FinancialLibrary.rsi(14, series, 0);
            // double[] longRsi = org.baffa.utils.FinancialLibrary.rsi(27, series,
            // 0);

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (shortRsi[i] > 70 && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (shortRsi[i] < 30 && buy)
                {
                    double perc = (series2.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i] + "\t"
                            + StatisticLib.percFormat(perc, 2);

                    lastMessage = sVenda;
                    lastVenda = sVenda;
                    Console.WriteLine(sVenda);

                    buy = false;
                    ret[i] = "V";
                }

            }

            Console.WriteLine("RSI2\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("RSI2\tResultado\tLUCRO\n");
		else
			Console.WriteLine("RSI2\tResultado\tPREJUIZO\n");

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] ost1(Series series, Series series2)
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

            double[][] cOst = FinancialLibrary.ost(14, 3, series, 0);
            double[] K_perc = cOst[0];
            double[] D_perc = cOst[1];

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (K_perc[i] > D_perc[i] && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (K_perc[i] < D_perc[i] && buy)
                {
                    double perc = (series2.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i] + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] ost2(Series series, Series series2)
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

            double[][] cOst = FinancialLibrary.ost(14, 3, series, 0);
            double[] K_perc = cOst[0];
            


        double[] D_perc = cOst[1];

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (K_perc[i] > 0.70 && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (K_perc[i] < 0.30 && buy)
                {
                    double perc = (series2.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i] + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] ost3(Series series, Series series2)
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

            double[][] cOst = FinancialLibrary.ost(14, 3, series, 0);
            


        double[] K_perc = cOst[0];
            double[] D_perc = cOst[1];

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (D_perc[i] > 0.70 && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (D_perc[i] < 0.30 && buy)
                {
                    double perc = (series2.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i] + "\t"
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

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }

        public static String[] sar(Series series, Series series2)
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

            double[] sar = FinancialLibrary.sar(0.02, 0.02, 0.20, series, 0);

            for (int i = 0; i < series2.getClosePrice().Length; i++)
            {

                if (i > 0)
                    ret[i] = ret[i - 1];
                else
                    ret[i] = "-";

                if (series.getClosePrice()[i] > sar[i] && !buy)
                {
                    compra = series2.getClosePrice()[i];

                    totalCompra = totalCompra + compra;

                    String sCompra = "compra" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i];

                    lastMessage = sCompra;
                    lastCompra = sCompra;
                    Console.WriteLine(sCompra);

                    buy = true;
                    ret[i] = "C";
                }
                else if (series.getClosePrice()[i] < sar[i] && buy)
                {
                    double perc = (series2.getClosePrice()[i] / compra) - 1;
                    percTotal = percTotal + perc;

                    totalVenda = totalVenda + series2.getClosePrice()[i];

                    String sVenda = "venda" + "\t" + series.getDate()[i].ToString("yyyy-MM-dd HH:mm:ss") + "\t"
                            + series2.getClosePrice()[i] + "\t"
                            + StatisticLib.percFormat(perc, 2);

                    lastMessage = sVenda;
                    lastVenda = sVenda;
                    Console.WriteLine(sVenda);

                    buy = false;
                    ret[i] = "V";
                }

            }

            Console.WriteLine("SAR\tResultado\t"
                    + StatisticLib.percFormat(percTotal, 2));

            if (percTotal > 0)
                Console.WriteLine("SAR\tResultado\tLUCRO\n");
		else
			Console.WriteLine("SAR\tResultado\tPREJUIZO\n");

            TotalTotal = TotalTotal + percTotal;
            return ret;
        }




        private static readonly HashSet<DateTime> Holidays = new HashSet<DateTime>();

        private static bool IsHoliday(DateTime date)
        {
            return Holidays.Contains(date);
        }

        private static bool IsWeekEnd(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday
                || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private static DateTime nextWorkday(DateTime date)
        {

            do
            {
                date = date.AddDays(1);
            } while (IsHoliday(date) || IsWeekEnd(date));

            return date;
        }

    }
}
