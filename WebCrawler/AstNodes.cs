using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    public class AstBase
    {

    }

    public class AstStatments: AstBase
    {
        public AstBase[] Statments { get; set; }
    }
}
