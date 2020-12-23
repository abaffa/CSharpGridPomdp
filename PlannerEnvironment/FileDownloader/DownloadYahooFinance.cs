using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileDownloader
{
    public class DownloadYahooFinance
    {
        private static Regex _regex = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})", RegexOptions.Compiled);
        public string Decoder(string value)
        {
            return _regex.Replace(
                value,
                m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString()
            );
        }

        public String DownloadFile(String paper, DateTime dtFrom, DateTime dtTo)
        {
            String _paper = WebUtility.UrlEncode(paper);

            Int32 unixDtFrom = (Int32)(dtFrom.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Int32 unixDtTo = (Int32)(dtTo.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            //https://query1.finance.yahoo.com/v7/finance/download/SAN?period1=1483250400&period2=1495256400&interval=1d&events=history&crumb=vrc3eD69ygJ
            String filterParams = "period1=" + unixDtFrom + "&period2=" + unixDtTo;
            
            var client = new CookieAwareWebClient();
            String sHtml = client.RequestGet("https://finance.yahoo.com/quote/" + _paper + "/history?" + filterParams + "&interval=1d&filter=history&frequency=1d");

            String crumbParam = "{crumb}";
            int startIndex = 0;
            int lastIndex = 0;
            do
            {
                String crumbStr = "\"crumb\"";
                int crumbStart = sHtml.IndexOf(crumbStr, startIndex);
                int paramStart = crumbStart + crumbStr.Length + 1;
                int firstQuote = sHtml.IndexOf("\"", paramStart) + 1;
                int lastQuote = sHtml.IndexOf("\"", firstQuote);
                int paramLength = lastQuote - firstQuote;
                crumbParam = Decoder(sHtml.Substring(firstQuote, paramLength));
                lastIndex = startIndex;
                startIndex = lastQuote;
            } while (lastIndex != startIndex && crumbParam == "{crumb}");

            Console.WriteLine("Crumb: " + crumbParam);
            String sCsv = client.RequestGet("https://query1.finance.yahoo.com/v7/finance/download/" + paper + "?" + filterParams + "&interval=1d&events=history&crumb=" + crumbParam);

            return sCsv;
        }
    }
}
