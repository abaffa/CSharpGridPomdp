using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomdpPBVI1
{
    public class Utils
    {

        public static int UnixTimeStamp()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public static void writeFile(String filename, String content)
        {
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                sw.Write(content);
            }
        }

        public static double Round(double v, int i)
        {

            double ret = v;

            if (i > 0)
            {
                double fact = Math.Pow((double)10, (double)i);
                ret = ((double)Math.Round(v * fact) / fact);
            }

            return ret;
        }

        public static String formatIntoHHMMSS(long secsIn)
        {

            int hours = (int)(secsIn / 3600.0);
            long remainder = (long)(secsIn % 3600.0);
            int minutes = (int)(remainder / 60.0);
            int seconds = (int)(remainder % 60.0);

            return ((hours < 10 ? "0" : "") + hours
            + " hrs., " + (minutes < 10 ? "0" : "") + minutes
            + " mins, " + (seconds < 10 ? "0" : "") + seconds + " secs");
            /*
             return ( (hours < 10 ? "0" : "") + hours
            + ":" + (minutes < 10 ? "0" : "") + minutes
            + ":" + (seconds< 10 ? "0" : "") + seconds );
             */

        }

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
    }

}
