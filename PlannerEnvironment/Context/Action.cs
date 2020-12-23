using PlannerInterfaces.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerEnvironment.Context
{
    public class Action
    {

        LogControl log;

        ActionDefinitions definitions = new ActionDefinitions();

        public Action(LogControl log)
        {
            this.log = log;
        }

        public List<String> getActionDefs()
        {
            return definitions.getActionDefs();
        }
    }
}
