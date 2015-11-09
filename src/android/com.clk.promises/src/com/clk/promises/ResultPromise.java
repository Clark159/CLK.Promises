package com.clk.promises;

import com.clk.Action;
import com.clk.Func;

public class ResultPromise<TResult> extends PromiseBase<TResult>{
	
    // methods 
    public void resolve(TResult result)
    {
    	// resolve
    	this.resolveBase(result);
    }
    
    
    public Promise thenEmpty(
    		final Action.Type1<TResult> onResolved, 
    		final Action.Type1<Exception> onRejected, 
			final Action.Type1<Progress> onNotified)
	{ 
		// promise
    	final Promise thenPromise = new Promise();
    	
    	// resolveHandler
        Action.Type1<TResult> resolveHandler = new Action.Type1<TResult> ()
        {        	
			@Override public void raise(TResult result) {
				try
	            {
	                // execute
	                onResolved.raise(result);

	                // distribute
	                thenPromise.resolve();
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
	                onRejected.raise(error);
	
	                // distribute
	                thenPromise.resolve();
	            }
	            catch (Exception ex)
	            {
	                thenPromise.reject(ex);
	            }
        	}
        };
    	    	
        // notifiedHandler
        Action.Type1<Progress> notifiedHandler = new Action.Type1<Progress>() 
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
        this.push(resolveHandler, rejectHandler, notifiedHandler);    	
    	
    	// return
    	return thenPromise;
	};	
	
    public <TResolvedResult, TRejectedResult> Promise thenEmpty(
    		final Func.Type1<TResult, TResolvedResult> onResolved, final ResultType onResolvedResultType,
    		final Func.Type1<Exception, TRejectedResult> onRejected, final ResultType onRejectedResultType, 
			final Action.Type1<Progress> onNotified)
	{ 
		// promise
    	final Promise thenPromise = new Promise();
    	    	
    	// resolveHandler
        Action.Type1<TResult> resolveHandler = new Action.Type1<TResult> ()
        {        	
			@Override public void raise(TResult result) {
				try
	            {
	                // execute
	                Object resultObject = onResolved.raise(result);

	                // distribute
	                switch (onResolvedResultType)
                    {
                    	case Empty:
	                        thenPromise.resolve();
	                        break;
                        
                        case EmptyPromise:
                        	((Promise)resultObject).thenEmpty(
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
                        	((Promise)resultObject).thenEmpty(
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
        
        // notifiedHandler
        Action.Type1<Progress> notifiedHandler = new Action.Type1<Progress>() 
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
        this.push(resolveHandler, rejectHandler, notifiedHandler);    	
    	
    	// return
    	return thenPromise;
	};	
	
	public <TNewResult, TResolvedResult, TRejectedResult> ResultPromise<TNewResult> thenResult(
    		final Func.Type1<TResult, TResolvedResult> onResolved, final ResultType onResolvedResultType,
    		final Func.Type1<Exception, TRejectedResult> onRejected, final ResultType onRejectedResultType, 
			final Action.Type1<Progress> onNotified)
	{
		// promise
    	final ResultPromise<TNewResult> thenPromise = new ResultPromise<TNewResult>();
    	    	
    	// resolveHandler
        Action.Type1<TResult> resolveHandler = new Action.Type1<TResult> ()
        {        	
			@SuppressWarnings("unchecked")
			@Override public void raise(TResult result) {
				try
	            {
	                // execute
	                Object resultObject = onResolved.raise(result);

	                // distribute
	                switch (onResolvedResultType)
                    {
                    	case Empty:
	                        thenPromise.resolve(null);
	                        break;
                        
                        case EmptyPromise:
                        	((Promise)resultObject).thenEmpty(
    	                		new Action.Type0() { @Override public void raise() {  thenPromise.resolve(null); }},	
    	                        new Action.Type1<Exception>(){ @Override public void raise(Exception thenError) { thenPromise.reject(thenError); }},
    	                        new Action.Type1<Progress>(){ @Override public void raise(Progress thenProgress) { thenPromise.notify(thenProgress); }}
    	                    );
                            break;
                            
                        case Result:
                            thenPromise.resolve((TNewResult)resultObject);
                            break;
                            
                        case ResultPromise:
                        	((ResultPromise<TNewResult>)resultObject).thenEmpty(
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
                        	((Promise)resultObject).thenEmpty(
    	                		new Action.Type0() { @Override public void raise() {  thenPromise.resolve(null); }},	
    	                        new Action.Type1<Exception>(){ @Override public void raise(Exception thenError) { thenPromise.reject(thenError); }},
    	                        new Action.Type1<Progress>(){ @Override public void raise(Progress thenProgress) { thenPromise.notify(thenProgress); }}
    	                    );
                            break;
                            
                        case Result:
                            thenPromise.resolve((TNewResult)resultObject);
                            break;
                            
                        case ResultPromise:
                        	((ResultPromise<TNewResult>)resultObject).thenEmpty(
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

        // notifiedHandler
        Action.Type1<Progress> notifiedHandler = new Action.Type1<Progress>() 
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
        this.push(resolveHandler, rejectHandler, notifiedHandler);    	
        
    	// return
        return thenPromise;
	};	
}
