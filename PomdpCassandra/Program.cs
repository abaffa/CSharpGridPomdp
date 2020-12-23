using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomdpCassandra
{
    class Program
    {
        static void Main(string[] args)
        {
            Planner p = new Planner();
            p.setEpoch(3);
            p.pomdp(@"C:\Backup\pomdp\tiger.95");
            //p.rewriteFiles(@"C:\Backup\pomdp\temp382059508"); //  NAO TEM ESSE METODO?!
        }
    }
}
