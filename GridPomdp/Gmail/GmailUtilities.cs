using S22.Imap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GridPomdp.Utils
{
    public class GmailUtilities
    {
        private String username, password;
        private int port = 993; //995; 
        private string hostname = "imap.gmail.com"; // "pop.gmail.com"
        ImapClient client;

        public GmailUtilities()
        {
        }

        public void SetUserPass(String username, String password)
        {
            this.username = username;
            this.password = password;
        }


        public void Connect()
        {

            client = new ImapClient(hostname, port, username, password, AuthMethod.Login, true);
        }

        public void OpenFolder(String folderName)
        {
            client.DefaultMailbox = folderName;
            //IEnumerable<string> mailbox = client.ListMailboxes();
        }

        /*
        public void closeFolder()
        {
            //folder.close(false);
        }
        */

        public int GetMessageCount()
        {
            return client.Search(SearchCondition.All(), client.DefaultMailbox).Count();
        }

        public int GetNewMessageCount()
        {
            return client.Search(SearchCondition.Unseen(), client.DefaultMailbox).Count();
        }

        public void Disconnect()
        {
            client.Dispose();
        }

        public void PrintMessage(uint messageNo)
        {
            Console.WriteLine("Getting message number: " + messageNo);

            MailMessage m = null;

            try
            {
                MailMessage mail = client.GetMessage(messageNo);
                dumpPart(m);
            }
            catch (Exception iex)
            {
                Console.WriteLine("Message number out of range");
            }

        }

        public void PrintAllMessageEnvelopes()
        {

            // Attributes & Flags for all messages ..
            IEnumerable<uint> allMailsID = client.Search(SearchCondition.Unseen(), client.DefaultMailbox); //, "INBOX");
            IEnumerable<MailMessage> allMails = client.GetMessages(allMailsID, false);//, "INBOX");

            int i = 0;
            // Use a suitable FetchProfile
            foreach (MailMessage mail in allMails)
            {
                Console.WriteLine("--------------------------");
                Console.WriteLine("MESSAGE #" + (i + 1) + ":");


                dumpEnvelope(mail);

                i++;
            }

        }



        public List<String[]> GetMessages()
        {
            List<String[]> ret = new List<String[]>();
            // Attributes & Flags for all messages ..
            IEnumerable<uint> allMailsID = client.Search(SearchCondition.Unseen(), client.DefaultMailbox); //, "INBOX");
                                                                                                        //IEnumerable<MailMessage> allMails = client.GetMessages(allMailsID, false);//, "INBOX");

            int i = 0;
            // Use a suitable FetchProfile
            foreach (uint uid in allMailsID)
            {
                MailMessage mail = client.GetMessage(uid);

                MessageFlag[] flags = { MessageFlag.Seen };
                client.SetMessageFlags(uid, client.DefaultMailbox, flags);

                String[] body = new String[3];
                body[0] = mail.From.Address;
                body[1] = mail.Subject;
                body[2] = mail.Body;
                ret.Add(body);               

            }


            return ret;

        }


        public void PrintAllMessages()
        {

            // Attributes & Flags for all messages ..
            IEnumerable<uint> allMailsID = client.Search(SearchCondition.Unseen(), client.DefaultMailbox); //, "INBOX");
                                                                                                        //IEnumerable<MailMessage> allMails = client.GetMessages(allMailsID, false);//, "INBOX");

            int i = 0;
            // Use a suitable FetchProfile
            foreach (uint uid in allMailsID)
            {
                MailMessage mail = client.GetMessage(uid);

                MessageFlag[] flags = { MessageFlag.Seen };
                client.SetMessageFlags(uid, client.DefaultMailbox, flags);
                Console.WriteLine("--------------------------");
                Console.WriteLine("MESSAGE #" + (i + 1) + ":");


                dumpPart(mail);

                i++;
            }

        }



        public void dumpPart(MailMessage mail)
        {
            Console.WriteLine("Subject:" + mail.Subject);
            foreach (MailAddress to in mail.To)
            {
                Console.WriteLine("To:" + to.Address);
            }
            Console.WriteLine("From:" + mail.From);
            Console.WriteLine("CC:" + mail.CC);
            Console.WriteLine("BCC:" + mail.Bcc);
            Console.WriteLine("Attachments:" + mail.Attachments.Count);
            Console.WriteLine("Body:" + mail.Body);
            Console.WriteLine("Composed Maild Date:" + mail.Date());
            Console.WriteLine("Priority" + mail.Priority);
            //Console.WriteLine("Reply To: " + mail.ReplyTo);
            Console.WriteLine("Reply To List" + mail.ReplyToList);
            Console.WriteLine("Sender:" + mail.Sender);

            AttachmentCollection attachColl = mail.Attachments;
            if (attachColl.Count > 0)
            {
                foreach (var item in attachColl)
                {
                    Console.WriteLine("Name:" + item.Name);
                    //CopyStream(item.ContentStream, (filePath + item.Name));

                }
            }
        }

        public static void dumpEnvelope(MailMessage mail)
        {

            // FROM
            Console.WriteLine("From:" + mail.From);

            // TO
            foreach (MailAddress to in mail.To)
            {
                Console.WriteLine("To:" + to.Address);
            }

            // SUBJECT
            Console.WriteLine("Subject:" + mail.Subject);

            // DATE
            Console.WriteLine("SendDate: " + (mail.Date().HasValue ? mail.Date().Value.ToString() : "UNKNOWN"));

        }
    }
}
