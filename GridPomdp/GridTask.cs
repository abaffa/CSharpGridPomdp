using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridPomdp
{
    public class GridTask
    { // extends GridTaskAdapter<List<Job>, String> {

        // Inject load balancer.
        //@GridLoadBalancerResource
        //GridLoadBalancer balancer;

        //@GridTaskSessionResource
        //private GridTaskSession ses = null;

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

        // Dictionary jobs to grid nodes.
        public Dictionary<? extends GridJob, GridNode> Dictionary(List<GridNode> subgrid,
                List<Job> arg)
        {
            Dictionary<GridJob, GridNode> jobs = new Dictionary<GridJob, GridNode>(subgrid
                    .Count);

            // In more complex cases, you can actually do
            // more complicated assignments of jobs to nodes.
            for (int i = 0; i < arg.Count; i++)
            {

                GridJob t = new GridJob(arg[i]);

                List<GridNode> excludeGrids = new List<GridNode>();
                excludeGrids.Add(GridMain.masterNode);

                // arg[i].getStatus() != Job.eStatus.failed
                if (arg[i].getStatus() != Job.eStatus.done
                        && arg[i].getStatus() != Job.eStatus.cancelled)
                {
                    Object[] gridUUID = GridMain.getGridNodes().keySet().ToArray();

                    for (int g = 0; g < gridUUID.Length; g++)
                    {

                        for (int z = 0; z < GridMain.getJobList().Count; z++)

                            if (((UUID)gridUUID[g]) == GridMain.getJobList()
                                    .get(z).getGridUUID()
                                    && (GridMain.getJobList().get(z).status == Job.eStatus.running || GridMain
                                            .getJobList().get(z).status == Job.eStatus.rerun))
                            {
                                excludeGrids.Add(GridMain.getGrid().getNode(
                                        (UUID)gridUUID[g]));
                                break;
                            }

                    }

                    Object[] excG = excludeGrids.ToArray();
                    GridNode[] excL = new GridNode[excG.Length];
                    for (int g2 = 0; g2 < excG.Length; g2++)
                    {
                        excL[g2] = (GridNode)excG[g2];
                    }
                    GridNode targetGrid = balancer.getBalancedNode(t, excL);

                    if (targetGrid != null)
                    {
                        if (arg[i].getStatus() == Job.eStatus.waiting)
                        {

                            arg[i].setGridNode(targetGrid);
                            // arg[i].setGridUUID(targetGrid.getId());
                            // arg[i].setGridName(targetGrid.getPhysicalAddress());

                            arg[i].setGridTaskSession(ses);
                            // arg[i].setSessionUUID(ses.getId());

                            arg[i].setStatus(Job.eStatus.running);

                            jobs.Add(t, targetGrid);
                        }
                        else if (arg[i].getStatus() == Job.eStatus.failed)
                        {

                            arg[i].setGridNode(targetGrid);
                            // arg[i].setGridUUID(targetGrid.getId());
                            // arg[i].setGridName(targetGrid.getPhysicalAddress());

                            arg[i].setGridTaskSession(ses);
                            // arg[i].setSessionUUID(ses.getId());

                            arg[i].setStatus(Job.eStatus.rerun);

                            jobs.Add(t, targetGrid);
                        }
                        else if (arg[i].getStatus() == Job.eStatus.running
                              || arg[i].getStatus() == Job.eStatus.rerun)
                        {

                            arg[i].setStatus(Job.eStatus.failed);

                            arg[i].setGridNode(targetGrid);
                            // arg[i].setGridUUID(targetGrid.getId());
                            // arg[i].setGridName(targetGrid.getPhysicalAddress());

                            arg[i].setGridTaskSession(ses);
                            // arg[i].setSessionUUID(ses.getId());

                            arg[i].setStatus(Job.eStatus.rerun);

                            jobs.Add(t, targetGrid);

                        }
                        else if (arg[i].getStatus() == Job.eStatus.done)
                        {
                        }

                    }

                    arg[i].setWarming(false);

                    if (jobs.Count < 1)
                        Console.WriteLine("erro");
                }
            }

            GridMain.setWarming(false);

            return jobs;
        }

        // Aggregate results into one compound result.
        public String reduce(List<GridJobResult> results)
        {
            // For the purpose of this example we simply
            // concatenate string representation of every
            // job result

            String buf = "";

            for (GridJobResult res : results)
            {
                try
                {

                    List<int> ret = (List<int>)res.getData();

                    // Append string representation of result
                    // returned by every job.
                    // if(!res.isCancelled())
                    buf = buf + (ret[0].ToString());
                }
                catch (Exception ex)
                {
                    buf = buf + (-1);
                }
            }

            if (buf.Equals("0"))
                GridMain.setStatusJob(ses.getId(), Job.eStatus.cancelled);
            else if (buf.Equals("1"))
                GridMain.setStatusJob(ses.getId(), Job.eStatus.done);
            else
                GridMain.setStatusJob(ses.getId(), Job.eStatus.failed);

            return buf.ToString();
        }


        public GridJobResultPolicy result(GridJobResult result,
                List<GridJobResult> received) throws GridException
        {

            GridJobResultPolicy ret = GridJobResultPolicy.REDUCE;

		return ret;
        }
    }
}
