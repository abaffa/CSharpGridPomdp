using FileDownloader;
using org.baffa.db;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace TraderWhatever.Business
{



    public class Series
    {
        List<Cotacao> cotacaoList;

        private readonly String tempFilesPath = ""; //org.baffa.config.Settings.YAHOO_TempFilesPath;

        bool zeroUm = false;
        bool useReturns = false;
        bool useNormalizer = true;
        double closePriceRetNormalizer = 1;
        double closePriceNormalizer = 1;

        double volumeRetNormalizer = 1;
        double volumeNormalizer = 1;

        String codNeg;
        String descNeg;

        DateTime[] date;
        double[] openPrice;
        double[] higherPrice;
        double[] lowerPrice;
        double[] closePrice;
        double[] volume;
        double[] quantity;
        double[] totalNegociated;


        public Series()
        {

        }

        public Series(List<Cotacao> cotacaoList)
        {
            this.cotacaoList = cotacaoList;
            load();
        }

        public Series(double[] c, DateTime[] d)
        {
            load(c, d);

        }

        public Series(String codNeg, String from, String to, bool zeroUm)
        {

            try
            {
                DateTime cIni = DateTime.Now;

                DateTime cFim = DateTime.Now;

                try
                {

                    cIni = DateTime.ParseExact(from, "yyyy-MM-dd", null);
                    cFim = DateTime.ParseExact(to, "yyyy-MM-dd", null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                this.useNormalizer = !zeroUm;
                this.zeroUm = zeroUm;
                getFile(codNeg, cIni, cFim);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                load();
            }

        }

        public Series(String codNeg, DateTime from, DateTime to)
        {
            
            try
            {
                getFile(codNeg, from, to);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                load();
            }

        }

        public void getFile(String codNeg, DateTime from, DateTime to)
        {




            DownloadYahooFinance dlYahoo = new DownloadYahooFinance();
            String sCsv = dlYahoo.DownloadFile(codNeg, from, to);

            String[] lines = sCsv.Trim('\n').Replace("\r", "").Split('\n');

            long iTotal = lines.Length - 1;
            long iActual = 1;
            double preUltOntem = 0;




            List<Cotacao> lCotacao = new List<Cotacao>();
            //  try
            // {

            if (iTotal > 0)
            {

                while (iActual <= iTotal)
                {

                    String[] actualLine = (String[])lines[(int)iActual].Split(',');

                    Cotacao b = new Cotacao();
                    b.IDARQUIVO = 0;
                    b.DTPREG = stringToDate(actualLine[0], "yyyy-MM-dd");
                    b.CODBDI = "00";
                    b.CODNEG = codNeg;
                    b.TPMERC = 10;
                    b.NOMRES = "";
                    b.ESPECI = "";
                    b.PRAZOT = "";
                    b.MODREF = "R$";
                    b.PREMED = 0;
                    b.PREOFC = 0;
                    b.PREOFV = 0;
                    b.TOTNEG = 0;
                    b.QUATOT = 0;
                    b.INDOPC = 0;
                    b.DATVEN = stringToDate("99991231");
                    b.FATCOT = 1;
                    b.PTOEXE = 0;
                    b.CODISI = "IBOV";
                    b.DISMES = 0;

                    if (zeroUm)
                    {
                        double preUlt = Double.Parse(actualLine[4]);
                        double vlr = 0;
                        if (iActual != 0)
                        {
                            double perc = (preUlt / preUltOntem) - 1;

                            if (perc >= 0)
                                vlr = 1;

                        }

                        b.PREABE = vlr;
                        b.PREMAX = vlr;
                        b.PREMIN = vlr;
                        b.PREULT = vlr;
                        b.VOLTOT = vlr;
                        b.PREEXE = vlr;

                        preUltOntem = preUlt;
                    }
                    else
                    {
                        //[0] = "Date,Open,High,Low,Close,Adj Close,Volume"
                        double PREABE = 0; Double.TryParse(actualLine[1], out PREABE); b.PREABE = PREABE;
                        double PREMAX = 0; Double.TryParse(actualLine[2], out PREMAX); b.PREMAX = PREMAX;
                        double PREMIN = 0; Double.TryParse(actualLine[3], out PREMIN); b.PREMIN = PREMIN;

                        /*ajustado para preco corrigido */
                        //b.PREULT(Double.Parse(actualLine[4])); // close
                        double PREULT = 0; Double.TryParse(actualLine[5], out PREULT); b.PREULT = PREULT;
                        double VOLTOT = 0; Double.TryParse(actualLine[6], out VOLTOT); b.VOLTOT = VOLTOT;

                        b.PREEXE = 0.0;
                    }

                    lCotacao.Add(b);
                    try
                    {
                        iActual++;

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw (e);
                    }

                }
            }

            cotacaoList = lCotacao.Where(p=> p.PREABE !=0 && p.PREULT != 0).OrderBy(p=> p.DTPREG).ToList();


            /*         
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine(ex);
                 }
                 */

            // System.out.println("fim");



        }

        public void load(double[] c, DateTime[] d)
        {

            date = new DateTime[c.Length];
            openPrice = new double[c.Length];
            higherPrice = new double[c.Length];
            lowerPrice = new double[c.Length];
            closePrice = new double[c.Length];
            volume = new double[c.Length];
            quantity = new double[c.Length];
            totalNegociated = new double[c.Length];

            for (int i = 0; i < c.Length; i++)
            {

                date[i] = d[i];
                openPrice[i] = c[i];
                higherPrice[i] = c[i];
                lowerPrice[i] = c[i];
                closePrice[i] = c[i];
                volume[i] = 0;
                quantity[i] = 0;
                totalNegociated[i] = c[i];
            }

            refreshDescription();

            if (useNormalizer)
                calculateNormalizer();

        }

        public void load()
        {
            if (cotacaoList != null)
            {
                date = new DateTime[cotacaoList.Count];
                openPrice = new double[cotacaoList.Count];
                higherPrice = new double[cotacaoList.Count];
                lowerPrice = new double[cotacaoList.Count];
                closePrice = new double[cotacaoList.Count];
                volume = new double[cotacaoList.Count];
                quantity = new double[cotacaoList.Count];
                totalNegociated = new double[cotacaoList.Count];

                for (int i = 0; i < cotacaoList.Count; i++)
                {

                    date[i] = ((Cotacao)cotacaoList[i]).DTPREG;
                    openPrice[i] = ((Cotacao)cotacaoList[i]).PREABE;
                    higherPrice[i] = ((Cotacao)cotacaoList[i]).PREMAX;
                    lowerPrice[i] = ((Cotacao)cotacaoList[i]).PREMIN;
                    closePrice[i] = ((Cotacao)cotacaoList[i]).PREULT;
                    volume[i] = ((Cotacao)cotacaoList[i]).VOLTOT;
                    quantity[i] = ((Cotacao)cotacaoList[i]).QUATOT;
                    totalNegociated[i] = ((Cotacao)cotacaoList[i]).TOTNEG;
                }

                refreshDescription();

                if (useNormalizer)
                    calculateNormalizer();
            }
        }

        public void reset()
        {
            codNeg = null;
            descNeg = null;
            date = null;
            openPrice = null;
            higherPrice = null;
            lowerPrice = null;
            closePrice = null;
            volume = null;
            quantity = null;
            totalNegociated = null;
        }

        public void refreshDescription()
        {

            if (cotacaoList != null)
            {
                if (cotacaoList.Count > 0)
                {
                    codNeg = ((Cotacao)cotacaoList[0]).CODNEG;
                    descNeg = ((Cotacao)cotacaoList[0]).NOMRES + " "
                            + ((Cotacao)cotacaoList[0]).ESPECI;
                    descNeg = descNeg.Trim();
                }
            }

        }

        public void setCotacaoList(List<Cotacao> cotacaoList)
        {
            this.cotacaoList = cotacaoList;
            load();
        }

        public List<Cotacao> getCotacaoList()
        {
            return cotacaoList;
        }

        public String getCodNeg()
        {
            return codNeg;
        }

        public String getDescNeg()
        {
            return descNeg;
        }

        public DateTime[] getDate()
        {
            return date;
        }

        public double[] getOpenPrice()
        {
            return openPrice;
        }

        public double[] getHigherPrice()
        {
            return higherPrice;
        }

        public double[] getLowerPrice()
        {
            return lowerPrice;
        }

        public double[] getClosePrice()
        {
            return closePrice;
        }

        public double[] getVolume()
        {
            return volume;
        }

        public double[] getQuantity()
        {
            return quantity;
        }

        public double[] getTotalNegociated()
        {
            return totalNegociated;
        }

        // ////////////////////////////

        public String[] getColumnNames()
        {
            String[] columnNames = { "CODNEG", "DATE", "FIST PRICE", "MIN PRICE",
                "MAX PRICE", "LAST PRICE", "TOTAL VOLUME", "QUANTITY", "TOTAL" };

            return columnNames;
        }

        public Object[][] getTableData()
        {

            Object[][] data = new Object[date.Length][];


            for (int i = 0; i < cotacaoList.Count; i++)
            {
                data[i] = new Object[9];
                data[i][0] = ((Cotacao)cotacaoList[i]).CODNEG;
                data[i][1] = ((Cotacao)cotacaoList[i]).DTPREG;
                data[i][2] = (double)(((Cotacao)cotacaoList[i]).PREABE);
                data[i][3] = (double)(((Cotacao)cotacaoList[i]).PREMIN);
                data[i][4] = (double)(((Cotacao)cotacaoList[i]).PREMAX);
                data[i][5] = (double)(((Cotacao)cotacaoList[i]).PREULT);
                data[i][6] = (double)(((Cotacao)cotacaoList[i]).VOLTOT);
                data[i][7] = (double)(((Cotacao)cotacaoList[i]).QUATOT);
                data[i][8] = (double)(((Cotacao)cotacaoList[i]).TOTNEG);
            }

            return data;
        }

        // //////////////////////////////

        public TimeSeries getVolumeTimeSeries()
        {

            TimeSeries paperSeries = new TimeSeries(codNeg + " volumes");

            for (int i = 0; i < cotacaoList.Count; i++)
                paperSeries.Add(date[i], volume[i]);

            return paperSeries;
        }

        public TimeSeries getPriceTimeSeries()
        {

            TimeSeries paperSeries = new TimeSeries(codNeg + " prices");

            for (int i = 0; i < cotacaoList.Count; i++)
                paperSeries.Add(date[i],
                        closePrice[i]);

            return paperSeries;
        }

        public TimeSeries getReturnTimeSeries()
        {

            TimeSeries paperSeries = new TimeSeries(codNeg + " returns");

            double[] returns = getReturn();

            for (int i = 0; i < cotacaoList.Count; i++)
                paperSeries
                        .Add(date[i], returns[i]);

            return paperSeries;
        }

        // /////////////////////
        public double[] getReturn()
        {

            double[] v1 = new double[closePrice.Length];
            v1[0] = 0;

            for (int i = 1; i < closePrice.Length; i++)
                v1[i] = fixNumber((closePrice[i] - closePrice[i - 1])
                        / closePrice[i - 1]);

            return v1;
        }

        public double getAverageReturn()
        {

            double v1 = 0;

            for (int i = 1; i < closePrice.Length; i++)
                v1 = v1 + (closePrice[i] - closePrice[i - 1]) / closePrice[i - 1];

            return v1 / (closePrice.Length - 1);
        }

        // ///////////////////////////////
        // Generate learn Output Values
        // ///////////////////////////////
        public double[][] generateLearnOuput(int windowSize)
        {
            return generateLearnOuput(windowSize, 1);
        }

        public double[][] generateLearnOuput(int windowSize, int horizon)
        {

            double[][] dRet;

            if (!useReturns)
            {
                dRet = new double[closePrice.Length - (windowSize)][];

                for (int i = 0; i < closePrice.Length - (windowSize)
                        - (horizon - 1); i++)
                {
                    dRet[i] = new double[horizon];
                    for (int j = 0; j < horizon; j++)
                    {
                        dRet[i][j] = (closePrice[(i + windowSize) + j] / closePriceNormalizer);
                    }
                }
            }
            else
            {

                double[] v = getReturn();
                dRet = new double[v.Length - (windowSize)][];

                for (int i = 0; i < v.Length - (windowSize) - (horizon - 1); i++)
                {
                    dRet[i] = new double[horizon];
                    for (int j = 0; j < horizon; j++)
                    {
                        dRet[i][j] = (v[(i + windowSize) + j] / closePriceRetNormalizer);
                    }
                }
            }

            return dRet;
        }

        // ///////////////////////////////
        // Generate Last Price input Values
        // ///////////////////////////////

        public double[][] generateEvaluateInput(int windowSize)
        {

            double[][] dRet;
            if (!useReturns)
            {
                dRet = new double[closePrice.Length - (windowSize - 1)][];

                for (int i = 0; i < closePrice.Length - (windowSize - 1); i++)
                {
                    dRet[i] = new double[windowSize];
                    for (int x = 0; x < windowSize; x++)
                    {
                        dRet[i][x] = (closePrice[i + x] / closePriceNormalizer);
                    }
                }
            }
            else
            {
                double[] v = getReturn();
                dRet = new double[v.Length - (windowSize - 1)][];

                for (int i = 0; i < v.Length - (windowSize - 1); i++)
                {
                    dRet[i] = new double[windowSize];
                    for (int x = 0; x < windowSize; x++)
                    {
                        dRet[i][x] = (v[i + x] / closePriceRetNormalizer);
                    }
                }
            }
            return dRet;
        }

        public double[][] generateLearnInput(int windowSize, int horizon)
        {

            double[][] dRet;

            if (!useReturns)
            {
                dRet = new double[closePrice.Length - (windowSize) - (horizon - 1)][];

                for (int i = 0; i < closePrice.Length - (windowSize)
                        - (horizon - 1); i++)
                {
                    dRet[i] = new double[windowSize];
                    for (int x = 0; x < windowSize; x++)
                    {
                        dRet[i][x] = (closePrice[i + x] / closePriceNormalizer);
                    }
                }
            }
            else
            {

                double[] v = getReturn();
                dRet = new double[v.Length - (windowSize) - (horizon - 1)][];

                for (int i = 0; i < v.Length - (windowSize) - (horizon - 1); i++)
                {
                    dRet[i] = new double[windowSize];
                    for (int x = 0; x < windowSize; x++)
                    {
                        dRet[i][x] = (v[i + x] / closePriceRetNormalizer);
                    }
                }
            }

            return dRet;
        }

        // ///////////////////////////////
        // Generate Last Price + Volume input Values
        // ///////////////////////////////

        public double[][] generateEvaluateInput2(int windowSize)
        {

            double[][] dRet;
            if (!useReturns)
            {
                dRet = new double[closePrice.Length - (windowSize - 1)][];

                int z = 0;
                for (int i = 0; i < closePrice.Length - (windowSize - 1); i++)
                {
                    dRet[i] = new double[windowSize];
                    z = 0;
                    for (int x = 0; x < windowSize; x = x + 2)
                    {
                        dRet[i][x] = (closePrice[i + z] / closePriceNormalizer);
                        dRet[i][x + 1] = (volume[i + z] / volumeNormalizer);
                        z++;
                    }
                }
            }
            else
            {
                double[] priceReturns = getReturn();
                double[] volumeReturns = getReturn(volume);

                dRet = new double[priceReturns.Length - (windowSize - 1)][];

                int z = 0;
                for (int i = 0; i < priceReturns.Length - (windowSize - 1); i++)
                {
                    dRet[i] = new double[windowSize];
                    z = 0;
                    for (int x = 0; x < windowSize; x = x + 2)
                    {
                        dRet[i][x] = (priceReturns[i + z] / closePriceRetNormalizer);
                        dRet[i][x + 1] = (volumeReturns[i + z] / volumeRetNormalizer);
                        z++;
                    }
                }
            }
            return dRet;
        }

        public double[][] generateLearnInput2(int windowSize)
        {

            double[][] dRet;

            if (!useReturns)
            {
                dRet = new double[closePrice.Length - (windowSize)][];

                int z = 0;
                for (int i = 0; i < closePrice.Length - (windowSize); i++)
                {
                    dRet[i] = new double[windowSize];
                    z = 0;
                    for (int x = 0; x < windowSize; x = x + 2)
                    {
                        dRet[i][x] = (closePrice[i + z] / closePriceNormalizer);
                        dRet[i][x + 1] = (volume[i + z] / volumeNormalizer);
                        z++;
                    }
                }
            }
            else
            {
                double[] priceReturns = getReturn();
                double[] volumeReturns = getReturn(volume);

                dRet = new double[priceReturns.Length - (windowSize)][];

                int z = 0;
                for (int i = 0; i < priceReturns.Length - (windowSize); i++)
                {
                    dRet[i] = new double[windowSize];
                    z = 0;
                    for (int x = 0; x < windowSize; x = x + 2)
                    {
                        dRet[i][x] = (priceReturns[i + z] / closePriceRetNormalizer);
                        dRet[i][x + 1] = (volumeReturns[i + z] / volumeRetNormalizer);
                        z++;
                    }
                }
            }
            return dRet;
        }

        public void calculateNormalizer()
        {

            closePriceNormalizer = normalize(bigger(getClosePrice()));
            closePriceRetNormalizer = normalize(bigger(getReturn(getClosePrice())));

            volumeNormalizer = normalize(bigger(getVolume()));
            volumeRetNormalizer = normalize(bigger(getReturn(getVolume())));
        }

        public double getClosePriceRetNormalizer()
        {
            return closePriceRetNormalizer;
        }

        public double getClosePriceNormalizer()
        {
            return closePriceNormalizer;
        }

        public double getVolumeRetNormalizer()
        {
            return volumeRetNormalizer;
        }

        public double getVolumeNormalizer()
        {
            return volumeNormalizer;
        }

        // /////////////////////////////
        /*
        public DefaultHighLowDataset getHighLowDataset()
        {
            return getHighLowDataset(false);
        }

        public DefaultHighLowDataset getHighLowDataset(bool showVolume)
        {

            double[] lvolume;

            if (showVolume)
                lvolume = volume;
            else
            {
                lvolume = new double[volume.Length];
                for (int i = 0; i < volume.Length; i++)
                {
                    lvolume[i] = 0;
                }
            }
            return new DefaultHighLowDataset(codNeg + " prices", date, higherPrice,
                    lowerPrice, openPrice, closePrice, lvolume);
        }
        */
        // /////////////////////////////
        public static double[] getReturn(double[] v)
        {

            double[] v1 = new double[v.Length];
            v1[0] = 0;

            for (int i = 1; i < v.Length; i++)
            {
                v1[i] = fixNumber((v[i] - v[i - 1]) / v[i - 1]);
            }

            return v1;
        }

        public static double getAverageReturn(double[] v)
        {

            double v1 = 0;

            for (int i = 1; i < v.Length; i++)
                v1 = v1 + (v[i] - v[i - 1]) / v[i - 1];

            return v1 / (v.Length - 1);
        }

        // ///////////////////////////////////

        public static double bigger(double[] v)
        {
            double ret = 0;

            for (int i = 0; i < v.Length; i++)
            {

                if (v[i] > ret)
                    ret = v[i];

            }

            return ret;
        }

        public static double normalize(double v)
        {
            double ret = 1;

            if (v >= 1000000000)
                ret = 10000000000.0;
            else if (v >= 100000000)
                ret = 1000000000;
            else if (v >= 10000000)
                ret = 100000000;
            else if (v >= 1000000)
                ret = 10000000;
            else if (v >= 100000)
                ret = 1000000;
            else if (v >= 10000)
                ret = 100000;
            else if (v < 10000 && v >= 1000)
                ret = 10000;
            else if (v < 1000 && v >= 100)
                ret = 1000;
            else if (v < 100 && v >= 10)
                ret = 100;
            else if (v < 10 && v >= 1)
                ret = 10;
            else if (v < 1 && v >= 0.1)
                ret = 1;
            else if (v < 0.1 && v >= 0.01)
                ret = 0.1;

            return ret;
        }

        public static double fixNumber(double d)
        {
            double ret = 0;

            if (Double.IsInfinity(d) || Double.IsNaN(d))
                ret = 0;
            else
                ret = d;

            return ret;

        }

        public bool isUseReturns()
        {
            return useReturns;
        }

        public void setUseReturns(bool useReturns)
        {
            this.useReturns = useReturns;
        }

        public void setNormalizer(bool useNormalizer)
        {
            this.useNormalizer = useNormalizer;
        }




        /// <summary>
        /// /////////////////
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>

        public static String stringToString(String s)
        {
            return s.Trim();
        }

        public static double stringToDouble(String s)
        {
            return stringToDouble(s, 0);
        }

        public static double stringToDouble(String s, int dec)
        {
            double ret = Double.MinValue;
            String sep = ".";
            String d = s.Substring(0, s.Length - dec) + sep
                    + s.Substring(s.Length - dec);

            try
            {
                ret = Double.Parse(d);
            }
            catch (Exception e)
            {

            }
            return ret;

        }

        public static long stringToLong(String s)
        {
            long ret = long.MinValue;

            try
            {
                ret = long.Parse(s);
            }
            catch (Exception e)
            {

            }
            return ret;

        }

        public static int stringToInt(String s)
        {
            int ret = int.MinValue;

            try
            {
                ret = int.Parse(s);
            }
            catch (Exception e)
            {

            }
            return ret;

        }

        public static DateTime stringToDate(String s, String mask = "yyyyMMdd")
        {
            DateTime ret;

            try
            {
                ret = DateTime.ParseExact(s, mask, null);
            }
            catch (Exception e)
            {
                ret = DateTime.MinValue;
            }
            return ret;

        }

        /* Folder Methods */
        protected bool folderExists(String folder)
        {

            return Directory.Exists(folder);
        }

        protected bool createFolder(String folder)
        {

            try
            {
                Directory.CreateDirectory(folder);
                return true;
            }
            catch { }
            return false;

        }

        protected bool deleteFolder(String folder)
        {
            try
            {
                Directory.Delete(folder);
                return true;
            }
            catch { }
            return false;
        }

        /* File Methods */
        protected bool fileExists(String filename)
        {
            return File.Exists(filename);
        }

        protected bool deleteFile(String filename)
        {
            try
            {
                File.Delete(filename);
                return true;
            }
            catch { }
            return false;
        }

    }

}
