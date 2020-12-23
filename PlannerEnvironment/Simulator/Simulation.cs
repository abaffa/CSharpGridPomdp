using PlannerEnvironment.Context;
using PlannerEnvironment.ProbabilityGenerator;
using PlannerInterfaces.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraderWhatever.Business;

namespace PlannerEnvironment.Simulator
{
    public class Simulation
    {

        String twitLog = "";

        LogControl log;

        State s;
        StateProbability sProb;
        Observation o;
        ObservationGenerator.ObservationGenerator oGen;
        ObservationProbability oProb;
        Context.Action a;

        String planProjectName = "";
        String projectName = "temp";
        // String pomdpFileName = projectName + ".pomdp";
        // String alphaFileName = projectName + ".alpha";
        String polFileName =  "temp.pg";
        String simulFileName =  "temp.simul";
        String resultFileName =  "temp.result";
        String simulProjFileName =  "temp.simulProj";

        String simulLog = "";
        String resultLog = "";

        int initialNode = -1;
        int nextNode = -1; // initialNode;

        List<List<int>> policy = new List<List<int>>();

        public Simulation(State s, Observation o, Context.Action a, String projectName,
                LogControl log)
        {
            this.s = s;
            this.sProb = new StateProbability(s, log);
            this.o = o;
            this.oGen = new ObservationGenerator.ObservationGenerator(o, log);
            this.oProb = new ObservationProbability(oGen, log);
            this.a = a;

            this.projectName = projectName;
            this.planProjectName = projectName;
            // this.pomdpFileName = projectName + ".pomdp";
            // this.alphaFileName = projectName + ".alpha";
            this.polFileName = projectName + ".pg";
            this.simulFileName = projectName + ".simul";
            this.resultFileName = projectName + ".result";
            this.simulProjFileName = projectName + ".simulProj";

            this.log = log;
        }

        public Simulation(State s, Observation o, Context.Action a, String projectName, String planProjectName,
                LogControl log)
        {
            this.s = s;
            this.sProb = new StateProbability(s, log);
            this.o = o;
            this.oGen = new ObservationGenerator.ObservationGenerator(o, log);
            this.oProb = new ObservationProbability(oGen, log);
            this.a = a;

            this.projectName = projectName;
            this.planProjectName = planProjectName;
            // this.pomdpFileName = projectName + ".pomdp";
            // this.alphaFileName = projectName + ".alpha";
            this.polFileName = projectName + ".pg";
            this.simulFileName = projectName + ".simul";
            this.resultFileName = projectName + ".result";
            this.simulProjFileName = projectName + ".simulProj";

            this.log = log;
        }

        public void run(String codNeg, String datInicio, String datFim, int iniNode)
        {

            twitLog = "";
            simulLog = "";
            resultLog = "";

            log.writeln("############################");
            log.writeln("#Starting policy simulation#");
            log.writeln("############################");
            log.writeln("Project Name : " + this.projectName);
            log.writeln("State Def    : " + s.getSettings());
            log.writeln("Obs Def      : " + o.getSettings());
            log.writeln("----------------------------");
            log.writeln("Paper        : " + codNeg);
            log.writeln("From         : " + datInicio);
            log.writeln("To           : " + datFim);
            log.writeln("Init B Node  : " + iniNode);
            log.writeln("----------------------------");

            Series series = new Series(codNeg, datInicio, datFim, false);

            sProb.setSeries(series);
            oGen.setSeries(series);

            List<String> states = sProb.StateColRow();
            states.Add("erro");

            List<String> observations = o.observationList();
            List<String> analise = oGen.getObservacoes();

            List<String> actions = a.getActionDefs();

            Dictionary<String, int> obsIndex = new Dictionary<String, int>();
            for (int i = 0; i < observations.Count; i++)
                obsIndex.Add(observations[i], i + 2);

            getPolicy();

            log.writeln("Observation series done!");
            log.writeln("");

            bool iniNodeFound = false;

            if (iniNode > -1)
            {
                initialNode = iniNode;
                nextNode = initialNode;
                iniNodeFound = true;
            }

            if (iniNodeFound)
            {

                double[] comprado = new double[analise.Count];
                double[] vendido = new double[analise.Count];

                double[] compradoVar = new double[analise.Count];
                double[] vendidoVar = new double[analise.Count];
                double resultadoParcial = 0;

                String action = "fora";
                String state = "fora";

                for (int i = 0; i < analise.Count; i++)
                {
                    List<int> actualPol = policy[this.nextNode];

                    String obsAtual = analise[i];

                    if (obsAtual.Trim().Length == 0)
                        obsAtual = oGen.getDefault();

                    int actualNode = actualPol[0];
                    int executeAction = actualPol[1];
                    int nextNode = actualPol[obsIndex[obsAtual]];
                    
                     //Console.WriteLine(this.nextNode + " : " + actualNode + " : "+ executeAction + " : " + nextNode);

                    double actionValue = 0;
                    double buyValue = 0;
                    double sellValue = 0;
                    action = actions[executeAction];

                    if (i == (analise.Count - 1))
                    {
                        if (state.Equals("comprado"))
                            action = "venda";
                        else if (state.Equals("vendido"))
                            action = "compra";
                    }
                    // ////////////////////////////////////////

                    comprado[i] = 0;
                    vendido[i] = 0;

                    compradoVar[i] = 0;
                    vendidoVar[i] = 0;

                    if (action.Equals("compra") && state.Equals("fora"))
                    {
                        actionValue = -series.getClosePrice()[i];
                        buyValue = actionValue;
                        sellValue = 0;

                        if (i == 0)
                        {
                            comprado[i] = buyValue;
                            vendido[i] = 0;
                        }
                        else
                        {
                            if (comprado[i - 1] >= 0)
                                comprado[i] = buyValue;

                            else if (vendido[i - 1] >= 0)
                                vendido[i] = buyValue;
                        }

                        if (state.Equals("vendido"))
                            state = "fora";
                        else if (state.Equals("fora"))
                            state = "comprado";

                    }
                    else if (action.Equals("compra") && state.Equals("vendido"))
                    {
                        actionValue = -series.getClosePrice()[i];
                        buyValue = actionValue;
                        sellValue = 0;

                        if (i == 0)
                        {
                            comprado[i] = buyValue;
                            vendido[i] = 0;
                        }
                        else
                        {
                            if (vendido[i - 1] >= 0)
                                vendido[i] = buyValue;

                            else if (comprado[i - 1] >= 0)
                                comprado[i] = buyValue;
                        }

                        if (state.Equals("vendido"))
                            state = "fora";
                        else if (state.Equals("fora"))
                            state = "comprado";

                    }
                    else if (action.Equals("venda") && state.Equals("fora"))
                    {
                        actionValue = series.getClosePrice()[i];
                        buyValue = 0;
                        sellValue = actionValue;

                        if (i == 0)
                        {
                            comprado[i] = 0;
                            vendido[i] = sellValue;
                        }
                        else
                        {
                            if (vendido[i - 1] <= 0)
                                vendido[i] = sellValue;
                            else if (comprado[i - 1] <= 0)
                                comprado[i] = sellValue;
                        }

                        if (state.Equals("comprado"))
                            state = "fora";
                        else if (state.Equals("fora"))
                            state = "vendido";

                    }
                    else if (action.Equals("venda") && state.Equals("comprado"))
                    {
                        actionValue = series.getClosePrice()[i];
                        buyValue = 0;
                        sellValue = actionValue;

                        if (i == 0)
                        {
                            comprado[i] = 0;
                            vendido[i] = sellValue;
                        }
                        else
                        {
                            if (comprado[i - 1] <= 0)
                                comprado[i] = sellValue;
                            else if (vendido[i - 1] <= 0)
                                vendido[i] = sellValue;
                        }

                        if (state.Equals("comprado"))
                            state = "fora";
                        else if (state.Equals("fora"))
                            state = "vendido";

                    }
                    else if (action.Equals("compra-dupla")
                          && state.Equals("vendido"))
                    {
                        actionValue = -series.getClosePrice()[i];
                        buyValue = actionValue;
                        sellValue = actionValue;

                        comprado[i] = buyValue;
                        vendido[i] = buyValue;

                        state = "comprado";
                    }
                    else if (action.Equals("venda-dupla")
                          && state.Equals("comprado"))
                    {
                        actionValue = series.getClosePrice()[i];
                        buyValue = actionValue;
                        sellValue = actionValue;

                        comprado[i] = sellValue;
                        vendido[i] = sellValue;

                        state = "vendido";
                    }
                    else
                    {

                        if (i > 0)
                        {
                            if (comprado[i - 1] < 0)
                                comprado[i] = comprado[i - 1];

                            if (vendido[i - 1] > 0)
                                vendido[i] = vendido[i - 1];
                        }
                    }
                    // //////////////////////////////////////////

                    if (i > 0)
                    {
                        if (comprado[i - 1] < 0 && comprado[i] > 0)
                            compradoVar[i] = (comprado[i] / Math
                                    .Abs(comprado[i - 1])) - 1;

                        if (vendido[i] < 0 && vendido[i - 1] > 0)
                            vendidoVar[i] = (vendido[i - 1] / Math.Abs(vendido[i])) - 1;
                    }

                    resultadoParcial = resultadoParcial
                            + (Math
                                    .Round((compradoVar[i] + vendidoVar[i]) * 10000.0) / 100.0);

                    // log.writeln(dateformat.format(series.getDate()[i]) + " : "
                    // + series.getClosePrice()[i] + " : "
                    // + actions.get(executeAction) + " : " + analise[i]
                    // + " : " + resultadoParcial + "%");

                    String simulLine =series.getDate()[i].ToString("yyyy-MM-dd")
                            + " : " + actualNode + " : " + executeAction + " : "
                            + obsAtual + " : " + nextNode;
                    simulLog = simulLog + simulLine + "\r\n";

                    String resultLine = series.getDate()[i].ToString("yyyy-MM-dd")
                            + " : " + series.getClosePrice()[i] + " : " + action
                            + " : " + analise[i] + " : " + resultadoParcial
                            + "%" + " # " + state + " # " + comprado[i] + " : "
                            + compradoVar[i] + " # " + vendido[i] + " : "
                            + vendidoVar[i];
                    resultLog = resultLog + resultLine + "\r\n";

                    this.nextNode = nextNode;

                    twitLog = series.getDate()[i].ToString("yyyy-MM-dd") + " : "
                            + series.getClosePrice()[i] + " : " + action + " : "
                            + analise[i] + " : " + resultadoParcial + "%"
                            + " # " + state;
                }

                log.writeln("++++++++++++++++++++++++++++++++++++++++");
                log.writeln("Results found.  See file:");

                if (simulLog.Length > 1)
                {
                    log.write("	Writing simulation file...");
                    writeFile(simulFileName, simulLog);
                    log.writeln(simulFileName, false);
                }

                if (resultLog.Length > 1)
                {
                    log.write("	Writing results file...");
                    writeFile(resultFileName, resultLog);
                    log.writeln(resultFileName, false);
                }

                if (resultLog.Length > 1)
                {
                    String simulProjContent = "";
                    simulProjContent = simulProjContent + "PlanProjName : "
                    + planProjectName + "\n";
                    simulProjContent = simulProjContent + "State Def    : "
                            + s.getSettings() + "\n";
                    simulProjContent = simulProjContent + "Obs Def      : "
                            + o.getSettings() + "\n";
                    simulProjContent = simulProjContent + "Paper        : "
                            + codNeg + "\n";
                    simulProjContent = simulProjContent + "From         : "
                            + datInicio + "\n";
                    simulProjContent = simulProjContent + "To           : "
                            + datFim + "\n";
                    simulProjContent = simulProjContent + "Init B Node  : "
                            + iniNode + "\n";
                    writeFile(simulProjFileName, simulProjContent);
                }

                log.writeln("++++++++++++++++++++++++++++++++++++++++");
                log.writeln("Simulation Result:");
                log.writeln(twitLog);
            }

        }

        public void getPolicy()
        {

            policy.Clear();

            try
            {
                using (StreamReader reader = new StreamReader(polFileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {

                        List<int> pol = new List<int>();
                        String[] p = line.Split(' ');

                        for (int i = 0; i < p.Length; i++)
                            if (!p[i].Equals(""))
                                pol.Add(int.Parse(p[i]));

                        policy.Add(pol);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private void writeFile(String filename, String content)
        {
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                sw.Write(content);
            }
        }

        public String getLastStatus()
        {
            return twitLog;
        }

    }

}
