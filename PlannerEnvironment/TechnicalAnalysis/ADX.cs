using Baffa.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraderWhatever.Business;
namespace PlannerEnvironment.TechnicalAnalysis
{
    public class ADX : PluginAbstract
{

    private int period = 14;
    private int skipdays = 0;

    private bool useExponential = false;

    double[] dmP; // Directional Move Indicator
    double[] dmN; // Directional Move Indicator
    double[] admP; // Average Directional Move Indicator
    double[] admN; // Average Directional Move Indicator

    double[] tr; // True Range
    double[] atr; // Average True Range

    double[] diP; // Directional Index
    double[] diN; // Directional Index

    double[] dx; // Directional Movement Index

    double[] adx; // Average Directional Movement Index

    double[] retADX;
    double[] retDIP;
    double[] retDIN;



    public ADX()
    {
        InitialSetup();
    }

    public ADX(Series series)
    {
        InitialSetup();
        this.series = series;
    }

    private void InitialSetup()
    {
        isConfigurable = true;
        isSimilarToSeries = false;
        chartType = chartTypes.CONTINUOUS;

        pluginName = "ADX";
        pluginDescription = "Average Directional Index";
    }

    public double[][] Dm(Series series, int skipdays)
    {
        double[][] retDM = new double[2][];
        retDM[0] = new double[series.getClosePrice().Length];
        retDM[1] = new double[series.getClosePrice().Length];


        dmP = new double[series.getClosePrice().Length];
        dmN = new double[series.getClosePrice().Length];

        dmP[0] = 0;
        dmN[0] = 0;

        for (int i = 1; i < retDM[0].Length; i++)
        {

            double highVar = series.getHigherPrice()[i]
                    - series.getHigherPrice()[i - 1];
            double lowVar = series.getLowerPrice()[i - 1]
                    - series.getLowerPrice()[i];

            double dmPositive = 0;
            double dmNegative = 0;
            // Calculate DM
            if ((highVar < 0 && lowVar < 0) || (highVar == lowVar))
            {
                dmPositive = 0;
                dmNegative = 0;
            }
            else if (highVar > lowVar)
            {
                dmPositive = highVar;
                dmNegative = 0;
            }
            else if (highVar < lowVar)
            {
                dmPositive = 0;
                dmNegative = lowVar;
            }

            dmP[i] = dmPositive;
            dmN[i] = dmNegative;

            //			Console.WriteLine("dmP = " + dmP[i]);
            //			Console.WriteLine("dmN = " + dmP[i]);
        }
        retDM[0] = dmP;
        retDM[1] = dmN;

        return retDM;
    }

    public double[][] Adm(int n, double[] dmP_, double[] dmN_)
    {
        double[][] retADM = new double[2][];
        retADM[0] = new double[dmP_.Length];
        retADM[1] = new double[dmP_.Length];


        admP = new double[dmP_.Length];
        admN = new double[dmN_.Length];

        admP[0] = 0;
        admN[0] = 0;

        for (int i = 1; i < retADM[0].Length; i++)
        {

            //			if (i == 1) {
            //				admP[i] = dmP_[i];
            //				admN[i] = dmN_[i];
            //
            //			} else {
            // Calculate ADM
            if (!useExponential)
            {
                admP[i] = (admP[i - 1] * (((double)n - (double)1) / (double)n))
                        + (dmP_[i] * ((double)1 / (double)n));

                admN[i] = (admN[i - 1] * (((double)n - (double)1) / (double)n))
                        + (dmN_[i] * ((double)1 / (double)n));
            }
            else
            {
                // /Variance using exponential moving average//
                admP[i] = (admP[i - 1] * ((double)1 - ((double)2 / ((double)n + (double)1))))
                        + (dmP_[i] * ((double)2 / ((double)n + (double)1)));

                admN[i] = (admN[i - 1] * ((double)1 - ((double)2 / ((double)n + (double)1))))
                        + (dmN_[i] * ((double)2 / ((double)n + (double)1)));
            }

            //			}
            //			Console.WriteLine("admP = " + admP[i]);
            //			Console.WriteLine("admN = " + admP[i]);

        }

        retADM[0] = admP;
        retADM[1] = admN;

        return retADM;
    }

    public double[] Tr(Series series)
    {

        double[] retTr = new double[series.getClosePrice().Length];

        tr = new double[series.getClosePrice().Length];
        tr[0] = 0;

        for (int i = 1; i < retTr.Length; i++)
        {

            double tr_ = 0;

            // Calculate TR
            double max1 = Math.Abs(series.getHigherPrice()[i]
                    - series.getLowerPrice()[i]);
            double max2 = Math.Abs(series.getHigherPrice()[i]
                    - series.getClosePrice()[i - 1]);
            double max3 = Math.Abs(series.getClosePrice()[i - 1]
                    - series.getLowerPrice()[i]);

            tr_ = Math.Max(Math.Max(max1, max2), max3);
            tr[i] = tr_;

            //			Console.WriteLine("tr = " + tr[i]);
        }
        retTr = tr;

        return retTr;
    }

    public double[] Atr(int n, double[] tr_)
    {

        double[] retAtr = new double[tr_.Length];

        atr = new double[tr_.Length];
        atr[0] = 0;

        for (int i = 1; i < retAtr.Length; i++)
        {

            //			if (i == 1)
            //				atr[i] = tr_[i];
            //
            //			else {

            // Calculate ATR
            if (!useExponential)
            {
                atr[i] = (atr[i - 1] * (((double)n - (double)1) / (double)n))
                        + (tr_[i] * ((double)1 / (double)n));
            }
            else
            {
                // /Variance using exponential moving average//
                atr[i] = (atr[i - 1] * ((double)1 - ((double)2 / ((double)n + (double)1))))
                        + (tr_[i] * ((double)2 / ((double)n + (double)1)));
            }
            //			}

            //			Console.WriteLine("atr = " + atr[i]);

        }

        retAtr = atr;

        return retAtr;
    }

    public double[][] Di(double[] atr_, double[] admP_, double[] admN_)
    {
        double[][] retDI = new double[2][];
        retDI[0] = new double[atr_.Length];
        retDI[1] = new double[atr_.Length];

        diP = new double[atr_.Length];
        diN = new double[atr_.Length];

        diP[0] = 0;
        diN[0] = 0;

        for (int i = 1; i < atr_.Length; i++)
        {

            double diPositive = 0;
            double diNegative = 0;

            // Calculate DI
            diPositive = (admP_[i] / atr_[i]) * 100;
            diP[i] = diPositive;

            diNegative = (admN_[i] / atr_[i]) * 100;
            diN[i] = diNegative;

            // Console.WriteLine("diP = " + diPositive);
            // Console.WriteLine("diN = " + diNegative);

        }

        retDI[0] = diP;
        retDI[1] = diN;
        return retDI;
    }

    public double[] Dx(double[] diP_, double[] diN_)
    {
        double[] retDX = new double[diP_.Length];

        dx = new double[series.getClosePrice().Length];
        dx[0] = 0;

        for (int i = 1; i < diP_.Length; i++)
        {

            double dx_ = 0;

            // Calculate DX
            if ((diP_[i] + diN_[i]) != 0)
                dx_ = (Math.Abs(diP_[i] - diN_[i]) / (diP_[i] + diN_[i])) * 100;
            else
                dx_ = dx[i - 1];

            if (Double.IsNaN(dx_))
                dx_ = 0;

            dx[i] = dx_;

            // Console.WriteLine("dx = " + dx[i]);

        }

        retDX = dx;
        return retDX;
    }

    public double[] Adx(int n, double[] dx_)
    {
        double[] retADX = new double[dx_.Length];

        adx = new double[series.getClosePrice().Length];
        adx[0] = 0;

        for (int i = 1; i < dx_.Length; i++)
        {

            //			if (i == 1)
            //				adx[i] = dx_[i];
            //
            //			else {

            // Calculate ADX
            if (!useExponential)
            {
                adx[i] = (adx[i - 1] * (((double)n - (double)1) / (double)n))
                        + (dx_[i] * ((double)1 / (double)n));
            }
            else
            {
                // /Variance using exponential moving average//
                adx[i] = (adx[i - 1] * ((double)1 - ((double)2 / ((double)n + (double)1))))
                        + (dx_[i] * ((double)2 / ((double)n + (double)1)));
            }
            //			}
            // Console.WriteLine("adx = " + adx[i]);
            //
            // Console.WriteLine("################################");

        }

        retADX = adx;

        return retADX;
    }

    public override List<String> GetResultDescription()
    {

        List<String> results = new List<String>();

        results.Add("ADX");
        results.Add("+DI");
        results.Add("-DI");

        return results;
    }

    public override List<double[]> GetResultValues()
    {

        List<double[]> results = new List<double[]>();

        double[][] retDM = Dm(series, skipdays);
        dmP = retDM[0];
        dmN = retDM[1];

        double[][] retADM = Adm(period, retDM[0], retDM[1]);
        admP = retADM[0];
        admN = retADM[1];

        double[] retTr = Tr(series);
        tr = retTr;

        double[] retAtr = Atr(period, retTr);
        atr = retAtr;

        double[][] retDI = Di(retAtr, retADM[0], retADM[1]);
        diP = retDI[0];
        diN = retDI[1];

        double[] retDX = Dx(retDI[0], retDI[1]);
        dx = retDX;

        double[] retADX = Adx(period, retDX);
        adx = retADX;

        //retADX = retADX;
        retDIP = retDI[0];
        retDIN = retDI[1];

        results.Add(retADX);
        results.Add(retDIP);
        results.Add(retDIN);

        return results;
    }

    public String[] calculateBuySellMethod1()
    {

        String[] ret = new String[series.getClosePrice().Length];
        //
        // bool buy = false;
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
        // for (int i = 0; i < series.getClosePrice().Length; i++) {
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
        // Console.WriteLine(sCompra);
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
        // + StatisticLib.percFormat(perc, 2);
        //
        // lastMessage = sVenda;
        // lastVenda = sVenda;
        // Console.WriteLine(sVenda);
        //
        // buy = false;
        // ret[i] = "V";
        // }
        //
        // }
        //
        // Console.WriteLine("MACD1\tResultado\t"
        // + StatisticLib.percFormat(percTotal, 2));
        //
        // if (percTotal > 0)
        // Console.WriteLine("MACD1\tResultado\tLUCRO\n");
        // else
        // Console.WriteLine("MACD1\tResultado\tPREJUIZO\n");
        //
        // // TotalTotal = TotalTotal + percTotal;
        //
        return ret;
    }

    public String[] calculateBuySellMethod2()
    {

        String[] ret = new String[series.getClosePrice().Length];
        //
        // bool buy = false;
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
        // for (int i = 0; i < series.getClosePrice().Length; i++) {
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
        // Console.WriteLine(sCompra);
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
        // + StatisticLib.percFormat(perc, 2);
        //
        // lastMessage = sVenda;
        // lastVenda = sVenda;
        // Console.WriteLine(sVenda);
        //
        // buy = false;
        // ret[i] = "V";
        // }
        //
        // }
        //
        // Console.WriteLine("MACD2\tResultado\t"
        // + StatisticLib.percFormat(percTotal, 2));
        //
        // if (percTotal > 0)
        // Console.WriteLine("MACD2\tResultado\tLUCRO\n");
        // else
        // Console.WriteLine("MACD2\tResultado\tPREJUIZO\n");
        //
        // // TotalTotal = TotalTotal + percTotal;
        return ret;
    }

    public String[] calculateBuySellMethod3()
    {

        String[] ret = new String[series.getClosePrice().Length];
        //
        // bool buy = false;
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
        // for (int i = 0; i < series.getClosePrice().Length; i++) {
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
        // Console.WriteLine(sCompra);
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
        // + StatisticLib.percFormat(perc, 2);
        //
        // lastMessage = sVenda;
        // lastVenda = sVenda;
        // Console.WriteLine(sVenda);
        //
        // buy = false;
        // ret[i] = "V";
        // }
        //
        // }
        //
        // Console.WriteLine("MACD3\tResultado\t"
        // + StatisticLib.percFormat(percTotal, 2));
        //
        // if (percTotal > 0)
        // Console.WriteLine("MACD3\tResultado\tLUCRO\n");
        // else
        // Console.WriteLine("MACD3\tResultado\tPREJUIZO\n");
        //
        // // TotalTotal = TotalTotal + percTotal;
        return ret;
    }

    public override List<String> GetBuySellDescription()
    {
        List<String> results = new List<String>();

        results.Add("MACD X Signal");

        results.Add("MACD");

        results.Add("Histogram");

        return results;
    }

    public override List<String[]> GetBuySellValues()
    {
        List<String[]> results = new List<String[]>();

        results.Add(calculateBuySellMethod1());

        results.Add(calculateBuySellMethod2());

        results.Add(calculateBuySellMethod3());

        return results;
    }
}

}