using PlannerEnvironment.Context;
using PlannerEnvironment.Simulator;
using PlannerInterfaces.Utils;
using PlannerEnvironment.Planner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;

namespace PlannerEnvironment
{
    class Program
    {

        public static void SimulTest()
        {

            String projectName = "testeAqui";

            LogControl log = new LogControl("c:/recalc/" + projectName + ".log");

            State state = new State("pv5#pv-5", log);
            Observation obs = new Observation("RSI2", state, log);
            Context.Action action = new Context.Action(log);

            String paper = "^DJI";
            String simFrom = "2009-01-01";
            String simTo = "2009-12-31";
            int initBIndex = 0;//p.getBInitIndex();

            Simulation s = new Simulation(state, obs, action, "c:/recalc/" + projectName, log);
            s.run(paper, simFrom, simTo, initBIndex);
        }
        public static void Test()
        {

            String projectName = "testeAqui";

            LogControl log = new LogControl("c:/recalc/" + projectName + ".log");

            State state = new State("pv5#pv-5", log);
            Observation obs = new Observation("RSI2", state, log);
            Context.Action action = new Context.Action(log);
            Reward reward = new Reward(log);

            Planner.Planner p = new Planner.Planner(state, obs, action, reward, projectName,
                     log);
            p.setEpoch(20);


            PomdpPBVI2.Planner pbvi = new PomdpPBVI2.Planner();
            pbvi.pomdp(projectName);
            pbvi.setEpoch(20);
            

            String paper = "^DJI";
            String datInicio = "2001-01-01";
            String datFim = "2008-12-31";
            String simFrom = "2009-01-01";
            String simTo = "2009-12-31";
            int initBIndex = 0;//p.getBInitIndex();

            int planner = 3;

            p.GeneratePolicy(planner, paper, datInicio, datFim);

            Simulation s = new Simulation(state, obs, action, "c:/recalc/" + projectName, log);
            s.run(paper, simFrom, simTo, initBIndex);
        }

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            if (args.Length > 0)
            {
                if (args[0].Equals("-h"))
                {
                    Console.WriteLine("Prototipo Gerador Politicas");
                    Console.WriteLine("###########################");
                    Console.WriteLine("");
                    Console.WriteLine("Sintaxe: ");
                    Console.WriteLine("   mono PlannerEnvironment.exe -h ");
                    Console.WriteLine("   mono PlannerEnvironment.exe projeto defEstado defObs papelGeracao dateStart dateEnd defObsAvaliacao papelAvaliacao dateStart dateEnd");
                    Console.WriteLine("   mono PlannerEnvironment.exe projeto defEstado defObs g papelGeracao dateStart dateEnd");
                    Console.WriteLine("   mono PlannerEnvironment.exe projeto defEstado defObs s papelAvaliacao dateStart dateEnd iniNode");
                    Console.WriteLine("");
                }
                else
                {
                    String projectName = args[0];

                    LogControl log = new LogControl(projectName + ".log");

                    State state = new State(args[1], log);
                    Observation obs = new Observation(args[2], state, log);
                    Context.Action action = new Context.Action(log);
                    Reward reward = new Reward(log);

                    if (args[3].Equals("g"))
                    {
                        Planner.Planner m = new Planner.Planner(state, obs, action, reward, projectName,
                                log);
                        m.GeneratePolicy(args[4], args[5], args[6]);
                    }
                    else if (args[3].Equals("s"))
                    {
                        Simulation s = new Simulation(state, obs, action,
                                projectName, log);

                        if (args.Length > 7)
                            s.run(args[4], args[5], args[6],int.Parse(args[7]));
                        //					else
                        //						s.run(args[4], args[5], args[6]);

                    }
                    else
                    {
                        Planner.Planner m = new Planner.Planner(state, obs, action, reward, projectName,
                                log);
                        m.GeneratePolicy(args[3], args[4], args[5]);
                        // m.GeneratePolicy("^BVSP", "2000-01-01", "2000-02-01");
                        int initBIndex = m.getBInitIndex();

                        obs = new Observation(args[6], state, log);
                        Simulation s = new Simulation(state, obs, action,
                                projectName, log);
                        s.run(args[7], args[8], args[9], initBIndex);
                    }
                }
            }
            else
            {
                String projectName = "temp";

                LogControl log = new LogControl(projectName + ".log");

                State state = new State("pv5#pv-5", log);
                Observation obs = new Observation("RSI2", state, log);
                Context.Action action = new Context.Action(log);
                Reward reward = new Reward(log);

                Planner.Planner m = new Planner.Planner(state, obs, action, reward, projectName, log);
                m.setEpoch(3);
                m.GeneratePolicy("^BVSP", "2007-01-01", "2013-12-31");
                int initBIndex = m.getBInitIndex();

                //obs = new Observation("RSI", state, log);
                Simulation s = new Simulation(state, obs, action, projectName, log);
                s.run("^BVSP", "2014-01-01", "2017-08-01", initBIndex);
            }

            
        }
    }
}
