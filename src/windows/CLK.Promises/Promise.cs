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
        public Promise(Action<Action, Action<Exception>, Action<Progress>> resolver) : base(
           delegate (Action<Object> resolve, Action<Exception> reject, Action<Progress> notify)
           {
               resolver(delegate () { resolve(null); }, reject, notify);
           }
        )
        { }


        // Methods 
        public static Promise<TResult> Resolve<TResult>(TResult result)
        {
            // Create
            return new Promise<TResult>(delegate (Action<TResult> resolve, Action<Exception> reject, Action<Progress> notify)
            {
                resolve(result);
            });
        }

        public static Promise Reject(Exception error)
        {
            // Create
            return new Promise(delegate (Action resolve, Action<Exception> reject, Action<Progress> notify)
            {
                reject(error);
            });
        }
    }

    public class Promise<TResult>
    {
        // Fields
        private readonly object _syncRoot = new object();

        private PromiseState _state = PromiseState.Pending;

        private TResult _result = default(TResult);

        private Exception _error = null;


        private List<Action<TResult>> _resolveHandlers = new List<Action<TResult>>();

        private List<Action<Exception>> _rejectHandlers = new List<Action<Exception>>();

        private List<Action<Progress>> _notifyHandlers = new List<Action<Progress>>();

        private List<Action<Progress>> _notifyHandlersSnapshot = null;


        private Action _passResolved = null;

        private Action<Exception> _passRejected = null;

        private Action<Progress> _passNotified = null;


        // Constructors
        public Promise(Action<Action<TResult>, Action<Exception>, Action<Progress>> resolver)
        {
            #region Contracts

            if (resolver == null) throw new ArgumentNullException();

            #endregion

            // Execute
            try
            {
                resolver(this.DoResolve, this.DoReject, this.DoNotify);
            }
            catch (Exception ex)
            {
                this.DoReject(ex);
            }
        }


        // Methods 
        private IEnumerable<Action<TResult>> GetResolveHandlers()
        {
            // Return
            return _resolveHandlers;
        }

        private IEnumerable<Action<Exception>> GetRejectHandlers()
        {
            // Return
            return _rejectHandlers;
        }

        private IEnumerable<Action<Progress>> GetNotifyHandlers()
        {
            // Variables
            IEnumerable<Action<Progress>> notifyHandlers = null;

            // Sync
            lock (_syncRoot)
            {
                // NotifyHandlers
                if (_notifyHandlersSnapshot == null)
                {
                    _notifyHandlersSnapshot = _notifyHandlers.ToList();
                }
                notifyHandlers = _notifyHandlersSnapshot;
            }

            // Return
            return notifyHandlers;
        }


        private void DoResolve(TResult result = default(TResult))
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

            // Resolve
            foreach (var resolveHandler in this.GetResolveHandlers())
            {
                resolveHandler(result);
            }
        }

        private void DoReject(Exception error)
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

            // Reject
            foreach (var rejectHandler in this.GetRejectHandlers())
            {
                rejectHandler(error);
            }
        }

        private void DoNotify(Progress progress)
        {
            #region Contracts

            if (progress == null) throw new ArgumentNullException();

            #endregion

            // Notify
            foreach (var notifyHandler in this.GetNotifyHandlers())
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
                    _resolveHandlers.Add(resolveHandler);
                }

                // RejectHandler
                if (_state == PromiseState.Pending)
                {
                    _rejectHandlers.Add(rejectHandler);
                }

                // NotifyHandler
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

        private Promise PushThen(Func<TResult, Object> onResolved, Func<Exception, Object> onRejected, Action<Progress> onNotified)
        {
            // Promise
            Action thenResolve = null;
            Action<Exception> thenReject = null;
            Action<Progress> thenNotify = null;
            var thenPromise = new Promise(delegate (Action resolve, Action<Exception> reject, Action<Progress> notify)
            {
                // Initialize
                thenResolve = resolve;
                thenReject = reject;
                thenNotify = notify;
            });

            // Handlers
            Action<TResult> resolveHandler = delegate (TResult result)
            {
                try
                {
                    // Result
                    var resultObject = onResolved(result);

                    // Distribute
                    if (resultObject == null)
                    {
                        thenResolve();
                    }
                    else if (resultObject is Promise)
                    {
                        ((Promise)resultObject).Then(
                            delegate () { thenResolve(); },
                            delegate (Exception thenError) { thenReject(thenError); },
                            delegate (Progress thenProgress) { thenNotify(thenProgress); }
                        );
                    }
                    else
                    {
                        throw new Exception("Invalid Result");
                    }
                }
                catch (Exception ex)
                {
                    thenReject(ex);
                }
            };

            Action<Exception> rejectHandler = delegate (Exception error)
            {
                try
                {
                    // Result
                    var resultObject = onRejected(error);

                    // Distribute
                    if (resultObject == null)
                    {
                        thenResolve();
                    }
                    else if (resultObject is Promise)
                    {
                        ((Promise)resultObject).Then(
                            delegate () { thenResolve(); },
                            delegate (Exception thenError) { thenReject(thenError); },
                            delegate (Progress thenProgress) { thenNotify(thenProgress); }
                        );
                    }
                    else
                    {
                        throw new Exception("Invalid Result");
                    }
                }
                catch (Exception ex)
                {
                    thenReject(ex);
                }
            };

            Action<Progress> notifiedHandler = delegate (Progress progress)
            {
                try
                {
                    onNotified(progress);
                }
                catch (Exception ex)
                {
                    thenReject(ex);
                }
            };

            // PushHandlers
            this.PushHandlers(resolveHandler, rejectHandler, notifiedHandler);

            // Return
            return thenPromise;
        }

        private Promise<TNewResult> PushThen<TNewResult>(Func<TResult, Object> onResolved, Func<Exception, Object> onRejected, Action<Progress> onNotified)
        {
            // Promise
            Action<TNewResult> thenResolve = null;
            Action<Exception> thenReject = null;
            Action<Progress> thenNotify = null;
            var thenPromise = new Promise<TNewResult>(delegate (Action<TNewResult> resolve, Action<Exception> reject, Action<Progress> notify)
            {
                // Initialize
                thenResolve = resolve;
                thenReject = reject;
                thenNotify = notify;
            });

            // Handlers
            Action<TResult> resolveHandler = delegate (TResult result)
            {
                try
                {
                    // Result
                    var resultObject = onResolved(result);

                    // Distribute
                    if (resultObject == null)
                    {
                        thenResolve(default(TNewResult));
                    }
                    else if (resultObject is Promise)
                    {
                        ((Promise)resultObject).Then(
                            delegate () { thenResolve(default(TNewResult)); },
                            delegate (Exception thenError) { thenReject(thenError); },
                            delegate (Progress thenProgress) { thenNotify(thenProgress); }
                        );
                    }
                    else if (resultObject is TNewResult)
                    {
                        thenResolve((TNewResult)resultObject);
                    }
                    else if (resultObject is Promise<TNewResult>)
                    {
                        ((Promise<TNewResult>)resultObject).Then(
                            delegate (TNewResult thenResult) { thenResolve(thenResult); },
                            delegate (Exception thenError) { thenReject(thenError); },
                            delegate (Progress thenProgress) { thenNotify(thenProgress); }
                        );
                    }
                    else
                    {
                        throw new Exception("Invalid Result");
                    }
                }
                catch (Exception ex)
                {
                    thenReject(ex);
                }
            };

            Action<Exception> rejectHandler = delegate (Exception error)
            {
                try
                {
                    // Result
                    var resultObject = onRejected(error);

                    // Distribute
                    if (resultObject == null)
                    {
                        thenResolve(default(TNewResult));
                    }
                    else if (resultObject is Promise)
                    {
                        ((Promise)resultObject).Then(
                            delegate () { thenResolve(default(TNewResult)); },
                            delegate (Exception thenError) { thenReject(thenError); },
                            delegate (Progress thenProgress) { thenNotify(thenProgress); }
                        );
                    }
                    else if (resultObject is TNewResult)
                    {
                        thenResolve((TNewResult)resultObject);
                    }
                    else if (resultObject is Promise<TNewResult>)
                    {
                        ((Promise<TNewResult>)resultObject).Then(
                            delegate (TNewResult thenResult) { thenResolve(thenResult); },
                            delegate (Exception thenError) { thenReject(thenError); },
                            delegate (Progress thenProgress) { thenNotify(thenProgress); }
                        );
                    }
                    else
                    {
                        throw new Exception("Invalid Result");
                    }
                }
                catch (Exception ex)
                {
                    thenReject(ex);
                }
            };

            Action<Progress> notifiedHandler = delegate (Progress progress)
            {
                try
                {
                    onNotified(progress);
                }
                catch (Exception ex)
                {
                    thenReject(ex);
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

                };
            }
            return _passNotified;
        }


        // Then(Func<out *>)
        public Promise Then(Action onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { onResolved(); return null; },
                delegate (Exception error) { onRejected(error); return null; },
                onNotified
            );
        }

        public Promise Then(Action onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { onResolved(); return null; },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }


        public Promise Then(Func<Promise> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { onRejected(error); return null; },
                onNotified
            );
        }

        public Promise Then(Func<Promise> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }


        // Then(Func<TResult, out *>)
        public Promise Then(Action<TResult> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { onResolved(result); return null; },
                delegate (Exception error) { onRejected(error); return null; },
                onNotified
            );
        }

        public Promise Then(Action<TResult> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { onResolved(result); return null; },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }


        public Promise Then(Func<TResult, Promise> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { onRejected(error); return null; },
                onNotified
            );
        }

        public Promise Then(Func<TResult, Promise> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }


        // Then<TNewResult>(Func<out *>)
        public Promise<TNewResult> Then<TNewResult>(Action onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(); return null; },
                delegate (Exception error) { onRejected(error); return null; },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Action onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(); return null; },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Action onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(); return null; },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Action onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(); return null; },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }


        public Promise<TNewResult> Then<TNewResult>(Func<Promise> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { onRejected(error); return null; },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }


        public Promise<TNewResult> Then<TNewResult>(Func<TNewResult> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { onRejected(error); return null; },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TNewResult> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TNewResult> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TNewResult> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }


        public Promise<TNewResult> Then<TNewResult>(Func<Promise<TNewResult>> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { onRejected(error); return null; },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise<TNewResult>> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise<TNewResult>> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<Promise<TNewResult>> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }


        // Then<TNewResult>(Func<TResult, out *>)
        public Promise<TNewResult> Then<TNewResult>(Action<TResult> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(result); return null; },
                delegate (Exception error) { onRejected(error); return null; },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Action<TResult> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(result); return null; },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Action<TResult> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(result); return null; },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Action<TResult> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { onResolved(result); return null; },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }


        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { onRejected(error); return null; },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }


        public Promise<TNewResult> Then<TNewResult>(Func<TResult, TNewResult> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { onRejected(error); return null; },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, TNewResult> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, TNewResult> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, TNewResult> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }


        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise<TNewResult>> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { onRejected(error); return null; },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise<TNewResult>> onResolved, Func<Exception, Promise> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise<TNewResult>> onResolved, Func<Exception, TNewResult> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { return onRejected(error); },
                onNotified
            );
        }

        public Promise<TNewResult> Then<TNewResult>(Func<TResult, Promise<TNewResult>> onResolved, Func<Exception, Promise<TNewResult>> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen<TNewResult>(
                delegate (TResult result) { return onResolved(result); },
                delegate (Exception error) { return onRejected(error); },
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


        // Notify - Syntactic sugar
        public Promise Notify(Action<Progress> onNotified)
        {
            return this.Then(this.PassResolved(), this.PassRejected(), onNotified);
        }
    }
}
