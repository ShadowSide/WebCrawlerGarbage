using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    class RAII: IDisposable
    {
        public RAII(Action disposer = null, Action initializer = null)
        {
            initializer?.Invoke();
            _disposer = disposer;
        }

        private Action _disposer;

        public void Dispose()
        {
            if (_disposer is var d && d != null)
            {
                _disposer = null;
                d();
            }
        }
    }
}
