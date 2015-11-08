package com.clk.promises;


import java.util.ArrayList;

import com.clk.Action;
import com.clk.Func;
import com.clk.Func.Type1;


public class PromiseBase<TResult> {

	// fields
    private Object _syncRoot = new Object();

    private PromiseState _state = PromiseState.Pending;

    private TResult _result = null;

    private Exception _error = null;
    
    private ArrayList<Action.Type1<TResult>> _resolveHandlers = null;

    private ArrayList<Action.Type1<Exception>> _rejectHandlers = null;

    private ArrayList<Action.Type1<Progress>> _notifyHandlers = null;

    private ArrayList<Action.Type1<Progress>> _notifyHandlersSnapshot = null;
    
    
    private Action.Type0 _passResolved = null;

    private Action.Type1<Exception> _passRejected = null;

    private Action.Type1<Progress> _passNotified = null;
    
    
    // constructors
    public PromiseBase() { }
    
    
    // methods 
    public void innerResolve(TResult result)
    {
        // sync
        synchronized (_syncRoot)
        {
            // State
            if (_state != PromiseState.Pending) return;
            _state = PromiseState.Resolved;

            // Results
            _result = result;
            _error = null;
        }

        // handlers
        ArrayList<Action.Type1<TResult>> resolveHandlers = _resolveHandlers;
        if (resolveHandlers == null) return;

        // resolve
        for (Action.Type1<TResult> resolveHandler : resolveHandlers)
        {
        	resolveHandler.raise(result);
        }
    }

    public void innerReject(Exception error)
    {
        // contracts
        if (error == null) throw new IllegalArgumentException();

        // sync            
        synchronized (_syncRoot)
        {
            // state
            if (_state != PromiseState.Pending) return;
            _state = PromiseState.Rejected;

            // result
            _result = null;
            _error = error;
        }

        // handlers
        ArrayList<Action.Type1<Exception>> rejectHandlers = _rejectHandlers;
        if (rejectHandlers == null) return;

        // resolve
        for (Action.Type1<Exception> rejectHandler : rejectHandlers)
        {
        	rejectHandler.raise(error);
        }
    }

    public void innerNotify(Progress progress)
    {
    	// contracts
        if (progress == null) throw new IllegalArgumentException();

        // handlers
        ArrayList<Action.Type1<Progress>> notifyHandlers = null;            
        synchronized (_syncRoot)
        {
            if (_notifyHandlersSnapshot == null && _notifyHandlers != null)
            {
                _notifyHandlersSnapshot = new ArrayList<Action.Type1<Progress>>();
                for (Action.Type1<Progress> notifyHandler : _notifyHandlers)
                {
                	_notifyHandlersSnapshot.add(notifyHandler);
                }                
            }
            notifyHandlers = _notifyHandlersSnapshot;
        }
        if (notifyHandlers == null) return;

        // notify
        for (Action.Type1<Progress> notifyHandler : notifyHandlers)
        {
        	notifyHandler.raise(progress);
        }
    }

    
    private void pushHandlers(Action.Type1<TResult> resolveHandler, Action.Type1<Exception> rejectHandler, Action.Type1<Progress> notifyHandler)
    {
        // contracts
        if (resolveHandler == null) throw new IllegalArgumentException();
        if (rejectHandler == null) throw new IllegalArgumentException();
        if (notifyHandler == null) throw new IllegalArgumentException();

        // sync         
        synchronized (_syncRoot)
        {
            // resolveHandler
            if (_state == PromiseState.Pending)
            {
                if(_resolveHandlers==null)
                {
                    _resolveHandlers = new ArrayList<Action.Type1<TResult>>();
                }
                _resolveHandlers.add(resolveHandler);
            }

            // rejectHandler
            if (_state == PromiseState.Pending)
            {
                if (_rejectHandlers == null)
                {
                    _rejectHandlers = new ArrayList<Action.Type1<Exception>>();
                }
                _rejectHandlers.add(rejectHandler);
            }

            // notifyHandler
            if (_notifyHandlers == null)
            {
                _notifyHandlers = new ArrayList<Action.Type1<Progress>>();
            }
            _notifyHandlers.add(notifyHandler);
            _notifyHandlersSnapshot = null;

            // pending
            if (_state == PromiseState.Pending)
            {
                return;
            }
        }

        // resolved
        if (_state == PromiseState.Resolved)
        {
            resolveHandler.raise(_result);
        }

        // rejected
        if (_state == PromiseState.Rejected)
        {
            rejectHandler.raise(_error);
        }
    }

    private Promise pushThen(final Func.Type1<TResult, Object> onResolved, final Func.Type1<Exception, Object> onRejected, final Action.Type1<Progress> onNotified)
    {
    	// promise
    	final Promise thenPromise = new Promise();
    	
    	// handlers
        Action.Type1<TResult> resolveHandler = new Action.Type1<TResult> ()
        {        	
			@Override public void raise(TResult result) {
				try
	            {
	                // execute
	                Object resultObject = onResolved.raise(result);

	                // distribute
	                if (resultObject == null)
	                {
	                    thenPromise.innerResolve(null);
	                }
	                else if (resultObject instanceof Promise)
	                {
	                	((Promise)resultObject).then(
	                		new Action.Type0() { @Override public void raise() {  thenPromise.innerResolve(null); }},	
	                        new Action.Type1<Exception>(){ @Override public void raise(Exception thenError) { thenPromise.innerReject(thenError); }},
	                        new Action.Type1<Progress>(){ @Override public void raise(Progress thenProgress) { thenPromise.innerNotify(thenProgress); }}
	                    );
	                }
	                else
	                {
	                    throw new Exception("Invalid Result");
	                }
	            }
	            catch (Exception ex)
	            {
	                thenPromise.innerReject(ex);
	            }
			}        	
        };
        
        Action.Type1<Exception> rejectHandler = new Action.Type1<Exception>() 
        {
        	@Override public void raise(Exception error) {        		
	            try
	            {
	                // execute
	                Object resultObject = onRejected.raise(error);
	
	                // distribute
	                if (resultObject == null)
	                {
	                    thenPromise.innerResolve(null);
	                }
	                else if (resultObject instanceof Promise)
	                {
	                	((Promise)resultObject).then(
	                		new Action.Type0() { @Override public void raise() {  thenPromise.innerResolve(null); }},	
	                        new Action.Type1<Exception>(){ @Override public void raise(Exception thenError) { thenPromise.innerReject(thenError); }},
	                        new Action.Type1<Progress>(){ @Override public void raise(Progress thenProgress) { thenPromise.innerNotify(thenProgress); }}
	                    );
	                }
	                else
	                {
	                    throw new Exception("Invalid Result");
	                }
	            }
	            catch (Exception ex)
	            {
	                thenPromise.innerReject(ex);
	            }
        	}
        };
        
        Action.Type1<Progress> notifiedHandler = new Action.Type1<Progress>() 
        {
        	@Override public void raise(Progress progress) {
	            try
	            {
	                // execute
	                onNotified.raise(progress);
	
	                // distribute
	                thenPromise.innerNotify(progress);
	            }
	            catch (Exception ex)
	            {
	                thenPromise.innerReject(ex);
	            }
			}
        };
        
        // pushHandlers
        this.pushHandlers(resolveHandler, rejectHandler, notifiedHandler);
                
    	// return
        return thenPromise;
    }

    private <TNewResult> PromiseBase<TNewResult> pushThenBase(final Func.Type1<TResult, Object> onResolved, final Func.Type1<Exception, Object> onRejected, final Action.Type1<Progress> onNotified)
    {
        // promise
    	final PromiseBase<TNewResult> thenPromise = new PromiseBase<TNewResult>();
    	   	        
    	
        // Return
        return thenPromise;
    }

    
    private Action.Type0 passResolved()
    {
        if (_passResolved == null)
        {
            _passResolved = new Action.Type0()
            {
				@Override public void raise() {
					// nothing
				}               
            };
        }
        return _passResolved;
    }

    private Action.Type1<Exception> passRejected()
    {
        if (_passRejected == null)
        {
        	_passRejected = new Action.Type1<Exception>()
            {
				@Override public void raise(Exception error) {		
					// throw
					throw new RuntimeException(error.getMessage());
				}                
            };
        }
        return _passRejected;
    }

    private Action.Type1<Progress> passNotified()
    {
        if (_passNotified == null)
        {
            _passNotified = new Action.Type1<Progress>()
            {
				@Override public void raise(Progress progress) {
					// nothing
				}                
            };
        }
        return _passNotified;
    }


    // Then(Func<out *>)
    public Promise then(final Action.Type0 onResolved, final Action.Type1<Exception> onRejected, final Action.Type1<Progress> onNotified)
    {
    	return this.pushThen(
			new Func.Type1<TResult, Object>() { @Override public Object raise(TResult result) { onResolved.raise(); return null; }},	
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) { onRejected.raise(error); return null; }},
			onNotified	
    	);
    }

    public Promise then(final Action.Type0 onResolved, final Func.Type1<Exception, Promise> onRejected, final Action.Type1<Progress> onNotified)
    {
    	return this.pushThen(
			new Func.Type1<TResult, Object>() { @Override public Object raise(TResult result) { onResolved.raise(); return null; }},	
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) { return onRejected.raise(error); }},
			onNotified	
    	);
    }


    public Promise then(final Func.Type0<Promise> onResolved, final Action.Type1<Exception> onRejected, final Action.Type1<Progress> onNotified)
    {
    	return this.pushThen(
			new Func.Type1<TResult, Object>() { @Override public Object raise(TResult result) { return onResolved.raise(); }},	
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) { onRejected.raise(error); return null; }},
			onNotified	
    	);
    }

    public Promise then(final Func.Type0<Promise> onResolved, final Func.Type1<Exception, Promise> onRejected, final Action.Type1<Progress> onNotified)
    {
    	return this.pushThen(
			new Func.Type1<TResult, Object>() { @Override public Object raise(TResult result) { return onResolved.raise(); }},	
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) { return onRejected.raise(error); }},
			onNotified	
    	);
    }

    
    // Then(Func<TResult, out *>)
    public Promise then(final Action.Type1<TResult> onResolved, final Action.Type1<Exception> onRejected, final Action.Type1<Progress> onNotified)
    {
    	return this.pushThen(
			new Func.Type1<TResult, Object>() { @Override public Object raise(TResult result) { onResolved.raise(result); return null; }},	
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) { onRejected.raise(error); return null; }},
			onNotified	
    	);
    }

    public Promise then(final Action.Type1<TResult> onResolved, final Func.Type1<Exception, Promise> onRejected, final Action.Type1<Progress> onNotified)
    {
    	return this.pushThen(
			new Func.Type1<TResult, Object>() { @Override public Object raise(TResult result) { onResolved.raise(result); return null; }},	
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) { return onRejected.raise(error); }},
			onNotified	
    	);
    }


    public Promise then(final Func.Type1<TResult, Promise> onResolved, final Action.Type1<Exception> onRejected, final Action.Type1<Progress> onNotified)
    {
    	return this.pushThen(
			new Func.Type1<TResult, Object>() { @Override public Object raise(TResult result) { return onResolved.raise(result); }},	
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) { onRejected.raise(error); return null; }},
			onNotified	
    	);
    }

    public Promise then(final Func.Type1<TResult, Promise> onResolved, final Func.Type1<Exception, Promise> onRejected, final Action.Type1<Progress> onNotified)
    {
    	return this.pushThen(
			new Func.Type1<TResult, Object>() { @Override public Object raise(TResult result) { return onResolved.raise(result); }},	
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) { return onRejected.raise(error); }},
			onNotified	
    	);
    }

}