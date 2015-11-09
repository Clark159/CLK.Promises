using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Promises
{
    public class Promise : Promise<Object>
    {
        // Constructors
        public Promise() { }


        // Methods 
        public static Promise<TResult> Resolve<TResult>(TResult result)
        {
            // Promise
            var promise = new Promise<TResult>();
            promise.InnerResolve(result);

            // Return
            return promise;
        }

        public static Promise Reject(Exception error)
        {
            // Promise
            var promise = new Promise();
            promise.InnerReject(error);

            // Return
            return promise;
        }
    }

    public class Promise<TResult>
    {
        // Enumerations
        private enum PromiseState
        {
            Pending,  // unresolved  
            Resolved, // has-resolution
            Rejected, // has-rejection
        };

        private enum ResultType
        {
            NullResult,
            NullResultPromise,
            NewResult,
            NewResultPromise,
        }


        // Fields
        private readonly object _syncRoot = new object();

        private PromiseState _state = PromiseState.Pending;

        private TResult _result = default(TResult);

        private Exception _error = null;


        private List<Action<TResult>> _resolveHandlers = null;

        private List<Action<Exception>> _rejectHandlers = null;

        private List<Action<Progress>> _notifyHandlers = null;

        private List<Action<Progress>> _notifyHandlersSnapshot = null;


        private Action _passResolved = null;

        private Action<Exception> _passRejected = null;

        private Action<Progress> _passNotified = null;


        // Constructors
        public Promise() { }


        // Methods 
        internal void InnerResolve(TResult result = default(TResult))
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
            var resolveHandlers = _resolveHandlers;
            if (resolveHandlers == null) return;

            // Resolve
            foreach (var resolveHandler in resolveHandlers)
            {
                resolveHandler(result);
            }
        }

        internal void InnerReject(Exception error)
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
            var rejectHandlers = _rejectHandlers;
            if (rejectHandlers == null) return;

            // Resolve
            foreach (var rejectHandler in rejectHandlers)
            {
                rejectHandler(error);
            }
        }

        internal void InnerNotify(Progress progress)
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
                    _notifyHandlersSnapshot = _notifyHandlers.ToList();
                }
                notifyHandlers = _notifyHandlersSnapshot;
            }
            if (notifyHandlers == null) return;

            // Notify
            foreach (var notifyHandler in notifyHandlers)
            {
                notifyHandler(progress);
            }
        }


        private void PushHandlers(Action<TResult> resolveHandler, Action<Exception> rejectHandler, Action<Progress> notifyHandler)
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
                    if(_resolveHandlers==null)
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
                    _notifyHandlers = new List<Action<Promises.Progress>>();
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

        private Promise PushThen(Func<TResult, Object> onResolved, ResultType onResolvedResultType, 
                                 Func<Exception, Object> onRejected, ResultType onRejectedResultType, 
                                 Action<Progress> onNotified)
        {
            // Promise
            var thenPromise = new Promise();

            // Handlers
            Action<TResult> resolveHandler = delegate (TResult result)
            {
                try
                {
                    // Execute
                    var resultObject = onResolved(result);

                    // Distribute
                    switch (onResolvedResultType)
                    {
                        case ResultType.NullResult:
                            thenPromise.InnerResolve();
                            break;

                        case ResultType.NullResultPromise:
                            ((Promise)resultObject).Then(
                                delegate () { thenPromise.InnerResolve(); },
                                delegate (Exception thenError) { thenPromise.InnerReject(thenError); },
                                delegate (Progress thenProgress) { thenPromise.InnerNotify(thenProgress); }
                            );
                            break;

                        default:
                            throw new Exception("Invalid Result");
                    }
                }
                catch (Exception ex)
                {
                    thenPromise.InnerReject(ex);
                }
            };

            Action<Exception> rejectHandler = delegate (Exception error)
            {
                try
                {
                    // Execute
                    var resultObject = onRejected(error);

                    // Distribute
                    switch (onRejectedResultType)
                    {
                        case ResultType.NullResult:
                            thenPromise.InnerResolve();
                            break;

                        case ResultType.NullResultPromise:
                            ((Promise)resultObject).Then(
                                delegate () { thenPromise.InnerResolve(); },
                                delegate (Exception thenError) { thenPromise.InnerReject(thenError); },
                                delegate (Progress thenProgress) { thenPromise.InnerNotify(thenProgress); }
                            );
                            break;

                        default:
                            throw new Exception("Invalid Result");
                    }
                }
                catch (Exception ex)
                {
                    thenPromise.InnerReject(ex);
                }
            };

            Action<Progress> notifiedHandler = delegate (Progress progress)
            {
                try
                {
                    // Execute
                    onNotified(progress);

                    // Distribute
                    thenPromise.InnerNotify(progress);
                }
                catch (Exception ex)
                {
                    thenPromise.InnerReject(ex);
                }
            };

            // PushHandlers
            this.PushHandlers(resolveHandler, rejectHandler, notifiedHandler);

            // Return
            return thenPromise;
        }

        private Promise<TNewResult> PushThen<TNewResult>(Func<TResult, Object> onResolved, ResultType onResolvedResultType,
                                                         Func<Exception, Object> onRejected, ResultType onRejectedResultType,
                                                         Action<Progress> onNotified)
        {
            // Promise
            var thenPromise = new Promise<TNewResult>();

            // Handlers
            Action<TResult> resolveHandler = delegate (TResult result)
            {
                try
                {
                    // Execute
                    var resultObject = onResolved(result);

                    // Distribute
                    switch (onResolvedResultType)
                    {
                        case ResultType.NullResult:
                            thenPromise.InnerResolve(default(TNewResult));
                            break;

                        case ResultType.NullResultPromise:
                            ((Promise)resultObject).Then(
                                delegate () { thenPromise.InnerResolve(default(TNewResult)); },
                                delegate (Exception thenError) { thenPromise.InnerReject(thenError); },
                                delegate (Progress thenProgress) { thenPromise.InnerNotify(thenProgress); }
                            );
                            break;

                        case ResultType.NewResult:
                            thenPromise.InnerResolve((TNewResult)resultObject);
                            break;

                        case ResultType.NewResultPromise:
                            ((Promise<TNewResult>)resultObject).Then(
                                delegate (TNewResult thenResult) { thenPromise.InnerResolve(thenResult); },
                                delegate (Exception thenError) { thenPromise.InnerReject(thenError); },
                                delegate (Progress thenProgress) { thenPromise.InnerNotify(thenProgress); }
                            );
                            break;

                        default:
                            throw new Exception("Invalid Result");
                    }
                }
                catch (Exception ex)
                {
                    thenPromise.InnerReject(ex);
                }
            };

            Action<Exception> rejectHandler = delegate (Exception error)
            {
                try
                {
                    // Execute
                    var resultObject = onRejected(error);

                    // Distribute
                    switch (onRejectedResultType)
                    {
                        case ResultType.NullResult:
                            thenPromise.InnerResolve(default(TNewResult));
                            break;

                        case ResultType.NullResultPromise:
                            ((Promise)resultObject).Then(
                                delegate () { thenPromise.InnerResolve(default(TNewResult)); },
                                delegate (Exception thenError) { thenPromise.InnerReject(thenError); },
                                delegate (Progress thenProgress) { thenPromise.InnerNotify(thenProgress); }
                            );
                            break;

                        case ResultType.NewResult:
                            thenPromise.InnerResolve((TNewResult)resultObject);
                            break;

                        case ResultType.NewResultPromise:
                            ((Promise<TNewResult>)resultObject).Then(
                                delegate (TNewResult thenResult) { thenPromise.InnerResolve(thenResult); },
                                delegate (Exception thenError) { thenPromise.InnerReject(thenError); },
                                delegate (Progress thenProgress) { thenPromise.InnerNotify(thenProgress); }
                            );
                            break;

                        default:
                            throw new Exception("Invalid Result");
                    }
                }
                catch (Exception ex)
                {
                    thenPromise.InnerReject(ex);
                }
            };

            Action<Progress> notifiedHandler = delegate (Progress progress)
            {
                try
                {
                    // Execute
                    onNotified(progress);

                    // Distribute
                    thenPromise.InnerNotify(progress);
                }
                catch (Exception ex)
                {
                    thenPromise.InnerReject(ex);
                }
            };

            // PushHandlers
            this.PushHandlers(resolveHandler, rejectHandler, notifiedHandler);

            // Return
            return thenPromise;
        }


        private Action PassResolved()
        {
            if (_passResolved == null)
            {
                _passResolved = delegate ()
                {
                   // Nothing

                };
            }
            return _passResolved;
        }

        private Action<Exception> PassRejected()
        {
            if (_passRejected == null)
            {
                _passRejected = delegate (Exception error)
                {
                    // Throw
                    throw error;
                };
            }
            return _passRejected;
        }

        private Action<Progress> PassNotified()
        {
            if (_passNotified == null)
            {
                _passNotified = delegate (Progress progress)
                {
                    // Nothing

                };
            }
            return _passNotified;
        }


        // Then(Func<out *>)
        public Promise Then(Action onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { onResolved(); return null; }, ResultType.NullResult,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.NullResult,
                onNotified
            );
        }

        public Promise Then(Action onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { onResolved(); return null; }, ResultType.NullResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NullResultPromise,
                onNotified
            );
        }


        public Promise Then(Func<Promise> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { return onResolved(); }, ResultType.NullResultPromise,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.NullResult,
                onNotified
            );
        }

        public Promise Then(Func<Promise> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { return onResolved(); }, ResultType.NullResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NullResultPromise,
                onNotified
            );
        }


        // Then(Func<TResult, out *>)
        public Promise Then(Action<TResult> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { onResolved(result); return null; }, ResultType.NullResult,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.NullResult,
                onNotified
            );
        }

        public Promise Then(Action<TResult> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { onResolved(result); return null; }, ResultType.NullResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NullResultPromise,
                onNotified
            );
        }


        public Promise Then(Func<TResult, Promise> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { return onResolved(result); }, ResultType.NullResultPromise,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.NullResult,
                onNotified
            );
        }

        public Promise Then(Func<TResult, Promise> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { return onResolved(result); }, ResultType.NullResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NullResultPromise,
                onNotified
            );
        }


        // Then<TNewResult>(Func<out *>)
        public Promise<TNewResult> Then<TNewResult>(Action onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(); return null; }, ResultType.NullResult,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.NullResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Action onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(); return null; }, ResultType.NullResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NullResultPromise,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Action onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(); return null; }, ResultType.NullResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Action onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(); return null; }, ResultType.NullResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResultPromise,
                onNotified
            );
        }


        public Promise<TNewResult> Then<TNewResult>(Func<Promise> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); }, ResultType.NullResultPromise,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.NullResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); }, ResultType.NullResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NullResultPromise,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); }, ResultType.NullResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); }, ResultType.NullResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResultPromise,
                onNotified
            );
        }


        public Promise<TNewResult> Then<TNewResult>(Func<TNewResult> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); }, ResultType.NewResult,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.NullResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TNewResult> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); }, ResultType.NewResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NullResultPromise,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TNewResult> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); }, ResultType.NewResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TNewResult> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); }, ResultType.NewResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResultPromise,
                onNotified
            );
        }


        public Promise<TNewResult> Then<TNewResult>(Func<Promise<TNewResult>> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); }, ResultType.NewResultPromise,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.NullResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise<TNewResult>> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); }, ResultType.NewResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NullResultPromise,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise<TNewResult>> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); }, ResultType.NewResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise<TNewResult>> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); }, ResultType.NewResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResultPromise,
                onNotified
            );
        }


        // Then<TNewResult>(Func<TResult, out *>)
        public Promise<TNewResult> Then<TNewResult>(Action<TResult> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(result); return null; }, ResultType.NullResult,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.NullResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Action<TResult> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(result); return null; }, ResultType.NullResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NullResultPromise,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Action<TResult> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(result); return null; }, ResultType.NullResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Action<TResult> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(result); return null; }, ResultType.NullResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResultPromise,
                onNotified
            );
        }


        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NullResultPromise,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.NullResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NullResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NullResultPromise,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NullResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NullResultPromise,
                delegate (Exception error) { return onRejected(error); },  ResultType.NewResultPromise,
                onNotified
            );
        }


        public Promise<TNewResult> Then<TNewResult>(Func<TResult, TNewResult> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NewResult,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.NullResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, TNewResult> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NewResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NullResultPromise,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, TNewResult> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NewResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, TNewResult> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NewResult,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResultPromise,
                onNotified
            );
        }


        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise<TNewResult>> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NewResultPromise,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.NullResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise<TNewResult>> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NewResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NullResultPromise,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise<TNewResult>> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NewResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResult,
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise<TNewResult>> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NewResultPromise,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewResultPromise,
                onNotified
            );
        }


        // Then(Func<out *>) - Syntactic sugar
        public Promise Then(Action onResolved)
        {
            return this.Then(onResolved, this.PassRejected(), this.PassNotified());
        }

        public Promise Then(Func<Promise> onResolved)
        {
            return this.Then(onResolved, this.PassRejected(), this.PassNotified());
        }


        // Then(Func<TResult, out *>) - Syntactic sugar
        public Promise Then(Action<TResult> onResolved)
        {
            return this.Then(onResolved, this.PassRejected(), this.PassNotified());
        }

        public Promise Then(Func<TResult, Promise> onResolved)
        {
            return this.Then(onResolved, this.PassRejected(), this.PassNotified());
        }


        // Then<TNewResult>(Func<out *>) - Syntactic sugar
        public Promise<TNewResult> Then<TNewResult>(Action onResolved)
        {
            return this.Then<TNewResult>(onResolved, this.PassRejected(), this.PassNotified());
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise> onResolved)
        {
            return this.Then<TNewResult>(onResolved, this.PassRejected(), this.PassNotified());
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TNewResult> onResolved)
        {
            return this.Then<TNewResult>(onResolved, this.PassRejected(), this.PassNotified());
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise<TNewResult>> onResolved)
        {
            return this.Then<TNewResult>(onResolved, this.PassRejected(), this.PassNotified());
        }


        // Then<TNewResult>(Func<TResult, out *>) - Syntactic sugar
        public Promise<TNewResult> Then<TNewResult>(Action<TResult> onResolved)
        {
            return this.Then<TNewResult>(onResolved, this.PassRejected(), this.PassNotified());
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise> onResolved)
        {
            return this.Then<TNewResult>(onResolved, this.PassRejected(), this.PassNotified());
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, TNewResult> onResolved)
        {
            return this.Then<TNewResult>(onResolved, this.PassRejected(), this.PassNotified());
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise<TNewResult>> onResolved)
        {
            return this.Then<TNewResult>(onResolved, this.PassRejected(), this.PassNotified());
        }


        // Catch - Syntactic sugar
        public Promise Catch(Action<Exception> onRejected)
        {
            return this.Then(this.PassResolved(), onRejected, this.PassNotified());
        }

        public Promise Catch(Func<Exception, Promise> onRejected)
        {
            return this.Then(this.PassResolved(), onRejected, this.PassNotified());
        }


        // Catch<TNewResult> - Syntactic sugar
        public Promise<TNewResult> Catch<TNewResult>(Action<Exception> onRejected)
        {
            return this.Then<TNewResult>(this.PassResolved(), onRejected, this.PassNotified());
        }

        public Promise<TNewResult> Catch<TNewResult>(Func<Exception, Promise> onRejected)
        {
            return this.Then<TNewResult>(this.PassResolved(), onRejected, this.PassNotified());
        }

        public Promise<TNewResult> Catch<TNewResult>(Func<Exception, TNewResult> onRejected)
        {
            return this.Then<TNewResult>(this.PassResolved(), onRejected, this.PassNotified());
        }

        public Promise<TNewResult> Catch<TNewResult>(Func<Exception, Promise<TNewResult>> onRejected)
        {
            return this.Then<TNewResult>(this.PassResolved(), onRejected, this.PassNotified());
        }


        // Progress - Syntactic sugar
        public Promise Progress(Action<Progress> onNotified)
        {
            return this.Then(this.PassResolved(), this.PassRejected(), onNotified);
        }
    }
}
