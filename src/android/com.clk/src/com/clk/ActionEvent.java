package com.clk;


import java.util.ArrayList;


public final class ActionEvent {

	// constructors
	private ActionEvent() { }
		
		
	// class
	public static class Type0 extends ActionEventBase<Action.Type0>	{
		
		// methods
		public void raise() throws Exception {	
			
			// eventHandlerList
			ArrayList<Action.Type0> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return;
			
			// raise	
			for (Action.Type0 eventHandler : eventHandlerList) 
			{
				eventHandler.raise();
			}
		}	
	}

	public static class Type1<T1> extends ActionEventBase<Action.Type1<T1>>	{
		
		// methods
		public void raise(T1 t1)  throws Exception {
			
			// eventHandlerList
			ArrayList<Action.Type1<T1>> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return;
			
			// raise	
			for (Action.Type1<T1> eventHandler : eventHandlerList) 
			{
				eventHandler.raise(t1);
			}
		}	
	}
	
	public static class Type2<T1, T2> extends ActionEventBase<Action.Type2<T1, T2>>	{
		
		// methods
		public void raise(T1 t1, T2 t2) throws Exception {
			
			// eventHandlerList
			ArrayList<Action.Type2<T1, T2>> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return;
			
			// raise	
			for (Action.Type2<T1, T2> eventHandler : eventHandlerList) 
			{
				eventHandler.raise(t1, t2);
			}
		}	
	}
	
	public static class Type3<T1, T2, T3> extends ActionEventBase<Action.Type3<T1, T2, T3>>	{
		
		// methods
		public void raise(T1 t1, T2 t2, T3 t3) throws Exception	{
			
			// eventHandlerList
			ArrayList<Action.Type3<T1, T2, T3>> eventHandlerList = this.getEventHandlerList();
			if(eventHandlerList == null) return;
			
			// raise	
			for (Action.Type3<T1, T2, T3> eventHandler : eventHandlerList) 
			{
				eventHandler.raise(t1, t2, t3);
			}
		}	
	}
	
	public static class ActionEventBase<TEventHandler> {
		
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
