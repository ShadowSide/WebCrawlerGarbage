using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WebCrawler
{
    static class ProgramConsts
    {
        public static readonly string CurrentDir = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string TargetsDir = Path.Combine(CurrentDir, "Targets");
    }
}
