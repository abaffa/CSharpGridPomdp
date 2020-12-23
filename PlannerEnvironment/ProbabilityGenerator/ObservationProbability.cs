using PlannerInterfaces.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerEnvironment.ProbabilityGenerator
{
    public class ObservationProbability
    {

        LogControl log;

        ObservationGenerator.ObservationGenerator observationGenerator;

        public ObservationProbability(ObservationGenerator.ObservationGenerator observationGenerator, LogControl log)
        {
            this.observationGenerator = observationGenerator; ;

            this.log = log;
        }

        public String generateTables()
        {

            String ret = "";

            List<String> observacoes = observationGenerator.getObservacoes();
            List<String> stateSeries = observationGenerator.getStateSeries();
            List<String> stateList = observationGenerator.getStateList();
            List<String> observationList = observationGenerator.getObservationList();
            // /////////////////////////////////////////////

            List<String> observacoesFull = new List<String>();
            for (int i = 0; i < stateSeries.Count; i++)
            {
                if (!stateSeries[i].Equals("")
                        && !observacoes[i].Equals(""))
                    observacoesFull.Add(stateSeries[i] + "#"
                            + observacoes[i]);
                else
                    observacoesFull.Add("");

                // System.out.println(observacoesFull.get(observacoesFull.Count-1));
            }

            // /////////////////////////////////////////////

            List<List<Double>> matrizObservacoes = new List<List<Double>>();
            for (int row = 0; row < stateList.Count; row++)
            {
                List<Double> rowList = new List<Double>();

                for (int col = 0; col < observationList.Count; col++)
                {

                    String item = "";
                    if (stateList[row].IndexOf("_") > -1)
                        item = stateList[row].Substring(0,
                                stateList[row].IndexOf("_"))
                                + "#" + observationList[col];
                    else
                        item = stateList[row] + "#" + observationList[col];

                    double count = 0;

                    for (int i = 0; i < observacoesFull.Count; i++)
                        if (observacoesFull[i].Equals(item))
                            count++;

                    rowList.Add(count);
                    // System.out.print("\t" + item);
                }
                matrizObservacoes.Add(rowList);

                // System.out.println("");
            }

            List<List<Double>> obsPerc = numToPerc(matrizObservacoes);

            ret = "O: *\n";
            ret = ret + matrixToString(obsPerc);
            ret = ret + "\n";

            return ret;
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
                        matriz[row][col] = 1.0 / matriz[row].Count;
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


    }

}
