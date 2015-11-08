package com.clk.promises.mobileapp;

import com.clk.promises.Progress;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.view.View;


public class MainActivity extends Activity {

	// Constructors
	@Override protected void onCreate(Bundle savedInstanceState) {
		
		// Base
        super.onCreate(savedInstanceState);
        
        // View
        setContentView(R.layout.activity_main);        
	}
	
	
	// Handlers    
    public void testButton_onClick(View view) {
    	
    	// Progress
        Progress progress = new Progress(0,100,"Clark");
        		
		// Alert
 		AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
 		alertDialog.setNeutralButton("OK", new DialogInterface.OnClickListener()
 		{
 			public void onClick(DialogInterface dialog, int which) { }
 		});
 		alertDialog.setMessage(progress.getDescription());
 		alertDialog.show();
    }
}
