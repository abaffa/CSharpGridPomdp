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

public class RSI extends PluginAbstract implements ActionListener {

	public int shortPeriod = 14;
	public int shortSkipdays = 0;

	public int longPeriod = 27;
	public int longSkipdays = 0;

	double[] retShortRSI;
	double[] retLongRSI;

	JSpinner spShortPeriod;
	JSpinner spShortSkipdays;

	JSpinner spLongPeriod;
	JSpinner spLongSkipdays;

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
	// // System.out.println("RSI(" + period + ") for "
	// // + series.getClosePrice().length + " skipd:" + skipdays);
	//
	// double[] retValue = new double[series.getClosePrice().length];
	//
	// retValue[0] = 0;
	// for (int counter = 1; counter < series.getClosePrice().length; counter++)
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
	// D += Math.abs(change);
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
	// // System.out.println(counter + " # "
	// // + series.getClosePrice()[counter - 1] + " - "
	// // + retValue[counter - 1]);
	//
	// }
	//
	// return retValue;
	// }

	public double[] rsi(int period, Series series, int skipdays) {

		//		
		// System.out.println("RSI(" + period + ") for "
		// + series.getClosePrice().length + " skipd:" + skipdays);


		double[] U = new double[series.getClosePrice().length];
		double[] D = new double[series.getClosePrice().length];

		double[] RS = new double[series.getClosePrice().length];
		double[] RSI = new double[series.getClosePrice().length];

		U[0] = 0;
		D[0] = 0;

		RS[0] = 0;
		RSI[0] = 0;

		for (int i = 1; i < series.getClosePrice().length; i++) {
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

//			System.out.println(series.getDate()[i] + "\t" + U[i]+ "\t" + D[i]);
		}

		double[] U_EMA = ema(period, U, skipdays);
		double[] D_EMA = ema(period, D, skipdays);

		for (int i = 1; i < series.getClosePrice().length; i++) {
			RS[i] = U_EMA[i] / D_EMA[i];

			RSI[i] = 100 - (100 * (1 / (1 + RS[i])));
			
//			System.out.println(series.getDate()[i] + "\t" + U_EMA[i] + "\t" + D_EMA[i]);
//			System.out.println(series.getDate()[i] + "\t" + RS[i] + "\t" + RSI[i]);
		}

		return RSI;
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
	
	public ArrayList<String> getResultDescription() {

		ArrayList<String> results = new ArrayList<String>();

		results.add("Short RSI");

		results.add("Long RSI");

		return results;
	}

	public ArrayList<double []> getResultValues() {

		ArrayList<double []> results = new ArrayList<double []>();

		retShortRSI = rsi(shortPeriod, series, shortSkipdays);
		results.add(retShortRSI);

		retLongRSI = rsi(longPeriod, series, longSkipdays);
		results.add(retLongRSI);

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

		// double[] shortRsi = org.baffa.utils.FinancialLibrary.rsi(5, series,
		// 0);
		// double[] longRsi = org.baffa.utils.FinancialLibrary.rsi(14, series,
		// 0);
		double[] shortRsi = retShortRSI;
		double[] longRsi = retLongRSI;

		for (int i = 0; i < series.getClosePrice().length; i++) {

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
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (shortRsi[i] < longRsi[i] && buy) {
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

		System.out.println("RSI1\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("RSI1\tResultado\tLUCRO\n");
		else
			System.out.println("RSI1\tResultado\tPREJUIZO\n");

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

		double[] shortRsi = retShortRSI;

		for (int i = 0; i < series.getClosePrice().length; i++) {

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
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (shortRsi[i] < 30 && buy) {
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

		System.out.println("RSI2\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("RSI2\tResultado\tLUCRO\n");
		else
			System.out.println("RSI2\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public ArrayList<String> getBuySellDescription() {
		ArrayList<String> results = new ArrayList<String>();

		results.add("Short X Long");

		results.add("RSI X Frontier");

		return results;
	}

	public ArrayList<String []> getBuySellValues() {
		ArrayList<String []> results = new ArrayList<String []>();

		results.add(calculateBuySellMethod1());

		results.add(calculateBuySellMethod2());

		return results;
	}

	public void showConfig() {
		isCanceled = false;

		confFrame = new JDialog();
		confFrame.setName("RSI Configuration");
		confFrame.setTitle("RSI Configuration");
		confFrame
				.setDefaultCloseOperation(javax.swing.WindowConstants.DISPOSE_ON_CLOSE);
		confFrame.setModal(true);

		confFrame.setPreferredSize(new Dimension(160, 240));
		confFrame.setLayout(new BorderLayout());

		JPanel pane = new JPanel(new GridBagLayout());

		confFrame.setLocationRelativeTo(null);

		pane.add(getShortContainer(), new GBC(0, 0, 1, 1));
		pane.add(getLongContainer(), new GBC(0, 1, 1, 1));
		pane.add(getButtons(), new GBC(0, 2, 1, 1));

		confFrame.add(pane, BorderLayout.CENTER);
		confFrame.pack();
		confFrame.setVisible(true);
	}

	public static void main(String[] args) {

		RSI s = new RSI(new Series());
		s.showConfig();

	}

	// ////////////////////////////////////////////
	// /CONFIG LAYOUT
	// ////////////////////////////////////////////

	Container getButtons() {

		// //////////////
		Container container1 = new Container();
		container1.setLayout(new BorderLayout());

		Dimension cDimension = new Dimension(140, 20);
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
				shortSkipdays = ((Integer) spShortSkipdays.getValue())
						.intValue();

				longPeriod = ((Integer) spLongPeriod.getValue()).intValue();
				longSkipdays = ((Integer) spLongSkipdays.getValue()).intValue();

				closeConfFrame();
			}
		}
	}

	Container getShortContainer() {
		Dimension lDimension = new Dimension(80, 20);
		Dimension spDimension = new Dimension(40, 20);
		Dimension cDimension = new Dimension(140, 60);

		// //////////////
		Container container1 = new Container();
		container1.setLayout(new GridBagLayout());
		container1.setPreferredSize(cDimension);
		container1.setMinimumSize(cDimension);

		JLabel lShortPeriod = new JLabel();
		lShortPeriod.setMinimumSize(lDimension);
		lShortPeriod.setPreferredSize(lDimension);
		lShortPeriod.setText("Period: ");

		JLabel lShortSkipdays = new JLabel();
		lShortSkipdays.setMinimumSize(lDimension);
		lShortSkipdays.setPreferredSize(lDimension);
		lShortSkipdays.setText("Skip Days: ");

		container1.add(lShortPeriod, new GBC(0, 0, 1, 1));
		container1.add(lShortSkipdays, new GBC(0, 1, 1, 1));

		spShortPeriod = new JSpinner(new SpinnerNumberModel(shortPeriod, // initial
				// value
				5, // min
				360, // max
				1));
		spShortPeriod.setMinimumSize(spDimension);
		spShortPeriod.setPreferredSize(spDimension);

		spShortSkipdays = new JSpinner(new SpinnerNumberModel(shortSkipdays, // initial
				// value
				0, // min
				270, // max
				1));
		spShortSkipdays.setMinimumSize(spDimension);
		spShortSkipdays.setPreferredSize(spDimension);

		container1.add(spShortPeriod, new GBC(1, 0, 1, 1));
		container1.add(spShortSkipdays, new GBC(1, 1, 1, 1));

		container1 = createFrame(container1, "Short RSI");

		return container1;
	}

	Container getLongContainer() {
		Dimension lDimension = new Dimension(80, 20);
		Dimension spDimension = new Dimension(40, 20);
		Dimension cDimension = new Dimension(140, 60);

		// //////////////
		Container container2 = new Container();
		container2.setLayout(new GridBagLayout());
		container2.setPreferredSize(cDimension);
		container2.setMinimumSize(cDimension);

		JLabel lLongPeriod = new JLabel();
		lLongPeriod.setMinimumSize(lDimension);
		lLongPeriod.setPreferredSize(lDimension);
		lLongPeriod.setText("Period: ");

		JLabel lLongSkipdays = new JLabel();
		lLongSkipdays.setMinimumSize(lDimension);
		lLongSkipdays.setPreferredSize(lDimension);
		lLongSkipdays.setText("Skip Days: ");

		container2.add(lLongPeriod, new GBC(0, 0, 1, 1));
		container2.add(lLongSkipdays, new GBC(0, 1, 1, 1));

		spLongPeriod = new JSpinner(new SpinnerNumberModel(longPeriod, // initial
				// value
				10, // min
				720, // max
				1));
		spLongPeriod.setMinimumSize(spDimension);
		spLongPeriod.setPreferredSize(spDimension);

		spLongSkipdays = new JSpinner(new SpinnerNumberModel(longSkipdays, // initial
				// value
				0, // min
				360, // max
				1));
		spLongSkipdays.setMinimumSize(spDimension);
		spLongSkipdays.setPreferredSize(spDimension);

		container2.add(spLongPeriod, new GBC(1, 0, 1, 1));
		container2.add(spLongSkipdays, new GBC(1, 1, 1, 1));

		container2 = createFrame(container2, "Long RSI");

		return container2;
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