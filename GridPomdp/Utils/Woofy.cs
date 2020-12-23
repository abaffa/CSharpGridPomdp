using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridPomdp.Utils
{
    public static class Woofy
    {
        public static void woofy_galgos(String msg)
        {
            woofy(-25501523, msg);
        }
        public static void woofy_baffa(String msg)
        {
            woofy(50963017, msg);
        }
        public static void woofy(int chat_id, String msg)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "/export/home/public/woofy.sh";
                startInfo.Arguments = chat_id.ToString() + " \"" + msg.Replace("\"", "") + "\"";
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception)
            {
            }
        }

    }
}
