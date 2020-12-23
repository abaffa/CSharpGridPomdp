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

public class MACD extends PluginAbstract implements ActionListener {

	int shortPeriod = 12;
	int longPeriod = 26;
	int smoothingPeriod = 9;

	int skipdays = 0;

	double[] retMacd;
	double[] retSignal;
	double[] retHistogram;

	JSpinner spShortPeriod;
	JSpinner spLongPeriod;
	JSpinner spSmoothingPeriod;

	JSpinner spSkipdays;

	public MACD() {
		initialSetup();
	}

	public MACD(Series series) {
		initialSetup();
		this.series = series;
	}

	private void initialSetup() {
		isConfigurable = true;
		isSimilarToSeries = false;
		chartType = chartTypes.CONTINUOUS;

		pluginName = "MACD";
		pluginDescription = "Moving Average Convergence-Divergence";
	}

	public double[] ema(int n, Series series, int skipdays) {

		return ema(n, series.getClosePrice(), skipdays);
	}

public double[] ema(int n, double[] series, int skipdays) {

		// System.out.println("EMA(" + n + ") for " + series.length + " skipd:"
		// + skipdays);

		double[] retValue = new double[series.length];

		double exponent = 2 / (double) (n + 1);

		retValue[0] = series[0];// * (1- exponent);

		for (int counter = 1; counter < series.length; counter++) {

			double value = 0;
			// old
			// value = ((series[counter] - retValue[counter - 1]) * exponent)
			// + retValue[counter - 1];

			// new
			value = ((series[counter] * exponent) + (retValue[counter - 1] * (1 - exponent)));

//			System.out.println("\t" + series[counter] + "\t" + value);
			
			// System.out.println("Value:"+value);

			retValue[counter] = value;
		}

		return retValue;
	}


	public double[][] macd(int p1, int p2, int signal, Series series,
			int skipdays) {
		double[][] retMACD = new double[3][series.getClosePrice().length];

		double[] ema1 = ema(p1, series, skipdays);
		double[] ema2 = ema(p2, series, skipdays);

		for (int i = 0; i < retMACD[0].length; i++) {
			retMACD[0][i] = ema1[i] - ema2[i];
			// System.out.println("#" + i + "#" + ema1[i] + " - " + ema2[i]
			// + " = " + retMACD[0][i]);
		}

		retMACD[1] = ema(signal, retMACD[0], skipdays);

		for (int i = 0; i < retMACD[0].length; i++) {
			retMACD[2][i] = retMACD[0][i] - retMACD[1][i];
			// System.out.println("#" + i + "#" + retMACD[0][i] + " - "
			// + retMACD[1][i] + " = " + retMACD[2][i]);

		}

		return retMACD;
	}

	public ArrayList<String> getResultDescription() {

		ArrayList<String> results = new ArrayList<String>();

		// results.add("MACD.OSC");
		// results.add("MACD.EMAOSC");
		// results.add("MACD.DIFF");

		results.add("MACD");
		results.add("Signal");
		results.add("Histogram");

		return results;
	}

	public ArrayList<double []> getResultValues() {

		ArrayList<double []> results = new ArrayList<double []>();

		double[][] values = macd(shortPeriod, longPeriod, smoothingPeriod,
				series, skipdays);

		retMacd = values[0];
		retSignal = values[1];
		retHistogram = values[2];

		results.add(retMacd);
		results.add(retSignal);
		results.add(retHistogram);

		return results;
	}

	public String[] calculateBuySellMethod1() {

		String[] ret = new String[series.getClosePrice().length];

		boolean buy = false;

		double compra = 0;
		double totalCompra = 0;
		double totalVenda = 0;

		double percTotal = 0;

		@SuppressWarnings("unused")
		String lastVenda = "";
		@SuppressWarnings("unused")
		String lastCompra = "";
		@SuppressWarnings("unused")
		String lastMessage = "";

		double[] macd = retMacd;
		double[] signal = retSignal;
		@SuppressWarnings("unused")
		double[] histogram = retHistogram;

		for (int i = 0; i < series.getClosePrice().length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (macd[i] > signal[i] && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ compra;

				lastMessage = sCompra;
				lastCompra = sCompra;
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (macd[i] < signal[i] && buy) {
				double venda = series.getClosePrice()[i];
				double perc = (venda / compra) - 1;
				percTotal = percTotal + perc;

				totalVenda = totalVenda + venda;

				String sVenda = "venda" + "\t" + series.getDate()[i] + "\t"
						+ venda + "\t"
						+ org.baffa.StatisticLib.percFormat(perc, 2);

				lastMessage = sVenda;
				lastVenda = sVenda;
				System.out.println(sVenda);

				buy = false;
				ret[i] = "V";
			}

		}

		System.out.println("MACD1\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("MACD1\tResultado\tLUCRO\n");
		else
			System.out.println("MACD1\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;

		return ret;
	}

	public String[] calculateBuySellMethod2() {

		String[] ret = new String[series.getClosePrice().length];

		boolean buy = false;

		double compra = 0;
		double totalCompra = 0;
		double totalVenda = 0;

		double percTotal = 0;

		@SuppressWarnings("unused")
		String lastVenda = "";
		@SuppressWarnings("unused")
		String lastCompra = "";
		@SuppressWarnings("unused")
		String lastMessage = "";

		double[] macd = retMacd;
		@SuppressWarnings("unused")
		double[] signal = retSignal;
		@SuppressWarnings("unused")
		double[] histogram = retHistogram;

		for (int i = 0; i < series.getClosePrice().length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (macd[i] > 0 && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ series.getClosePrice()[i];

				lastMessage = sCompra;
				lastCompra = sCompra;
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (macd[i] < 0 && buy) {
				double perc = (series.getClosePrice()[i] / compra) - 1;
				percTotal = percTotal + perc;

				totalVenda = totalVenda + series.getClosePrice()[i];

				String sVenda = "venda" + "\t" + series.getDate()[i] + "\t"
						+ series.getClosePrice()[i] + "\t"
						+ org.baffa.StatisticLib.percFormat(perc, 2);

				lastMessage = sVenda;
				lastVenda = sVenda;
				System.out.println(sVenda);

				buy = false;
				ret[i] = "V";
			}

		}

		System.out.println("MACD2\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("MACD2\tResultado\tLUCRO\n");
		else
			System.out.println("MACD2\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public String[] calculateBuySellMethod3() {

		String[] ret = new String[series.getClosePrice().length];

		boolean buy = false;

		double compra = 0;
		double totalCompra = 0;
		double totalVenda = 0;

		double percTotal = 0;

		@SuppressWarnings("unused")
		String lastVenda = "";
		@SuppressWarnings("unused")
		String lastCompra = "";
		@SuppressWarnings("unused")
		String lastMessage = "";

		@SuppressWarnings("unused")
		double[] macd = retMacd;
		@SuppressWarnings("unused")
		double[] signal = retSignal;
		double[] histogram = retHistogram;

		for (int i = 0; i < series.getClosePrice().length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (histogram[i] > 0 && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ series.getClosePrice()[i];

				lastMessage = sCompra;
				lastCompra = sCompra;
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (histogram[i] < 0 && buy) {
				double perc = (series.getClosePrice()[i] / compra) - 1;
				percTotal = percTotal + perc;

				totalVenda = totalVenda + series.getClosePrice()[i];

				String sVenda = "venda" + "\t" + series.getDate()[i] + "\t"
						+ series.getClosePrice()[i] + "\t"
						+ org.baffa.StatisticLib.percFormat(perc, 2);

				lastMessage = sVenda;
				lastVenda = sVenda;
				System.out.println(sVenda);

				buy = false;
				ret[i] = "V";
			}

		}

		System.out.println("MACD3\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("MACD3\tResultado\tLUCRO\n");
		else
			System.out.println("MACD3\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;
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
		confFrame.setName("MACD Configuration");
		confFrame.setTitle("MACD Configuration");
		confFrame
				.setDefaultCloseOperation(javax.swing.WindowConstants.DISPOSE_ON_CLOSE);
		confFrame.setModal(true);

		confFrame.setPreferredSize(new Dimension(200, 200));
		confFrame.setLayout(new BorderLayout());

		JPanel pane = new JPanel(new GridBagLayout());

		confFrame.setLocationRelativeTo(null);

		pane.add(getMACDContainer(), new GBC(0, 0, 1, 1));
		pane.add(getButtons(), new GBC(0, 1, 1, 1));

		confFrame.add(pane, BorderLayout.CENTER);
		confFrame.pack();
		confFrame.setVisible(true);
	}

	public static void main(String[] args) {

		MACD s = new MACD(new Series());
		s.showConfig();

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

				shortPeriod = ((Integer) spShortPeriod.getValue()).intValue();
				longPeriod = ((Integer) spLongPeriod.getValue()).intValue();
				smoothingPeriod = ((Integer) spSmoothingPeriod.getValue())
						.intValue();
				skipdays = ((Integer) spSkipdays.getValue()).intValue();

				closeConfFrame();
			}
		}
	}

	Container getMACDContainer() {
		Dimension lDimension = new Dimension(120, 20);
		Dimension spDimension = new Dimension(40, 20);
		Dimension cDimension = new Dimension(180, 100);

		// //////////////
		Container container1 = new Container();
		container1.setLayout(new GridBagLayout());
		container1.setPreferredSize(cDimension);
		container1.setMinimumSize(cDimension);

		JLabel lShortPeriod = new JLabel();
		lShortPeriod.setMinimumSize(lDimension);
		lShortPeriod.setPreferredSize(lDimension);
		lShortPeriod.setText("Short Period: ");

		JLabel lLongPeriod = new JLabel();
		lLongPeriod.setMinimumSize(lDimension);
		lLongPeriod.setPreferredSize(lDimension);
		lLongPeriod.setText("Long Period: ");

		JLabel lSmoothingPeriod = new JLabel();
		lSmoothingPeriod.setMinimumSize(lDimension);
		lSmoothingPeriod.setPreferredSize(lDimension);
		lSmoothingPeriod.setText("Smoothing Period: ");

		JLabel lSkipdays = new JLabel();
		lSkipdays.setMinimumSize(lDimension);
		lSkipdays.setPreferredSize(lDimension);
		lSkipdays.setText("Skip Days: ");

		container1.add(lShortPeriod, new GBC(0, 0, 1, 1));
		container1.add(lLongPeriod, new GBC(0, 1, 1, 1));
		container1.add(lSmoothingPeriod, new GBC(0, 2, 1, 1));
		container1.add(lSkipdays, new GBC(0, 3, 1, 1));

		spShortPeriod = new JSpinner(new SpinnerNumberModel(shortPeriod, // initial
				// value
				5, // min
				360, // max
				1));
		spShortPeriod.setMinimumSize(spDimension);
		spShortPeriod.setPreferredSize(spDimension);

		spLongPeriod = new JSpinner(new SpinnerNumberModel(longPeriod, // initial
				// value
				5, // min
				720, // max
				1));
		spLongPeriod.setMinimumSize(spDimension);
		spLongPeriod.setPreferredSize(spDimension);

		spSmoothingPeriod = new JSpinner(new SpinnerNumberModel(
				smoothingPeriod, // initial
				// value
				5, // min
				720, // max
				1));
		spSmoothingPeriod.setMinimumSize(spDimension);
		spSmoothingPeriod.setPreferredSize(spDimension);

		spSkipdays = new JSpinner(new SpinnerNumberModel(skipdays, // initial
				// value
				0, // min
				270, // max
				1));
		spSkipdays.setMinimumSize(spDimension);
		spSkipdays.setPreferredSize(spDimension);

		container1.add(spShortPeriod, new GBC(1, 0, 1, 1));
		container1.add(spLongPeriod, new GBC(1, 1, 1, 1));
		container1.add(spSmoothingPeriod, new GBC(1, 2, 1, 1));
		container1.add(spSkipdays, new GBC(1, 3, 1, 1));

		container1 = createFrame(container1, "MACD Settings");

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