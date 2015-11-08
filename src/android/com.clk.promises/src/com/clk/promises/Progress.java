package com.clk.promises;

public class Progress {
	
	// constructors
    public Progress(int completedCount, int totalCount, String description)
    {
        // default            
    	_completedCount = completedCount;
    	_totalCount = totalCount;
    	_description = description;
    }


    // properties
    private int _completedCount;
	public final int getCompletedCount()
	{
		return _completedCount;
	}
	
	private int _totalCount;
	public final int getTotalCount()
	{
		return _totalCount;
	}
	
	private String _description;
	public final String getDescription()
	{
		return _description;
	}
}