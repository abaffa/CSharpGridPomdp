package org.baffa.traderwhatever.plugins;

import org.baffa.business.*;
import java.util.ArrayList;


public class PivotResistance extends PluginAbstract {

	public PivotResistance() {
		initialSetup();
	}

	public PivotResistance(Series series) {
		initialSetup();
		this.series = series;
	}

	public void initialSetup() {
		isConfigurable = false;
		isSimilarToSeries = true;
		chartType = chartTypes.DOTTY;
		
		pluginName = "Pivots and Resistances";
		pluginDescription = "...";
	}

	public ArrayList<String> getResultDescription() {

		ArrayList<String> results = new ArrayList<String>();

		results.add("Pivot");
		results.add("Resistance");

		return results;
	}

	public ArrayList<double []> getResultValues() {

		ArrayList<double []> results = new ArrayList<double []>();

		double[] r3 = new double[series.getLowerPrice().length];
		double[] p3 = new double[series.getLowerPrice().length];

		@SuppressWarnings("unused")
		boolean comprado = false;
		@SuppressWarnings("unused")
		double acum = 0;
		@SuppressWarnings("unused")
		double lcc = 0;
		@SuppressWarnings("unused")
		double pacum = 0;

		for (int i = 0; i < series.getLowerPrice().length; i++) {

			double CC = series.getClosePrice()[i];
			double H = series.getHigherPrice()[i];
			double L = series.getLowerPrice()[i];
			double PP = (H + L + CC) / 3;

			if (i == 0) {
				r3[i] = H + 2 * (PP - L);
				p3[i] = L - 2 * (H - PP);
			} else {
				if (CC > r3[i - 1] || CC < p3[i - 1])
					r3[i] = H + 2 * (PP - L);
				else
					r3[i] = r3[i - 1];

				if (CC > r3[i - 1] || CC < p3[i - 1])
					p3[i] = L - 2 * (H - PP);
				else
					p3[i] = p3[i - 1];

			}
		}

		results.add(p3);
		results.add(r3);

		return results;
	}

	public ArrayList<String> getBuySellDescription() {
		ArrayList<String> results = new ArrayList<String>();

		results.add("Pivot x Resistance");

		return results;
	}

	public ArrayList<String []> getBuySellValues() {
		ArrayList<String []> results = new ArrayList<String []>();

		
		String[] ret = new String[series.getClosePrice().length];
		
		double[] r3 = new double[series.getLowerPrice().length];
		double[] s3 = new double[series.getLowerPrice().length];

		boolean comprado = false;
		double acum = 0;
		double lcc = 0;
		double pacum = 0;

		for (int i = 0; i < series.getLowerPrice().length; i++) {

			double CC = series.getClosePrice()[i];
			double H = series.getHigherPrice()[i];
			double L = series.getLowerPrice()[i];
			double PP = (H + L + CC) / 3;

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";
			
			if (i == 0) {
				r3[i] = H + 2 * (PP - L);
				s3[i] = L - 2 * (H - PP);
			} else {
				if (CC > r3[i - 1] || CC < s3[i - 1])
					r3[i] = H + 2 * (PP - L);
				else
					r3[i] = r3[i - 1];

				if (CC > r3[i - 1] || CC < s3[i - 1])
					s3[i] = L - 2 * (H - PP);
				else
					s3[i] = s3[i - 1];

				if (CC >= r3[i - 1] && !comprado) {
					lcc = CC;
					acum = acum - lcc;
					comprado = true;
					
					ret[i] = "C";
					System.out.println("C" + "\t" + series.getDate()[i] + "\t"
							+ (-CC));

				} else if (CC <= s3[i - 1] && comprado) {
					acum = acum + CC;
					pacum = pacum + (((CC / lcc) - 1) * 100);
					comprado = false;
					
					ret[i] = "V";
					System.out.println("V" + "\t" + series.getDate()[i] + "\t"
							+ CC);

				}

			}
		}
		if (comprado)
			acum = acum + lcc;

		System.out.println("Lucro =" + acum);
		System.out.println("LucrP =" + pacum + "%");

		results.add(ret);

		return results;
	}

	public void showConfig() {
	}

}