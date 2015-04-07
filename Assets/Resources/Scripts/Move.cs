using UnityEngine;
using System.Collections;

public class Move 
{
	private int player;
	private int row;
	private int col;
	private int tier;
	private int removedTier;
	
	public Move (int player, int row, int col,int tier)
	{
		this.player = player;
		this.row = row;
		this.col = col;
		this.tier = tier;
		this.removedTier = 0;
	}	
	public Move (int player, int row, int col, int tier,int removedTier)
	{
		this.player = player;
		this.row = row;
		this.col = col;
		this.tier = tier;
		this.removedTier = removedTier;
	}
	
	public int getPlayer()
	{
		return player;
	}
	public int getRow()
	{
		return row;
	}
	public int getCol()
	{
		return col;
	}
	public int getTier()
	{
		return tier;
	}
	public int getRemovedTier()
	{
		return removedTier;
	}
	
}


