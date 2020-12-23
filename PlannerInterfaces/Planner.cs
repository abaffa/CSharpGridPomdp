using PlannerInterfaces.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerInterfaces.POMDP
{
    
    public abstract class Planner
    {

        protected LogControl log;

        protected void write(String str)
        {
            if (log == null)
                Console.Write(str);
		else
			log.write(str);
        }
        protected void writeln(String str)
        {
            if (log == null)
                Console.WriteLine(str);
		else
			log.writeln(str);
        }

        public void writeln(String str, Boolean setDate)
        {
            if (log == null)
                Console.WriteLine(str);
		else
			log.writeln(str, setDate);
        }

        public abstract String getName();
        public abstract String getVersion();

        public abstract void cancel();
        public abstract void setEpoch(int maxEpoch);
        public abstract void setTimeLimit(int timeLimit);
        public abstract void pomdp(String projectName);
        public abstract int getBInitIndex();
        public abstract Boolean SearchInitialNode();
    }

}
