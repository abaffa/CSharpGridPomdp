/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathModels
{
    class Program
    {

        public static String datInicio = "2009-01-01";
        public static String datFim = "2009-12-31";

        public static int minTot = 100;

        public static String excludePapers = " and codneg not like \'TELB4\'"
            + " and codneg not like \'TELB3\'";

        public static String[] paperList;
        // = { "PETR4", "VALE5", "BBDC4", "USIM5", "VALE3", "PETR3",
        //		"ITAU4", "CSNA3", "UBBR11", "GGBR4", "IBOV" };// , "ITSA4",

        // "BBAS3", "TNLP4",
        // "ALLL11", "BRAP4", "CMIG4", "NETC4", "CESP6", "AMBV4", "TAMM4" };

        static void Main(string[] args)
        {
            / *
            //String[] paperList = getPaperList(datInicio, datFim);


            String[] paperList = { "^BVSP" };




            for (int z = 0; z < paperList.length; z++)
            {

                String paper = paperList[z];
                Console.WriteLine(paper);
                Console.WriteLine("////////////////////");

                Series series = new Series(paper, datInicio, datFim, false); ;  // this is the real 
                AT(series, series); // this is the real 



                //		Console.WriteLine("\nNEURAL NET");
                //		Console.WriteLine("NEURAL NET\n");
                //
                //		datInicio = "2006-07-01";
                //		datFim = "2007-01-01";
                //		Console.WriteLine("P1 REAL");
                //		series = new Series(paper, datInicio, datFim);
                //		AT(series,series);
                //		Console.WriteLine("P1 NN");
                //		AT(evaluate(paper,"P1"),series);
                //
                //		datInicio = "2007-01-01";
                //		datFim = "2007-06-01";
                //
                //		Console.WriteLine("P2 REAL");
                //		series = new Series(paper, datInicio, datFim);
                //		AT(series,series);
                //
                //		Console.WriteLine("P2 NN");
                //		AT(evaluate(paper,"P2"),series);
                //		
                //		datInicio = "2007-06-01";
                //		datFim = "2007-12-01";
                //		Console.WriteLine("P3 REAL");
                //		series = new Series(paper, datInicio, datFim);
                //		AT(series,series);
                //
                //		Console.WriteLine("P3 NN");
                //		AT(evaluate(paper,"P3"),series);
                //		
                //		datInicio = "2007-09-01";
                //		datFim = "2008-02-01";
                //		
                //		Console.WriteLine("P4 REAL");
                //		series = new Series(paper, datInicio, datFim);
                //		AT(series,series);
                //
                //		Console.WriteLine("P4 NN");
                //		AT(evaluate(paper,"P4"),series);
            }
            //		train("P1");
            //
            //		datInicio = "2006-07-01";
            //		datFim = "2007-01-01";
            //		train("P2");
            //
            //		datInicio = "2007-01-01";
            //		datFim = "2007-06-01";
            //		train("P3");
            //
            //		datInicio = "2007-03-01";
            //		datFim = "2007-09-01";
            //		train("P4");
            * /
        }

        / *
        public static void AT(Series series, Series series2)
        {
            EvaluateMethods.TotalTotal = 0;

            //		EvaluateMethods.sma3(series, series2);
            //		// ////////////////////////////////////////
            //		EvaluateMethods.ema3(series, series2);
            //		// ////////////////////////////////////////
            //		EvaluateMethods.mao(series, series2);
            //		// ////////////////////////////////////////
            EvaluateMethods.macd1(series, series2);
            EvaluateMethods.macd2(series, series2);
            EvaluateMethods.macd3(series, series2);
            //		// ////////////////////////////////////////
            //		EvaluateMethods.cci1(series, series2);
            //		EvaluateMethods.cci2(series, series2);
            //		// ////////////////////////////////////////
            //		EvaluateMethods.sar(series, series2);
            //		// ////////////////////////////////////////
            //		EvaluateMethods.momentum(series, series2);
            //		// ////////////////////////////////////////
            EvaluateMethods.roc(series, series2);
            // ////////////////////////////////////////
            EvaluateMethods.rsi1(series, series2);
            EvaluateMethods.rsi2(series, series2);
            // ////////////////////////////////////////
            EvaluateMethods.roc(series, series2);
            // ////////////////////////////////////////
            EvaluateMethods.ost1(series, series2);
            EvaluateMethods.ost2(series, series2);
            EvaluateMethods.ost3(series, series2);

            if (EvaluateMethods.TotalTotal > 0)
                Console.WriteLine("Total\t\t\t\tLUCRO\n");
		else
			Console.WriteLine("Total\t\t\t\tPREJUIZO\n");

            Console.WriteLine("///////////////////////////");

            // for (int x = 0; x < series.getClosePrice().length; x++)
            // Console.WriteLine(series.getClosePrice()[x] + "\t" +
            // +shortSMA[x]
            // + "\t" + +longSMA[x] + "\t" + +shortEMA[x] + "\t"
            // + +longEMA[x] + "\t" + +mao[x] + "\t" + +macd[x] + "\t"
            // + +signal[x] + "\t" + +histogram[x] + "\t" + +cci[x] + "\t"
            // + +sar[x] + "\t" + +mom[x] + "\t" + +roc[x] + "\t"
            // + +shortRsi[x] + "\t" + +longRsi[x] + "\t" + +K_perc[x]
            // + "\t" + D_perc[x]);
            //
        }
        * /
        
    }

}
*/
  