package com.clk.promises;

public class Progress {
	
	// Constructors
    public Progress(int completedCount, int totalCount, String description)
    {
        // Default            
    	_completedCount = completedCount;
    	_totalCount = totalCount;
    	_description = description;
    }


    // Properties
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