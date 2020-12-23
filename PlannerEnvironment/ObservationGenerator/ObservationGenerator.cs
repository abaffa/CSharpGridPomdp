using PlannerEnvironment.Context;
using PlannerEnvironment.TechnicalAnalysis;
using PlannerInterfaces.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraderWhatever.Business;

namespace PlannerEnvironment.ObservationGenerator
{
    public class ObservationGenerator
    {

        LogControl log;

        Observation observation;

        Series series;
        List<String> stateSeries;
        List<String> stateList;

        List<String> observationList;
        List<String> observacoes;

        PluginAbstract plugin = null;

        public ObservationGenerator(Observation observation, LogControl log)
        {
            this.observation = observation;

            this.log = log;
        }

        public Observation getObservation()
        {
            return observation;
        }

        public List<String> getStateSeries()
        {
            return stateSeries;
        }

        public List<String> getStateList()
        {
            return stateList;
        }

        public List<String> getObservationList()
        {
            return observationList;
        }

        public void setSeries(Series series)
        {
            this.series = series;
        }

        public String getDefault()
        {

            String ret = "";
            String[] aDef = observation.getObservationDefinition().Split('#');

            for (int i = 0; i < aDef.Length; i++)
            {
                String type = aDef[i].Substring(0, 1);

                if (type.Equals("p") || type.Equals("v") || type.Equals("q"))
                    ret = ret + "-" + observation.getStates().getStateDefinitions().getDefault();
                else
                    ret = ret + "-"
                            + (new ObservationDefinitions(aDef[i])).getDefault();

            }

            if (ret.IndexOf("-") == 0 && ret.Length > 1)
                ret = ret.Substring(1);

            return ret;
        }

        public void setStateList(List<String> stateList)
        {
            this.stateList = stateList;
        }

        public void setStateSeries(List<String> stateSeries)
        {
            this.stateSeries = stateSeries;
        }

        public List<String> cotacao(String type)
        {

            List<String> ret = new List<String>();
            int oldperc = -1;
            log.writeln("Starting Observation " + type);

            // String cod = type.Substring(0, 1);
            // String mod = type.Substring(1, 2);
            int period = 0;

            if (type.Length > 2)
                period = int.Parse(type.Substring(2));

            for (int i = 0; i < series.getDate().Length; i++)

                if (i < (period))
                    ret.Add("");
                else if (i >= (series.getDate().Length - (-period)))
                    ret.Add("");
                else
                {
                    double val = 0;

                    if (type.Substring(0, 2).Equals("pv"))
                    {
                        if (period > 0)
                            val = (series.getClosePrice()[i] / series
                                    .getClosePrice()[i - period]) - 1;
                        else
                            val = (series.getClosePrice()[i - period] / series
                                    .getClosePrice()[i]) - 1;
                    }
                    else if (type.Substring(0, 2).Equals("vv"))
                    {
                        if (period > 0)
                            val = (series.getVolume()[i] / series.getVolume()[i
                                    - period]) - 1;
                        else
                            val = (series.getVolume()[i - period] / series
                                    .getVolume()[i]) - 1;
                    }
                    else if (type.Substring(0, 2).Equals("qv"))
                    {
                        if (period > 0)
                            val = (series.getQuantity()[i] / series.getQuantity()[i
                                    - period]) - 1;
                        else
                            val = (series.getQuantity()[i - period] / series
                                    .getQuantity()[i]) - 1;
                    }
                    else

                  if (type.Substring(0, 2).Equals("pm"))
                    {

                        if (period > 0)
                        {
                            double d_1 = 0;
                            double d = 0;

                            for (int x = (i - period); x < i; x++)
                                d_1 = d_1 + series.getClosePrice()[x];

                            d_1 = d_1 / (double)Math.Abs(period);

                            for (int x = (i - period) + 1; x <= i; x++)
                                d = d + series.getClosePrice()[x];

                            d = d / (double)Math.Abs(period);

                            val = (d / d_1) - 1;

                        }
                        else
                        {
                            double d = 0;
                            double d_1 = 0;

                            for (int x = i; x < i - period; x++)
                                d = d + series.getClosePrice()[x];

                            d = d / (double)Math.Abs(period);

                            for (int x = i + 1; x <= (i - period); x++)
                                d_1 = d_1 + series.getClosePrice()[x];

                            d_1 = d_1 / (double)Math.Abs(period);

                            val = (d_1 / d) - 1;
                        }

                    }
                    else if (type.Substring(0, 2).Equals("vm"))
                    {

                        if (period > 0)
                        {
                            double d_1 = 0;
                            double d = 0;

                            for (int x = (i - period); x < i; x++)
                                d_1 = d_1 + series.getVolume()[x];

                            d_1 = d_1 / (double)Math.Abs(period);

                            for (int x = (i - period) + 1; x <= i; x++)
                                d = d + series.getVolume()[x];

                            d = d / (double)Math.Abs(period);

                            val = (d / d_1) - 1;

                        }
                        else
                        {
                            double d = 0;
                            double d_1 = 0;

                            for (int x = i; x < i - period; x++)
                                d = d + series.getVolume()[x];

                            d = d / (double)Math.Abs(period);

                            for (int x = i + 1; x <= (i - period); x++)
                                d_1 = d_1 + series.getVolume()[x];

                            d_1 = d_1 / (double)Math.Abs(period);

                            val = (d_1 / d) - 1;
                        }

                    }
                    else if (type.Substring(0, 2).Equals("qm"))
                    {

                        if (period > 0)
                        {
                            double d_1 = 0;
                            double d = 0;

                            for (int x = (i - period); x < i; x++)
                                d_1 = d_1 + series.getQuantity()[x];

                            d_1 = d_1 / (double)Math.Abs(period);

                            for (int x = (i - period) + 1; x <= i; x++)
                                d = d + series.getQuantity()[x];

                            d = d / (double)Math.Abs(period);

                            val = (d / d_1) - 1;

                        }
                        else
                        {
                            double d = 0;
                            double d_1 = 0;

                            for (int x = i; x < i - period; x++)
                                d = d + series.getQuantity()[x];

                            d = d / (double)Math.Abs(period);

                            for (int x = i + 1; x <= (i - period); x++)
                                d_1 = d_1 + series.getQuantity()[x];

                            d_1 = d_1 / (double)Math.Abs(period);

                            val = (d_1 / d) - 1;
                        }

                    }

                    Dictionary<String, String> priceDefs = observation.getStates()
                            .getStateDefinitions().getPriceDefHash();

                    foreach(String key in priceDefs.Keys)
                    {

                        if (key.IndexOf("VAL") > 0
                                && key.IndexOf("VAL") < key.Length - 3)
                        {
                            String[] equations = key.Split(new string[] { "VAL" }, StringSplitOptions.None);

                            double val1 = eval(equations[0] + val.ToString());
                            double val2 = eval(val.ToString() + equations[1]);

                            if (val1 == 1 && val2 == 1)
                            {
                                ret.Add(priceDefs[key]);
                                break;
                            }
                        }
                        else
                        {
                            double vl = eval(key.Replace("VAL", val.ToString()));
                            if (vl == 1)
                            {
                                ret.Add(priceDefs[key]);
                                break;
                            }
                        }
                    }

                    int perc = (int)(((double)i / (double)series.getDate().Length) * 100.0);

                    if ((perc % 10) == 0)
                        if (oldperc != perc)
                        {
                            log.writeln("Processing: " + perc);
                            oldperc = perc;
                        }

                }

            return ret;
        }

        public List<String> analise(String indicador)
        {

            List<String> ret = new List<String>();
            
            try
            {

                if (indicador.Equals("RSI1"))
                {
                    plugin = new TechnicalAnalysis.RSI();
                    plugin.SetSeries(series);
                    ret = processRSI1();

                }
                else if (indicador.Equals("RSI2"))
                {
                    plugin = new TechnicalAnalysis.RSI();
                    plugin.SetSeries(series);
                    ret = processRSI2();

                }
                else if (indicador.Equals("RSI3"))
                {
                    plugin = new TechnicalAnalysis.RSI();
                    plugin.SetSeries(series);
                    ret = processRSI3();

                }
                else if (indicador.Equals("MACD1"))
                {
                    plugin = new TechnicalAnalysis.MACD();
                    plugin.SetSeries(series);
                    ret = processMACD1();

                }
                else if (indicador.Equals("MACD2"))
                {
                    plugin = new TechnicalAnalysis.MACD();
                    plugin.SetSeries(series);
                    ret = processMACD2();

                }
                else if (indicador.Equals("MACD3"))
                {
                    plugin = new TechnicalAnalysis.MACD();
                    plugin.SetSeries(series);
                    ret = processMACD3();

                }
                else if (indicador.Equals("OST1"))
                {
                    plugin = new TechnicalAnalysis.OST();
                    plugin.SetSeries(series);
                    ret = processOST1();

                }
                else if (indicador.Equals("OST2"))
                {
                    plugin = new TechnicalAnalysis.OST();
                    plugin.SetSeries(series);
                    ret = processOST2();

                }
                else if (indicador.Equals("OST3"))
                {
                    plugin = new TechnicalAnalysis.OST();
                    plugin.SetSeries(series);
                    ret = processOST3();
                }

            }
            catch (Exception ex)
            {

            }
            
            return ret;
        }


        public List<String> getObservacoes()
        {
            List<List<String>> seriesList = new List<List<String>>();

            observationList = observation.observationList();
            // observationDefList = definitions.getObservationDefs();

            // /////////////////////////////////////////////
            String[] aDef = observation.getObservationDefinition().Split('#');

            // processa cotacoes
            for (int i = 0; i < aDef.Length; i++)
            {
                String type = aDef[i].Substring(0, 1);

                if (type.Equals("p") || type.Equals("v") || type.Equals("q"))
                    seriesList.Add(cotacao(aDef[i]));
                else
                    seriesList.Add(analise(aDef[i]));

            }
            // transforma cotacoes em estados de preco
            observacoes = new List<String>();
            for (int i = 0; i < seriesList[0].Count; i++)
            {
                bool valid = true;
                String linha = "";
                for (int j = 0; j < seriesList.Count; j++)
                {
                    linha = linha + "-" + seriesList[j][i];
                    if (seriesList[j][i].Trim().Equals(""))
                        valid = false;
                }
                if (!valid)
                    linha = "";
                else
                    linha = linha.Substring(1);

                observacoes.Add(linha);
                // System.out.println(linha);
            }

            return observacoes;
        }





        ///////////////////
        //Obs Generator
        ///////////////////





        public double eval(String key)
        {
            double ret = Convert.ToDouble(new DataTable().Compute(key, null));
            // System.out.println(key + " : " + (ret == 1));

            return ret;
        }

        public bool evalF(String key, double val, double old)
        {

            bool ret = false;

            if (key.IndexOf("VAL") > 0 && key.IndexOf("VAL") < key.Length - 3)
            {
                String[] equations = key.Split(new string[] { "VAL" }, StringSplitOptions.None);

                double val1 = eval(equations[0] + val.ToString());

                double val2 = eval(val.ToString() + equations[1]);

                ret = (val1 == 1 && val2 == 1);

            }
            else if (key.IndexOf("VAL") == 0
                  || key.IndexOf("VAL") == key.Length - 3)
            {

                ret = (eval(key.Replace("VAL", val.ToString())) == 1);
            }
            else if (key.IndexOf("OLD") > 0
                  && key.IndexOf("OLD") < key.Length - 3)
            {
                String[] equations = key.Split(new string[] { "OLD" }, StringSplitOptions.None);

                double val1 = eval(equations[0] + old.ToString());

                double val2 = eval(old.ToString() + equations[1]);

                ret = (val1 == 1 && val2 == 1);

            }
            else if (key.IndexOf("OLD") == 0
                  || key.IndexOf("OLD") == key.Length - 3)
            {

                ret = (eval(key.Replace("OLD", old.ToString())) == 1);
            }

            return ret;
        }

        public bool evalF2(String key, double shortValue, double longValue)
        {

            bool ret = false;

            key = key.Replace("SHORT", shortValue.ToString());
            key = key.Replace("LONG", longValue.ToString());

            ret = (eval(key) == 1);

            return ret;
        }

        public bool evalF3(String key, double macdValue, double signalValue)
        {

            bool ret = false;

            key = key.Replace("MACD", macdValue.ToString());
            key = key.Replace("SIGNAL", signalValue.ToString());

            ret = (eval(key) == 1);

            return ret;
        }





        private List<String> processRSI1()
        {

            Dictionary<String, String> obsDef = (new ObservationDefinitions("RSI1"))
                    .getobservationDefHash();
            List<String> ret = new List<String>();



            List<String> lD = plugin.GetResultDescription();
            List<double[]> lV = plugin.GetResultValues();

            double[] rsiShort = (double[])lV[0];
            double[] rsiLong = (double[])lV[1];

            for (int i = 0; i < rsiShort.Length; i++)
            {
                rsiShort[i] = rsiShort[i] / 100;
            }

            for (int i = 0; i < rsiLong.Length; i++)
            {
                rsiLong[i] = rsiLong[i] / 100;
            }

            int oldperc = -1;
            log.writeln("Starting Observation RSI1");

            for (int x = 0; x < rsiShort.Length; x++)
            {

                if (x > 0)
                {
                    double shortValue = rsiShort[x];
                    double longValue = rsiLong[x];

                    bool resp = false;
                    foreach(String key in obsDef.Keys)
                    {
                        
                        resp = false;

                        if (!key.Equals("else"))
                        {
                            String[] aKey = key.Split('!');

                            int bTrue = 0;

                            for (int y = 0; y < aKey.Length; y++)
                            {
                                if (evalF2(aKey[y], shortValue, longValue))
                                    bTrue++;
                            }

                            resp = (bTrue == aKey.Length);
                        }

                        if (resp)
                        {
                            ret.Add(obsDef[key]);
                            break;
                        }
                    }

                    if (!resp)
                        ret.Add(obsDef["else"]);

                }
                else
                    ret.Add(obsDef["else"]);

                int perc = (int)(((double)x / (double)rsiShort.Length) * 100.0);

                if ((perc % 10) == 0)
                    if (oldperc != perc)
                    {
                        log.writeln("Processing: " + perc);
                        oldperc = perc;
                    }

            }
            return ret;
        }




        private List<String> processRSI2()
        {

            Dictionary<String, String> obsDef = (new ObservationDefinitions("RSI2"))
                    .getobservationDefHash();
            List<String> ret = new List<String>();



            List<String> lD = plugin.GetResultDescription();
            List<double[]> lV = plugin.GetResultValues();

            List<Double> rsi = new List<Double>();

            double[] rsiShort = (double[])lV[0];


            double[] rsiLong = (double[])lV[1];

            for (int i = 0; i < rsiShort.Length; i++)
            {
                rsi.Add(rsiShort[i] / 100);
            }

            int oldperc = -1;
            log.writeln("Starting Observation RSI2");

            for (int x = 0; x < rsi.Count; x++)
            {

                if (x > 0)
                {
                    double old = rsi[x - 1];
                    double val = rsi[x];

                    bool resp = false;
                    foreach (String key in obsDef.Keys)
                    {
                        resp = false;

                        if (!key.Equals("else"))
                        {
                            String[] aKey = key.Split('!');

                            int bTrue = 0;

                            for (int y = 0; y < aKey.Length; y++)
                            {
                                if (evalF(aKey[y], val, old))
                                    bTrue++;
                            }

                            resp = (bTrue == aKey.Length);
                        }

                        if (resp)
                        {
                            ret.Add(obsDef[key]);
                            break;
                        }
                    }

                    if (!resp)
                        ret.Add(obsDef["else"]);

                }
                else
                    ret.Add(obsDef["else"]);

                int perc = (int)(((double)x / (double)rsi.Count) * 100.0);

                if ((perc % 10) == 0)
                    if (oldperc != perc)
                    {
                        log.writeln("Processing: " + perc);
                        oldperc = perc;
                    }

            }
            return ret;
        }




        private List<String> processRSI3()
        {

            Dictionary<String, String> obsDef = (new ObservationDefinitions("RSI3"))
                    .getobservationDefHash();
            List<String> ret = new List<String>();



            List<String> lD = plugin.GetResultDescription();
            List<double[]> lV = plugin.GetResultValues();

            List<Double> rsi = new List<Double>();

            double[] rsiShort = (double[])lV[0];


            double[] rsiLong = (double[])lV[1];

            for (int i = 0; i < rsiShort.Length; i++)
            {
                rsi.Add(rsiShort[i] / 100);
            }

            int oldperc = -1;
            log.writeln("Starting Observation RSI3");

            for (int x = 0; x < rsi.Count; x++)
            {

                if (x > 0)
                {
                    double old = rsi[x - 1];
                    double val = rsi[x];

                    bool resp = false;
                    foreach (String key in obsDef.Keys)
                    {
                        resp = false;

                        if (!key.Equals("else"))
                        {
                            String[] aKey = key.Split('!');

                            int bTrue = 0;

                            for (int y = 0; y < aKey.Length; y++)
                            {
                                if (evalF(aKey[y], val, old))
                                    bTrue++;
                            }

                            resp = (bTrue == aKey.Length);
                        }

                        if (resp)
                        {
                            ret.Add(obsDef[key]);
                            break;
                        }
                    }

                    if (!resp)
                        ret.Add(obsDef["else"]);

                }
                else
                    ret.Add(obsDef["else"]);

                int perc = (int)(((double)x / (double)rsi.Count) * 100.0);

                if ((perc % 10) == 0)
                    if (oldperc != perc)
                    {
                        log.writeln("Processing: " + perc);
                        oldperc = perc;
                    }

            }
            return ret;
        }




        private List<String> processMACD1()
        {

            Dictionary<String, String> obsDef = (new ObservationDefinitions("MACD1"))
                    .getobservationDefHash();
            List<String> ret = new List<String>();



            List<String> lD = plugin.GetResultDescription();
            List<double[]> lV = plugin.GetResultValues();

            double[] macd = (double[])lV[0];

            double[] signal = (double[])lV[1];


            double[] histogram = (double[])lV[2];

            int oldperc = -1;
            log.writeln("Starting Observation MACD1");

            for (int x = 0; x < macd.Length; x++)
            {

                if (x > 0)
                {
                    double macdValue = macd[x];
                    double sgnlValue = signal[x];

                    bool resp = false;
                    foreach (String key in obsDef.Keys)
                    {

                        resp = false;

                        if (!key.Equals("else"))
                        {
                            String[] aKey = key.Split('!');

                            int bTrue = 0;

                            for (int y = 0; y < aKey.Length; y++)
                            {
                                if (evalF3(aKey[y], macdValue, sgnlValue))
                                    bTrue++;
                            }

                            resp = (bTrue == aKey.Length);
                        }

                        if (resp)
                        {
                            ret.Add(obsDef[key]);
                            break;
                        }
                    }

                    if (!resp)
                        ret.Add(obsDef["else"]);

                }
                else
                    ret.Add(obsDef["else"]);

                int perc = (int)(((double)x / (double)macd.Length) * 100.0);

                if ((perc % 10) == 0)
                    if (oldperc != perc)
                    {
                        log.writeln("Processing: " + perc);
                        oldperc = perc;
                    }

            }
            return ret;
        }




        private List<String> processMACD2()
        {

            Dictionary<String, String> obsDef = (new ObservationDefinitions("MACD2"))
                    .getobservationDefHash();
            List<String> ret = new List<String>();



            List<String> lD = plugin.GetResultDescription();
            List<double[]> lV = plugin.GetResultValues();

            double[] macd = (double[])lV[0];


            double[] signal = (double[])lV[1];


            double[] histogram = (double[])lV[2];

            int oldperc = -1;
            log.writeln("Starting Observation MACD2");

            for (int x = 0; x < macd.Length; x++)
            {

                if (x > 0)
                {
                    double old = macd[x - 1];
                    double val = macd[x];

                    bool resp = false;
                    foreach (String key in obsDef.Keys)
                    {

                        resp = false;

                        if (!key.Equals("else"))
                        {
                            String[] aKey = key.Split('!');

                            int bTrue = 0;

                            for (int y = 0; y < aKey.Length; y++)
                            {
                                if (evalF(aKey[y], val, old))
                                    bTrue++;
                            }

                            resp = (bTrue == aKey.Length);
                        }

                        if (resp)
                        {
                            ret.Add(obsDef[key]);
                            break;
                        }
                    }

                    if (!resp)
                        ret.Add(obsDef["else"]);

                }
                else
                    ret.Add(obsDef["else"]);

                int perc = (int)(((double)x / (double)macd.Length) * 100.0);

                if ((perc % 10) == 0)
                    if (oldperc != perc)
                    {
                        log.writeln("Processing: " + perc);
                        oldperc = perc;
                    }

            }
            return ret;
        }




        private List<String> processMACD3()
        {

            Dictionary<String, String> obsDef = (new ObservationDefinitions("MACD3"))
                    .getobservationDefHash();
            List<String> ret = new List<String>();



            List<String> lD = plugin.GetResultDescription();
            List<double[]> lV = plugin.GetResultValues();



            double[] macd = (double[])lV[0];


            double[] signal = (double[])lV[1];

            double[] histogram = (double[])lV[2];

            int oldperc = -1;
            log.writeln("Starting Observation MACD3");

            for (int x = 0; x < histogram.Length; x++)
            {

                if (x > 0)
                {
                    double old = histogram[x - 1];
                    double val = histogram[x];

                    bool resp = false;
                    foreach (String key in obsDef.Keys)
                    {

                        resp = false;

                        if (!key.Equals("else"))
                        {
                            String[] aKey = key.Split('!');

                            int bTrue = 0;

                            for (int y = 0; y < aKey.Length; y++)
                            {
                                if (evalF(aKey[y], val, old))
                                    bTrue++;
                            }

                            resp = (bTrue == aKey.Length);
                        }

                        if (resp)
                        {
                            ret.Add(obsDef[key]);
                            break;
                        }
                    }

                    if (!resp)
                        ret.Add(obsDef["else"]);

                }
                else
                    ret.Add(obsDef["else"]);

                int perc = (int)(((double)x / (double)histogram.Length) * 100.0);

                if ((perc % 10) == 0)
                    if (oldperc != perc)
                    {
                        log.writeln("Processing: " + perc);
                        oldperc = perc;
                    }

            }
            return ret;
        }




        private List<String> processOST1()
        {

            Dictionary<String, String> obsDef = (new ObservationDefinitions("OST1"))
                    .getobservationDefHash();
            List<String> ret = new List<String>();



            List<String> lD = plugin.GetResultDescription();
            List<double[]> lV = plugin.GetResultValues();

            List<Double> ost = new List<Double>();

            double[] ostShort = (double[])lV[0];
            // 
            double[] ostLong = (double[])lV[1];

            for (int i = 0; i < ostShort.Length; i++)
            {
                // ost.Add(ostShort[i] / 100);
                ost.Add(ostShort[i] - ostLong[i]);
            }

            int oldperc = -1;
            log.writeln("Starting Observation OST1");

            for (int x = 0; x < ost.Count; x++)
            {

                if (x > 0)
                {
                    double old = ost[x - 1];
                    double val = ost[x];

                    bool resp = false;
                    foreach (String key in obsDef.Keys)
                    {

                        resp = false;

                        if (!key.Equals("else"))
                        {
                            String[] aKey = key.Split('!');

                            int bTrue = 0;

                            for (int y = 0; y < aKey.Length; y++)
                            {
                                if (evalF(aKey[y], val, old))
                                    bTrue++;
                            }

                            resp = (bTrue == aKey.Length);
                        }

                        if (resp)
                        {
                            ret.Add(obsDef[key]);
                            break;
                        }
                    }

                    if (!resp)
                        ret.Add(obsDef["else"]);

                }
                else
                    ret.Add(obsDef["else"]);

                int perc = (int)(((double)x / (double)ost.Count) * 100.0);

                if ((perc % 10) == 0)
                    if (oldperc != perc)
                    {
                        log.writeln("Processing: " + perc);
                        oldperc = perc;
                    }

            }
            return ret;
        }




        private List<String> processOST2()
        {

            Dictionary<String, String> obsDef = (new ObservationDefinitions("OST2"))
                    .getobservationDefHash();
            List<String> ret = new List<String>();



            List<String> lD = plugin.GetResultDescription();
            List<double[]> lV = plugin.GetResultValues();

            List<Double> ost = new List<Double>();

            double[] ostShort = (double[])lV[0];


            double[] ostLong = (double[])lV[1];

            for (int i = 0; i < ostShort.Length; i++)
            {

                ost.Add(ostShort[i]);
            }

            int oldperc = -1;
            log.writeln("Starting Observation OST2");

            for (int x = 0; x < ost.Count; x++)
            {

                if (x > 0)
                {
                    double old = ost[x - 1];
                    double val = ost[x];

                    bool resp = false;
                    foreach (String key in obsDef.Keys)
                    {

                        resp = false;

                        if (!key.Equals("else"))
                        {
                            String[] aKey = key.Split('!');

                            int bTrue = 0;

                            for (int y = 0; y < aKey.Length; y++)
                            {
                                if (evalF(aKey[y], val, old))
                                    bTrue++;
                            }

                            resp = (bTrue == aKey.Length);
                        }

                        if (resp)
                        {
                            ret.Add(obsDef[key]);
                            break;
                        }
                    }

                    if (!resp)
                        ret.Add(obsDef["else"]);

                }
                else
                    ret.Add(obsDef["else"]);

                int perc = (int)(((double)x / (double)ost.Count) * 100.0);

                if ((perc % 10) == 0)
                    if (oldperc != perc)
                    {
                        log.writeln("Processing: " + perc);
                        oldperc = perc;
                    }

            }
            return ret;
        }




        private List<String> processOST3()
        {

            Dictionary<String, String> obsDef = (new ObservationDefinitions("OST3"))
                    .getobservationDefHash();
            List<String> ret = new List<String>();



            List<String> lD = plugin.GetResultDescription();
            List<double[]> lV = plugin.GetResultValues();

            List<Double> ost = new List<Double>();



            double[] ostShort = (double[])lV[0];

            double[] ostLong = (double[])lV[1];

            for (int i = 0; i < ostLong.Length; i++)
            {
                ost.Add(ostLong[i]);
            }

            int oldperc = -1;
            log.writeln("Starting Observation OST3");

            for (int x = 0; x < ost.Count; x++)
            {

                if (x > 0)
                {
                    double old = ost[x - 1];
                    double val = ost[x];

                    bool resp = false;
                    foreach (String key in obsDef.Keys)
                    {

                        resp = false;

                        if (!key.Equals("else"))
                        {
                            String[] aKey = key.Split('!');

                            int bTrue = 0;

                            for (int y = 0; y < aKey.Length; y++)
                            {
                                if (evalF(aKey[y], val, old))
                                    bTrue++;
                            }

                            resp = (bTrue == aKey.Length);
                        }

                        if (resp)
                        {
                            ret.Add(obsDef[key]);
                            break;
                        }
                    }

                    if (!resp)
                        ret.Add(obsDef["else"]);

                }
                else
                    ret.Add(obsDef["else"]);

                int perc = (int)(((double)x / (double)ost.Count) * 100.0);

                if ((perc % 10) == 0)
                    if (oldperc != perc)
                    {
                        log.writeln("Processing: " + perc);
                        oldperc = perc;
                    }

            }
            return ret;
        }

    }

}
