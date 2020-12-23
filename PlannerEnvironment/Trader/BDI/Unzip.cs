using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace org.baffa.utils
{

    public class Unzip
    {
        public static bool Extract(String filename)
        {
            return Extract(filename, filename.Substring(0, filename.LastIndexOf("/")));
        }




        public static bool Extract(String filename, String path)
        {
            bool ret = false;
            try
            {
                ZipFile.ExtractToDirectory(filename, path);
                ret = true;
            }
            catch (Exception ex)
            {

                ret = false;
                Console.WriteLine("Unhandled exception:");
                Console.WriteLine(ex.ToString());

            }
            return ret;
        }

    }
}
