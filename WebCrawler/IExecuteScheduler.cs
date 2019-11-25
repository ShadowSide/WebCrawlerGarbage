using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    interface IExecuteScheduler
    {
        IDisposable Schedule(Action action, TimeSpan occureEvery);
    }
}
