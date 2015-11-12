using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Promises
{
    public abstract class PromiseBase<TResult>
    {
        // Enumerations
        private enum PromiseState
        {
            Pending,  // unresolved  
            Resolved, // has-resolution
            Rejected, // has-rejection
        };

        protected enum ResultType
        {
            Empty,
            EmptyPromise,
            New,
            NewPromise,
        }


        // Fields
        private Object _syncRoot = new Object();

        private PromiseState _state = PromiseState.Pending;

        private TResult _result = default(TResult);

        private Exception _error = null;

        private List<Action<TResult>> _resolveHandlers = null;

        private List<Action<Exception>> _rejectHandlers = null;

        private List<Action<Progress>> _notifyHandlers = null;

        private List<Action<Progress>> _notifyHandlersSnapshot = null;


        // Methods 
        protected void Push(Action<TResult> resolveHandler, Action<Exception> rejectHandler, Action<Progress> notifyHandler)
        {
            #region Contracts

            if (resolveHandler == null) throw new ArgumentNullException();
            if (rejectHandler == null) throw new ArgumentNullException();
            if (notifyHandler == null) throw new ArgumentNullException();

            #endregion

            // Sync         
            lock (_syncRoot)
            {
                // ResolveHandler
                if (_state == PromiseState.Pending)
                {
                    if (_resolveHandlers == null)
                    {
                        _resolveHandlers = new List<Action<TResult>>();
                    }
                    _resolveHandlers.Add(resolveHandler);
                }

                // RejectHandler
                if (_state == PromiseState.Pending)
                {
                    if (_rejectHandlers == null)
                    {
                        _rejectHandlers = new List<Action<Exception>>();
                    }
                    _rejectHandlers.Add(rejectHandler);
                }

                // NotifyHandler
                if (_notifyHandlers == null)
                {
                    _notifyHandlers = new List<Action<Progress>>();
                }
                _notifyHandlers.Add(notifyHandler);
                _notifyHandlersSnapshot = null;

                // Pending
                if (_state == PromiseState.Pending)
                {
                    return;
                }
            }

            // Resolved
            if (_state == PromiseState.Resolved)
            {
                resolveHandler(_result);
            }

            // Rejected
            if (_state == PromiseState.Rejected)
            {
                rejectHandler(_error);
            }
        }

        protected void ResolveBase(TResult result)
        {
            // Sync
            lock (_syncRoot)
            {
                // State
                if (_state != PromiseState.Pending) return;
                _state = PromiseState.Resolved;

                // Results
                _result = result;
                _error = null;
            }

            // Handlers
            List<Action<TResult>> resolveHandlers = _resolveHandlers;
            if (resolveHandlers == null) return;

            // Resolve
            foreach (Action<TResult> resolveHandler in resolveHandlers)
            {
                resolveHandler(result);
            }
        }

        public void Reject(Exception error)
        {
            #region Contracts

            if (error == null) throw new ArgumentNullException();

            #endregion

            // Sync            
            lock (_syncRoot)
            {
                // State
                if (_state != PromiseState.Pending) return;
                _state = PromiseState.Rejected;

                // Result
                _result = default(TResult);
                _error = error;
            }

            // Handlers
            List<Action<Exception>> rejectHandlers = _rejectHandlers;
            if (rejectHandlers == null) return;

            // Reject
            foreach (Action<Exception> rejectHandler in rejectHandlers)
            {
                rejectHandler(error);
            }
        }

        public void Notify(Progress progress)
        {
            #region Contracts

            if (progress == null) throw new ArgumentNullException();

            #endregion

            // Handlers
            List<Action<Progress>> notifyHandlers = null;
            lock (_syncRoot)
            {
                if (_notifyHandlersSnapshot == null && _notifyHandlers != null)
                {
                    _notifyHandlersSnapshot = new List<Action<Progress>>();
                    foreach (Action<Progress> notifyHandler in _notifyHandlers)
                    {
                        _notifyHandlersSnapshot.Add(notifyHandler);
                    }
                }
                notifyHandlers = _notifyHandlersSnapshot;
            }
            if (notifyHandlers == null) return;

            // Notify
            foreach (Action<Progress> notifyHandler in notifyHandlers)
            {
                notifyHandler(progress);
            }
        }
    }
}
