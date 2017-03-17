using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace tzskSite_v1
{
    public static class Logger
    {
        public static void user (string log)
        {
            string path = HttpContext.Current.Server.MapPath("userLog.txt");

            using (StreamWriter sw = File.AppendText(path))          
                sw.WriteLine(log);           
        }
    }
}