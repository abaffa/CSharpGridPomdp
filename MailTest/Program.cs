using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailTest
{
    class Program
    {


        static void Main(string[] args)
        {
            try
            {

                            GmailUtilities gmail = new GmailUtilities();
                            gmail.SetUserPass("gridpomdp@gmail.com", "q1w2e3r4t5");
                            gmail.Connect();
                            gmail.OpenFolder("INBOX");
                            
                            gmail.PrintAllMessages();

                //GmailSender sender = new GmailSender("gridpomdp@gmail.com", "q1w2e3r4t5");
                //sender.SendMail("POMDPGrid", "teste", "gridpomdp@gmail.com", "augbaffa@gmail.com");



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }
        }
    }
}
