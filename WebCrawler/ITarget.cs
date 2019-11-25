using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    interface ITarget: IDisposable
    {
        string Name { get; }
        string Description { get; }
        TimeSpan OccurEvery { get; }
        void ProcessTarget();
        void Schedule(IExecuteScheduler scheduler);
    }
}
