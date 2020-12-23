using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;

namespace GridPomdp.Utils
{
    //   import org.baffa.grid.Settings;


    public class TwitterLogger
    {

        //	SimpleDateFormat dateformat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");

        TwitterService twitter;



        public TwitterLogger()
        {
            twitter = new TwitterService(Settings.gridTwitterConsumerKey, Settings.gridTwitterConsumerSecret);
            twitter.AuthenticateWith(Settings.gridTwitterToken, Settings.gridTwitterTokenSecret);
        }

        private void publish(String msg)
        {
            TwitterStatus result = twitter.SendTweet(new SendTweetOptions
            {
                Status = msg
            });
        }
        public void SendMsg(String arg0)
        {

            try
            {

                if (arg0.Length > 160)
                    publish(arg0.Substring(0, 160));
                else
                    publish(arg0);

            }
            catch (Exception e)
            {
                Log.WriteLine(e.ToString(), Log.Level.error);
            }
        }
    }
}
