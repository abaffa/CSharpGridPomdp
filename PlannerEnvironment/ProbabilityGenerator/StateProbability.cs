using PlannerEnvironment.Context;
using PlannerInterfaces.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraderWhatever.Business;

namespace PlannerEnvironment.ProbabilityGenerator
{
    public class StateProbability
    {

        LogControl log;
        State state;
        Series series;

        public StateProbability(State state, LogControl log)
        {
            this.state = state;

            this.log = log;
        }

        public void setSeries(Series series)
        {
            this.series = series;
        }

        public List<String> cotacao(String type)
        {

            List<String> ret = new List<String>();
            int oldperc = -1;
            log.writeln("Starting " + type);

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

                    Dictionary<String, String> priceDefs = state.getStateDefinitions()
                            .getPriceDefHash();

                    

                    foreach(String key in priceDefs.Keys)
                    {

                        if (key.IndexOf("VAL") > 0
                                && key.IndexOf("VAL") < key.Length - 3)
                        {
                            String[] equations = key.Split(new string[] { "VAL" }, StringSplitOptions.None);

                            ;
                            double val1 = eval(equations[0] + val.ToString());

                            ;
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

        public double eval(String key)
        {
            double ret = Convert.ToDouble(new DataTable().Compute(key, null));
            // System.out.println(key + " : " + (ret == 1));

            return ret;
        }

        // /////////
        // Generate Transition tables
        // ////////

        List<String> estados;
        List<String> stateDefList;
        List<List<Double>> matrizTransicoes;
        List<String> stateList;

        public List<String> getStateSeries()
        {
            return estados;
        }

        public String generateTables()
        {

            List<List<String>> seriesList = new List<List<String>>();

            stateList = state.stateList(state.getSettings());
            stateDefList = state.getStateDefinitions().getStateDefs();

            // /////////////////////////////////////////////
            String[] aDef = state.getSettings().Split('#');

            // processa cotacoes
            for (int i = 0; i < aDef.Length; i++)
                seriesList.Add(cotacao(aDef[i]));

            // transforma cotacoes em estados de preco
            estados = new List<String>();
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

                estados.Add(linha);
            }
            // /////////////////////////////////////////////

            // pega lista de possiveis transicoes de preco
            List<String> stateListFull = StateColRow(state.getSettings(),
                    stateList, stateDefList);

            // Transicoes de estado de todas as series
            List<String> transicoesEstados = new List<String>();
            for (int i = 0; i < estados.Count; i++)
            {

                if (i > 0)
                {
                    if (!estados[i - 1].Equals("")
                            && !estados[i].Equals(""))
                        transicoesEstados.Add(estados[i - 1] + "#"
                                + estados[i]);
                    else
                        transicoesEstados.Add("");
                }
                else
                    transicoesEstados.Add("");

                // System.out.println(transicoesEstados
                // .get(transicoesEstados.Count - 1));
            }

            // cria matriz basica de transicao com valores
            matrizTransicoes = new List<List<Double>>();
            for (int row = 0; row < stateList.Count; row++)
            {
                List<Double> rowList = new List<Double>();

                for (int col = 0; col < stateList.Count; col++)
                {

                    String item = stateList[row] + "#" + stateList[col];
                    double count = 0;

                    for (int i = 0; i < transicoesEstados.Count; i++)
                        if (transicoesEstados[i].Equals(item))
                            count++;

                    rowList.Add(count);
                }
                matrizTransicoes.Add(rowList);
            }

            // matriiz de transicao completa para todos os estados de compra venda
            // etc...

            ActionDefinitions actionDefinitions = new ActionDefinitions();
            List<String> actionDef = actionDefinitions.getActionDefs();

            String matrixTransitions = "";
            for (int i = 0; i < actionDef.Count; i++)
            {
                List<List<Double>> matrizTransicoesFull = createFullMatrix();

                matrixTransitions = matrixTransitions
                        + "\n"
                        + createActionStateMatrix(actionDef[i], stateListFull,
                                matrizTransicoesFull);
            }

            return matrixTransitions;
        }

    // ///////////////////////////////////////
    

    public String createActionStateMatrix(String action,
            List<String> stateListFull,
            List<List<Double>> matrizTransicoesFull)
        {

            String matrix = "";

            ActionDefinitions actionDefinitions = new ActionDefinitions();


            List<String> rowColName = stateListFull.Select(x => (String)x.Clone()).ToList();

            List<List<Double>> matriz = matrizTransicoesFull.Select(x => x.ToList()).ToList();

            List<List<Double>> matrizPerc = new List<List<Double>>();

            rowColName.Add("erro");

            String validTransitions = actionDefinitions.getActionDefHash()[action];

            for (int row = 0; row < matriz.Count; row++)
            {
                String rowName = rowColName[row];
                rowName = rowName.Substring(rowName.IndexOf("_") + 1);
                for (int col = 0; col < matriz[row].Count; col++)
                {
                    String colName = rowColName[col];
                    colName = colName.Substring(colName.IndexOf("_") + 1);

                    String transitionName = rowName + "-" + colName;

                    if (!colName.Equals("erro"))
                    {
                        if (validTransitions.IndexOf(transitionName) == -1)
                            matriz[row][col] = 0.0;
                    }

                }
            }

            // transforma matriz em percentual
            matrizPerc = numToPerc(matriz);
            //matrizPerc = numToPerc(matrizTransicoesFull);

            matrix = "T:" + action + "\n";
            matrix = matrix + matrixToString(matrizPerc) + "\n";

            return matrix;
        }

        // ///////////////////////////////////////
        public List<List<Double>> createFullMatrix()
        {
            List<List<Double>> matrizTransicoesFull = new List<List<Double>>();
            for (int i = 0; i < stateDefList.Count; i++)
            {
                for (int row = 0; row < matrizTransicoes.Count; row++)
                {
                    List<Double> rowList = new List<Double>();
                    for (int j = 0; j < stateDefList.Count; j++)
                    {
                        for (int col = 0; col < matrizTransicoes[row].Count; col++)
                        {
                            rowList.Add(matrizTransicoes[row][col]);
                        }
                    }
                    rowList.Add(0.0); // erro
                    matrizTransicoesFull.Add(rowList);
                }
            }

            // linha erro
            {
                List<Double> rowList = new List<Double>();
                for (int j = 0; j < stateDefList.Count; j++)
                {
                    for (int col = 0; col < matrizTransicoes[0].Count; col++)
                    {
                        rowList.Add(0.0);
                    }
                }
                rowList.Add(1.0); // erro
                matrizTransicoesFull.Add(rowList);
            }

            return matrizTransicoesFull;
        }


    
    // transforma matriz em percentual
        public List<List<Double>> numToPerc(List<List<Double>> m)
        {
            List<List<Double>> matriz = m.Select(x => x.ToList()).ToList();

            for (int row = 0; row < matriz.Count; row++)
            {
                double sum = 0;
                for (int col = 0; col < matriz[row].Count; col++)
                {
                    sum = sum + matriz[row][col];
                }

                for (int col = 0; col < matriz[row].Count; col++)
                {
                    if (sum == 0)
                    {
                        if (col == matriz[row].Count - 1)
                            matriz[row][col] = 1.0;
                        else
                            matriz[row][col] = 0.0;
                    }
                    else
                        matriz[row][col] = matriz[row][col] / sum;
                }

            }

            return matriz;
        }

        // matrix to string
        public String matrixToString(List<List<Double>> m)
        {

            String matriz = "";

            
            for (int row = 0; row < m.Count; row++)
            {
                if (row > 0)
                    matriz = matriz + "\n";

                for (int col = 0; col < m[row].Count; col++)
                {
                    if (col > 0)
                        matriz = matriz + "\t";

                    matriz = matriz
                            + m[row][col].ToString("0.0000000");
                }

            }

            return matriz;
        }

        // //////////////////////////
        public List<String> StateColRow()
        {

            return StateColRow(state.getSettings());
        }

        public List<String> StateColRow(String definition)
        {
            state.setSettings(definition);
            List<String> stateList = state.stateList(definition);
            List<String> stateDefList = state.getStateDefinitions().getStateDefs();

            return StateColRow(definition, stateList, stateDefList);
        }

        public List<String> StateColRow(String definition,
                List<String> stateList, List<String> stateDefList)
        {

            // pega lista de possiveis transicoes de preco
            List<String> stateListFull = new List<String>();
            for (int i = 0; i < stateDefList.Count; i++)
            {
                for (int j = 0; j < stateList.Count; j++)
                {
                    stateListFull.Add(stateList[j] + "_" + stateDefList[i]);

                }
            }

            return stateListFull;
        }

    }

}
