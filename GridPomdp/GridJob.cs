using Baffa.Utils;
using GridPomdp.Utils;
using PlannerEnvironment.Context;
using PlannerEnvironment.Planner;
using PlannerEnvironment.Simulator;
using PlannerInterfaces.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridPomdp
{
    public class GridJob
    { // extends GridJobAdapter<Job>


        private bool cancelled = false;
        private Planner m;
        private Simulation s;

        //@GridTaskSessionResource
        //private GridTaskSession ses = null;

        /** Job context will be injected. */
        //@GridJobContextResource
        //private GridJobContext jobCtx = null;

        //@GridInstanceResource
        //private Grid grid = null;

        //@GridLoggerResource
        //private GridLogger gridLog = null;

        //@GridHomeResource
        //private String gridHome = null;

        //@GridExecutorServiceResource
        //private ExecutorService gridExecSvc = null;

        //@GridLocalNodeIdResource
        //private UUID gridNodeId = null;

        //@GridMarshallerResource
        //private GridMarshaller gridMarshaller = null;

        public GridJob(Job args)
        {
            //super(args);
        }

        //public Serializable execute()
        public void execute()
        {
            cancelled = false;
            Job args = getArgument();
            List<int> ret = new List<int>();

            /*
            // Set session attribute with value of this job's word.
            ses.setAttribute(jobCtx.getJobId(), (Serializable)args);

            try
            {
                // Wait for all other jobs within this task to set their attributes
                // on
                // the session.
                for (GridJobSibling sibling : ses.getJobSiblings())
                {
                    // Waits for attribute with sibling's job ID as a key.
                    if (ses.waitForAttribute(sibling.getJobId()) == null)
                    {
                        throw new GridException(
                                "Failed to get session attribute from job: "
                                        + sibling.getJobId());
                    }
                }
            }
            catch (InterruptedException e)
            {
                throw new GridException(
                        "Got interrupted while waiting for session attributes.", e);
            }
            */

            // Execute gridified method again and return the number
            // characters in the passed in word.

            try
            {

                String projectName = Settings.projectPrefix;
                projectName = Settings.projectPrefix + Randomize.Integer(1,10000);

                String stateDef = args.getStateDef();
                String observDef = args.getObservDef();
                String paper = args.getPaper();
                String plFrom = args.getPlFrom();
                String plTo = args.getPlTo();
                String simFrom = args.getSimFrom();
                String simTo = args.getSimTo();
                String ownerEmail = args.getOwnerEmail();
                int epoch = args.getEpoch();

                LogControl log = new LogControl(projectName + ".log");

                GridTwitterEvents.twitterStartJobEvent(projectName, args);

                // Model Definition
                State state = new State(stateDef, log);
                Observation obs = new Observation(observDef, state, log);
                PlannerEnvironment.Context.Action action = new PlannerEnvironment.Context.Action(log);
                Reward reward = new Reward(log);
                int initBIndex = -1;

                if (!cancelled)
                {
                    m = new Planner(state, obs, action, reward, projectName, log);
                    if (epoch > 0)
                        m.setEpoch(epoch);
                }

                if (!cancelled)
                {
                    m.GeneratePolicy(args.getPlanner(), paper, plFrom, plTo);
                    initBIndex = m.getBInitIndex();
                }

                if (!cancelled)
                {
                    GridTwitterEvents.twitterFinishedJobEvent(projectName, args);
                }

                if (!cancelled)
                {
                    s = new Simulation(state, obs, action, projectName, log);
                    s.run(paper, simFrom, simTo, initBIndex);
                }

                log.close();

                if (!cancelled)
                {
                    String twitLog = s.getLastStatus();

                    if (twitLog.Length > 0)
                        GridTwitterEvents.twitterStatusJobEvent(projectName, "JobResult", twitLog);
                }

                if (!cancelled)
                {
                    if (ownerEmail.Trim().Length > 0)
                    {
                        GridEmailEvents.TaskResults(projectName, args, ownerEmail.Trim(), log
                                .realAll());

                    }
                }

                if (cancelled)
                {
                    GridTwitterEvents.twitterCancelledJobEvent(projectName, args);
                    ret.Add(0);
                }
                else
                    ret.Add(1);

            }
            catch (Exception e)
            {
                args.setStatus(eStatus.failed);
                ret.Add(-1);
            }

            //return ret;
        }

        public void cancel()
        {
            cancelled = true;
            if (m != null)
                m.cancel();

            //super.cancel();
        }
    }

}
