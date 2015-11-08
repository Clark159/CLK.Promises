package com.clk;


import java.util.ArrayList;


public final class FuncEvent {
		
	// Constructors
	private FuncEvent() { }
		
		
	// Class
	public static class Type0<TResult> extends FuncEventBase<FuncHandler.Type0<TResult>> {
		
		// Methods
		public TResult raise()
		{	
			// EventHandlerList
			ArrayList<FuncHandler.Type0<TResult>> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return null;
			
			// Raise	
			TResult result = null;
			for (FuncHandler.Type0<TResult> eventHandler : eventHandlerList) 
			{
				result = eventHandler.raise();
			}
			
			// Return
			return result;
		}	
	}

	public static class Type1<T1, TResult> extends FuncEventBase<FuncHandler.Type1<T1, TResult>> {
				
		// Methods
		public TResult raise(T1 t1)
		{	
			// EventHandlerList
			ArrayList<FuncHandler.Type1<T1, TResult>> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return null;
			
			// Raise	
			TResult result = null;
			for (FuncHandler.Type1<T1, TResult> eventHandler : eventHandlerList) 
			{
				result = eventHandler.raise(t1);
			}
			
			// Return
			return result;
		}	
	}
	
	public static class Type2<T1, T2, TResult> extends FuncEventBase<FuncHandler.Type2<T1, T2, TResult>> {
		
		// Methods
		public TResult raise(T1 t1, T2 t2)
		{
			// EventHandlerList
			ArrayList<FuncHandler.Type2<T1, T2, TResult>> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return null;
			
			// Raise	
			TResult result = null;
			for (FuncHandler.Type2<T1, T2, TResult> eventHandler : eventHandlerList) 
			{
				result = eventHandler.raise(t1, t2);
			}
			
			// Return
			return result;
		}	
	}
	
	public static class FuncEventBase<TEventHandler> {
		
		// Fields
		private Object _syncRoot = new Object();
		
		private ArrayList<TEventHandler> _eventHandlerList = new ArrayList<TEventHandler>();
		
		private ArrayList<TEventHandler> _eventHandlerIterator = null;
		
		
		// Methods
		public void add(TEventHandler eventHandler)
		{
			// Contracts
			if (eventHandler == null) throw new IllegalArgumentException();

			// Sync
			synchronized(_syncRoot) 
			{
				// Iterator
				_eventHandlerIterator = null;			
				
				// Add
				_eventHandlerList.add(eventHandler);
			}
		}
		
		public void remove(TEventHandler eventHandler)
		{
			// Contracts
			if (eventHandler == null) throw new IllegalArgumentException();
					
			// Sync
			synchronized(_syncRoot) 
			{
				// Iterator
				_eventHandlerIterator = null;
				
				// Remove
				_eventHandlerList.remove(eventHandler);
			}
		}
		
		protected ArrayList<TEventHandler> getEventHandlerList()
		{
			// Result
			ArrayList<TEventHandler> eventHandlerIterator = null;
			
			// Sync
			synchronized(_syncRoot) 
			{
				// Require			
				if(_eventHandlerList.isEmpty() == true) return null;
				
				// Iterator
				if(_eventHandlerIterator == null)
				{
					// Create
					_eventHandlerIterator = new ArrayList<TEventHandler>();
					
					// Copy
					for (TEventHandler eventHandler : _eventHandlerList) 
					{
						_eventHandlerIterator.add(eventHandler);
					}
				}	
				eventHandlerIterator = _eventHandlerIterator;
			}
			
			// Return
			return eventHandlerIterator;
		}		
	}	
}
