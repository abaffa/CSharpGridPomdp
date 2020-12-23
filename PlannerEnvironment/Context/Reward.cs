using PlannerInterfaces.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerEnvironment.Context
{
    public class Reward
    {

        LogControl log;

        String defLine = "R: * : * : ";
        RewardDefinitions definitions = new RewardDefinitions();

        public Reward(LogControl log)
        {
            this.log = log;
        }

        public String generateTables()
        {
            String ret = "";

            List<String> rewardDef = definitions.getRewardDefs();

            for (int i = 0; i < rewardDef.Count; i++)
                ret = ret + defLine + rewardDef[i] + " : * "
                        + definitions.getRewardDefHash()[rewardDef[i]]
                        + "\n";

            return ret;
        }
    }

}
