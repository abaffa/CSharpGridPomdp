using GridPomdp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GridPomdp
{
    public class GridEmailInterface     {

        
    public void run()
    {

        while (Settings.gridEmailInterfaceMonitor)
        {
            try
            {
                Thread.Sleep(Settings.gridEmailInterfaceInterval);

                GmailUtilities gmail = new GmailUtilities();
                gmail.SetUserPass(Settings.gridEmailInterfaceAcount,
                        Settings.gridEmailInterfacePassword);
                gmail.Connect();
                gmail.OpenFolder("INBOX");

                List<String[]> msgs = gmail.GetMessages();

                // System.out.println(">> Processing " + msgs.Count +
                // " email(s)");

                for (int i = 0; i < msgs.Count; i++)
                {
                    String from = msgs[i][0].Trim().ToLower();
                    String subject = msgs[i][1].Trim().ToLower();
                    String body = msgs[i][2];

                    if (subject.Equals("GridStatus".ToLower()))
                    {
                        GridEmailEvents.GridStatus(from,
                                GridMain.getGridList(), GridMain
                                        .getPendingJobs(), GridMain
                                        .getRunningJobs());
                    }
                    else if (subject.Equals("GridList".ToLower()))
                    {
                        GridEmailEvents.GridList(from, GridMain.getGridList());
                    }
                    else if (subject.Equals("PendingJobs".ToLower()))
                    {
                        GridEmailEvents.PendingJobs(from, GridMain
                                .getPendingJobs());
                    }
                    else if (subject.Equals("RunningJobs".ToLower()))
                    {
                        GridEmailEvents.RunningJobs(from, GridMain
                                .getRunningJobs());
                    }
                    else if (subject.Equals("AddTask".ToLower()))
                    {

                        body = body.Replace("\r", "");

                        if (body.IndexOf("\n") >= 0)
                        {
                            String[] tasks = body.Split('\n');
                            String retBody = "";
                            String retTo = from;

                            // pv5#pv-5, RSI1, ^BVSP, 2000-06-01, 2008-02-01,
                            // 2008-02-01, 2009-06-01
                            for (int t = 0; t < tasks.Length; t++)
                            {
                                    String[] args = tasks[t].Split(',');

                                if (args.Length == 7)
                                {

                                    GridMain.addItem(new Job(-1,
                                            args[0].Trim(), args[1].Trim(),
                                            args[2].Trim(), args[3].Trim(),
                                            args[4].Trim(), args[5].Trim(),
                                            args[6].Trim(), retTo.Trim()));
                                }
                                else if (args.Length == 8)
                                {

                                    GridMain.addItem(new Job(-1,
                                            args[0].Trim(), args[1].Trim(),
                                            args[2].Trim(), args[3].Trim(),
                                            args[4].Trim(), args[5].Trim(),
                                            args[6].Trim(), int.Parse(args[7].Trim()),
                                            retTo.Trim()));
                                }
                                else
                                    retBody = retBody + "Has Invalid Tasks!\n";
                            }

                            retBody = retBody + body + "\n"
                                    + GridMain.getPendingJobs() + "\n"
                                    + GridMain.getRunningJobs();

                            GridMain.addNextTask();

                            GridEmailEvents.NewTask(from, retBody);

                        }
                        else
                        {
                            String[] args = body.Split(",");

                            if (args.Length == 7)
                            {

                                GridMain.addItem(new Job(-1, args[0].Trim(),
                                        args[1].Trim(), args[2].Trim(), args[3]
                                                .Trim(), args[4].Trim(),
                                        args[5].Trim(), args[6].Trim(), from));

                                // pv5#pv-5, RSI1, ^BVSP, 2000-06-01,
                                // 2008-02-01,
                                // 2008-02-01, 2009-06-01
                                String retBody = body + "\n"
                                        + GridMain.getPendingJobs() + "\n"
                                        + GridMain.getRunningJobs();
                                GridEmailEvents.NewTask(from, retBody);
                            }
                            else if (args.Length == 8)
                            {

                                GridMain
                                        .addItem(new Job(-1, args[0].Trim(),
                                                args[1].Trim(), args[2].Trim(),
                                                args[3].Trim(), args[4].Trim(),
                                                args[5].Trim(), args[6].Trim(),
                                                int.Parse(args[7]
                                                                .Trim()), from));

                                // pv5#pv-5, RSI1, ^BVSP, 2000-06-01,
                                // 2008-02-01,
                                // 2008-02-01, 2009-06-01
                                String retBody = body + "\n"
                                        + GridMain.getPendingJobs() + "\n"
                                        + GridMain.getRunningJobs();

                                GridEmailEvents.NewTask(from, retBody);
                            }
                            else
                            {
                                GridEmailEvents.InvalidTask(from, body);
                            }

                            GridMain.addNextTask();
                        }

                    }
                    else if (subject.Equals("Help".ToLower()))
                    {
                        GridEmailEvents.Help(from);
                    }
                }

            }
            catch (Exception e)
            {
                Log.WriteLine(e.ToString(),Log.Level.error);
            }

        }

    }

}

}
