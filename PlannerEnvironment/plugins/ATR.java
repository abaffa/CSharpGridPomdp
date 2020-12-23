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

public class ATR extends PluginAbstract implements ActionListener {

	private int period = 14;
	private int skipdays = 0;

	private boolean useExponential = false;

	double[] tr; // True Range
	double[] atr; // Average True Range

	double[] retATR;

	JSpinner spPeriod;
	JSpinner spSkipdays;

	public ATR() {
		initialSetup();
	}

	public ATR(Series series) {
		initialSetup();
		this.series = series;
	}

	private void initialSetup() {
		isConfigurable = true;
		isSimilarToSeries = false;
		chartType = chartTypes.CONTINUOUS;

		pluginName = "ATR";
		pluginDescription = "Average True Range";
	}

	public double[] tr(Series series) {

		double[] retTr = new double[series.getClosePrice().length];

		tr = new double[series.getClosePrice().length];
		tr[0] = 0;

		for (int i = 1; i < retTr.length; i++) {

			double tr_ = 0;

			// Calculate TR
			double max1 = Math.abs(series.getHigherPrice()[i]
					- series.getLowerPrice()[i]);
			double max2 = Math.abs(series.getHigherPrice()[i]
					- series.getClosePrice()[i - 1]);
			double max3 = Math.abs(series.getClosePrice()[i - 1]
					- series.getLowerPrice()[i]);

			tr_ = Math.max(Math.max(max1, max2), max3);
			tr[i] = tr_;

//			System.out.println("tr = " + tr[i]);
		}
		retTr = tr;

		return retTr;
	}

	public double[] atr(int n, double[] tr_) {

		double[] retAtr = new double[tr_.length];

		atr = new double[tr_.length];
		atr[0] = 0;

		for (int i = 1; i < retAtr.length; i++) {

//			if (i == 1)
//				atr[i] = tr_[i];
//
//			else {

				// Calculate ATR
				if (!useExponential) {
					atr[i] = (atr[i - 1] * (((double) n - (double) 1) / (double) n))
							+ (tr_[i] * ((double) 1 / (double) n));
				} else {
					// /Variance using exponential moving average//
					atr[i] = (atr[i - 1] * ((double) 1 - ((double) 2 / ((double) n + (double) 1))))
							+ (tr_[i] * ((double) 2 / ((double) n + (double) 1)));
				}
//			}

//			System.out.println("atr = " + atr[i]);

		}

		retAtr = atr;

		return retAtr;
	}


	public ArrayList<String> getResultDescription() {

		ArrayList<String> results = new ArrayList<String>();

		results.add("ATR");
		
		return results;
	}

	public ArrayList<double[]> getResultValues() {

		ArrayList<double[]> results = new ArrayList<double[]>();

		double[] retTr = tr(series);
		tr = retTr;

		retATR = atr(period, retTr);
		atr = retATR;

		results.add(retATR);

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

	public ArrayList<String[]> getBuySellValues() {
		ArrayList<String[]> results = new ArrayList<String[]>();

		results.add(calculateBuySellMethod1());

		results.add(calculateBuySellMethod2());

		results.add(calculateBuySellMethod3());

		return results;
	}

	public void showConfig() {
		isCanceled = false;

		confFrame = new JDialog();
		confFrame.setName("ATR Configuration");
		confFrame.setTitle("ATR Configuration");
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

		JCheckBox chkShortLong = new JCheckBox("Use Exponential Smoothing",
				useExponential);
		chkShortLong.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				if (((JCheckBox) e.getSource()).isSelected()) {
					useExponential = true;
				} else {
					useExponential = false;
				}

			}

		});

		JLabel lPeriod = new JLabel();
		lPeriod.setMinimumSize(lDimension);
		lPeriod.setPreferredSize(lDimension);
		lPeriod.setText("Period: ");

		JLabel lSkipdays = new JLabel();
		lSkipdays.setMinimumSize(lDimension);
		lSkipdays.setPreferredSize(lDimension);
		lSkipdays.setText("Skip Days: ");

		container1.add(chkShortLong, new GBC(0, 0, 2, 1).setInsets(0, 5, 13, 5)
				.setFill(java.awt.GridBagConstraints.BOTH));
		container1.add(lPeriod, new GBC(0, 1, 1, 1));
		container1.add(lSkipdays, new GBC(0, 2, 1, 1));

		spPeriod = new JSpinner(new SpinnerNumberModel(period, // initial
				// value
				5, // min
				360, // max
				1));
		spPeriod.setMinimumSize(spDimension);
		spPeriod.setPreferredSize(spDimension);

		spSkipdays = new JSpinner(new SpinnerNumberModel(skipdays, // initial
				// value
				0, // min
				270, // max
				1));
		spSkipdays.setMinimumSize(spDimension);
		spSkipdays.setPreferredSize(spDimension);

		container1.add(spPeriod, new GBC(1, 1, 1, 1));
		container1.add(spSkipdays, new GBC(1, 2, 1, 1));

		container1 = createFrame(container1, "ATR Settings");

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