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

public class EMA extends PluginAbstract implements ActionListener {

	boolean useLongOnly = false;

	public int shortPeriod = 10;
	public int shortSkipdays = 0;

	public int longPeriod = 40;
	public int longSkipdays = 0;

	double[] retShortEMA;
	double[] retLongEMA;

	JSpinner spShortPeriod;
	JSpinner spShortSkipdays;

	JSpinner spLongPeriod;
	JSpinner spLongSkipdays;

	public EMA() {
		initialSetup();
	}

	public EMA(Series series) {
		initialSetup();
		this.series = series;
	}

	private void initialSetup(){
		isConfigurable = true;
		isSimilarToSeries = true;
		chartType = chartTypes.CONTINUOUS;
		
		pluginName = "EMA";
		pluginDescription = "Exponential Moving Average";
	}
	
	public double[] ema(int n, Series series, int skipdays) {

		return ema(n, series.getClosePrice(), skipdays);
	}

//old
//	public double[] ema(int n, double[] series, int skipdays) {
//
//		// System.out.println("EMA(" + n + ") for " + series.length + " skipd:"
//		// + skipdays);
//
//		double[] retValue = new double[series.length];
//
//		double exponent = 2 / (double) (n + 1);
//
//		retValue[0] = series[0];// * (1- exponent);
//
//		for (int counter = 1; counter < series.length; counter++) {
//
//			double value = 0;
//
//			value = ((series[counter] - retValue[counter - 1]) * exponent)
//					+ retValue[counter - 1];
//			// System.out.println("Value:"+value);
//
//			retValue[counter] = value;
//		}
//
//		return retValue;
//	}

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

		if (!useLongOnly) {
			results.add("Short EMA");
		}

		results.add("Long EMA");

		return results;
	}

	public ArrayList<double []> getResultValues() {

		ArrayList<double []> results = new ArrayList<double []>();

		if (!useLongOnly) {
			retShortEMA = ema(shortPeriod, series, shortSkipdays);
			results.add(retShortEMA);
		}

		retLongEMA = ema(longPeriod, series, longSkipdays);
		results.add(retLongEMA);

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

		for (int i = 0; i < series.getClosePrice().length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (retShortEMA[i] >= retLongEMA[i] && !buy) {
				compra = series.getClosePrice()[i];
				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ compra;

				lastMessage = sCompra;
				lastCompra = sCompra;
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (retShortEMA[i] <= retLongEMA[i] && buy) {
				double venda = series.getClosePrice()[i];

				double perc = (venda / compra) - 1;

				percTotal = percTotal + perc;

				totalVenda = totalVenda + series.getClosePrice()[i];

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
		System.out.println("EMA1\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("EMA1\tResultado\tLUCRO\n");
		else
			System.out.println("EMA1\tResultado\tPREJUIZO\n");

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

		for (int i = 0; i < series.getClosePrice().length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (series.getClosePrice()[i] >= retLongEMA[i] && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ compra;

				lastMessage = sCompra;
				lastCompra = sCompra;
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (series.getClosePrice()[i] <= retLongEMA[i] && buy) {
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
		System.out.println("EMA2\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("EMA2\tResultado\tLUCRO\n");
		else
			System.out.println("EMA2\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public String[] calculateBuySellMethod3() {

		String[] ret = new String[series.getClosePrice().length];

		String[] ret1 = calculateBuySellMethod1();
		String[] ret2 = calculateBuySellMethod2();

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

		for (int i = 0; i < series.getClosePrice().length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (ret1[i] == "C" && ret2[i] == "C" && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ compra;

				lastMessage = sCompra;
				lastCompra = sCompra;
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (ret1[i] == "V" && ret2[i] == "V" && buy) {
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
		System.out.println("EMA3\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("EMA3\tResultado\tLUCRO\n");
		else
			System.out.println("EMA3\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public ArrayList<String> getBuySellDescription() {
		ArrayList<String> results = new ArrayList<String>();

		if (!useLongOnly)
			results.add("Short x Long");

		results.add("Serie x Long");

		if (!useLongOnly)
			results.add("(Short x Long) x (Serie x Long)");

		return results;
	}

	public ArrayList<String []> getBuySellValues() {
		ArrayList<String []> results = new ArrayList<String []>();

		if (!useLongOnly)
			results.add(calculateBuySellMethod1());

		results.add(calculateBuySellMethod2());

		if (!useLongOnly)
			results.add(calculateBuySellMethod3());

		return results;
	}

	public void showConfig() {
		isCanceled = false;

		confFrame = new JDialog();
		confFrame.setName("EMA Configuration");
		confFrame.setTitle("EMA Configuration");
		confFrame
				.setDefaultCloseOperation(javax.swing.WindowConstants.DISPOSE_ON_CLOSE);
		confFrame.setModal(true);

		confFrame.setPreferredSize(new Dimension(160, 290));
		confFrame.setLayout(new BorderLayout());

		JPanel pane = new JPanel(new GridBagLayout());

		confFrame.setLocationRelativeTo(null);

		pane.add(getShortLong(), new GBC(0, 0, 1, 1));
		pane.add(getShortContainer(), new GBC(0, 1, 1, 1));
		pane.add(getLongContainer(), new GBC(0, 2, 1, 1));
		pane.add(getButtons(), new GBC(0, 3, 1, 1));

		confFrame.add(pane, BorderLayout.CENTER);
		confFrame.pack();
		confFrame.setVisible(true);
	}

	public static void main(String[] args) {

		EMA s = new EMA(new Series());
		s.showConfig();

		System.out.println(s.shortPeriod);
		System.out.println(s.shortSkipdays);
		System.out.println(s.longPeriod);
		System.out.println(s.longSkipdays);
	}

	// ////////////////////////////////////////////
	// /CONFIG LAYOUT
	// ////////////////////////////////////////////

	Container getShortLong() {

		// //////////////
		Container container1 = new Container();
		container1.setLayout(new BorderLayout());

		Dimension cDimension = new Dimension(140, 40);
		container1.setPreferredSize(cDimension);
		container1.setMinimumSize(cDimension);

		JCheckBox chkShortLong = new JCheckBox("Use Long EMA Only", useLongOnly);
		chkShortLong.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				if (((JCheckBox) e.getSource()).isSelected()) {
					useLongOnly = true;
					spLongPeriod.setEnabled(!useLongOnly);
					spLongSkipdays.setEnabled(!useLongOnly);
				} else {
					useLongOnly = false;
					spLongPeriod.setEnabled(!useLongOnly);
					spLongSkipdays.setEnabled(!useLongOnly);
				}

			}

		});

		container1.add(chkShortLong);

		container1 = createFrame(container1, "");

		return container1;
	}

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
				if (!useLongOnly) {
					shortPeriod = ((Integer) spShortPeriod.getValue())
							.intValue();
					shortSkipdays = ((Integer) spShortSkipdays.getValue())
							.intValue();
				}

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

		container1 = createFrame(container1, "Short EMA");

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
		spLongPeriod.setEnabled(!useLongOnly);

		spLongSkipdays = new JSpinner(new SpinnerNumberModel(longSkipdays, // initial
				// value
				0, // min
				360, // max
				1));
		spLongSkipdays.setMinimumSize(spDimension);
		spLongSkipdays.setPreferredSize(spDimension);
		spLongSkipdays.setEnabled(!useLongOnly);

		container2.add(spLongPeriod, new GBC(1, 0, 1, 1));
		container2.add(spLongSkipdays, new GBC(1, 1, 1, 1));

		container2 = createFrame(container2, "Long EMA");

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