package com.clk.promises;

import com.clk.StringHelper;

public class Progress {
	
	// constructors
    public Progress(String localizedDescription, int completedCount, int totalCount)
    {
    	// contracts
        if (StringHelper.isNullOrEmpty(localizedDescription) == true) throw new IllegalArgumentException();
        
        // default            
    	_completedCount = completedCount;
    	_totalCount = totalCount;
    	_localizedDescription = localizedDescription;
    }


    // properties
    private String _localizedDescription;
	public final String getLocalizedDescription()
	{
		return _localizedDescription;
	}
	
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
}