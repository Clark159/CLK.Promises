using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Promises
{
    public class ResultPromise<TResult> : PromiseBase<TResult>
    {
        // Fields
        private Func<TResult, Object> _passResolved = null;

        private Func<Exception, Object> _passRejected = null;

        private Action<Progress> _passNotified = null;


        // Methods 	
        protected Promise PushThen(
             Func<TResult, Object> onResolved, ResultType onResolvedResultType,
             Func<Exception, Object> onRejected, ResultType onRejectedResultType,
             Action<Progress> onNotified)
        {
            #region Contracts

            if (onResolved == null) throw new ArgumentNullException();
            if (onRejected == null) throw new ArgumentNullException();
            if (onNotified == null) throw new ArgumentNullException();

            #endregion

            // Promise
            Promise thenPromise = new Promise();

            // ResolveHandler
            Action<TResult> resolveHandler = delegate (TResult result)
            {
                try
                {
                    // Execute
                    Object resultObject = onResolved(result);

                    // Distribute
                    switch (onResolvedResultType)
                    {
                        case ResultType.Empty:
                            thenPromise.Resolve();
                            break;

                        case ResultType.EmptyPromise:
                            if (resultObject != null)
                            {
                                ((Promise)resultObject).Then(
                                    delegate () { thenPromise.Resolve(); },
                                    delegate (Exception thenError) { thenPromise.Reject(thenError); },
                                    delegate (Progress thenProgress) { thenPromise.Notify(thenProgress); }
                                );
                            }
                            else { throw new Exception("Invalid Result"); }
                            break;

                        default:
                            throw new Exception("Invalid Result Type");
                    }
                }
                catch (Exception ex)
                {
                    thenPromise.Reject(ex);
                }
            };

            // RejectHandler
            Action<Exception> rejectHandler = delegate (Exception error)
            {
                try
                {
                    // Execute
                    Object resultObject = onRejected(error);

                    // Distribute
                    switch (onRejectedResultType)
                    {
                        case ResultType.Empty:
                            thenPromise.Resolve();
                            break;

                        case ResultType.EmptyPromise:
                            if (resultObject != null)
                            {
                                ((Promise)resultObject).Then(
                                    delegate () { thenPromise.Resolve(); },
                                    delegate (Exception thenError) { thenPromise.Reject(thenError); },
                                    delegate (Progress thenProgress) { thenPromise.Notify(thenProgress); }
                                );
                            }
                            else { throw new Exception("Invalid Result"); }
                            break;

                        default:
                            throw new Exception("Invalid Result Type");
                    }
                }
                catch (Exception ex)
                {
                    thenPromise.Reject(ex);
                }
            };

            // NotifyHandler
            Action<Progress> notifyHandler = delegate (Progress progress)
            {
                try
                {
                    // Execute
                    onNotified(progress);

                    // Distribute
                    thenPromise.Notify(progress);
                }
                catch (Exception ex)
                {
                    thenPromise.Reject(ex);
                }
            };

            // Push
            this.Push(resolveHandler, rejectHandler, notifyHandler);

            // Return
            return thenPromise;
        }

        protected ResultPromise<TNewResult> PushThenNew<TNewResult>(
               Func<TResult, Object> onResolved, ResultType onResolvedResultType,
               Func<Exception, Object> onRejected, ResultType onRejectedResultType,
               Action<Progress> onNotified)
        {
            #region Contracts

            if (onResolved == null) throw new ArgumentNullException();
            if (onRejected == null) throw new ArgumentNullException();
            if (onNotified == null) throw new ArgumentNullException();

            #endregion

            // Promise
            ResultPromise<TNewResult> thenPromise = new ResultPromise<TNewResult>();

            // ResolveHandler
            Action<TResult> resolveHandler = delegate (TResult result)
            {
                try
                {
                    // Execute
                    Object resultObject = onResolved(result);

                    // Distribute
                    switch (onResolvedResultType)
                    {
                        case ResultType.Empty:
                            thenPromise.Resolve(default(TNewResult));
                            break;

                        case ResultType.EmptyPromise:
                            if (resultObject != null)
                            {
                                ((Promise)resultObject).Then(
                                    delegate () { thenPromise.Resolve(default(TNewResult)); },
                                    delegate (Exception thenError) { thenPromise.Reject(thenError); },
                                    delegate (Progress thenProgress) { thenPromise.Notify(thenProgress); }
                                );
                            }
                            else { throw new Exception("Invalid Result"); }
                            break;

                        case ResultType.New:
                            thenPromise.Resolve((TNewResult)resultObject);
                            break;

                        case ResultType.NewPromise:
                            if (resultObject != null)
                            {
                                ((ResultPromise<TNewResult>)resultObject).Then(
                                    delegate (TNewResult thenResult) { thenPromise.Resolve(thenResult); },
                                    delegate (Exception thenError) { thenPromise.Reject(thenError); },
                                    delegate (Progress thenProgress) { thenPromise.Notify(thenProgress); }
                                );
                            }
                            else { throw new Exception("Invalid Result"); }
                            break;

                        default:
                            throw new Exception("Invalid Result Type");
                    }
                }
                catch (Exception ex)
                {
                    thenPromise.Reject(ex);
                }
            };

            // RejectHandler
            Action<Exception> rejectHandler = delegate (Exception error)
            {
                try
                {
                    // Execute
                    Object resultObject = onRejected(error);

                    // Distribute
                    switch (onRejectedResultType)
                    {
                        case ResultType.Empty:
                            thenPromise.Resolve(default(TNewResult));
                            break;

                        case ResultType.EmptyPromise:
                            if (resultObject != null)
                            {
                                ((Promise)resultObject).Then(
                                    delegate () { thenPromise.Resolve(default(TNewResult)); },
                                    delegate (Exception thenError) { thenPromise.Reject(thenError); },
                                    delegate (Progress thenProgress) { thenPromise.Notify(thenProgress); }
                                );
                            }
                            else { throw new Exception("Invalid Result"); }
                            break;

                        case ResultType.New:
                            thenPromise.Resolve((TNewResult)resultObject);
                            break;

                        case ResultType.NewPromise:
                            if (resultObject != null)
                            {
                                ((ResultPromise<TNewResult>)resultObject).Then(
                                    delegate (TNewResult thenResult) { thenPromise.Resolve(thenResult); },
                                    delegate (Exception thenError) { thenPromise.Reject(thenError); },
                                    delegate (Progress thenProgress) { thenPromise.Notify(thenProgress); }
                                );
                            }
                            else { throw new Exception("Invalid Result"); }
                            break;

                        default:
                            throw new Exception("Invalid Result Type");
                    }
                }
                catch (Exception ex)
                {
                    thenPromise.Reject(ex);
                }
            };

            // NotifyHandler
            Action<Progress> notifyHandler = delegate (Progress progress)
            {
                try
                {
                    // Execute
                    onNotified(progress);

                    // Distribute
                    thenPromise.Notify(progress);
                }
                catch (Exception ex)
                {
                    thenPromise.Reject(ex);
                }
            };

            // Push
            this.Push(resolveHandler, rejectHandler, notifyHandler);

            // Return
            return thenPromise;
        }


        private Func<TResult, Object> PassResolved()
        {
            if (_passResolved == null)
            {
                _passResolved = delegate (TResult result)
                {
                    return null;
                };
            }
            return _passResolved;
        }

        private Func<Exception, Object> PassRejected()
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


        // Resolve
        public void Resolve(TResult result)
        {
            // Resolve
            this.ResolveBase(result);
        }


        // Then
        public Promise Then(Action<TResult> onResolved)
        {
            return this.PushThen(
                delegate (TResult result) { onResolved(result); return null; }, ResultType.Empty,
                this.PassRejected(), ResultType.Empty,
                this.PassNotified()
            );
        }

        public Promise Then(Action<TResult> onResolved, Action<Exception> onRejected)
        {
            return this.PushThen(
                delegate (TResult result) { onResolved(result); return null; }, ResultType.Empty,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.Empty,
                this.PassNotified()
            );
        }

        public Promise Then(Action<TResult> onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate (TResult result) { onResolved(result); return null; }, ResultType.Empty,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.Empty,
                onNotified
            );
        }

        public Promise ThenPromise(Func<TResult, Promise> onResolved)
        {
            return this.PushThen(
                delegate (TResult result) { return onResolved(result); }, ResultType.EmptyPromise,
                this.PassRejected(), ResultType.Empty,
                this.PassNotified()
            );
        }


        // ThenNew
        public ResultPromise<TNewResult> thenNew<TNewResult>(Func<TResult, TNewResult> onResolved)
        {
            return this.PushThenNew<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.New,
                this.PassRejected(), ResultType.Empty,
                this.PassNotified()
            );
        }

        public ResultPromise<TNewResult> thenNewPromise<TNewResult>(Func<TResult, ResultPromise<TNewResult>> onResolved)
        {
            return this.PushThenNew<TNewResult>(
                delegate (TResult result) { return onResolved(result); }, ResultType.NewPromise,
                this.PassRejected(), ResultType.Empty,
                this.PassNotified()
            );
        }


        // Fail
        public Promise Fail(Action<Exception> onRejected)
        {
            return this.PushThen(
                this.PassResolved(), ResultType.Empty,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.Empty,
                this.PassNotified()
            );
        }

        public Promise FailPromise(Func<Exception, Promise> onRejected)
        {
            return this.PushThen(
                this.PassResolved(), ResultType.Empty,
                delegate (Exception error) { return onRejected(error); }, ResultType.EmptyPromise,
                this.PassNotified()
            );
        }


        // FailNew
        public PromiseBase<TNewResult> FailNew<TNewResult>(Func<Exception, TNewResult> onRejected)
        {
            return this.PushThenNew<TNewResult>(
                this.PassResolved(), ResultType.Empty,
                delegate (Exception error) { return onRejected(error); }, ResultType.New,
                this.PassNotified()
            );
        }

        public PromiseBase<TNewResult> FailNewPromise<TNewResult>(Func<Exception, PromiseBase<TNewResult>> onRejected)
        {
            return this.PushThenNew<TNewResult>(
                this.PassResolved(), ResultType.Empty,
                delegate (Exception error) { return onRejected(error); }, ResultType.NewPromise,
                this.PassNotified()
            );
        }


        // Progress
        public Promise Progress(Action<Progress> onNotified)
        {
            return this.PushThen(
                this.PassResolved(), ResultType.Empty,
                this.PassRejected(), ResultType.Empty,
                onNotified
            );
        }
    }
}