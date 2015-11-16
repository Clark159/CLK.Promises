using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Promises
{
    public class Promise : PromiseBase<Object>
    {
        // Fields
        private Func<Object> _passResolved = null;

        private Func<Exception, Object> _passRejected = null;

        private Action<Progress> _passNotified = null;


        // Methods 	
        private Promise PushThen(
            Func<Object> onResolved, ResultType onResolvedResultType,
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
            Action<Object> resolveHandler = delegate (Object result) {
                try
                {
                    // Execute
                    Object resultObject = onResolved();

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
                };
            };

            // Push
            this.Push(resolveHandler, rejectHandler, notifyHandler);

            // Return
            return thenPromise;
        }

        private ResultPromise<TNewResult> PushThenNew<TNewResult>(
                Func<Object> onResolved, ResultType onResolvedResultType,
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
            Action<Object> resolveHandler = delegate (Object result)
            {
                try
                {
                    // Execute
                    Object resultObject = onResolved();

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


        private Func<Object> PassResolved()
        {
            if (_passResolved == null)
            {
                _passResolved = delegate ()
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
        public void Resolve()
        {
            // Resolve
            this.ResolveBase(null);
        }


        // Then
        public Promise Then(Action onResolved)
        {
            return this.PushThen(
                delegate () { onResolved(); return null; }, ResultType.Empty,
                this.PassRejected(), ResultType.Empty,
                this.PassNotified()
            );
        }

        public Promise Then(Action onResolved, Action<Exception> onRejected)
        {
            return this.PushThen(
                delegate () { onResolved(); return null; }, ResultType.Empty,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.Empty,
                this.PassNotified()
            );
        }

        public Promise Then(Action onResolved, Action<Exception> onRejected, Action<Progress> onNotified)
        {
            return this.PushThen(
                delegate () { onResolved(); return null; }, ResultType.Empty,
                delegate (Exception error) { onRejected(error); return null; }, ResultType.Empty,
                onNotified
            );
        }

        public Promise ThenPromise(Func<Promise> onResolved)
        {
            return this.PushThen(
                delegate () { return onResolved(); }, ResultType.EmptyPromise,
                this.PassRejected(), ResultType.Empty,
                this.PassNotified()
            );
        }


        // ThenNew
        public ResultPromise<TNewResult> ThenNew<TNewResult>(Func<TNewResult> onResolved)
        {
            return this.PushThenNew<TNewResult>(
                delegate () { return onResolved(); }, ResultType.New,
                this.PassRejected(), ResultType.Empty,
                this.PassNotified()
            );
        }

        public ResultPromise<TNewResult> ThenNewPromise<TNewResult>(Func<ResultPromise<TNewResult>> onResolved)
        {
            return this.PushThenNew<TNewResult>(
                delegate () { return onResolved(); }, ResultType.NewPromise,
                this.PassRejected(), ResultType.Empty,
                this.PassNotified()
            );
        }
        

        // Catch
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


        // CatchNew
        public PromiseBase<TNewResult> FailNew<TNewResult>(Func<Exception, TNewResult> onRejected)
        {
            return this.PushThenNew<TNewResult>(
                this.PassResolved(), ResultType.Empty,
                delegate (Exception error) { return onRejected(error); }, ResultType.New,
                this.PassNotified()
            );
        }

        public PromiseBase<TNewResult> FailNewPromise<TNewResult>(Func<Exception, ResultPromise<TNewResult>> onRejected)
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


        // All
        public static Promise AllPromise(List<Promise> promiseList)
        {
            #region Contracts

            if (promiseList == null) throw new ArgumentNullException();

            #endregion

            // Promise
            Promise allPromise = new Promise();

            // AllPromise
            int thenResultCount = 0;
            Action<int> thenAction = delegate (int thenPromiseIndex)
            {
                promiseList[thenPromiseIndex].Then(
                    delegate ()
                    {
                        thenResultCount++;
                        if (thenResultCount == promiseList.Count) allPromise.Resolve();
                    },
                    delegate (Exception thenError) { allPromise.Reject(thenError); },
                    delegate (Progress thenProgress) { allPromise.Notify(thenProgress); }
                );
            };

            // Execute           
            if (promiseList.Count != 0)
            {
                for (int i = 0; i < promiseList.Count; i++)
                {
                    thenAction(i);
                }
            }
            else
            {
                allPromise.Resolve();
            }

            // Return
            return allPromise;
        }

        public static ResultPromise<List<TNewResult>> AllNewPromise<TNewResult>(List<ResultPromise<TNewResult>> promiseList)
        {
            #region Contracts

            if (promiseList == null) throw new ArgumentNullException();

            #endregion

            // Promise
            ResultPromise<List<TNewResult>> allPromise = new ResultPromise<List<TNewResult>>();

            // ResultArray
            List<TNewResult> thenResultList = new List<TNewResult>();
            for (int i = 0; i < promiseList.Count; i++) thenResultList.Add(default(TNewResult));

            // AllNewPromise
            int thenResultCount = 0;            
            Action<int> thenAction = delegate (int thenPromiseIndex)
            {
                promiseList[thenPromiseIndex].Then(
                    delegate (TNewResult thenResult)
                    {
                        thenResultList[thenPromiseIndex] = thenResult;
                        thenResultCount++;                                     
                        if (thenResultCount == promiseList.Count) allPromise.Resolve(thenResultList);
                    },
                    delegate (Exception thenError) { allPromise.Reject(thenError); },
                    delegate (Progress thenProgress) { allPromise.Notify(thenProgress); }
                );
            };

            // Execute           
            if (promiseList.Count != 0)
            {
                for (int i = 0; i < promiseList.Count; i++)
                {
                    thenAction(i);
                }
            }
            else
            {
                allPromise.Resolve(thenResultList);
            }

            // Return
            return allPromise;
        }
    }
}
