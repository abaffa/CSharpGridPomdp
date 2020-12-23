using GridPomdp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GridPomdp
{
    public class GridTwitter_events
    {

        public static void twitterStatusJob_event(String projectName,
                 String __event, String status)
        {

            Thread t = new Thread(delegate ()
            {
                try
                {
                    TwitterLogger gridTwitter = new TwitterLogger();
                    gridTwitter.SendMsg("[" + __event + "] " + status);
                }
                catch (Exception e)
                {
                    Log.WriteLine(e.ToString(), Log.Level.error);
                }

            });
            t.Start();

        }

        public static void twitterStatusJob_event(String _event,
            String status)
        {

            Thread t = new Thread(delegate ()
            {
                try
                {
                    TwitterLogger gridTwitter = new TwitterLogger();
                    gridTwitter.SendMsg("[" + _event + "] " + status);
                }
                catch (Exception e)
                {
                    Log.WriteLine(e.ToString(), Log.Level.error);
                }

            });
            t.Start();
        }

        public static void twitterGridStart()
        {
            String twitLog = "";
            twitLog += "Master is up and running...";

            twitterStatusJob_event("Grid", twitLog);
        }

        public static void twitterGridStop()
        {
            String twitLog = "";
            twitLog += "Master is down...";

            twitterStatusJob_event("Grid", twitLog);
        }

        public static void twitterGridNodeLeft(String nodeName)
        {
            String twitLog = "";
            twitLog += nodeName;

            // twitterStatusJob_event("NodeLeft", twitLog);
        }

        public static void twitterGridStatus()
        {
            String twitLog = "";

            twitLog += " running(" + GridMain.runningGridNodeCount() + ")";
            twitLog += " available(" + GridMain.availableGridNodeCount() + ")";
            // twitterStatusJob_event("NodeStatus", twitLog);
        }

        public static void twitterTaskReceived(Job newJob)
        {
            String twitLog = "";
            twitLog += newJob.getStateDef() + ":" + newJob.getPaper() + ":"
                    + newJob.getObservDef() + " ";

            // twitterStatusJob_event("TaskReceived", twitLog);
        }

        public static void twitterStartJob_event(String projectName, Job args)
        {

            if (args != null)
            {
                String stateDef = args.getStateDef();
                String observDef = args.getObservDef();
                String paper = args.getPaper();
                String plFrom = args.getPlFrom();
                String plTo = args.getPlTo();
                String simFrom = args.getSimFrom();
                String simTo = args.getSimTo();
                int epoch = args.getEpoch();

                twitterStatusJob_event(projectName, "JobStarting", paper + ":"
                        + stateDef + ":" + observDef + ":" + plFrom + ":" + plTo
                        + ":" + simFrom + ":" + simTo + ":" + epoch);
            }
        }

        public static void twitterFinishedJob_event(String projectName, Job args)
        {

            if (args != null)
            {
                String stateDef = args.getStateDef();
                String observDef = args.getObservDef();
                String paper = args.getPaper();
                String plFrom = args.getPlFrom();
                String plTo = args.getPlTo();
                String simFrom = args.getSimFrom();
                String simTo = args.getSimTo();
                int epoch = args.getEpoch();

                twitterStatusJob_event(projectName, "PlannerFinished", paper + ":"
                        + stateDef + ":" + observDef + ":" + plFrom + ":" + plTo
                        + ":" + simFrom + ":" + simTo + ":" + epoch);

            }
        }

        public static void twitterCancelledJob_event(String projectName, Job args)
        {

            if (args != null)
            {
                String stateDef = args.getStateDef();
                String observDef = args.getObservDef();
                String paper = args.getPaper();
                String plFrom = args.getPlFrom();
                String plTo = args.getPlTo();
                String simFrom = args.getSimFrom();
                String simTo = args.getSimTo();
                int epoch = args.getEpoch();

                twitterStatusJob_event(projectName, "JobCancelled", paper + ":"
                        + stateDef + ":" + observDef + ":" + plFrom + ":" + plTo
                        + ":" + simFrom + ":" + simTo + ":" + epoch);

            }
        }
    }
}
