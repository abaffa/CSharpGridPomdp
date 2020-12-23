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
    public class MAO  :PluginAbstract{

	public int emaShortPeriod = 3;
	public int emaLongPeriod = 13;
	public int smaPeriod = 3;

	public int skipdays = 0;

	double[] retMAO;


	public MAO() {
		initialSetup();
	}

	public MAO(Series series) {
		initialSetup();
		this.series = series;
	}

	private void initialSetup() {
		isConfigurable = true;
		isSimilarToSeries = false;
		chartType = chartTypes.CONTINUOUS;

		pluginName = "MAO";
		pluginDescription = "Moving Average Oscillator";
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
			value /= (double) per;
			retValue[counter - 1] = value;

			// Console.WriteLine(counter + " # " + series[counter - 1] + " - "
			// + retValue[counter - 1]);

		}

		return retValue;
	}

	public double[] ema(int n, Series series, int skipdays) {

		return ema(n, series.getClosePrice(), skipdays);
	}

	public double[] ema(int n, double[] series, int skipdays) {

		// Console.WriteLine("EMA(" + n + ") for " + series.Length + " skipd:"
		// + skipdays);

		double[] retValue = new double[series.Length];

		double exponent = 2 / (double) (n + 1);

		retValue[0] = series[0];// * (1- exponent);

		for (int counter = 1; counter < series.Length; counter++) {

			double value = 0;

			value = ((series[counter] - retValue[counter - 1]) * exponent)
					+ retValue[counter - 1];
			// Console.WriteLine("Value:"+value);

			retValue[counter] = value;
		}

		return retValue;
	}

	public double[] mao(Series series, int skipdays) {

		double[] retMAO = new double[series.getClosePrice().Length];

		double[] ema1 = ema(3, series, skipdays);
		double[] ema2 = ema(13, series, skipdays);

		for (int i = 0; i < retMAO.Length; i++) {
			retMAO[i] = ema1[i] - ema2[i];
		}

		retMAO = sma(3, retMAO, skipdays);

		return retMAO;
	}

	public override List<String> GetResultDescription() {

		List<String> results = new List<String>();

		results.Add("MAO");

		return results;
	}

	public override List<double []> GetResultValues() {

		List<double []> results = new List<double []>();

		retMAO = mao(series, skipdays);
		results.Add(retMAO);

		return results;
	}

	public String[] calculateBuySellMethod() {

		String[] ret = new String[series.getClosePrice().Length];

		bool buy = false;

		double compra = 0;
		double totalCompra = 0;
		double totalVenda = 0;

		double percTotal = 0;

		
		String lastVenda = "";
		
		String lastCompra = "";
		
		String lastMessage = "";

		double[] mao = retMAO;

		ret[0] = "-";
		for (int i = 1; i < series.getClosePrice().Length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (mao[i] >= 0 && mao[i - 1] < 0 && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ compra;

				lastMessage = sCompra;
				lastCompra = sCompra;
				Console.WriteLine(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (mao[i] < 0 && mao[i - 1] >= 0 && buy) {
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
		Console.WriteLine("MAO\tResultado\t"
				+ StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			Console.WriteLine("MAO\tResultado\tLUCRO\n");
		else
			Console.WriteLine("MAO\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public override List<String> GetBuySellDescription() {
		List<String> results = new List<String>();

		results.Add("MAO");

		return results;
	}

	public override List<String []> GetBuySellValues() {
		List<String []> results = new List<String []>();

		results.Add(calculateBuySellMethod());

		return results;
	}

	}

}