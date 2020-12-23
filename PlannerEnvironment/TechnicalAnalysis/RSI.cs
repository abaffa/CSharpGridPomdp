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

    public class RSI  : PluginAbstract{

	public int shortPeriod = 14;
	public int shortSkipdays = 0;

	public int longPeriod = 27;
	public int longSkipdays = 0;

	double[] retShortRSI;
	double[] retLongRSI;

	public RSI() {
		initialSetup();
	}

	public RSI(Series series) {
		initialSetup();
		this.series = series;
	}

	private void initialSetup() {
		isConfigurable = true;
		isSimilarToSeries = false;
		chartType = chartTypes.CONTINUOUS;

		pluginName = "RSI";
		pluginDescription = "Relative Strength Index";
	}

	// OLD
	// public double[] rsi(int period, Series series, int skipdays) {
	//
	// //
	// // Console.WriteLine("RSI(" + period + ") for "
	// // + series.getClosePrice().Length + " skipd:" + skipdays);
	//
	// double[] retValue = new double[series.getClosePrice().Length];
	//
	// retValue[0] = 0;
	// for (int counter = 1; counter < series.getClosePrice().Length; counter++)
	// {
	//
	// // double value = 0.0;
	// int per = period;
	// if (counter < period)
	// per = counter;
	//
	// double U = 0.0;
	// double D = 0.0;
	//
	// for (int i = counter - per; i < counter; i++) {
	// // debugPrint("i: "+i);
	// double change = series.getClosePrice()[i + 1]
	// - series.getClosePrice()[i];
	//
	// if (change > 0) {
	// U += change;
	// } else {
	// D += Math.Abs(change);
	// }
	//
	// }
	//
	// try {
	//
	// retValue[counter] = 100 - (100 / (1 + (U / D)));
	// } catch (Exception e) {
	// }
	// //
	// // Console.WriteLine(counter + " # "
	// // + series.getClosePrice()[counter - 1] + " - "
	// // + retValue[counter - 1]);
	//
	// }
	//
	// return retValue;
	// }

	public double[] rsi(int period, Series series, int skipdays) {

		//		
		// Console.WriteLine("RSI(" + period + ") for "
		// + series.getClosePrice().Length + " skipd:" + skipdays);


		double[] U = new double[series.getClosePrice().Length];
		double[] D = new double[series.getClosePrice().Length];

		double[] RS = new double[series.getClosePrice().Length];
		double[] RSI = new double[series.getClosePrice().Length];

		U[0] = 0;
		D[0] = 0;

		RS[0] = 0;
		RSI[0] = 0;

		for (int i = 1; i < series.getClosePrice().Length; i++) {
			double yesterday = series.getClosePrice()[i - 1];
			double today = series.getClosePrice()[i];

			if (today > yesterday)
				U[i] = today - yesterday;
			else
				U[i] = 0;

			if (today < yesterday)
				D[i] = yesterday - today;
			else
				D[i] = 0;

//			Console.WriteLine(series.getDate()[i] + "\t" + U[i]+ "\t" + D[i]);
		}

		double[] U_EMA = ema(period, U, skipdays);
		double[] D_EMA = ema(period, D, skipdays);

		for (int i = 1; i < series.getClosePrice().Length; i++) {
			RS[i] = U_EMA[i] / D_EMA[i];

			RSI[i] = 100 - (100 * (1 / (1 + RS[i])));
			
//			Console.WriteLine(series.getDate()[i] + "\t" + U_EMA[i] + "\t" + D_EMA[i]);
//			Console.WriteLine(series.getDate()[i] + "\t" + RS[i] + "\t" + RSI[i]);
		}

		return RSI;
	}

	public double[] ema(int n, double[] series, int skipdays) {

		// Console.WriteLine("EMA(" + n + ") for " + series.Length + " skipd:"
		// + skipdays);

		double[] retValue = new double[series.Length];

		double exponent = 2 / (double) (n + 1);

		retValue[0] = series[0];// * (1- exponent);

		for (int counter = 1; counter < series.Length; counter++) {

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
	
	public override List<String> GetResultDescription() {

		List<String> results = new List<String>();

		results.Add("Short RSI");

		results.Add("Long RSI");

		return results;
	}

	public override List<double []> GetResultValues() {

		List<double []> results = new List<double []>();

		retShortRSI = rsi(shortPeriod, series, shortSkipdays);
		results.Add(retShortRSI);

		retLongRSI = rsi(longPeriod, series, longSkipdays);
		results.Add(retLongRSI);

		return results;
	}

	public String[] calculateBuySellMethod1() {

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
		double[] shortRsi = retShortRSI;
		double[] longRsi = retLongRSI;

		for (int i = 0; i < series.getClosePrice().Length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (shortRsi[i] > longRsi[i] && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ series.getClosePrice()[i];

				lastMessage = sCompra;
				lastCompra = sCompra;
				Console.WriteLine(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (shortRsi[i] < longRsi[i] && buy) {
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

		Console.WriteLine("RSI1\tResultado\t"
				+ StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			Console.WriteLine("RSI1\tResultado\tLUCRO\n");
		else
			Console.WriteLine("RSI1\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public String[] calculateBuySellMethod2() {

		String[] ret = new String[series.getClosePrice().Length];

		bool buy = false;

		double compra = 0;
		double totalCompra = 0;
		double totalVenda = 0;

		double percTotal = 0;

		
		String lastVenda = "";
		
		String lastCompra = "";
		
		String lastMessage = "";

		double[] shortRsi = retShortRSI;

		for (int i = 0; i < series.getClosePrice().Length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (shortRsi[i] > 70 && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ series.getClosePrice()[i];

				lastMessage = sCompra;
				lastCompra = sCompra;
				Console.WriteLine(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (shortRsi[i] < 30 && buy) {
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

		Console.WriteLine("RSI2\tResultado\t"
				+ StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			Console.WriteLine("RSI2\tResultado\tLUCRO\n");
		else
			Console.WriteLine("RSI2\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public override List<String> GetBuySellDescription() {
		List<String> results = new List<String>();

		results.Add("Short X Long");

		results.Add("RSI X Frontier");

		return results;
	}

	public override List<String []> GetBuySellValues() {
		List<String []> results = new List<String []>();

		results.Add(calculateBuySellMethod1());

		results.Add(calculateBuySellMethod2());

		return results;
	}

	}

}