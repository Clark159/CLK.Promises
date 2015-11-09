package com.clk.promises;


import java.util.ArrayList;
import com.clk.Action;


public class Promise<TResult> {

	// enumerations
    private enum PromiseState {
        Pending,  // unresolved  
        Resolved, // has-resolution
        Rejected, // has-rejection
    };
    
    protected enum ResultType {
    	Empty,
    	EmptyPromise,
        New,
        NewPromise,
    }
    
    
	// fields
    private Object _syncRoot = new Object();

    private PromiseState _state = PromiseState.Pending;

    private TResult _result = null;

    private Exception _error = null;    
    
    private ArrayList<Action.Type1<TResult>> _resolveHandlers = null;

    private ArrayList<Action.Type1<Exception>> _rejectHandlers = null;

    private ArrayList<Action.Type1<Progress>> _notifyHandlers = null;

    private ArrayList<Action.Type1<Progress>> _notifyHandlersSnapshot = null;
        
    
    // methods 
    protected void push(Action.Type1<TResult> resolveHandler, Action.Type1<Exception> rejectHandler, Action.Type1<Progress> notifyHandler)
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
            
    protected void resolveBase(TResult result)
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

    public void reject(Exception error)
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

    public void notify(Progress progress)
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
}