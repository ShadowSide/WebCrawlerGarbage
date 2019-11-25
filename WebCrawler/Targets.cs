using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace WebCrawler
{
    class Targets
    {
        public Targets(IExecuteScheduler scheduler)
        {
            Load(scheduler);
        }

        public void Clear()
        {
            foreach (var target in _nameToTarget.Values)
                target.Dispose();
            _nameToTarget.Clear();
        }

        public void Load(IExecuteScheduler scheduler)
        {   
            foreach (var targetPath in Directory.GetFiles(ProgramConsts.TargetsDir))
            {
                try
                {
                    var target = LoadTarget(targetPath);
                    target.Schedule(scheduler);
                    _nameToTarget.Add(target.Name, target);
                } catch
                {

                }
            }
        }

        private ITarget LoadTarget(string path)
        {
            var targetJsonString = File.ReadAllText(path);
            var targetModel = JsonConvert.DeserializeObject<TargetsModel>(targetJsonString);
            return Target.LoadTarget(targetModel, Path.GetFileNameWithoutExtension(path));
        }

        private Dictionary<string, ITarget> _nameToTarget = new Dictionary<string, ITarget>();
    }
}
