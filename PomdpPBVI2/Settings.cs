using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomdpPBVI2
{
    public class Settings
    {

        public static String projectFolder = "";
        public static String projectName = "temp";
        public static String pomdpFileName =  "temp.pomdp";
        public static String alphaFileName =  "temp.alpha";
        public static String polFileName =  "temp.pg";
        public static String beliefFileName =  "temp.b";

        public static double gamma = 0.9;

        public static double diffErrPlan = 0.005;
        public static double diffErrPol = 0.005;
        public static int decPlan = 6;
        public static int decPol = 4;


        public static void setFolder(String projectFolder)
        {
            Settings.projectFolder = projectFolder;
        }

        public static void setProjectName(String projectName)
        {

            if (projectName.LastIndexOf("/") > -1)
            {
                Settings.projectFolder = projectName.Substring(0, projectName.LastIndexOf("/") + 1);
                Settings.projectName = projectName.Substring(projectName.LastIndexOf("/") + 1);
            }
            else
                Settings.projectName = projectName;

            Settings.pomdpFileName = Settings.projectName + ".pomdp";
            Settings.alphaFileName = Settings.projectName + ".alpha";
            Settings.polFileName = Settings.projectName + ".pg";
            Settings.beliefFileName = Settings.projectName + ".b";
        }
    }
}
