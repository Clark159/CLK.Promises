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


        // Constructors
        public Deferred()
        {
            // Promise
            _promise = new Promise();
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
            _promise.InnerResolve();
        }

        public void Reject(Exception error)
        {
            // Reject
            _promise.InnerReject(error);
        }

        public void Notify(Progress progress)
        {
            // Notify
            _promise.InnerNotify(progress);
        }
    }

    public sealed class Deferred<TResult>
    {
        // Fields
        private Promise<TResult> _promise = null;


        // Constructors
        public Deferred()
        {
            // Promise
            _promise = new Promise<TResult>();
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
            _promise.InnerResolve(result);
        }

        public void Reject(Exception error)
        {
            // Reject
            _promise.InnerReject(error);
        }

        public void Notify(Progress progress)
        {
            // Notify
            _promise.InnerNotify(progress);
        }
    }
}
