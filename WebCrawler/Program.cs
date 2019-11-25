using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using CsQuery;

namespace WebCrawler
{
    class Program
    {
        static readonly string Host = @"http://www.lostfilm.tv/";//@"https://anistar.me/";//@"https://gamedev.ru/flame/forum/";//@"http://www.lostfilm.tv/";
        static readonly string DownloadingLostPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloading/lostfilm.txt");

        static string FetchHttp(string uri) 
        {
            try
            {
                var webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                return webClient.DownloadString(uri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        static CQ ParseHtml(string html)
        {
            var dom = CQ.Create(html);
            return dom;
        }

        static void Process()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(DownloadingLostPath));
                var html = FetchHttp(Host);
                if (html == null)
                    return;
                File.WriteAllText(DownloadingLostPath,  html);
                var result = ParseHtml(html);
                foreach (IDomObject obj in result.Find("a")[@".new-movie"])
                {
                    Console.WriteLine(obj);
                    //
                }

                Console.WriteLine(result != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            Process();
        }
    }
}
