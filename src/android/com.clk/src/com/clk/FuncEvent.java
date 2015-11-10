package com.clk;


import java.util.ArrayList;


public final class FuncEvent {
		
	// constructors
	private FuncEvent() { }
		
		
	// class
	public static class Type0<TResult> extends FuncEventBase<Func.Type0<TResult>> {
		
		// methods
		public TResult raise() throws Exception	{
			
			// eventHandlerList
			ArrayList<Func.Type0<TResult>> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return null;
			
			// raise	
			TResult result = null;
			for (Func.Type0<TResult> eventHandler : eventHandlerList) 
			{
				result = eventHandler.raise();
			}
			
			// return
			return result;
		}	
	}

	public static class Type1<T1, TResult> extends FuncEventBase<Func.Type1<T1, TResult>> {
				
		// methods
		public TResult raise(T1 t1) throws Exception {
			
			// eventHandlerList
			ArrayList<Func.Type1<T1, TResult>> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return null;
			
			// raise	
			TResult result = null;
			for (Func.Type1<T1, TResult> eventHandler : eventHandlerList) 
			{
				result = eventHandler.raise(t1);
			}
			
			// return
			return result;
		}	
	}
	
	public static class Type2<T1, T2, TResult> extends FuncEventBase<Func.Type2<T1, T2, TResult>> {
		
		// methods
		public TResult raise(T1 t1, T2 t2) throws Exception {
			
			// eventHandlerList
			ArrayList<Func.Type2<T1, T2, TResult>> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return null;
			
			// raise	
			TResult result = null;
			for (Func.Type2<T1, T2, TResult> eventHandler : eventHandlerList) 
			{
				result = eventHandler.raise(t1, t2);
			}
			
			// return
			return result;
		}	
	}
	
	public static class FuncEventBase<TEventHandler> {
		
		// fields
		private Object _syncRoot = new Object();
		
		private ArrayList<TEventHandler> _eventHandlerList = new ArrayList<TEventHandler>();
		
		private ArrayList<TEventHandler> _eventHandlerIterator = null;
		
		
		// methods
		public void add(TEventHandler eventHandler)
		{
			// contracts
			if (eventHandler == null) throw new IllegalArgumentException();

			// sync
			synchronized(_syncRoot) 
			{
				// iterator
				_eventHandlerIterator = null;			
				
				// add
				_eventHandlerList.add(eventHandler);
			}
		}
		
		public void remove(TEventHandler eventHandler)
		{
			// contracts
			if (eventHandler == null) throw new IllegalArgumentException();
					
			// sync
			synchronized(_syncRoot) 
			{
				// iterator
				_eventHandlerIterator = null;
				
				// remove
				_eventHandlerList.remove(eventHandler);
			}
		}
		
		protected ArrayList<TEventHandler> getEventHandlerList()
		{
			// result
			ArrayList<TEventHandler> eventHandlerIterator = null;
			
			// sync
			synchronized(_syncRoot) 
			{
				// require			
				if(_eventHandlerList.isEmpty() == true) return null;
				
				// iterator
				if(_eventHandlerIterator == null)
				{
					// create
					_eventHandlerIterator = new ArrayList<TEventHandler>();
					
					// copy
					for (TEventHandler eventHandler : _eventHandlerList) 
					{
						_eventHandlerIterator.add(eventHandler);
					}
				}	
				eventHandlerIterator = _eventHandlerIterator;
			}
			
			// return
			return eventHandlerIterator;
		}		
	}	
}
