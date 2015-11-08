package com.clk;


import java.util.ArrayList;


public final class ActionEvent {

	// constructors
	private ActionEvent() { }
		
		
	// class
	public static class Type0 extends ActionEventBase<ActionHandler.Type0>	{
		
		// Methods
		public void raise()
		{	
			// EventHandlerList
			ArrayList<ActionHandler.Type0> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return;
			
			// Raise	
			for (ActionHandler.Type0 eventHandler : eventHandlerList) 
			{
				eventHandler.raise();
			}
		}	
	}

	public static class Type1<T1> extends ActionEventBase<ActionHandler.Type1<T1>>	{
		
		// Methods
		public void raise(T1 t1)
		{	
			// EventHandlerList
			ArrayList<ActionHandler.Type1<T1>> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return;
			
			// Raise	
			for (ActionHandler.Type1<T1> eventHandler : eventHandlerList) 
			{
				eventHandler.raise(t1);
			}
		}	
	}
	
	public static class Type2<T1, T2> extends ActionEventBase<ActionHandler.Type2<T1, T2>>	{
		
		// Methods
		public void raise(T1 t1, T2 t2)
		{
			// EventHandlerList
			ArrayList<ActionHandler.Type2<T1, T2>> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return;
			
			// Raise	
			for (ActionHandler.Type2<T1, T2> eventHandler : eventHandlerList) 
			{
				eventHandler.raise(t1, t2);
			}
		}	
	}
	
	public static class Type3<T1, T2, T3> extends ActionEventBase<ActionHandler.Type3<T1, T2, T3>>	{
		
		// Methods
		public void raise(T1 t1, T2 t2, T3 t3)
		{
			// EventHandlerList
			ArrayList<ActionHandler.Type3<T1, T2, T3>> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return;
			
			// Raise	
			for (ActionHandler.Type3<T1, T2, T3> eventHandler : eventHandlerList) 
			{
				eventHandler.raise(t1, t2, t3);
			}
		}	
	}
	
	public static class ActionEventBase<TEventHandler> {
		
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
