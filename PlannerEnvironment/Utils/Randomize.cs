using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baffa.Utils
{

    public class Randomize
    {
        private static Random rnd = null;

        public static int Integer(int max)
        {
            if (rnd == null)
                rnd = new Random();

            return rnd.Next(max);

        }
        public static int Integer(int min, int max)
        {
            if (rnd == null)
                rnd = new Random();

            return rnd.Next(min, max);
        }

        public static double Double()
        {
            if (rnd == null)
                //rnd = new Random(Guid.NewGuid().GetHashCode());
                rnd = new Random();

            return rnd.NextDouble();
        }

        public static double Double(double max)
        {
            if (rnd == null)
                //rnd = new Random(Guid.NewGuid().GetHashCode());
                rnd = new Random();

            return rnd.NextDouble() * max;
        }

    }


}
