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

    public class Momentum : PluginAbstract {

	public int period = 10;

	double[] retMOMENTUM;

	

	public Momentum() {
		initialSetup();
	}

	public Momentum(Series series) {
		initialSetup();
		this.series = series;
	}

	private void initialSetup() {
		isConfigurable = true;
		isSimilarToSeries = false;
		chartType = chartTypes.CONTINUOUS;

		pluginName = "MOM";
		pluginDescription = "Momentum";
	}


	public double[] momentum(int n, Series series) {

		double[] retMom = new double[series.getClosePrice().Length];

		for (int x = 0; x < series.getClosePrice().Length; x++) {
			int init = 0;

			if (x >= n)
				init = x - n;

			retMom[x] = series.getClosePrice()[x]
					- series.getClosePrice()[init];
		}
		return retMom;
	}
	
	public override List<String> GetResultDescription() {

		List<String> results = new List<String>();

		results.Add("Momentum");

		return results;
	}

	public override List<double []> GetResultValues() {

		List<double []> results = new List<double []>();

		retMOMENTUM = momentum(period, series);
		results.Add(retMOMENTUM);

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

		double[] mom = retMOMENTUM;

		for (int i = 0; i < series.getClosePrice().Length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (mom[i] > 0 && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ series.getClosePrice()[i];

				lastMessage = sCompra;
				lastCompra = sCompra;
				Console.WriteLine(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (mom[i] < 0 && buy) {
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
		Console.WriteLine("MOM\tResultado\t"
				+ StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			Console.WriteLine("MOM\tResultado\tLUCRO\n");
		else
			Console.WriteLine("MOM\tResultado\tPREJUIZO\n");

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