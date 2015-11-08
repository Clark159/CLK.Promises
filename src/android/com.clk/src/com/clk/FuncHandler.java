package com.clk;


public final class FuncHandler {

	// Constructors
	private FuncHandler() { }
	
	
	// Interface
	public static interface Type0<TResult> { TResult raise(); }
	
	public static interface Type1<T1, TResult> { TResult raise(T1 t1); }
	
	public static interface Type2<T1, T2, TResult> { TResult raise(T1 t1, T2 t2); }
}
