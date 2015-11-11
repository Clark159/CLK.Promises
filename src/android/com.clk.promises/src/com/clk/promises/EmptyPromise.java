package com.clk.promises;


import com.clk.Action;
import com.clk.Func;


public class EmptyPromise extends Promise<Object> {
    	
	// fields
	private Func.Type0<Object> _passResolved = null;

    private Func.Type1<Exception, Object> _passRejected = null;

    private Action.Type1<Progress> _passNotified = null;
    
    
    // methods 	
	private EmptyPromise pushThen(
    		final Func.Type0<Object> onResolved, final ResultType onResolvedResultType,
    		final Func.Type1<Exception, Object> onRejected, final ResultType onRejectedResultType, 
			final Action.Type1<Progress> onNotified)
	{ 
		// contracts
        if (onResolved == null) throw new IllegalArgumentException();
        if (onRejected == null) throw new IllegalArgumentException();
        if (onNotified == null) throw new IllegalArgumentException();
        
		// promise
    	final EmptyPromise thenPromise = new EmptyPromise();
    	
    	// resolveHandler
        Action.Type1<Object> resolveHandler = new Action.Type1<Object> ()
        {        	
			@Override public void raise(Object result) {
				try
	            {
	                // execute
	                Object resultObject = onResolved.raise();

	                // distribute
	                switch (onResolvedResultType)
                    {
                    	case Empty:
	                        thenPromise.resolve();
	                        break;
                        
                        case EmptyPromise:
                        	((EmptyPromise)resultObject).then(
    	                		new Action.Type0() { @Override public void raise() {  thenPromise.resolve(); }},	
    	                        new Action.Type1<Exception>(){ @Override public void raise(Exception thenError) { thenPromise.reject(thenError); }},
    	                        new Action.Type1<Progress>(){ @Override public void raise(Progress thenProgress) { thenPromise.notify(thenProgress); }}
    	                    );
                            break;
                        	
                        default:
                            throw new Exception("Invalid Result");
                    }
	            }
	            catch (Exception ex)
	            {
	                thenPromise.reject(ex);
	            }
			}        	
        };
        
        // rejectHandler
        Action.Type1<Exception> rejectHandler = new Action.Type1<Exception>() 
        {
        	@Override public void raise(Exception error) {        		
	            try
	            {
	                // execute
	                Object resultObject = onRejected.raise(error);
	
	                // distribute
	                switch (onRejectedResultType)
                    {
                    	case Empty:
	                        thenPromise.resolve();
	                        break;
                        
                        case EmptyPromise:
                        	((EmptyPromise)resultObject).then(
    	                		new Action.Type0() { @Override public void raise() {  thenPromise.resolve(); }},	
    	                        new Action.Type1<Exception>(){ @Override public void raise(Exception thenError) { thenPromise.reject(thenError); }},
    	                        new Action.Type1<Progress>(){ @Override public void raise(Progress thenProgress) { thenPromise.notify(thenProgress); }}
    	                    );
                            break;

                        default:
                            throw new Exception("Invalid Result");
                    }
	            }
	            catch (Exception ex)
	            {
	                thenPromise.reject(ex);
	            }
        	}
        };
        
        // notifyHandler
        Action.Type1<Progress> notifyHandler = new Action.Type1<Progress>() 
        {
        	@Override public void raise(Progress progress) {
	            try
	            {
	                // execute
	                onNotified.raise(progress);
	
	                // distribute
	                thenPromise.notify(progress);
	            }
	            catch (Exception ex)
	            {
	                thenPromise.reject(ex);
	            }
			}
        };
    	
        // push
        this.push(resolveHandler, rejectHandler, notifyHandler);   
    	
    	// return
    	return thenPromise;
	}
	
	private <TNewResult> ResultPromise<TNewResult> pushThenNew(
    		final Func.Type0<Object> onResolved, final ResultType onResolvedResultType,
    		final Func.Type1<Exception, Object> onRejected, final ResultType onRejectedResultType, 
			final Action.Type1<Progress> onNotified)
	{
		// contracts
        if (onResolved == null) throw new IllegalArgumentException();
        if (onRejected == null) throw new IllegalArgumentException();
        if (onNotified == null) throw new IllegalArgumentException();
        
		// promise
    	final ResultPromise<TNewResult> thenPromise = new ResultPromise<TNewResult>();
    	    
    	// resolveHandler
        Action.Type1<Object> resolveHandler = new Action.Type1<Object> ()
        {        	
			@SuppressWarnings("unchecked")
			@Override public void raise(Object result) {
				try
	            {
	                // execute
	                Object resultObject = onResolved.raise();

	                // distribute
	                switch (onResolvedResultType)
                    {
                    	case Empty:
	                        thenPromise.resolve(null);
	                        break;
                        
                        case EmptyPromise:
                        	((EmptyPromise)resultObject).then(
    	                		new Action.Type0() { @Override public void raise() {  thenPromise.resolve(null); }},	
    	                        new Action.Type1<Exception>(){ @Override public void raise(Exception thenError) { thenPromise.reject(thenError); }},
    	                        new Action.Type1<Progress>(){ @Override public void raise(Progress thenProgress) { thenPromise.notify(thenProgress); }}
    	                    );
                            break;
                            
                        case New:
                            thenPromise.resolve((TNewResult)resultObject);
                            break;
                            
                        case NewPromise:
                        	((ResultPromise<TNewResult>)resultObject).then(
    	                		new Action.Type1<TNewResult>() { @Override public void raise(TNewResult thenResult) {  thenPromise.resolve(thenResult); }},	
    	                        new Action.Type1<Exception>(){ @Override public void raise(Exception thenError) { thenPromise.reject(thenError); }},
    	                        new Action.Type1<Progress>(){ @Override public void raise(Progress thenProgress) { thenPromise.notify(thenProgress); }}
    	                    );
                        	break;
                        	
                        default:
                            throw new Exception("Invalid Result");
                    }
	            }
	            catch (Exception ex)
	            {
	                thenPromise.reject(ex);
	            }
			}        	
        };
        
        // rejectHandler
        Action.Type1<Exception> rejectHandler = new Action.Type1<Exception>() 
        {
        	@SuppressWarnings("unchecked")
        	@Override public void raise(Exception error) {        		
	            try
	            {
	                // execute
	                Object resultObject = onRejected.raise(error);
	
	                // distribute
	                switch (onRejectedResultType)
                    {
                    	case Empty:
	                        thenPromise.resolve(null);
	                        break;
                        
                        case EmptyPromise:
                        	((EmptyPromise)resultObject).then(
    	                		new Action.Type0() { @Override public void raise() {  thenPromise.resolve(null); }},	
    	                        new Action.Type1<Exception>(){ @Override public void raise(Exception thenError) { thenPromise.reject(thenError); }},
    	                        new Action.Type1<Progress>(){ @Override public void raise(Progress thenProgress) { thenPromise.notify(thenProgress); }}
    	                    );
                            break;
                            
                        case New:
                            thenPromise.resolve((TNewResult)resultObject);
                            break;
                            
                        case NewPromise:
                        	((ResultPromise<TNewResult>)resultObject).then(
    	                		new Action.Type1<TNewResult>() { @Override public void raise(TNewResult thenResult) {  thenPromise.resolve(thenResult); }},	
    	                        new Action.Type1<Exception>(){ @Override public void raise(Exception thenError) { thenPromise.reject(thenError); }},
    	                        new Action.Type1<Progress>(){ @Override public void raise(Progress thenProgress) { thenPromise.notify(thenProgress); }}
    	                    );
                        	break;

                        default:
                            throw new Exception("Invalid Result");
                    }
	            }
	            catch (Exception ex)
	            {
	                thenPromise.reject(ex);
	            }
        	}
        };

        // notifyHandler
        Action.Type1<Progress> notifyHandler = new Action.Type1<Progress>() 
        {
        	@Override public void raise(Progress progress) {
	            try
	            {
	                // execute
	                onNotified.raise(progress);
	
	                // distribute
	                thenPromise.notify(progress);
	            }
	            catch (Exception ex)
	            {
	                thenPromise.reject(ex);
	            }
			}
        };
    	
        // push
        this.push(resolveHandler, rejectHandler, notifyHandler);    	
                
    	// return
        return thenPromise;
	}
	
	
	private Func.Type0<Object> passResolved()
    {
        if (_passResolved == null)
        {
            _passResolved = new Func.Type0<Object>()
            {
				@Override public Object raise() {
					return null;
				}               
            };
        }
        return _passResolved;
    }

    private Func.Type1<Exception, Object> passRejected()
    {
        if (_passRejected == null)
        {
        	_passRejected = new Func.Type1<Exception, Object>()
            {
				@Override public Object raise(Exception error) {		
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
					
				}                
            };
        }
        return _passNotified;
    }


    // resolve
    public void resolve()
    {
    	// resolve
    	this.resolveBase(null);
    }
    
    
    // then
    public EmptyPromise then(final Action.Type0 onResolved)
    {
    	return this.pushThen(
			new Func.Type0<Object>() { @Override public Object raise() throws Exception { onResolved.raise(); return null; }}, ResultType.Empty,
			this.passRejected(), ResultType.Empty,
			this.passNotified()
    	);
    }
    
    public EmptyPromise then(final Action.Type0 onResolved, final Action.Type1<Exception> onRejected)
	{ 
		return this.pushThen(
			new Func.Type0<Object>() { @Override public Object raise() throws Exception { onResolved.raise(); return null; }}, ResultType.Empty,
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) throws Exception { onRejected.raise(error); return null; }}, ResultType.Empty,
			this.passNotified()
    	);
	};	
	
	public EmptyPromise then(final Action.Type0 onResolved, final Action.Type1<Exception> onRejected, final Action.Type1<Progress> onNotified)
	{ 
		return this.pushThen(
			new Func.Type0<Object>() { @Override public Object raise() throws Exception { onResolved.raise(); return null; }}, ResultType.Empty,
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) throws Exception { onRejected.raise(error); return null; }}, ResultType.Empty,
			onNotified	
    	);
	};	
	
	public EmptyPromise thenPromise(final Func.Type0<EmptyPromise> onResolved)
    {
		return this.pushThen(
			new Func.Type0<Object>() { @Override public Object raise() throws Exception { return onResolved.raise(); }}, ResultType.EmptyPromise,
			this.passRejected(), ResultType.Empty,
			this.passNotified()
    	);
    }

	
	// thenNew
    public <TNewResult> ResultPromise<TNewResult> thenNew(final Func.Type0<TNewResult> onResolved)
    {
    	return this.pushThenNew(
			new Func.Type0<Object>() { @Override public Object raise() throws Exception { return onResolved.raise(); }}, ResultType.New,
			this.passRejected(), ResultType.Empty,
			this.passNotified()
    	);
    }

    public <TNewResult> ResultPromise<TNewResult> thenNewPromise(final Func.Type0<ResultPromise<TNewResult>> onResolved)
    {
    	return this.pushThenNew(
			new Func.Type0<Object>() { @Override public Object raise() throws Exception { return onResolved.raise(); }}, ResultType.NewPromise,
			this.passRejected(), ResultType.Empty,
			this.passNotified()
    	);
    }

    
    // catch
    public EmptyPromise fail(final Action.Type1<Exception> onRejected)
    {
    	return this.pushThen(
    		this.passResolved(), ResultType.Empty,
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) throws Exception { onRejected.raise(error); return null; }}, ResultType.Empty,
			this.passNotified()
    	);
    }

    public EmptyPromise failPromise(final Func.Type1<Exception, EmptyPromise> onRejected)
    {
    	return this.pushThen(
    		this.passResolved(), ResultType.Empty,
    		new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) throws Exception { return onRejected.raise(error); }}, ResultType.EmptyPromise,
			this.passNotified()
    	);
    }
    
    
    // catchNew
    public <TNewResult> Promise<TNewResult> failNew(final Func.Type1<Exception, TNewResult> onRejected)
    {
    	return this.pushThenNew(
    		this.passResolved(), ResultType.Empty,
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) throws Exception { return onRejected.raise(error); }}, ResultType.New,
			this.passNotified()
    	);
    }

    public <TNewResult> Promise<TNewResult> failNewPromise(final Func.Type1<Exception, Promise<TNewResult>> onRejected)
    {
    	return this.pushThenNew(
    		this.passResolved(), ResultType.Empty,
			new Func.Type1<Exception, Object>() { @Override public Object raise(Exception error) throws Exception { return onRejected.raise(error); }}, ResultType.NewPromise,
			this.passNotified()
    	);
    }

    
    // progress
    public EmptyPromise progress(Action.Type1<Progress> onNotified)
    {
    	return this.pushThen(
    		this.passResolved(), ResultType.Empty,
    		this.passRejected(), ResultType.Empty,
    		onNotified
    	);
    }
}
