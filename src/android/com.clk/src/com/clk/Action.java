package com.clk;


public final class Action {

	// constructors
	private Action() { }
	
	
	// interface
	public static interface Type0 { void raise() throws Exception; }
	
	public static interface Type1<T1> { void raise(T1 t1) throws Exception; }
	
	public static interface Type2<T1, T2> { void raise(T1 t1, T2 t2) throws Exception; }
	
	public static interface Type3<T1, T2, T3> { void raise(T1 t1, T2 t2, T3 t3) throws Exception; }
}
