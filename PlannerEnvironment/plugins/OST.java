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

public class OST extends PluginAbstract implements ActionListener {

	public int ostPeriod = 14;

	public int smaPeriod = 3;
	public int skipdays = 0;

	double[] retKOST;
	double[] retDOST;

	JSpinner spOstPeriod;
	JSpinner spSmaPeriod;
	JSpinner spSkipdays;


	
	public OST() {
		initialSetup();
	}

	public OST(Series series) {
		initialSetup();
		this.series = series;
	}

	private void initialSetup() {
		isConfigurable = true;
		isSimilarToSeries = false;
		chartType = chartTypes.CONTINUOUS;

		pluginName = "OST";
		pluginDescription = "Stochastic Oscillator";
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
	public double[][] ost(int n1,int n2, Series series, int skipdays) {

		double[][] retPerc = new double[2][series.getClosePrice().length];

		for (int x = 0; x < series.getClosePrice().length; x++) {
			int init = 0;

			if (x >= n1)
				init = x - n1;

			double min = 0;
			double max = 0;

			for (int z = init; z < (x + 1); z++) {

				if (min > series.getLowerPrice()[z])
					min = series.getLowerPrice()[z];
				else if (min == 0)
					min = series.getLowerPrice()[z];

				if (max < series.getHigherPrice()[z])
					max = series.getHigherPrice()[z];
				else if (max == 0)
					max = series.getHigherPrice()[z];
			}
			
			 retPerc[0][x] = (series.getClosePrice()[x] - min) / (max - min);

		}

		retPerc[1] = sma(n2, retPerc[0], skipdays);

		return retPerc;
	}

	public ArrayList<String> getResultDescription() {

		ArrayList<String> results = new ArrayList<String>();

		results.add("%K (Fast)");

		results.add("%D (Slow)");

		return results;
	}

	public ArrayList<double []> getResultValues() {

		ArrayList<double []> results = new ArrayList<double []>();

		double[][] ret = ost(ostPeriod,smaPeriod, series, skipdays);
		retKOST = ret[0];
		results.add(retKOST);

		retDOST =  ret[1];
		results.add(retDOST);

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

		double[] K_perc = retKOST;
		double[] D_perc = retDOST;

		for (int i = 0; i < series.getClosePrice().length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (K_perc[i] > D_perc[i] && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ series.getClosePrice()[i];

				lastMessage = sCompra;
				lastCompra = sCompra;
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (K_perc[i] < D_perc[i] && buy) {
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

		System.out.println("OST1\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("OST1\tResultado\tLUCRO\n");
		else
			System.out.println("OST1\tResultado\tPREJUIZO\n");

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

		double[] K_perc = retKOST;
		@SuppressWarnings("unused")
		double[] D_perc = retDOST;

		for (int i = 0; i < series.getClosePrice().length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (K_perc[i] > 0.70 && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ series.getClosePrice()[i];

				lastMessage = sCompra;
				lastCompra = sCompra;
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (K_perc[i] < 0.30 && buy) {
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

		System.out.println("OST2\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("OST2\tResultado\tLUCRO\n");
		else
			System.out.println("OST2\tResultado\tPREJUIZO\n");

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
		double[] K_perc = retKOST;
		double[] D_perc = retDOST;

		for (int i = 0; i < series.getClosePrice().length; i++) {

			if (i > 0)
				ret[i] = ret[i - 1];
			else
				ret[i] = "-";

			if (D_perc[i] > 0.70 && !buy) {
				compra = series.getClosePrice()[i];

				totalCompra = totalCompra + compra;

				String sCompra = "compra" + "\t" + series.getDate()[i] + "\t"
						+ series.getClosePrice()[i];

				lastMessage = sCompra;
				lastCompra = sCompra;
				System.out.println(sCompra);

				buy = true;
				ret[i] = "C";
			} else if (D_perc[i] < 0.30 && buy) {
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

		System.out.println("OST3\tResultado\t"
				+ org.baffa.StatisticLib.percFormat(percTotal, 2));

		if (percTotal > 0)
			System.out.println("OST3\tResultado\tLUCRO\n");
		else
			System.out.println("OST3\tResultado\tPREJUIZO\n");

		// TotalTotal = TotalTotal + percTotal;
		return ret;
	}

	public ArrayList<String> getBuySellDescription() {
		ArrayList<String> results = new ArrayList<String>();

		results.add("%K x %D");

		results.add("%K x Frontier");
		
		results.add("%D x Frontier");

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
		confFrame.setName("OST Configuration");
		confFrame.setTitle("OST Configuration");
		confFrame
				.setDefaultCloseOperation(javax.swing.WindowConstants.DISPOSE_ON_CLOSE);
		confFrame.setModal(true);

		confFrame.setPreferredSize(new Dimension(160, 180));
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

		OST s = new OST(new Series());
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
				
				ostPeriod = ((Integer) spOstPeriod.getValue()).intValue();
				smaPeriod = ((Integer) spSmaPeriod.getValue()).intValue();
				
				skipdays = ((Integer) spSkipdays.getValue())
						.intValue();

				closeConfFrame();
			}
		}
	}

	Container getSettingsContainer() {
		Dimension lDimension = new Dimension(80, 20);
		Dimension spDimension = new Dimension(40, 20);
		Dimension cDimension = new Dimension(140, 90);

		// //////////////
		Container container1 = new Container();
		container1.setLayout(new GridBagLayout());
		container1.setPreferredSize(cDimension);
		container1.setMinimumSize(cDimension);

		JLabel lOSTPeriod = new JLabel();
		lOSTPeriod.setMinimumSize(lDimension);
		lOSTPeriod.setPreferredSize(lDimension);
		lOSTPeriod.setText("OST Period: ");

		JLabel lSMAPeriod = new JLabel();
		lSMAPeriod.setMinimumSize(lDimension);
		lSMAPeriod.setPreferredSize(lDimension);
		lSMAPeriod.setText("SMA Period: ");
		
		JLabel lShortSkipdays = new JLabel();
		lShortSkipdays.setMinimumSize(lDimension);
		lShortSkipdays.setPreferredSize(lDimension);
		lShortSkipdays.setText("Skip Days: ");

		container1.add(lOSTPeriod, new GBC(0, 0, 1, 1));
		container1.add(lSMAPeriod, new GBC(0, 1, 1, 1));
		container1.add(lShortSkipdays, new GBC(0, 2, 1, 1));

		spOstPeriod = new JSpinner(new SpinnerNumberModel(ostPeriod, // initial
				// value
				3, // min
				360, // max
				1));
		spOstPeriod.setMinimumSize(spDimension);
		spOstPeriod.setPreferredSize(spDimension);

		spSmaPeriod = new JSpinner(new SpinnerNumberModel(smaPeriod, // initial
				// value
				3, // min
				360, // max
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

		container1.add(spOstPeriod, new GBC(1, 0, 1, 1));
		container1.add(spSmaPeriod, new GBC(1, 1, 1, 1));
		container1.add(spSkipdays, new GBC(1, 2, 1, 1));

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