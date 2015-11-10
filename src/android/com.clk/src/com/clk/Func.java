package com.clk;


public final class Func {

	// constructors
	private Func() { }
	
	
	// interface
	public static interface Type0<TResult> { TResult raise() throws Exception; }
	
	public static interface Type1<T1, TResult> { TResult raise(T1 t1) throws Exception; }
	
	public static interface Type2<T1, T2, TResult> { TResult raise(T1 t1, T2 t2) throws Exception; }
}
