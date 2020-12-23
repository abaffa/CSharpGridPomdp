using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerEnvironment.Context
{
    public class ActionDefinitions
    {

        Dictionary<String, String> actionDef = new Dictionary<String, String>();

        public ActionDefinitions()
        {
            actionDef.Clear();
            actionDef.Add("nada", "comprado-comprado#vendido-vendido#nada-nada");
            actionDef.Add("compra", "nada-comprado#vendido-nada");
            actionDef.Add("venda", "nada-vendido#comprado-nada");
            actionDef.Add("compra-dupla", "vendido-comprado");
            actionDef.Add("venda-dupla", "comprado-vendido");
        }

        public Dictionary<String, String> getActionDefHash()
        {
            return actionDef;
        }

        public List<String> getActionDefs()
        {
            return new List<String>(actionDef.Keys);
        }

    }
}