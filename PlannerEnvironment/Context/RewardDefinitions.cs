using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerEnvironment.Context
{
    public class RewardDefinitions
    {

        Dictionary<String, Double> rewardDef = new Dictionary<String, Double>();

        public RewardDefinitions()
        {
            rewardDef.Clear();
            rewardDef.Add("erro", -25.0);
            rewardDef.Add("A-A_comprado", 10.0);
            rewardDef.Add("A-N_comprado", 3.0);
            rewardDef.Add("A-B_comprado", 0.0);
            rewardDef.Add("N-A_comprado", 5.0);
            rewardDef.Add("N-N_comprado", 0.0);
            rewardDef.Add("N-B_comprado", -5.0);
            rewardDef.Add("B-A_comprado", 5.0);
            rewardDef.Add("B-N_comprado", -2.0);
            rewardDef.Add("B-B_comprado", -10.0);
            rewardDef.Add("A-A_vendido", -10.0);
            rewardDef.Add("A-N_vendido", -2.0);
            rewardDef.Add("A-B_vendido", 5.0);
            rewardDef.Add("N-A_vendido", -5.0);
            rewardDef.Add("N-N_vendido", 0.0);
            rewardDef.Add("N-B_vendido", 5.0);
            rewardDef.Add("B-A_vendido", 0.0);
            rewardDef.Add("B-N_vendido", 3.0);
            rewardDef.Add("B-B_vendido", 10.0);
            rewardDef.Add("A-A_nada", -2.0);
            rewardDef.Add("A-N_nada", 0.0);
            rewardDef.Add("A-B_nada", 0.0);
            rewardDef.Add("N-A_nada", 0.0);
            rewardDef.Add("N-N_nada", 2.0);
            rewardDef.Add("N-B_nada", 0.0);
            rewardDef.Add("B-A_nada", 0.0);
            rewardDef.Add("B-N_nada", 0.0);
            rewardDef.Add("B-B_nada", -2.0);
        }

        public Dictionary<String, Double> getRewardDefHash()
        {
            return rewardDef;
        }

        public List<String> getRewardDefs()
        {
            return new List<String>(rewardDef.Keys);
        }

    }

}
