using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridPomdp
{
    public class Job
    { //implements Serializable

        public String getStateDef()
        {
            return stateDef;
        }

        public String getObservDef()
        {
            return observDef;
        }

        public String getPaper()
        {
            return paper;
        }

        public String getPlFrom()
        {
            return plFrom;
        }

        public String getPlTo()
        {
            return plTo;
        }

        public String getSimFrom()
        {
            return simFrom;
        }

        public String getSimTo()
        {
            return simTo;
        }

        public DateTime getDtStart()
        {
            return dtStart;
        }

        public DateTime getDtEnd()
        {
            return dtEnd;
        }

        String stateDef = "";
        String observDef = "";
        String paper = "";
        String plFrom = "";
        String plTo = "";
        String simFrom = "";
        String simTo = "";
        String ownerEmail = "";
        int epoch = 0;
        int planner = -1;

        eStatus status = eStatus.waiting;
        DateTime dtStart;
        DateTime dtEnd;

        String gridName = "";
        UUID gridUUID;
        UUID sessionUUID;

        public void setGridTaskSession(GridTaskSession gridTaskSession)
        {
            this.gridTaskSession = gridTaskSession;
            setSessionUUID(gridTaskSession.getId());
        }

        public void setGridNode(GridNode gridNode)
        {
            this.gridNode = gridNode;
            setGridUUID(gridNode.getId());
            setGridName(gridNode.getPhysicalAddress());

        }

        GridTaskSession gridTaskSession;
        GridNode gridNode;
        // GridTaskFuture<String> taskFuture;

        bool warming = false;

        public enum eStatus
        {
            waiting, running, failed, rerun, done, cancelled
        }

        public Job(int planner, String stateDef, String observDef, String paper, String plFrom,
                String plTo, String simFrom, String simTo)
        {

            this.planner = planner;
            this.stateDef = stateDef;
            this.observDef = observDef;
            this.paper = paper;
            this.plFrom = plFrom;
            this.plTo = plTo;
            this.simFrom = simFrom;
            this.simTo = simTo;

            status = eStatus.waiting;
        }

        public Job(int planner, String stateDef, String observDef, String paper, String plFrom,
                String plTo, String simFrom, String simTo, int epoch)
        {

            this.planner = planner;
            this.stateDef = stateDef;
            this.observDef = observDef;
            this.paper = paper;
            this.plFrom = plFrom;
            this.plTo = plTo;
            this.simFrom = simFrom;
            this.simTo = simTo;
            this.epoch = epoch;

            status = eStatus.waiting;
        }

        public Job(int planner, String stateDef, String observDef, String paper, String plFrom,
                String plTo, String simFrom, String simTo, String ownerEmail)
        {

            this.planner = planner;
            this.stateDef = stateDef;
            this.observDef = observDef;
            this.paper = paper;
            this.plFrom = plFrom;
            this.plTo = plTo;
            this.simFrom = simFrom;
            this.simTo = simTo;
            this.ownerEmail = ownerEmail;

            status = eStatus.waiting;
        }

        public Job(int planner, String stateDef, String observDef, String paper, String plFrom,
                String plTo, String simFrom, String simTo, int epoch,
                String ownerEmail)
        {

            this.planner = planner;
            this.stateDef = stateDef;
            this.observDef = observDef;
            this.paper = paper;
            this.plFrom = plFrom;
            this.plTo = plTo;
            this.simFrom = simFrom;
            this.simTo = simTo;
            this.epoch = epoch;
            this.ownerEmail = ownerEmail;

            status = eStatus.waiting;
        }

        public String[] getJobData()
        {

            String[] args = new String[7];

            args[0] = stateDef;
            args[1] = observDef;
            args[2] = paper;
            args[3] = plFrom;
            args[4] = plTo;
            args[5] = simFrom;
            args[6] = simTo;

            return args;
        }

        public String getGridName()
        {
            return gridName;
        }

        public void setGridName(String gridName)
        {
            this.gridName = gridName;
        }

        public UUID getGridUUID()
        {
            return gridUUID;
        }

        public void setGridUUID(UUID gridUUID)
        {
            this.gridUUID = gridUUID;
        }

        public UUID getSessionUUID()
        {
            return sessionUUID;
        }

        public void setSessionUUID(UUID sessionUUID)
        {
            this.sessionUUID = sessionUUID;
        }

        public void setStatus(eStatus status)
        {

            if (this.status != eStatus.cancelled)
            {
                if (status == eStatus.running || status == eStatus.rerun)
                {
                    warming = false;
                    dtStart = DateTime.Now;
                }
                else if (status == eStatus.failed || status == eStatus.done)
                {
                    warming = false;
                    dtEnd = DateTime.Now;
                }

                this.status = status;
            }
        }

        public eStatus getStatus()
        {
            return status;
        }

        public DateTime getDTStart()
        {
            return dtStart;
        }

        public DateTime getDTEnd()
        {
            return dtEnd;
        }

        public void setWarming(bool warming)
        {

            this.warming = warming;
        }

        public bool getWarming()
        {

            return warming;
        }

        public String getOwnerEmail()
        {
            return ownerEmail;
        }

        public void setOnwerEmail(String ownerEmail)
        {
            this.ownerEmail = ownerEmail;
        }

        public int getEpoch()
        {
            return epoch;
        }

        public void setEpoch(int epoch)
        {
            this.epoch = epoch;
        }

        public int getPlanner()
        {
            return planner;
        }

        public void setPlanner(int planner)
        {
            this.planner = planner;
        }

        public void cancel() 
        {

		if (status != eStatus.failed && status != eStatus.done)
			this.status = eStatus.cancelled;

		if (gridTaskSession != null) {
                Collection<GridJobSibling> jobs = gridTaskSession.getJobSiblings();

                for (int i = 0; i < jobs.size(); i++)
                {
                    if (jobs.toArray()[i] != null)
                        ((GridJobSibling)(jobs.toArray()[i])).cancel();
                }
            }
        }
    }
}
