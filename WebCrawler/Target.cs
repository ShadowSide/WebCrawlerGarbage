using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    class Target : ITarget
    {
        public string Name { get; set; }

        public string Description{ get; set; }

        public TimeSpan OccurEvery { get; set; }

        public static ITarget LoadTarget(TargetsModel target, string name)
        {
            Check(target);
            return new Target()
            {
                Name = target.Name ?? name,
                Description = target.Description,
                OccurEvery = TimeSpan.FromMinutes(target.RepeatAfterAmountMinutes ?? 1)
            };
        }

        private static void Check(TargetsModel model)
        {
            throw new NotImplementedException();
        }

        public void Schedule(IExecuteScheduler scheduler)
        {
            if (_subscriber != null)
                throw new InvalidOperationException(nameof(Target.Schedule));
            _subscriber = scheduler.Schedule(ProcessTarget, OccurEvery);
        }

        public void ProcessTarget()
        {
            //throw new NotImplementedException();
        }

        private IDisposable _subscriber;

        public void Dispose()
        {
            if (_subscriber is var s && s != null)
            {
                _subscriber = null;
                s.Dispose();
            }
        }
    }
}
