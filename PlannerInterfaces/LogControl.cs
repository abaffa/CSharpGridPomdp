using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerInterfaces.Utils
{

    public class LogControl
    {

        

        String logName = "";
        StreamWriter output;

        public LogControl(String logName)
        {
            open(logName);
        }

        public void open(String logName)
        {
            try
            {
                this.logName = logName;
                output = new StreamWriter(logName);
            }
            catch (Exception ex)
            {

            }
        }

        public String realAll()
        {

            String ret = "";
            try
            {

                StreamReader input = new StreamReader(logName);
                String s = "";
                while ((s = input.ReadLine()) != null)
                {
                    ret = ret + s + "\n";
                }

                input.Close();
            }
            catch (Exception ex)
            {

            }

            return ret;
        }

        public void write(String text)
        {
            write(text, true);
        }

        public void write(String text, Boolean setDate)
        {
            try
            {
                String screenText = text;
                String fileText = text;

                if (setDate)
                {
                    // screenText = "["
                    // + dateformat.format(Calendar.getInstance().getTime())
                    // + "][" + logName + "] " + screenText;
                    screenText = "[" + logName + "] " + screenText;
                    fileText = "["
                            + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            + "] " + fileText;
                }

                Console.WriteLine(screenText);
                output.Write(fileText);
                output.Flush();
            }
            catch (Exception ex)
            {

            }
        }

        public void writeln(String text)
        {
            writeln(text, true);
        }

        public void writeln(String text, Boolean setDate)
        {
            try
            {
                String screenText = text;
                String fileText = text;

                if (setDate)
                {
                    // screenText = "["
                    // + dateformat.format(Calendar.getInstance().getTime())
                    // + "][" + logName + "] " + screenText;
                    screenText = "[" + logName + "] " + screenText;
                    fileText = "["
                            + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            + "] " + fileText;
                }

                Console.WriteLine(screenText);
                output.Write(fileText + "\n");
                output.Flush();
            }
            catch (Exception ex)
            {

            }
        }

        public void close()
        {
            try
            {
                output.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }

}
