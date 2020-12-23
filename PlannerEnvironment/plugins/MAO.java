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

public class MAO extends PluginAbstract implements ActionListener {

	public int emaShortPeriod = 3;
	public int emaLongPeriod = 13;
	public int smaPeriod = 3;

	public int skipdays = 0;

	double[] retMAO;

	JSpinner spEmaShortPeriod;
	JSpinner spEmaLongPeriod;
	JSpinner spSkipdays;
	JSpinner spSmaPeriod;

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

			value = ((series[counter] - retValue[counter - 1]) * exponent)
					+ retValue[counter - 1];
			// System.out.println("Value:"+value);

			retValue[counter] = value;
		}

		return retValue;
	}

	public double[] mao(Series series, int skipdays) {

		double[] retMAO = new double[series.getClosePrice().length];

		double[] ema1 = ema(3, series, skipdays);
		double[] ema2 = ema(13, series, skipdays);

		for (int i = 0; i < retMAO.length; i++) {
			retMAO[i] = ema1[i] - ema2[i];
		}

		retMAO = sma(3, retMAO, skipdays);

		return retMAO;
	}

	public ArrayList<String> getResultDescription() {

		ArrayList<String> results = new ArrayList<String>();

		results.add("MAO");

		return results;
	}

	public ArrayList<double []> getResultValues() {

		ArrayList<double []> results = new ArrayList<double []>();

		retMAO = mao(series, skipdays);
		results.add(retMAO);

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

		double[] mao = retMAO;

		ret[0] = "-";
		for (int i = 1; i < series.getClosePrice().length; i++) {

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
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (mao[i] < 0 && mao[i - 1] >= 0 && buy) {
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
		System.out.println("MAO\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("MAO\tResultado\tLUCRO\n");
		else
			System.out.println("MAO\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public ArrayList<String> getBuySellDescription() {
		ArrayList<String> results = new ArrayList<String>();

		results.add("MAO");

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
		confFrame.setName("MAO Configuration");
		confFrame.setTitle("MAO Configuration");
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

	public static void main(String[] args) {

		MAO s = new MAO(new Series());
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

				emaShortPeriod = ((Integer) spEmaShortPeriod.getValue())
						.intValue();
				emaLongPeriod = ((Integer) spEmaLongPeriod.getValue())
						.intValue();

				smaPeriod = ((Integer) spSmaPeriod.getValue())
						.intValue();
				skipdays = ((Integer) spSkipdays.getValue()).intValue();

				closeConfFrame();
			}
		}
	}

	Container getSettingsContainer() {
		Dimension lDimension = new Dimension(120, 20);
		Dimension spDimension = new Dimension(40, 20);
		Dimension cDimension = new Dimension(180, 100);

		// //////////////
		Container container1 = new Container();
		container1.setLayout(new GridBagLayout());
		container1.setPreferredSize(cDimension);
		container1.setMinimumSize(cDimension);

		JLabel lShortEMAPeriod = new JLabel();
		lShortEMAPeriod.setMinimumSize(lDimension);
		lShortEMAPeriod.setPreferredSize(lDimension);
		lShortEMAPeriod.setText("Short EMA Period: ");

		JLabel lLongEMAPeriod = new JLabel();
		lLongEMAPeriod.setMinimumSize(lDimension);
		lLongEMAPeriod.setPreferredSize(lDimension);
		lLongEMAPeriod.setText("Long EMA Period: ");

		JLabel lSMAPeriod = new JLabel();
		lSMAPeriod.setMinimumSize(lDimension);
		lSMAPeriod.setPreferredSize(lDimension);
		lSMAPeriod.setText("SMA Period: ");

		JLabel lSkipdays = new JLabel();
		lSkipdays.setMinimumSize(lDimension);
		lSkipdays.setPreferredSize(lDimension);
		lSkipdays.setText("Skip Days: ");

		container1.add(lShortEMAPeriod, new GBC(0, 0, 1, 1));
		container1.add(lLongEMAPeriod, new GBC(0, 1, 1, 1));
		container1.add(lSMAPeriod, new GBC(0, 2, 1, 1));
		container1.add(lSkipdays, new GBC(0, 3, 1, 1));

		spEmaShortPeriod = new JSpinner(new SpinnerNumberModel(emaShortPeriod, // initial
				// value
				3, // min
				360, // max
				1));
		spEmaShortPeriod.setMinimumSize(spDimension);
		spEmaShortPeriod.setPreferredSize(spDimension);

		spEmaLongPeriod = new JSpinner(new SpinnerNumberModel(emaLongPeriod, // initial
				// value
				5, // min
				720, // max
				1));
		spEmaLongPeriod.setMinimumSize(spDimension);
		spEmaLongPeriod.setPreferredSize(spDimension);

		spSmaPeriod = new JSpinner(new SpinnerNumberModel(smaPeriod, // initial
				// value
				3, // min
				720, // max
				1));
		spSmaPeriod.setMinimumSize(spDimension);
		spSmaPeriod.setPreferredSize(spDimension);

		spSkipdays = new JSpinner(new SpinnerNumberModel(skipdays, // initial
				// value
				0, // min
				270, // max
				1));
		spSkipdays.setMinimumSize(spDimension);
		spSkipdays.setPreferredSize(spDimension);

		container1.add(spEmaShortPeriod, new GBC(1, 0, 1, 1));
		container1.add(spEmaLongPeriod, new GBC(1, 1, 1, 1));
		container1.add(spSmaPeriod, new GBC(1, 2, 1, 1));
		container1.add(spSkipdays, new GBC(1, 3, 1, 1));

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