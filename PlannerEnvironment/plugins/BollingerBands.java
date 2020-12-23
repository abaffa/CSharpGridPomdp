package org.baffa.traderwhatever.plugins;

import org.baffa.business.*;
import org.baffa.utils.GBC;

import java.awt.BorderLayout;
import java.awt.Container;
import java.awt.Dimension;
import java.awt.GridBagLayout;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.ArrayList;

import javax.swing.*;
import javax.swing.border.Border;
import javax.swing.border.TitledBorder;

public class BollingerBands extends PluginAbstract implements ActionListener {

	private int period = 20;
	private int stdDevNumber = 2;
	private int skipdays = 0;

	double[] middleBand;
	double[] upperBand;
	double[] lowerBand;
	       
	double[] retMiddleBand;
	double[] retUpperBand;
	double[] retLowerBand;

	JSpinner spPeriod;
	JSpinner spStdDev;
	JSpinner spSkipdays;

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

		// System.out.println("SMA(" + period + ") for " + series.length
		// + " skipd:" + skipdays);

		double[] retValue = new double[series.length];

		for (int counter = 1; counter <= series.length; counter++) {

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

			// System.out.println(counter + " # " + series[counter - 1] + " - "
			// + retValue[counter - 1]);

		}

		return retValue;
	}


	public double[] lowerB(int n, int stdDevNum, double[] middleB, Series series, int skipdays) {

		return lowerB(n, stdDevNum, middleB,series.getClosePrice(), skipdays);
	}

	public double[] lowerB(int period, int stdDevNum, double[] middleB, double[] series, int skipdays) {

		// System.out.println("SMA(" + period + ") for " + series.length
		// + " skipd:" + skipdays);

		double[] retValue = new double[series.length];

		for (int counter = 1; counter <= series.length; counter++) {

			double value = 0.0;
			int per = period;
			if (counter < period)
				per = counter;

			for (int i = counter - per; i < counter; i++) {
				// debugPrint("i: "+i);

				value += Math.pow((series[i] - middleB[i]), 2);
			}
			value /= (double) per;
			
			retValue[counter - 1] = middleB[counter - 1] - (stdDevNum * Math.sqrt(value));

			// System.out.println(counter + " # " + series[counter - 1] + " - "
			// + retValue[counter - 1]);

		}

		return retValue;
	}

	public double[] upperB(int n, int stdDevNum, double[] middleB, Series series, int skipdays) {

		return upperB(n, stdDevNum, middleB,series.getClosePrice(), skipdays);
	}

	public double[] upperB(int period, int stdDevNum, double[] middleB, double[] series, int skipdays) {

		// System.out.println("SMA(" + period + ") for " + series.length
		// + " skipd:" + skipdays);

		double[] retValue = new double[series.length];

		for (int counter = 1; counter <= series.length; counter++) {

			double value = 0.0;
			int per = period;
			if (counter < period)
				per = counter;

			for (int i = counter - per; i < counter; i++) {
				// debugPrint("i: "+i);

				value += Math.pow((series[i] - middleB[i]), 2);
			}
			value /= (double) per;
			
			retValue[counter - 1] = middleB[counter - 1] + (stdDevNum * Math.sqrt(value));

			// System.out.println(counter + " # " + series[counter - 1] + " - "
			// + retValue[counter - 1]);

		}

		return retValue;
	}




	
	public ArrayList<String> getResultDescription() {

		ArrayList<String> results = new ArrayList<String>();

		results.add("Middle Band");
		results.add("Lower Band");
		results.add("Upper Band");

		return results;
	}

	public ArrayList<double []> getResultValues() {

		ArrayList<double []> results = new ArrayList<double []>();

		middleBand = sma(period, series, skipdays);
		
		lowerBand = lowerB(period, stdDevNumber, middleBand, series, skipdays);
		upperBand = upperB(period, stdDevNumber, middleBand, series, skipdays);


		retMiddleBand = middleBand;
		retLowerBand = lowerBand;
		retUpperBand = upperBand;

		results.add(retMiddleBand);
		results.add(retLowerBand);
		results.add(retUpperBand);

		return results;
	}

	public String[] calculateBuySellMethod1() {

		String[] ret = new String[series.getClosePrice().length];
		//
		// boolean buy = false;
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
		// for (int i = 0; i < series.getClosePrice().length; i++) {
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
		// System.out.println(sCompra);
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
		// + org.baffa.StatisticLib.percFormat(perc, 2);
		//
		// lastMessage = sVenda;
		// lastVenda = sVenda;
		// System.out.println(sVenda);
		//
		// buy = false;
		// ret[i] = "V";
		// }
		//
		// }
		//
		// System.out.println("MACD1\tResultado\t"
		// + org.baffa.StatisticLib.percFormat(percTotal, 2));
		//
		// if (percTotal > 0)
		// System.out.println("MACD1\tResultado\tLUCRO\n");
		// else
		// System.out.println("MACD1\tResultado\tPREJUIZO\n");
		//
		// // TotalTotal = TotalTotal + percTotal;
		//
		return ret;
	}

	public String[] calculateBuySellMethod2() {

		String[] ret = new String[series.getClosePrice().length];
		//
		// boolean buy = false;
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
		// for (int i = 0; i < series.getClosePrice().length; i++) {
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
		// System.out.println(sCompra);
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
		// + org.baffa.StatisticLib.percFormat(perc, 2);
		//
		// lastMessage = sVenda;
		// lastVenda = sVenda;
		// System.out.println(sVenda);
		//
		// buy = false;
		// ret[i] = "V";
		// }
		//
		// }
		//
		// System.out.println("MACD2\tResultado\t"
		// + org.baffa.StatisticLib.percFormat(percTotal, 2));
		//
		// if (percTotal > 0)
		// System.out.println("MACD2\tResultado\tLUCRO\n");
		// else
		// System.out.println("MACD2\tResultado\tPREJUIZO\n");
		//
		// // TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public String[] calculateBuySellMethod3() {

		String[] ret = new String[series.getClosePrice().length];
		//
		// boolean buy = false;
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
		// for (int i = 0; i < series.getClosePrice().length; i++) {
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
		// System.out.println(sCompra);
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
		// + org.baffa.StatisticLib.percFormat(perc, 2);
		//
		// lastMessage = sVenda;
		// lastVenda = sVenda;
		// System.out.println(sVenda);
		//
		// buy = false;
		// ret[i] = "V";
		// }
		//
		// }
		//
		// System.out.println("MACD3\tResultado\t"
		// + org.baffa.StatisticLib.percFormat(percTotal, 2));
		//
		// if (percTotal > 0)
		// System.out.println("MACD3\tResultado\tLUCRO\n");
		// else
		// System.out.println("MACD3\tResultado\tPREJUIZO\n");
		//
		// // TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public ArrayList<String> getBuySellDescription() {
		ArrayList<String> results = new ArrayList<String>();

		results.add("MACD X Signal");

		results.add("MACD");

		results.add("Histogram");

		return results;
	}

	public ArrayList<String []> getBuySellValues() {
		ArrayList<String []> results = new ArrayList<String []>();

		results.add(calculateBuySellMethod1());

		results.add(calculateBuySellMethod2());

		results.add(calculateBuySellMethod3());

		return results;
	}

	public void showConfig() {
		isCanceled = false;

		confFrame = new JDialog();
		confFrame.setName("Bollinger Bands Configuration");
		confFrame.setTitle("Bollinger Bands Configuration");
		confFrame
				.setDefaultCloseOperation(javax.swing.WindowConstants.DISPOSE_ON_CLOSE);
		confFrame.setModal(true);

		confFrame.setPreferredSize(new Dimension(200, 190));
		confFrame.setLayout(new BorderLayout());

		JPanel pane = new JPanel(new GridBagLayout());

		confFrame.setLocationRelativeTo(null);

		pane.add(getADXContainer(), new GBC(0, 0, 1, 1));
		pane.add(getButtons(), new GBC(0, 1, 1, 1));

		confFrame.add(pane, BorderLayout.CENTER);
		confFrame.pack();
		confFrame.setVisible(true);
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
		// ArrayList d = s.getResultValues();
		// double[] x = (double[]) d.get(0);
		// for (int i = 0; i < x.length; i++)
		// System.out.println(x[i]);
		//		
		// } catch (Exception e) {
		// e.printStackTrace();
		// }
	}

	// ////////////////////////////////////////////
	// /CONFIG LAYOUT
	// ////////////////////////////////////////////

	Container getButtons() {

		// //////////////
		Container container1 = new Container();
		container1.setLayout(new BorderLayout());

		Dimension cDimension = new Dimension(180, 20);
		container1.setPreferredSize(cDimension);
		container1.setMinimumSize(cDimension);

		JButton btnCancel = new JButton("Cancel");
		btnCancel.addActionListener(this);

		JButton btnOk = new JButton("OK");
		btnOk.addActionListener(this);

		container1.add(btnCancel, BorderLayout.WEST);
		container1.add(btnOk, BorderLayout.EAST);

		container1 = createFrame(container1, "");

		return container1;
	}

	public void actionPerformed(ActionEvent e) {

		if (e.getSource() instanceof JButton) {
			if (((JButton) e.getSource()).getText() == "Cancel") {

				cancel();
				closeConfFrame();
			} else if (((JButton) e.getSource()).getText() == "OK") {

				period = ((Integer) spPeriod.getValue()).intValue();
				skipdays = ((Integer) spSkipdays.getValue()).intValue();

				closeConfFrame();
			}
		}
	}

	Container getADXContainer() {
		Dimension lDimension = new Dimension(120, 20);
		Dimension spDimension = new Dimension(40, 20);
		Dimension cDimension = new Dimension(180, 100);

		// //////////////
		Container container1 = new Container();
		container1.setLayout(new GridBagLayout());
		container1.setPreferredSize(cDimension);
		container1.setMinimumSize(cDimension);


		JLabel lPeriod = new JLabel();
		lPeriod.setMinimumSize(lDimension);
		lPeriod.setPreferredSize(lDimension);
		lPeriod.setText("Period: ");

		JLabel lStdDevN = new JLabel();
		lStdDevN.setMinimumSize(lDimension);
		lStdDevN.setPreferredSize(lDimension);
		lStdDevN.setText("Std Dev: ");
		
		JLabel lSkipdays = new JLabel();
		lSkipdays.setMinimumSize(lDimension);
		lSkipdays.setPreferredSize(lDimension);
		lSkipdays.setText("Skip Days: ");

		container1.add(lPeriod, new GBC(0, 0, 1, 1));
		container1.add(lStdDevN, new GBC(0, 1, 1, 1));
		container1.add(lSkipdays, new GBC(0, 2, 1, 1));

		spPeriod = new JSpinner(new SpinnerNumberModel(period, // initial
				// value
				5, // min
				360, // max
				1));
		spPeriod.setMinimumSize(spDimension);
		spPeriod.setPreferredSize(spDimension);

		
		spStdDev = new JSpinner(new SpinnerNumberModel(stdDevNumber, // initial
				// value
				1, // min
				10, // max
				1));
		spStdDev.setMinimumSize(spDimension);
		spStdDev.setPreferredSize(spDimension);
		
		spSkipdays = new JSpinner(new SpinnerNumberModel(skipdays, // initial
				// value
				0, // min
				270, // max
				1));
		spSkipdays.setMinimumSize(spDimension);
		spSkipdays.setPreferredSize(spDimension);

		container1.add(spPeriod, new GBC(1, 0, 1, 1));
		container1.add(spStdDev, new GBC(1, 1, 1, 1));
		container1.add(spSkipdays, new GBC(1, 2, 1, 1));

		container1 = createFrame(container1, "Bollinger Bands Settings");

		return container1;
	}

	Container createFrame(Container container, String title) {

		Border blackline;//, raisedetched, loweredetched, raisedbevel, loweredbevel, empty;

		blackline = BorderFactory.createLineBorder(java.awt.Color.black);
//		raisedetched = BorderFactory
//				.createEtchedBorder(javax.swing.border.EtchedBorder.RAISED);
//		loweredetched = BorderFactory
//				.createEtchedBorder(javax.swing.border.EtchedBorder.LOWERED);
//		raisedbevel = BorderFactory.createRaisedBevelBorder();
//		loweredbevel = BorderFactory.createLoweredBevelBorder();

		TitledBorder titled = BorderFactory.createTitledBorder(blackline,
				title, TitledBorder.CENTER, TitledBorder.BELOW_BOTTOM);

		return addCompForTitledBorder(titled, container, TitledBorder.LEFT,
				TitledBorder.DEFAULT_POSITION);
	}

	Container addCompForTitledBorder(TitledBorder border, Container c,
			int justification, int position) {
		border.setTitleJustification(justification);
		border.setTitlePosition(position);
		return addCompForBorder(border, c);
	}

	Container addCompForBorder(Border border, Container c) {
		JPanel panel = new JPanel(new GridLayout(1, 1), false);

		panel.add(c);
		panel.setBorder(border);

		Container container = new Container();
		container.setLayout(new BorderLayout());
		container.add(Box.createRigidArea(new Dimension(0, 10)));
		container.add(panel);
		return container;
	}

}