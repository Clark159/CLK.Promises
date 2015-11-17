package com.clk.promises.mobileapp;

import java.util.ArrayList;

import com.clk.Action;
import com.clk.Func;
import com.clk.promises.Promise;
import com.clk.promises.Progress;
import com.clk.promises.ResultPromise;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.widget.TextView;


public class MainActivity extends Activity {

	// constructors
	@Override protected void onCreate(Bundle savedInstanceState) {
		
		// Base
        super.onCreate(savedInstanceState);
        
        // View
        setContentView(R.layout.activity_main); 
        
        // tests
        promiseTests();
	}
	
	
	// methods
	private void promiseTests()	{
		
		// promise
    	Promise promise = new Promise();
    	
    	promise
    		
	    	// ========== then ==========
	        .then(new Action.Type0() {
				@Override public void raise() {
					writeLine("AAA");
				}    			
    		})
	        
	        // thenPromise - resolve
            .thenPromise(new Func.Type0<Promise>(){
				@Override public Promise raise() {
					Promise newPromise = new Promise();
                    newPromise.resolve();
                    return newPromise;
				}            	
            })
            .then(new Action.Type0() {
				@Override public void raise() {
					writeLine("BBB");
				}    			
    		})
            
            // thenPromise - reject
            .thenPromise(new Func.Type0<Promise>(){
				@Override public Promise raise() {
					Promise newPromise = new Promise();
					newPromise.reject(new Exception("CCC"));
                    return newPromise;
				}            	
            })
            .fail(new Action.Type1<Exception>(){
				@Override public void raise(Exception error) {
					writeLine(error.getMessage());
				}    			
    		})
            
            
            // ========== thenNew ==========
            .thenNew(new Func.Type0<String>(){
				@Override public String raise() {
					return "DDD";
				}            
            })
            .then(new Action.Type1<String>(){
				@Override public void raise(String result) {
					writeLine(result);
				}            	
            })
            
            // thenNewPromise - resolve
            .thenNewPromise(new Func.Type0<ResultPromise<String>>(){
				@Override public ResultPromise<String> raise() {
					ResultPromise<String> newPromise = new ResultPromise<String>();
					newPromise.resolve("EEE");
                    return newPromise;
				}            	
            })
            .then(new Action.Type1<String>(){
				@Override public void raise(String result) {
					writeLine(result);
				}            	
            })
                      
            // thenNewPromise - reject
            .thenNewPromise(new Func.Type0<ResultPromise<String>>(){
				@Override public ResultPromise<String> raise() {
					ResultPromise<String> newPromise = new ResultPromise<String>();
					newPromise.reject(new Exception("FFF"));
                    return newPromise;
				}            	
            })
            .fail(new Action.Type1<Exception>(){
				@Override public void raise(Exception error) {
					writeLine(error.getMessage());
				}    			
    		})
            
            
            // ========== all ==========
            .thenNewPromise(new Func.Type0<ResultPromise<ArrayList<String>>>(){
				@Override public ResultPromise<ArrayList<String>> raise() {
					
					ArrayList<ResultPromise<String>> promiseList = new ArrayList<ResultPromise<String>>();
					
					ResultPromise<String> promiseA = new ResultPromise<String>();
					promiseA.resolve("GGG");
					promiseList.add(promiseA);
					
					ResultPromise<String> promiseB = new ResultPromise<String>();
					promiseB.resolve("HHH");
					promiseList.add(promiseB);
					
					return Promise.allNewPromise(promiseList);
				}            	
            })
            .then(new Action.Type1<ArrayList<String>>(){
				@Override public void raise(ArrayList<String> resultList) {
					for(String result : resultList)
                    {
						writeLine(result);
                    }
				}            	
            })
            
            // ========== Throw ==========
            .then(new Action.Type0() {
				@Override public void raise() throws Exception {
					throw new Exception("III");
				}    			
    		})
            .fail(new Action.Type1<Exception>(){
				@Override public void raise(Exception error) throws Exception {
					throw error;
				}    			
    		})
            .fail(new Action.Type1<Exception>(){
				@Override public void raise(Exception error) {
					writeLine(error.getMessage());
				}    			
    		})
            
            
    		// ========== end ==========
    		.progress(new Action.Type1<Progress>() {
				@Override public void raise(Progress progress) {
					writeLine("Progress:" + progress.getLocalizedDescription());
				}    			
    		})
    		.fail(new Action.Type1<Exception>(){
				@Override public void raise(Exception error) {
					writeLine("Fail:" + error.getMessage());
				}    			
    		})
    		.then(new Action.Type0(){
				@Override public void raise() {
					writeLine("End");
				}    		
    		})   	
	    ;
    	
    	// operate 
        promise.notify(new Progress("0%", 0, 100));
        promise.notify(new Progress("50%", 50, 100));
        promise.notify(new Progress("100%", 100, 100));
        promise.resolve();
	}
	
	private void writeLine(String message) {
		
		// textView
		TextView textView = (TextView)findViewById(R.id.displayTextView);
		
		// writeLine
		textView.append(message + "\n");
	}
	
	
	// handlers    
    public void testButton_onClick(View view) {
    	    	
    }
}
