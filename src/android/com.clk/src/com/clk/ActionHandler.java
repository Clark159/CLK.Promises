package com.clk;


public final class ActionHandler {

	// constructors
	private ActionHandler() { }
	
	
	// interface
	public static interface Type0 { void raise(); }
	
	public static interface Type1<T1> { void raise(T1 t1); }
	
	public static interface Type2<T1, T2> { void raise(T1 t1, T2 t2); }
	
	public static interface Type3<T1, T2, T3> { void raise(T1 t1, T2 t2, T3 t3); }
}
