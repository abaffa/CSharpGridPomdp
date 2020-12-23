using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloader
{
    public class Downloader
    {
        bool finished = false;
        private String fileURL = "";

        private String filePath = "";

        private String fileName = "";

        private String fileTo = "";

        public Boolean IsFinished { get { return finished; } }
        public Downloader(String fileURL, String fileTo)
        {
            this.fileURL = fileURL;
            this.filePath = fileTo;
            this.fileName = fileURL.Substring(1 + fileURL.LastIndexOf("/"));
            this.fileTo = this.filePath + "/" + this.fileName;
        }

        public Downloader(String fileURL, String fileTo, String saveAs)
        {
            this.fileURL = fileURL;
            this.filePath = fileTo;
            this.fileName = saveAs;
            this.fileTo = this.filePath + "/" + this.fileName;
        }

        public void StartDownload()
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri(fileURL),
               fileTo);
            }
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            finished = (e.ProgressPercentage == 100);
        }
    }
}
