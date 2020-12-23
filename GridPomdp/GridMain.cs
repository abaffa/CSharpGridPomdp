using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridPomdp
{
    public class GridMain
    { //implements GridLocalEventListener
        /*

    private static Grid grid;
        public static GridNode masterNode;

        private static Hashtable<UUID, String> gridNodes = new Hashtable<UUID, String>();
        private static ArrayList<Job> jobList = new ArrayList<Job>();
        private static boolean warming = false;

        public GridMain() throws GridException
        {

            startGrid();

		try {
                addNextTask();

            } catch (Exception e) {
                Settings.gridLogger.error(e.getMessage());
            }

        }

        private void startGrid() throws GridException
        {
            GridConfigurationAdapter cfg = new GridConfigurationAdapter();
        cfg.setGridName(Settings.gridName);
		cfg.setGridLogger(Settings.gridLogger);

		GridRoundRobinLoadBalancingSpi loadBalancingSpi = new GridRoundRobinLoadBalancingSpi();
        cfg.setLoadBalancingSpi(loadBalancingSpi);

		GridBasicTopologySpi topologySpi = new GridBasicTopologySpi();
        topologySpi.setLocalNode(false); // Exclude local node from topology.
		topologySpi.setRemoteNodes(true); // Exclude local node from topology.
		cfg.setTopologySpi(topologySpi);

		GridMulticastDiscoverySpi discoverySpi = new GridMulticastDiscoverySpi();
        discoverySpi.setTcpPort(43010);
		discoverySpi.setMaxMissedHeartbeats(6);

		cfg.setDiscoverySpi(discoverySpi);

		if (Settings.gridType == Settings.gridTypes.email) {
			// Override default discovery SPI.
			cfg.setDiscoverySpi(new GridMailDiscoverySpi());

			// Override default communication SPI.
			cfg.setCommunicationSpi(new GridMailCommunicationSpi());

			cfg.setDeploymentSpi(new GridUriDeploymentSpi());
		}

    grid = GridFactory.start(cfg);
		masterNode = grid.getLocalNode();

		grid.addLocalEventListener(this); // Register event listener for local
		// events.

		for (int i = 0; i < grid.getRemoteNodes().size(); i++) {
        UUID gridUUID = ((GridNode)grid.getRemoteNodes().toArray()[i])
                .getId();
        gridNodes.put(gridUUID, "Ready");

    }

    Thread monitor = new Thread(new GridEmailInterface());
		if (Settings.gridEmailInterfaceMonitor)
			monitor.start();

		GridTwitterEvents.twitterGridStart();
		GridTwitterEvents.twitterGridStatus();
	}

public void stopGrid()
{
    GridFactory.stop(Settings.gridName, true);
    GridTwitterEvents.twitterGridStop();
}

public void onEvent(GridEvent evt)
{

    // JOB_STARTED
    // JOB_MAPPED
    if (evt.getType() == GridEventType.JOB_STARTED
            || evt.getType() == GridEventType.JOB_MAPPED)
    {

        if (evt.getEventNodeId() != null)
            gridNodes.put(evt.getEventNodeId(), "Working");

        // JOB_FINISHED
        // JOB_RESULTED
    }
    else if (evt.getType() == GridEventType.JOB_FINISHED
          || evt.getType() == GridEventType.JOB_RESULTED)
    {

        // setStatusJob(evt.getTaskSessionId(), Job.eStatus.done);
        if (evt.getEventNodeId() != null)
            gridNodes.put(evt.getEventNodeId(), "Ready");

        GridTwitterEvents.twitterGridStatus();

    }
    else if (evt.getType() == GridEventType.TASK_STARTED)
    {
    }
    else if (evt.getType() == GridEventType.TASK_REDUCED)
    {
    }
    else if (evt.getType() == GridEventType.TASK_TIMED_OUT)
    {

        // TASK_FINISHED
    }
    else if (evt.getType() == GridEventType.TASK_FINISHED)
    {

        // setStatusJob(evt.getTaskSessionId(), Job.eStatus.done);
        if (evt.getEventNodeId() != null)
            gridNodes.put(evt.getEventNodeId(), "Ready");

        // twitterGridStatus();
        // JOB_FAILED
        // JOB_FAILED_OVER
        // JOB_REJECTED
        // JOB_CANCELLED
        // JOB_TIMED_OUT
    }
    else if (evt.getType() == GridEventType.JOB_FAILED
          || evt.getType() == GridEventType.JOB_FAILED_OVER
          || evt.getType() == GridEventType.JOB_REJECTED
          || evt.getType() == GridEventType.JOB_CANCELLED
          || evt.getType() == GridEventType.JOB_TIMED_OUT)
    {

        String twitlog = "";

        if (evt.getTaskSessionId() != null)
        {
            setStatusJob(evt.getTaskSessionId(), Job.eStatus.failed);
            twitlog += "#jobFailed " + evt.getTaskSessionId().toString()
                    + " ";
        }

        if (evt.getEventNodeId() != null)
        {
            gridNodes.put(evt.getEventNodeId(), "Ready");
            twitlog += "#node " + evt.getEventNodeId().toString();
        }

        if (twitlog.length() > 0)
        {
            // gridTwitter.warning(twitlog);
        }
        // twitterGridStatus();

        // TASK_FAILED
    }
    else if (evt.getType() == GridEventType.TASK_FAILED)
    {

        String twitlog = "";
        if (evt.getTaskSessionId() != null)
        {
            setStatusJob(evt.getTaskSessionId(), Job.eStatus.failed);
            twitlog += "#taskFailed " + evt.getTaskSessionId().toString()
                    + " ";
        }

        if (evt.getEventNodeId() != null)
        {
            gridNodes.put(evt.getEventNodeId(), "Ready");
            twitlog += "#node " + evt.getEventNodeId().toString();
        }

        if (twitlog.length() > 0)
        {
            // gridTwitter.warning(twitlog);
        }
        // twitterGridStatus();

        // NODE_JOINED
    }
    else if (evt.getType() == GridEventType.NODE_JOINED)
    {
        if (evt.getEventNodeId() != null)
        {
            gridNodes.put(evt.getEventNodeId(), "Ready");
            // gridTwitter.warning("#nodeJoined " +
            // evt.getEventNodeId().toString());

        }
        GridTwitterEvents.twitterGridStatus();
        // NODE_LEFT
        // NODE_FAILED
    }
    else if (evt.getType() == GridEventType.NODE_LEFT
          || evt.getType() == GridEventType.NODE_FAILED)
    {

        if (evt.getEventNodeId() != null)
        {
            GridTwitterEvents.twitterGridNodeLeft(evt.getEventNodeId()
                    .toString());
            // gridNodes.remove(evt.getEventNodeId());
            // setStatusJobByGrid(evt.getEventNodeId(), Job.eStatus.failed);

        }
        GridTwitterEvents.twitterGridStatus();
    }

    Settings.gridLogger.warning(">>> Grid event occurred: " + evt);

    addNextTask();
}

public static int runningGridNodeCount()
{
    int count = 0;
    if (GridMain.gridNodes.keySet().size() > 0)
    {
        Object[] gridUUID = GridMain.gridNodes.keySet().toArray();

        for (int g = 0; g < gridUUID.length; g++)
        {

            for (int z = 0; z < GridMain.jobList.size(); z++)
                if (((UUID)gridUUID[g]) == jobList.get(z).getGridUUID()
                        && (GridMain.jobList.get(z).status == Job.eStatus.running || GridMain.jobList
                                .get(z).status == Job.eStatus.rerun))
                {
                    count++;
                    break;
                }
        }
    }
    return count;
}

public static int availableGridNodeCount()
{
    int countRunning = runningGridNodeCount();
    int countGrids = 0;

    if (GridMain.gridNodes.keySet().size() > 0)
    {
        Object[] gridUUID = GridMain.gridNodes.keySet().toArray();

        for (int g = 0; g < gridUUID.length; g++)
        {
            try
            {
                if (grid.getNode((UUID)gridUUID[g]) != grid.getLocalNode())
                    countGrids++;
            }
            finally
            {
            }
        }

    }

    if (grid.getRemoteNodes().size() < countGrids)
        countGrids = grid.getRemoteNodes().size();

    return countGrids - countRunning;
}

private static int getNextAvailableJob()
{
    int count = -1;

    for (int z = 0; z < GridMain.jobList.size(); z++)
        if (!GridMain.jobList.get(z).getWarming()
                && (GridMain.jobList.get(z).status == Job.eStatus.waiting || GridMain.jobList
                        .get(z).status == Job.eStatus.failed))
        {
            count = z;
            break;
        }

    return count;
}

private static Job getJob(UUID sessionUUID)
{
    Job job = null;
    for (int z = 0; z < GridMain.jobList.size(); z++)
        if (jobList.get(z).getSessionUUID() == sessionUUID)
        {
            job = jobList.get(z);
            break;
        }
    return job;
}

public static void setStatusJob(UUID sessionUUID, Job.eStatus status)
{
    Job job = getJob(sessionUUID);
    if (job != null)
        job.setStatus(status);
    else
        Settings.gridLogger.error("Job Session not Found!");
}

// ///////////////////////////////////////////
// ///////////////////////////////////////////
// ///////////////////////////////////////////

public static void resetGrid()
{

    for (int z = 0; z < GridMain.jobList.size(); z++)
    {
        if (GridMain.jobList.get(z).status != Job.eStatus.done
                && GridMain.jobList.get(z).status != Job.eStatus.cancelled
                && GridMain.jobList.get(z).status != Job.eStatus.failed)
        {

            try
            {
                GridMain.jobList.get(z).cancel();
            }
            catch (GridException e)
            {
                Settings.gridLogger.error(e.getMessage());
            }
        }
    }

}

public static void addItem(Job newJob)
{
    jobList.add(newJob);
    GridTwitterEvents.twitterTaskReceived(newJob);

    addNextTask();
}

public static void addNextTask()
{

    if (grid != null)
    {
        if (availableGridNodeCount() > 0 && !warming)
        {
            warming = true;
            int iIndex = getNextAvailableJob();

            if (iIndex > -1 && jobList.size() > iIndex)
            {

                Job actualJob = jobList.get(iIndex);
                actualJob.setWarming(true);

                ArrayList<Job> temp = new ArrayList<Job>();
                temp.add(actualJob);

                grid.execute(GridTask.class, temp);

				} else
					warming = false;
			}
		}
	}

	// ///////////////////////////////////////////
	// ///////////////////////////////////////////
	// ///////////////////////////////////////////

	public static String getGridList()
{
    String textFull = "Grid List:\n##########\n";
    textFull = textFull + grid.getLocalNode().getPhysicalAddress()
            + " : Master" + "\n";

    if (GridMain.gridNodes.keySet().size() > 0)
    {
        Object[] gridUUID = GridMain.gridNodes.keySet().toArray();

        for (int g = 0; g < gridUUID.length; g++)
        {

            if ((UUID)gridUUID[g] != null)
            {
                GridNode gridNode = grid.getNode((UUID)gridUUID[g]);

                if (gridNode != null
                        && masterNode.getId() != (UUID)gridUUID[g])
                {
                    String text = gridNode.getPhysicalAddress() + " : "
                            + GridMain.gridNodes.get((UUID)gridUUID[g]);
                    textFull = textFull + text + "\n";
                }
                else
                    GridMain.gridNodes.remove((UUID)gridUUID[g]);
            }
        }
    }
    return textFull;
}

public static String getPendingJobs()
{
    SimpleDateFormat formato = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");

    String textFull = "Pending Jobs:\n#############\n";

    for (int z = 0; z < GridMain.jobList.size(); z++)
    {
        if (GridMain.jobList.get(z).status == Job.eStatus.waiting
                || GridMain.jobList.get(z).status == Job.eStatus.failed
                || GridMain.jobList.get(z).status == Job.eStatus.done
                || GridMain.jobList.get(z).status == Job.eStatus.cancelled)
        {

            String status = "";
            if (GridMain.jobList.get(z).status == Job.eStatus.waiting)
                status = "Waiting";
            else if (GridMain.jobList.get(z).status == Job.eStatus.failed)
                status = "Failed";
            else if (GridMain.jobList.get(z).status == Job.eStatus.done)
                status = "Done";
            else if (GridMain.jobList.get(z).status == Job.eStatus.cancelled)
                status = "Cancelled";

            String text = "#" + String.valueOf(z + 1) + " : "
                    + GridMain.jobList.get(z).getStateDef() + " : "
                    + GridMain.jobList.get(z).getPaper() + " : "
                    + GridMain.jobList.get(z).getObservDef();

            if (GridMain.jobList.get(z).getDTStart() != null)
                text = text
                        + " : "
                        + formato.format(GridMain.jobList.get(z)
                                .getDTStart().getTime());

            if (GridMain.jobList.get(z).getDTEnd() != null)
                text = text
                        + " : "
                        + formato.format(GridMain.jobList.get(z).getDTEnd()
                                .getTime());

            text = text + " : " + status;

            textFull = textFull + text + "\n";

        }
    }
    return textFull;
}

public static String getRunningJobs()
{
    // JEditorPane teste = new JEditorPane();
    // String aux = "<font color="red"><b>Vermelho e Negrito</b></font>";
    // teste.setContentType("text/html");
    // teste.setText(aux);
    //		
    SimpleDateFormat formato = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss");

    String textFull = "Running Jobs:\n#############\n";

    for (int z = 0; z < GridMain.jobList.size(); z++)
    {
        if (GridMain.jobList.get(z).gridUUID != null)
            if (grid.getNode(GridMain.jobList.get(z).gridUUID) != null
                    && (GridMain.jobList.get(z).status == Job.eStatus.running || GridMain.jobList
                            .get(z).status == Job.eStatus.rerun))
            {

                String status = "";
                if (GridMain.jobList.get(z).status == Job.eStatus.running)
                    status = "Running";
                else if (GridMain.jobList.get(z).status == Job.eStatus.rerun)
                    status = "Re-run";

                String text = "#" + String.valueOf(z + 1) + " : "
                        + GridMain.jobList.get(z).getStateDef() + " : "
                        + GridMain.jobList.get(z).getPaper() + " : "
                        + GridMain.jobList.get(z).getObservDef();

                if (GridMain.jobList.get(z).getDTStart() != null)
                    text = text
                            + " : "
                            + formato.format(GridMain.jobList.get(z)
                                    .getDTStart().getTime());

                if (GridMain.jobList.get(z).getDTEnd() != null)
                    text = text
                            + " : "
                            + formato.format(GridMain.jobList.get(z)
                                    .getDTEnd().getTime());

                text = text
                        + " : "
                        + status
                        + " on "
                        + grid.getNode(GridMain.jobList.get(z).gridUUID)
                                .getPhysicalAddress();

                textFull = textFull + text + "\n";
            }
    }
    // else
    // GridMain.jobList.get(z).setStatus(Job.eStatus.failed);

    return textFull;
}

public static void setWarming(boolean warming)
{
    GridMain.warming = warming;
}

public static boolean isWarming()
{
    return warming;
}

public static ArrayList<Job> getJobList()
{
    return jobList;
}

public static Hashtable<UUID, String> getGridNodes()
{
    return gridNodes;
}

public static Grid getGrid()
{
    return grid;
}

*/
    }
}
