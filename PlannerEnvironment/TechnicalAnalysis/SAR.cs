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
    public class SAR : PluginAbstract  {

	int period = 13;
	double sarIni = 0.02;
	double sarInterval = 0.02;
	double sarMax = 0.20;

	int skipdays = 0;

	double[] retSAR;

	public SAR() {
		initialSetup();
	}

	public SAR(Series series) {
		initialSetup();
		this.series = series;
	}

	private void initialSetup() {
		isConfigurable = true;
		isSimilarToSeries = true;
		chartType = chartTypes.DOTTY;

		pluginName = "SAR";
		pluginDescription = "Parabolic Sar";
	}

	public double[] sar(double initialValue, double af, double max,
			Series series, int skipdays) {

		double[] retSAR = new double[series.getClosePrice().Length];
		bool uptrend = true;

		double fator = initialValue;
		double lowerPrice = series.getClosePrice()[0];
		double upperPrice = series.getClosePrice()[0];
		double EP = upperPrice;

		retSAR[0] = EP;

		double trendMean = 0;

		for (int z = 1; z < series.getClosePrice().Length; z++) {

			fator = fator + af;

			if (fator > max)
				fator = max;

			trendMean = 0;

			int actualPeriod = (z - (period - 2));
			
			if(z < (period-1))
				actualPeriod = 1;
			
			for (int x = actualPeriod; x < z; x++)
				trendMean = trendMean
						+ ((series.getClosePrice()[x] / series.getClosePrice()[x - 1]) - 1);

			if (trendMean > 0)
				uptrend = true;
			else if (trendMean < 0)
				uptrend = false;

			if (uptrend) {

				if (lowerPrice > 0) {
					lowerPrice = 0;
					fator = initialValue;
				}

				if (upperPrice < series.getClosePrice()[z])
					upperPrice = series.getClosePrice()[z];
				else if (upperPrice == 0)
					upperPrice = series.getClosePrice()[z];

				EP = upperPrice;

			} else if (!uptrend) {

				if (upperPrice > 0) {
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

	public override List<String> GetResultDescription() {

		List<String> results = new List<String>();

		results.Add("SAR");

		return results;
	}

	public override List<double []> GetResultValues() {

		List<double []> results = new List<double []>();

		retSAR = sar(sarIni, sarInterval, sarMax, series, skipdays);
		results.Add(retSAR);

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

		double[] sar = retSAR;

		for (int i = 0; i < series.getClosePrice().Length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (series.getClosePrice()[i] > sar[i] && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ series.getClosePrice()[i];

				lastMessage = sCompra;
				lastCompra = sCompra;
				Console.WriteLine(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (series.getClosePrice()[i] < sar[i] && buy) {
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

		Console.WriteLine("SAR\tResultado\t"
				+ StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			Console.WriteLine("SAR\tResultado\tLUCRO\n");
		else
			Console.WriteLine("SAR\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public override List<String> GetBuySellDescription() {
		List<String> results = new List<String>();

		results.Add("Serie x SAR");

		return results;
	}

	public override List<String []> GetBuySellValues() {
		List<String []> results = new List<String []>();

		results.Add(calculateBuySellMethod());

		return results;
	}

	}

}