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

public class SAR extends PluginAbstract implements ActionListener {

	int period = 13;
	double sarIni = 0.02;
	double sarInterval = 0.02;
	double sarMax = 0.20;

	int skipdays = 0;

	double[] retSAR;

	JSpinner spPeriod;
	JSpinner spSarIni;
	JSpinner spSarInterval;
	JSpinner spSarMax;
	JSpinner spSkipdays;

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

		double[] retSAR = new double[series.getClosePrice().length];
		boolean uptrend = true;

		double fator = initialValue;
		double lowerPrice = series.getClosePrice()[0];
		double upperPrice = series.getClosePrice()[0];
		double EP = upperPrice;

		retSAR[0] = EP;

		double trendMean = 0;

		for (int z = 1; z < series.getClosePrice().length; z++) {

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

	public ArrayList<String> getResultDescription() {

		ArrayList<String> results = new ArrayList<String>();

		results.add("SAR");

		return results;
	}

	public ArrayList<double []> getResultValues() {

		ArrayList<double []> results = new ArrayList<double []>();

		retSAR = sar(sarIni, sarInterval, sarMax, series, skipdays);
		results.add(retSAR);

		return results;
	}

	public String[] calculateBuySellMethod() {

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

		double[] sar = retSAR;

		for (int i = 0; i < series.getClosePrice().length; i++) {

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
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (series.getClosePrice()[i] < sar[i] && buy) {
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

		System.out.println("SAR\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("SAR\tResultado\tLUCRO\n");
		else
			System.out.println("SAR\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public ArrayList<String> getBuySellDescription() {
		ArrayList<String> results = new ArrayList<String>();

		results.add("Serie x SAR");

		return results;
	}

	public ArrayList<String []> getBuySellValues() {
		ArrayList<String []> results = new ArrayList<String []>();

		results.add(calculateBuySellMethod());

		return results;
	}

	public void showConfig() {
		isCanceled = false;

		confFrame = new JDialog();
		confFrame.setName("SAR Configuration");
		confFrame.setTitle("SAR Configuration");
		confFrame
				.setDefaultCloseOperation(javax.swing.WindowConstants.DISPOSE_ON_CLOSE);
		confFrame.setModal(true);

		confFrame.setPreferredSize(new Dimension(200, 200));
		confFrame.setLayout(new BorderLayout());

		JPanel pane = new JPanel(new GridBagLayout());

		confFrame.setLocationRelativeTo(null);

		pane.add(getSettingsContainer(), new GBC(0, 0, 1, 1));
		pane.add(getButtons(), new GBC(0, 1, 1, 1));

		confFrame.add(pane, BorderLayout.CENTER);
		confFrame.pack();
		confFrame.setVisible(true);
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
				sarIni = ((Double) spSarIni.getValue()).doubleValue();
				sarInterval = ((Double) spSarInterval.getValue()).doubleValue();
				sarMax = ((Double) spSarMax.getValue()).doubleValue();

				skipdays = ((Integer) spSkipdays.getValue()).intValue();

				closeConfFrame();
			}
		}
	}

	Container getSettingsContainer() {
		Dimension lDimension = new Dimension(100, 20);
		Dimension spDimension = new Dimension(60, 20);
		Dimension cDimension = new Dimension(180, 110);

		// //////////////
		Container container1 = new Container();
		container1.setLayout(new GridBagLayout());
		container1.setPreferredSize(cDimension);
		container1.setMinimumSize(cDimension);

		JLabel lPeriod = new JLabel();
		lPeriod.setMinimumSize(lDimension);
		lPeriod.setPreferredSize(lDimension);
		lPeriod.setText("Trend Period: ");

		JLabel lSarIni = new JLabel();
		lSarIni.setMinimumSize(lDimension);
		lSarIni.setPreferredSize(lDimension);
		lSarIni.setText("Initial Factor: ");

		JLabel lSarInterval = new JLabel();
		lSarInterval.setMinimumSize(lDimension);
		lSarInterval.setPreferredSize(lDimension);
		lSarInterval.setText("Interval Factor: ");

		JLabel lSarMax = new JLabel();
		lSarMax.setMinimumSize(lDimension);
		lSarMax.setPreferredSize(lDimension);
		lSarMax.setText("Max Factor: ");

		JLabel lSkipdays = new JLabel();
		lSkipdays.setMinimumSize(lDimension);
		lSkipdays.setPreferredSize(lDimension);
		lSkipdays.setText("Skip Days: ");

		container1.add(lPeriod, new GBC(0, 0, 1, 1));
		container1.add(lSarIni, new GBC(0, 1, 1, 1));
		container1.add(lSarInterval, new GBC(0, 2, 1, 1));
		container1.add(lSarMax, new GBC(0, 3, 1, 1));
		container1.add(lSkipdays, new GBC(0, 4, 1, 1));

		spPeriod = new JSpinner(new SpinnerNumberModel(period, // initial
				// value
				3, // min
				72, // max
				1));
		spPeriod.setMinimumSize(spDimension);
		spPeriod.setPreferredSize(spDimension);

		spSarIni = new JSpinner(new SpinnerNumberModel(sarIni, // initial
				// value
				0.005, // min
				1, // max
				0.005));
		spSarIni.setMinimumSize(spDimension);
		spSarIni.setPreferredSize(spDimension);

		spSarInterval = new JSpinner(new SpinnerNumberModel(sarInterval, // initial
				// value
				0.005, // min
				1, // max
				0.005));
		spSarInterval.setMinimumSize(spDimension);
		spSarInterval.setPreferredSize(spDimension);

		spSarMax = new JSpinner(new SpinnerNumberModel(sarMax, // initial
				// value
				0.005, // min
				1, // max
				0.005));
		spSarMax.setMinimumSize(spDimension);
		spSarMax.setPreferredSize(spDimension);

		spSkipdays = new JSpinner(new SpinnerNumberModel(skipdays, // initial
				// value
				0, // min
				270, // max
				1));
		spSkipdays.setMinimumSize(spDimension);
		spSkipdays.setPreferredSize(spDimension);

		container1.add(spPeriod, new GBC(1, 0, 1, 1));
		container1.add(spSarIni, new GBC(1, 1, 1, 1));
		container1.add(spSarInterval, new GBC(1, 2, 1, 1));
		container1.add(spSarMax, new GBC(1, 3, 1, 1));
		container1.add(spSkipdays, new GBC(1, 4, 1, 1));

		container1 = createFrame(container1, "Settings");

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