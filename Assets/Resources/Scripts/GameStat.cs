using UnityEngine;
using System.Collections;

public class GameStat 
{
	private int maxX;
	private int maxY;
	private double max;
	private int[][] grid;
	private bool[][] check;
	private int tier;
	
	public GameStat(int maxX,int maxY, double max,int[][] grid, bool[][] check,int tier)
	{
		this.maxX=maxX;
		this.maxY=maxY;
		this.max=max;
		this.grid=grid;
		this.check=check;
		this.tier=tier;
	}
	
	public int getMaxX()
	{
		return maxX;
	}
	
	public int getMaxY()
	{
		return maxY;
	}
	
	public double getMax()
	{
		return max;
	}
	
	public int[][] getGrid()
	{
		return grid;
	}
	
	public bool[][] getCheck()
	{
		return check;
	}
	public int getTier()
	{
		return tier;
	}
}


