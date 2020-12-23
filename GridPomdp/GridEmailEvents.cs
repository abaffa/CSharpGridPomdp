using GridPomdp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GridPomdp
{
    public class GridEmailEvents
    {
        public static void GridStatus(String to, String gridList,
               String pendingJobs, String runningJobs)
        {
            String retFrom = Settings.gridEmailInterfaceAcount;
            String retTo = to;
            String retSubject = "GridStatus";
            String retBody = gridList + "\n" + pendingJobs + "\n"
                    + runningJobs;

            Thread t = new Thread(delegate ()
            {

                try
                {
                    GmailSender sender = new GmailSender(retFrom,
                            Settings.gridEmailInterfacePassword);
                    sender.SendMail(retSubject, retBody, retFrom, retTo,
                            Settings.gridEmailInterfaceCC);
                }
                catch (Exception e)
                {
                    Log.WriteLine(e.ToString(), Log.Level.error);
                }

            });
            t.Start();

        }

        public static void GridList(String to, String gridList)
        {
            String retFrom = Settings.gridEmailInterfaceAcount;
            String retTo = to;
            String retSubject = "GridList";
            String retBody = gridList;


            Thread t = new Thread(delegate ()
            {
                try
                {
                    GmailSender sender = new GmailSender(retFrom,
                            Settings.gridEmailInterfacePassword);
                    sender.SendMail(retSubject, retBody, retFrom, retTo,
                            Settings.gridEmailInterfaceCC);
                }
                catch (Exception e)
                {
                    Log.WriteLine(e.ToString(), Log.Level.error);
                }
            });
            t.Start();
        }

        public static void PendingJobs(String to, String pendingJobs)
        {
            String retFrom = Settings.gridEmailInterfaceAcount;
            String retTo = to;
            String retSubject = "PendingJobs";
            String retBody = pendingJobs;

            Thread t = new Thread(delegate ()
            {
                try
                {
                    GmailSender sender = new GmailSender(retFrom,
                        Settings.gridEmailInterfacePassword);
                    sender.SendMail(retSubject, retBody, retFrom, retTo,
                            Settings.gridEmailInterfaceCC);
                }
                catch (Exception e)
                {
                    Log.WriteLine(e.ToString(), Log.Level.error);
                }
            });
            t.Start();
        }

        public static void RunningJobs(String to, String runningJobs)
        {
            String retFrom = Settings.gridEmailInterfaceAcount;
            String retTo = to;
            String retSubject = "RunningJobs";
            String retBody = runningJobs;

            Thread t = new Thread(delegate ()
            {
                try
                {
                    GmailSender sender = new GmailSender(retFrom,
                        Settings.gridEmailInterfacePassword);
                    sender.SendMail(retSubject, retBody, retFrom, retTo,
                            Settings.gridEmailInterfaceCC);
                }
                catch (Exception e)
                {
                    Log.WriteLine(e.ToString(), Log.Level.error);
                }
            });
            t.Start();
        }

        public static void Help(String to)
        {
            String retFrom = Settings.gridEmailInterfaceAcount;
            String retTo = to;
            String retSubject = "Help";

            String retBody = "GridStatus - Lista Status Completo do Grid\n"
                   + "\n"
                   + "GridList - Lista Nós do Grid\n"
                   + "\n"
                   + "PendingJobs - Lista Fila de Tarefas\n"
                   + "\n"
                   + "RunningJobs - Lista Tarefas em Execução\n"
                   + "\n"
                   + "AddTask - Adiciona nova tarefa ao Grid, parametros devem ser digitados no corpo como a seguir:\n"
                   + "\n"
                   + "defEstado, defObs, papelGeracao, DTIniAval, DTFimAval, DTIniSim, DTFimSim\n"
                   + " pv5#pv-5, RSI1, ^BVSP, 2000-06-01, 2008-02-01, 2008-02-01, 2009-06-01\n"
                   + "\n" + "Help - Esta mensagem\n";

            Thread t = new Thread(delegate ()
            {
                try
                {
                    GmailSender sender = new GmailSender(retFrom,
                            Settings.gridEmailInterfacePassword);
                    sender.SendMail(retSubject, retBody, retFrom, retTo,
                            Settings.gridEmailInterfaceCC);
                }
                catch (Exception e)
                {
                    Log.WriteLine(e.ToString(), Log.Level.error);
                }
            });
            t.Start();
        }

        public static void InvalidTask(String to, String body)
        {
            String retFrom = Settings.gridEmailInterfaceAcount;
            String retTo = to;
            String retSubject = "Invalid Task Received";
            String retBody = body;

            Thread t = new Thread(delegate ()
            {
                try
                {
                    GmailSender sender = new GmailSender(retFrom,
                        Settings.gridEmailInterfacePassword);
                    sender.SendMail(retSubject, retBody, retFrom, retTo,
                        Settings.gridEmailInterfaceCC);
                }
                catch (Exception e)
                {
                    Log.WriteLine(e.ToString(), Log.Level.error);
                }
            });
            t.Start();
        }

        public static void NewTask(String to, String body)
        {
            String retFrom = Settings.gridEmailInterfaceAcount;
            String retTo = to;
            String retSubject = "New Task Received";
            String retBody = body;

            Thread t = new Thread(delegate ()
            {
                try
                {
                    GmailSender sender = new GmailSender(retFrom,
                    Settings.gridEmailInterfacePassword);
                    sender.SendMail(retSubject, retBody, retFrom, retTo,
                    Settings.gridEmailInterfaceCC);
                }
                catch (Exception e)
                {
                    Log.WriteLine(e.ToString(), Log.Level.error);
                }
            });
            t.Start();
        }

        public static void TaskResults(String projectName, Job args, String to,
                String textLog)
        {

            if (args != null)
            {
                try
                {

                    String stateDef = args.getStateDef();
                    String observDef = args.getObservDef();
                    String paper = args.getPaper();
                    //  String plFrom = args.getPlFrom();
                    //  String plTo = args.getPlTo();
                    //  String simFrom = args.getSimFrom();
                    //  String simTo = args.getSimTo();
                    //  int epoch = args.getEpoch();

                    String retFrom = Settings.gridEmailInterfaceAcount;
                    String retTo = to + ", "
                           + Settings.gridEmailInterfaceAcount;
                    String retSubject = "GridTask Finished: " + projectName
                           + ", " + paper + ", " + stateDef + ", " + observDef;
                    String retBody = textLog;
                    String[] retAttachments = {
                projectName + ".log",
                        projectName + ".simul", projectName + ".result",
                        projectName + ".alpha", projectName + ".pg",
                        projectName + ".b", projectName + ".pomdp" };

                    Thread t = new Thread(delegate ()
                    {
                        try
                        {
                            GmailSender sender = new GmailSender(retFrom,
                            Settings.gridEmailInterfacePassword);
                            sender.SendMail(retSubject, retBody, retFrom,
                            retTo, Settings.gridEmailInterfaceCC,
                            retAttachments);
                        }
                        catch (Exception e)
                        {
                            Log.WriteLine(e.ToString(), Log.Level.error);
                        }
                    });
                    t.Start();

                }
                catch (Exception e)
                {
                    Log.WriteLine(e.ToString(), Log.Level.error);
                }
            }

        }
    }
}