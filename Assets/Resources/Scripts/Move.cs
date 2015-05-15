using UnityEngine;
using System.Collections;

public class Move 
{
	private PlayerManager.Faction faction;
	private int row;
	private int col;
	private int tier;
	private int removedTier;
	
	public Move (PlayerManager.Faction faction, int row, int col,int tier)
	{
		this.faction = faction;
		this.row = row;
		this.col = col;
		this.tier = tier;
		this.removedTier = 0;
	}	
	public Move (PlayerManager.Faction faction, int row, int col, int tier,int removedTier)
	{
		this.faction = faction;
		this.row = row;
		this.col = col;
		this.tier = tier;
		this.removedTier = removedTier;
	}
	
	public PlayerManager.Faction getFaction()
	{
		return faction;
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


