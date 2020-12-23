using PlannerEnvironment.Context;
using PlannerEnvironment.ObservationGenerator;
using PlannerEnvironment.ProbabilityGenerator;
using PlannerInterfaces.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TraderWhatever.Business;

namespace PlannerEnvironment.Planner
{
    public class Planner
    {

        bool cancelled = false;
        int initBIndex = -1;
        String projectName = "temp";
        String pomdpFileName = "temp.pomdp";
        String planProjFileName = "temp.planProj";

        String machineName = "";

        PlannerInterfaces.POMDP.Planner p;
        State s;
        StateProbability sProb;
        Observation o;
        PlannerEnvironment.ObservationGenerator.ObservationGenerator oGen;
        ObservationProbability oProb;
        Context.Action a;
        Reward r;
        LogControl log;

        List<String> stateDefList;
        List<List<Double>> matrizTransicoes;

        double discountValue = 0.9;
        int maxEpoch = 20;
        int timeLimit = 0;

        public Planner(State s, Observation o, Context.Action a, Reward r,
                String projectName, LogControl log)
        {
            cancelled = false;
            this.s = s;
            this.sProb = new StateProbability(s, log);
            this.o = o;
            this.oGen = new PlannerEnvironment.ObservationGenerator.ObservationGenerator(o, log);
            this.oProb = new ObservationProbability(oGen, log);
            this.a = a;
            this.r = r;

            this.projectName = projectName;
            this.pomdpFileName = projectName + ".pomdp";
            this.planProjFileName = projectName + ".planProj";

            this.log = log;
        }

        public void setEpoch(int epoch)
        {
            maxEpoch = epoch;
        }

        public int getEpoch()
        {
            return maxEpoch;
        }

        public void setDiscount(double discount)
        {
            discountValue = discount;
        }

        public double getDiscount()
        {
            return discountValue;
        }


        private static String GetCurrentIp()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ni.OperationalStatus == OperationalStatus.Up)
                        {
                            return ip.Address.ToString();
                        }
                    }
                }
            }

            return "127.0.0.1";
        }
        private void refreshMachineHost()
        {
            try
            {
                machineName = System.Environment.MachineName
                        + " - "
                        + GetCurrentIp();
            }
            catch (Exception e1)
            {
                machineName = "n/a";
                Console.WriteLine(e1);
            }
        }

        public void GeneratePolicy(String codNeg, String datInicio, String datFim)
        {
            GeneratePolicy(-1, codNeg, datInicio, datFim);
        }

        public void GeneratePolicy(int planner, String codNeg, String datInicio,
                String datFim)
        {
            cancelled = false;

            
            if (planner == 0)
            {
                p = new PomdpCassandra.Planner(log);
            }
            else if (planner == 1)
            {
                p = new PomdpPBVI0.Planner(log);
            }
            else if (planner == 2)
            {
                p = new PomdpPBVI1.Planner(log);
            }
            else
            {
                p = new PomdpPBVI2.Planner(log);
            }
            

            refreshMachineHost();

            log.writeln("############################");
            log.writeln("#Starting policy generation#");
            log.writeln("############################");
            log.writeln("Project Name : " + this.projectName);
            log.writeln("Node Name    : " + machineName);
            log.writeln("----------------------------");
            log.writeln("State Def    : " + s.getSettings());
            log.writeln("Obs Def      : " + o.getSettings());
            log.writeln("Discount     : " + discountValue);
            log.writeln("----------------------------");
            log.writeln("Paper        : " + codNeg);
            log.writeln("From         : " + datInicio);
            log.writeln("To           : " + datFim);
            log.writeln("----------------------------");
            log.writeln("Planner      : " + p.getName());
            log.writeln("Version      : " + p.getVersion());
            log.writeln("Epochs       : " + maxEpoch);
            log.writeln("----------------------------");

            // pega cotacoes
            Series series = new Series(codNeg, datInicio, datFim, false);



            sProb.setSeries(series);
            oGen.setSeries(series);

            List<String> states = sProb.StateColRow();
            states.Add("erro");

            List<String> actions = a.getActionDefs();
            List<String> observations = o.observationList();
            List<Double> start = getStart(states, "nada");

            String header = "";
            String stateTransitions = "";
            String obsTables = "";
            String rewTables = "";
            String completeFile = "";

            header = header + "discount: " + discountValue + "\n";
            header = header + "values: reward\n";

            header = header + "states:";
            for (int i = 0; i < states.Count; i++)
                header = header + " " + states[i];

            header = header + "\n";

            header = header + "actions:";
            for (int i = 0; i < actions.Count; i++)
                header = header + " " + actions[i];

            header = header + "\n";

            header = header + "observations:";
            for (int i = 0; i < observations.Count; i++)
                header = header + " " + observations[i];

            header = header + "\n";

            header = header + "start:";
            for (int i = 0; i < start.Count; i++)
                header = header + " "
                        + start[i].ToString("0.0000000");

            header = header + "\n";

            stateTransitions = sProb.generateTables();
            stateTransitions = stateTransitions + "\n";

            oGen.setStateList(states);
            oGen.setStateSeries(sProb.getStateSeries());

            obsTables = oProb.generateTables();
            obsTables = obsTables + "\n";

            rewTables = r.generateTables();
            rewTables = rewTables + "\n";

            completeFile = header + stateTransitions + obsTables + rewTables;

            log.writeln("POMDP Model done!");
            log.writeln("");
            log.writeln("++++++++++++++++++++++++++++++++++++++++");
            log.writeln("POMDP Data. See file:"); // log.writeln("POMDP Data:");
            log.write("	Writing model file...");

            writeFile(pomdpFileName, completeFile);

            log.writeln(pomdpFileName, false);// log.writeln(completeFile, false);
            log.writeln("++++++++++++++++++++++++++++++++++++++++");

            p.setEpoch(maxEpoch);
            p.setTimeLimit(timeLimit);

            if (!cancelled)
                p.pomdp(projectName);
            else
            {
                log.writeln("Execution cancelled.");
                log.writeln("++++++++++++++++++++++++++++++++++++++++");
            }

            if (!cancelled)
            {
                initBIndex = p.getBInitIndex();

                String planProjContent = "";
                planProjContent = planProjContent + "State Def    : "
                        + s.getSettings() + "\n";
                planProjContent = planProjContent + "Obs Def      : "
                        + o.getSettings() + "\n";
                planProjContent = planProjContent + "Discount     : "
                        + discountValue + "\n";
                planProjContent = planProjContent + "Paper        : " + codNeg
                        + "\n";
                planProjContent = planProjContent + "From         : " + datInicio
                        + "\n";
                planProjContent = planProjContent + "To           : " + datFim
                        + "\n";
                planProjContent = planProjContent + "Planner      : " + p.getName()
                        + "\n";
                planProjContent = planProjContent + "Version      : "
                        + p.getVersion() + "\n";
                planProjContent = planProjContent + "PlannerClass : "
                        + p.ToString() + "\n";
                planProjContent = planProjContent + "Epochs       : " + maxEpoch
                        + "\n";
                planProjContent = planProjContent + "Init B Node  : " + initBIndex
                        + "\n";
                writeFile(planProjFileName, planProjContent);
            }
        }

        public List<Double> getStart(List<String> states,
                String definition)
        {
            List<Double> array = new List<Double>();
            List<Double> arrayPerc = new List<Double>();

            double sum = 0;
            for (int i = 0; i < states.Count; i++)
            {
                String statesName = states[i];
                statesName = statesName.Substring(statesName.IndexOf("_") + 1);

                if (!statesName.Equals("erro"))
                {
                    if (definition.IndexOf(statesName) == -1)
                        array.Add(0.0);
                    else
                    {
                        array.Add(1.0);
                        sum = sum + 1.0;
                    }
                }
                else
                    array.Add(0.0);
            }

            for (int x = 0; x < array.Count; x++)
            {
                if (sum == 0)
                {
                    arrayPerc.Add(0.0);
                }
                else
                    arrayPerc.Add((array[x] / sum));
            }

            return arrayPerc;
        }

        public int getBInitIndex()
        {
            return initBIndex;
        }

        public void cancel()
        {
            cancelled = true;
            p.cancel();
        }

        private void writeFile(String filename, String content)
        {
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                sw.Write(content);
            }
        }
    }

}
