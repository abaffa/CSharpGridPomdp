using PlannerInterfaces.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomdpCassandra
{
    public class Planner : PlannerInterfaces.POMDP.Planner
    {

        String projectName = "temp";
        String pomdpFileName = "temp.pomdp";
        String alphaFileName = "temp.alpha";
        String polFileName = "temp.pg";

        int maxEpoch = 1000;
        int timeLimit = 0;

        int initialNode = -1;

        public override String getName()
        {
            return "Cassandra's pomdp-solve";
        }

        public override String getVersion()
        {
            return "v. 5.3";
        }


        public override void setEpoch(int maxEpoch)
        {
            this.maxEpoch = maxEpoch;
        }


        public override void setTimeLimit(int timeLimit)
        {
            this.timeLimit = timeLimit;
        }

        public Planner()
        {

        }

        public Planner(LogControl log)
        {
            this.log = log;
        }


        public override void cancel() { }


        public override void pomdp(String projectName)
        {

            this.projectName = projectName;
            this.pomdpFileName = projectName + ".pomdp";
            this.alphaFileName = projectName + ".alpha";
            this.polFileName = projectName + ".pg";

            String arg = "";

            if (maxEpoch > 0)
                arg = " -horizon " + maxEpoch;

            if (timeLimit > 0)
                arg = " -time_limit " + timeLimit;


            String plannerFile = "pomdp-solve.exe";

            exec(plannerFile, "-method grid -pomdp \"" + pomdpFileName + "\" -o \""
                    + projectName + "\" -witness_points true -stop_criteria bellman " + arg, true);

            SearchInitialNode();
        }


        public override int getBInitIndex()
        {
            return initialNode;
        }


        public override bool SearchInitialNode()
        {
            writeln("++++++++++++++++++++++++++++++++++++++++");
            writeln("Calculating initial node...");

            String plannerToolsFile = "pomdp-tools.exe";

            String ret = exec(plannerToolsFile,"-pomdp " + pomdpFileName
                    + " -tool pg_eval -pg1 \"" + polFileName + "\" -alpha1 \""
                    + alphaFileName + "\"", false);

            bool found = false;

            if (ret.IndexOf("state value:") > -1)
            {
                String initialNodeDesc = ret.Substring(ret.IndexOf("state value:"))
                        .Replace("\n", "");

                if (initialNodeDesc.IndexOf("node ") > -1)
                {
                    initialNode = int.Parse(initialNodeDesc
                            .Substring(initialNodeDesc.IndexOf("node ") + 5));


                    found = true;
                }
            }

            if (found)
                writeln("Initial Node: " + initialNode + " done!", false);
            else
                writeln("Initial node not found!", false);

            writeln("++++++++++++++++++++++++++++++++++++++++");

            return found;

        }

        private String exec(String prog, String _params, bool writeLog)
        {
            String ret = "";
            writeln("++++++" + prog + " " + _params);
            try
            {
                String line = "";

                Process process = new Process();
                process.StartInfo.FileName = prog;
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.StartInfo.Arguments = _params;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                //* Read the output (or the error)
                StreamReader output = process.StandardOutput; //.ReadToEnd();
                
                string err = process.StandardError.ReadToEnd();
                Console.WriteLine(err);
                process.WaitForExit();


                while ((line = output.ReadLine()) != null)
                {
                    ret = ret + line + "\n";
                    if (writeLog)
                        writeln(line);
                }

            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }

            return ret;
        }
    }

}
