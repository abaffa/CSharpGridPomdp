using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridPomdp.Utils
{

    public static class Log
    {
        public enum Level
        {
            none,
            debug,
            warning,
            error,
            log
        }
        public enum PrefixType
        {
            none,
            markerOnly,
            level,
            time,
            date,
            datetime
        }

        public static Level CurrentLevel = Level.debug;
        public static Level WoofyLevel = Level.none;

        public static PrefixType CurrentPrefixType = PrefixType.time;
        public static PrefixType WoofyPrefixType = PrefixType.none;

        private static string PrintPrefix(Level level, PrefixType type)
        {
            switch (type)
            {
                case PrefixType.datetime:
                    return "[" + DateTime.Now.ToString() + "] ";

                //  break;
                case PrefixType.date:
                    return "[" + DateTime.Now.ToShortDateString() + "] ";

                //  break;
                case PrefixType.time:
                    return "[" + DateTime.Now.ToLongTimeString() + "] ";

                //  break;
                case PrefixType.level:
                    return "[" + level.ToString() + "] ";

                //break;
                case PrefixType.markerOnly:
                    return " + ";

                //break;
                default:
                    break;

            }

            return "";
        }

        public static bool CheckLogLevel(Level level)
        {

            return CurrentLevel != Level.none && CurrentLevel <= level;
        }

        public static bool CheckWoofyLevel(Level level)
        {

            return WoofyLevel != Level.none && WoofyLevel <= level;
        }
        public static void Write(String text, Level level)
        {
            if (CheckLogLevel(level) && text.Trim().Length > 0)
            {
                Console.Write(PrintPrefix(level, CurrentPrefixType));
                Console.Write(text);
            }

            if (CheckWoofyLevel(level) && text.Trim().Length > 0)
            {
                Woofy.woofy_baffa(PrintPrefix(level, WoofyPrefixType) + text);
            }
        }
        public static void WriteLine(String text, Level level)
        {
            if (CheckLogLevel(level) && text.Trim().Length > 0)
            {
                Console.Write(PrintPrefix(level, CurrentPrefixType));
                Console.WriteLine(text);
            }

            if (CheckWoofyLevel(level) && text.Trim().Length > 0)
            {
                Woofy.woofy_baffa(PrintPrefix(level, WoofyPrefixType) + text);
            }
        }

        public static void CompleteText(String text, Level level)
        {
            if (CheckLogLevel(level))
            {
                Console.Write(text);
            }

            if (CheckWoofyLevel(level))
            {
                Woofy.woofy_baffa(text);
            }
        }
        public static void CompleteLine(String text, Level level)
        {
            if (CheckLogLevel(level))
            {
                Console.WriteLine(text);
            }

            if (CheckWoofyLevel(level))
            {
                Woofy.woofy_baffa(text);
            }
        }

        public static void CompleteText(String text)
        {
            CompleteText(text, CurrentLevel);
        }
        public static void CompleteLine(String text)
        {
            CompleteLine(text, CurrentLevel);
        }


        public static void WriteLine(String text)
        {
            WriteLine(text, CurrentLevel);
        }

        public static void Write(String text)
        {
            Write(text, CurrentLevel);
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }

    }
}
