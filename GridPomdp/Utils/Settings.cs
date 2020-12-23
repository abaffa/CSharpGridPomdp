using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridPomdp.Utils
{
    public static class Settings
    {

        public enum gridTypes
        {
            local, email
        }

        public static String gridName = "POMDPGrid";
        public static gridTypes gridType = gridTypes.local;

        public static int gridEmailInterfaceInterval = 30000;
        public static bool gridEmailInterfaceMonitor = true;

        public static long gridUiInterval = 5000;
        public static bool gridUiMonitor = true;

        public static String gridEmailCommunicationAcount = "gridpomdp.comm@gmail.com";
        public static String gridEmailCommunicationPassword = "q1w2e3r4t5";

        public static String gridEmailDiscoveryAcount = "gridpomdp.comm@gmail.com";
        public static String gridEmailDiscoveryPassword = "q1w2e3r4t5";

        public static String gridEmailInterfaceAcount = "gridpomdp@gmail.com";
        public static String gridEmailInterfacePassword = "q1w2e3r4t5";
        public static String gridEmailInterfaceCC = "";

        public static String gridTwitterInterfaceAcount = "gridpomdp";
        public static String gridTwitterInterfacePassword = "q1w2e3r4t5";
        public static String gridTwitterConsumerKey = "oyFMc9akymJaerfpdt7Ppw";
        public static String gridTwitterConsumerSecret = "ZdBTWk7BNHuJMlQ4DSXM6nB4xWxZF2LyCjhIrz3jQ";
        public static String gridTwitterToken = "100227033-HyRfJBV1sTfEFGCHknxEhNiI2ZrvwvctqbYO9JdA";
        public static String gridTwitterTokenSecret = "LhAy0SCQJmRYONil9ECtU1RvpbLiV5Eo13mKUSlWc";

        public static String projectPrefix = "temp";
        public static String projectFolder = "./gridFiles/";
    }

}
