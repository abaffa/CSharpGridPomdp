using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PomdpPBVI2
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Planner p = new Planner();
            p.setEpoch(20);
            //p.pomdp("c:/tiger");
            p.pomdp(@"C:\Backup\pomdp\temp-1588074886");
            //p.rewriteFiles(@"C:\Backup\pomdp\temp-1588074886");
        }
    }
}
