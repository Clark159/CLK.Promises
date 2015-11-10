package com.clk;


public final class StringHelper {

	// constructors
	private StringHelper() { }
		
		
	// methods
	public static boolean isNullOrEmpty(String value) {
		
		// Require
		if(value == null) return true;
		if(value.equals("") == true) return true;
		
		// Return
		return false;
	}
}
