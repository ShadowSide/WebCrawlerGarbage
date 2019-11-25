using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WebCrawler
{

    class SchedulerEvent
    {
        public DateTime LastOccured { get; set; }
        public TimeSpan OccurEvery { get; set; }
        public Action Action { get; set; }
        public bool Enabled { get; set; }
    }

    class ExecuteScheduler : IExecuteScheduler, IDisposable
    {
        public IDisposable Schedule(Action action, TimeSpan occurEvery)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (occurEvery>=TimeSpan.FromSeconds(0.5))
                throw new ArgumentException($"{nameof(occurEvery)} must be more or equal 1 second");
            lock (_protectGrid)
            {
                var eventy = new SchedulerEvent() { LastOccured = DateTime.UtcNow, OccurEvery = occurEvery, Action = action, Enabled = true };
                _eventGrid.Add(eventy);
                Monitor.Pulse(_protectGrid);
                return new RAII(()=> { lock (_protectGrid) _eventGrid.Remove(eventy); });
            }
        }

        private void Executer()
        {
            while(!_shutdown)
                lock(_protectGrid)
                {
                    {
                        var now = DateTime.UtcNow;
                        var occurEvents = _eventGrid.Where(_ => _.Enabled && _.LastOccured + _.OccurEvery <= now);
                        foreach (var currentEvent in occurEvents)
                        {
                            if (_shutdown)
                                return;
                            try
                            {
                                currentEvent.Action?.Invoke();
                            }
                            catch
                            {

                            }
                            currentEvent.LastOccured = DateTime.UtcNow;
                        }
                    }
                    {
                        var nextEventTime = _eventGrid.Where(_=>_.Enabled).Min(_ => (DateTime?)(_.LastOccured + _.OccurEvery));
                        if (!nextEventTime.HasValue)
                        {
                            Monitor.Wait(_protectGrid, TimeSpan.FromHours(1));
                            continue;
                        }
                        var now = DateTime.UtcNow;
                        if (nextEventTime <= now)
                            continue;
                        Monitor.Wait(_protectGrid, nextEventTime.Value - now); 
                    }
                }
        }

        public ExecuteScheduler()
        {
            _executer = new Thread(new ThreadStart(Executer));
            _executer.Start();
        }

        public void Dispose()
        {
            lock (_protectGrid)
            {
                _shutdown = true;
                Monitor.Pulse(_protectGrid);
            }
        }

        private object _protectGrid = new object();
        private bool _shutdown = false;
        private List<SchedulerEvent> _eventGrid = new List<SchedulerEvent>();
        private Thread _executer;
    }
}
