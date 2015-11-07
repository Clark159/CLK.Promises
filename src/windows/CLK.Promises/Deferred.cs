using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Promises
{
    public sealed class Deferred
    {
        // Fields
        private Promise _promise = null;

        private Action _resolve = null;

        private Action<Exception> _reject = null;

        private Action<Progress> _notify = null;


        // Constructors
        public Deferred()
        {
            // Promise
            _promise = new Promise(delegate (Action resolve, Action<Exception> reject, Action<Progress> notify)
            {
                // Initialize
                _resolve = resolve;
                _reject = reject;
                _notify = notify;
            });
        }


        // Properties
        public Promise Promise
        {
            get
            {
                return _promise;
            }
        }


        // Methods        
        public void Resolve()
        {
            // Resolve
            _resolve();
        }

        public void Reject(Exception error)
        {
            // Reject
            _reject(error);
        }

        public void Notify(Progress progress)
        {
            // Notify
            _notify(progress);
        }
    }

    public sealed class Deferred<TResult>
    {
        // Fields
        private Promise<TResult> _promise = null;

        private Action<TResult> _resolve = null;

        private Action<Exception> _reject = null;

        private Action<Progress> _notify = null;


        // Constructors
        public Deferred()
        {
            // Promise
            _promise = new Promise<TResult>(delegate (Action<TResult> resolve, Action<Exception> reject, Action<Progress> notify)
            {
                // Initialize
                _resolve = resolve;
                _reject = reject;
                _notify = notify;
            });
        }


        // Properties
        public Promise<TResult> Promise
        {
            get
            {
                return _promise;
            }
        }


        // Methods        
        public void Resolve(TResult result)
        {
            // Resolve
            _resolve(result);
        }

        public void Reject(Exception error)
        {
            // Reject
            _reject(error);
        }

        public void Notify(Progress progress)
        {
            // Notify
            _notify(progress);
        }
    }
}
