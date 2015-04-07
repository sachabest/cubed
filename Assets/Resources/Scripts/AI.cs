using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AI
{
	public static readonly double POTENTIAL_POINTS_RATIO = .500;
	/*public static void main (String args[])
	{
		int[][]grid = 
			{
				{2,2,2,0,0,0},
				{2,2,2,0,0,0},
				{2,2,0,0,1,1},
				{0,0,0,1,1,1},
				{0,0,0,1,1,1}
			};
		bool check [][] = 
			{
				{false,false,false,true,true,true},
				{false,false,false,true,true,true},
				{false,false,true,true,false,false},
				{true,true,false,false,false,false},
				{true,true,false,false,false,false}
			};
		int roll =6;
		AI ai = new AI();
		List<GameStat>array = ai.makeMove(grid,check,1,roll);
		for (GameStat arr : array)
		{
			//System.out.println("Tier: " + arr.getTier());
			//System.out.println("Max X: "+ arr.getMaxX());
			//System.out.println("Max Y: "+ arr.getMaxY());
		}
	}*/
	
	public int winningScore;
	public AI(int winningScore) {
		this.winningScore = winningScore;
	}
	public AI() {
		this.winningScore = 100;
	}
	//MakeMove method picks greatest NET move for 1 roll of the dice.
	//Compares best offensive move to best defensive move for any single roll of the dice.
	public List<GameStat> makeMove(int[][]grid,bool[][] check,int movingTeam, int roll)
	{
		List<GameStat> array= new List<GameStat>();
		if (roll==12)
		{
			//2 tier 2
			GameStat stat10 =tier2(grid,check,movingTeam);
			GameStat stat11 =tier2(stat10.getGrid(),stat10.getCheck(),movingTeam);
			double d = stat10.getMax()+stat11.getMax();
			
			//2 tier 1 1 tier 2
			GameStat stat5 =tier1(grid,check,movingTeam);
			GameStat stat6 =tier1(stat5.getGrid(),stat5.getCheck(),movingTeam);
			GameStat stat7 =tier2(stat6.getGrid(),stat6.getCheck(),movingTeam);
			double b = stat5.getMax()+stat6.getMax()+stat7.getMax();
			
			
			//1 tier 1 1 tier 3
			GameStat stat8 =tier1(grid,check,movingTeam);
			GameStat stat9 =tier3(stat8.getGrid(),stat8.getCheck(),movingTeam);
			double c = stat8.getMax()+stat9.getMax();
			
			//4 tier 1
			GameStat stat1 =tier1(grid,check,movingTeam);
			GameStat stat2 =tier1(stat1.getGrid(),stat1.getCheck(),movingTeam);
			GameStat stat3 =tier1(stat2.getGrid(),stat2.getCheck(),movingTeam);
			GameStat stat4 =tier1(stat3.getGrid(),stat3.getCheck(),movingTeam);
			double a = stat1.getMax() + stat2.getMax()+stat3.getMax()+stat4.getMax();
			
			//System.out.println("a: "+ a);
			//System.out.println("b: "+ b);
			//System.out.println("c: "+ c);
			//System.out.println("d: "+ d);

			double optimal = Math.Max(Math.Max(Math.Max(a, b), c), d);
			//System.out.println("optimal: "+ optimal);
		
			if(optimal==a)
			{
				array.Add(stat1);
				array.Add(stat2);
				array.Add(stat3);
				array.Add(stat4);
			}
			else if (optimal==b)
			{
				array.Add(stat5);
				array.Add(stat6);
				array.Add(stat7);
			}
			else if (optimal==c)
			{
				array.Add(stat8);
				array.Add(stat9);
			}
			else
			{
				array.Add(stat10);
				array.Add(stat11);
			}
		}
		else if(roll>=9)
		{
			//1 tier 3
			GameStat stat6 =tier3(grid,check,movingTeam);
			double c = stat6.getMax();
			
			//1 tier 2 and 1 tier 1
			GameStat stat4 =tier1(grid,check,movingTeam);
			GameStat stat5 =tier2(stat4.getGrid(),stat4.getCheck(),movingTeam);
			double b = stat4.getMax()+stat5.getMax();
			
			//3 tier 1
			GameStat stat1 =tier1(grid,check,movingTeam);
			GameStat stat2 =tier1(stat1.getGrid(),stat1.getCheck(),movingTeam);
			GameStat stat3 =tier1(stat2.getGrid(),stat2.getCheck(),movingTeam);
			double a = stat1.getMax()+stat2.getMax()+stat3.getMax();
			//System.out.println("a: "+ a);
			//System.out.println("b: "+ b);
			//System.out.println("c: "+ c);
			
			double optimal=Math.Max(Math.Max(a, b), c);
			//System.out.println("optimal: "+ optimal);
			
			if(optimal == a)
			{
				array.Add(stat1);
				array.Add(stat2);
				array.Add(stat3);
			}
			else if(optimal == b)
			{
				array.Add(stat4);
				array.Add(stat5);
			}
			else
			{
				array.Add(stat6);
			}
			
		}

		else if (roll>=6)
		{	
			
			//1 tier 2
			GameStat stat3 = tier2(grid,check,movingTeam);
			double b = stat3.getMax();
			
			// 2 tier 1
			GameStat stat1 =tier1(grid,check,movingTeam);
			GameStat stat2 =tier1(stat1.getGrid(),stat1.getCheck(),movingTeam);
			double a = stat1.getMax()+stat2.getMax();
			
			//System.out.println("a: "+ a);
			//System.out.println("b: "+ b);
			
			if(a>b)
			{
				array.Add(stat1);
				array.Add(stat2);
				//System.out.println("optimal: "+ a);
			}
			else
			{
				array.Add(stat3);
				//System.out.println("optimal: "+ b);
			}
		}		
		else// (roll>=3)
		{
			GameStat stat =tier1(grid,check,movingTeam);
			array.Add(stat);
		}
		return array;
	}
	private GameStat tier1(int[][] grid,bool[][]check, int movingTeam)
	{
		double max=0;
		int maxX=0;
		int maxY=0;
		double a=0;
		double b=0;
		for(int x=0;x<grid.GetLength(0);x++)
		{
			for(int y=0;y<grid[0].GetLength(0);y++)
			{
				if(check[x][y]==true)
				{
					try
					{
						a = aiGaines(grid,x,y,movingTeam);
						//if (a>0)
						//System.out.println("AI Gains: "+a);
						int opponentTeam = movingTeam == 1 ? 2:1; //UPDATE THIS WHEN WE HAVE DIFFERENT PLAYERS!!!
						
						b = opponentLosses(grid,check,x,y,opponentTeam,1);
						if(b>0)
						//System.out.println("Opponent Losses: "+b);
						if (a+b>max)
						{
							max=a+b;  //System.out.println("new max found");
							maxX=x;
							maxY=y;
						}
					}
					#pragma warning disable
					catch(Exception INDEXOUTOFBOUNDS) {}
					#pragma warning enable
				}
			}
		}
		// Can not make a new max
		if (max==0)
		{
			List<int> locs = this.mostAdjacentLocation(grid,movingTeam);  //System.out.println("No points can be scored, looking for a location with lots of adjacent friendlies.\nLocs with the most adjacent friendlies are: " + locs.toString());

			int numCols = grid[0].GetLength(0);
			try
			{
				maxX = (locs[0]/numCols);
				//maxX = (locs.get(0)-numCols)/numCols;
				//System.out.println("maxX: "+ maxX);
				maxY = (locs[0]%numCols);
			}
			catch(Exception e)
			{
				maxX = UnityEngine.Random.Range(0, grid.GetLength(0));
				maxY = UnityEngine.Random.Range(0, grid[0].GetLength(0));
				while(!check[maxX][maxY])
				{
					maxX = UnityEngine.Random.Range(0, grid.GetLength(0));
					maxY = UnityEngine.Random.Range(0, grid[0].GetLength(0));
				}
				
			}
			////System.out.println("maxY: "+ maxY);
			a = aiGaines(grid,maxX,maxY,movingTeam);
			max=a;
		}
		grid[maxX][maxY]=1;              //System.out.println("Moved to location (" + maxX + ", "+ maxY + ")");
		check[maxX][maxY]=false;
		GameStat stat = new GameStat(maxX,maxY,max,grid,check,1); //tier 1
		return stat;
		
	}
	
	private GameStat tier2(int[][] grid,bool[][] check, int movingTeam)
	{
		double max=0;
		int maxX=0;
		int maxY=0;
		for(int x=0;x<grid.GetLength(0);x++)
		{
			for(int y=0;y<grid[0].GetLength(0);y++)
			{
				if(check[x][y]==true)
				{
					try
					{
						double a = aiGaines(grid,x,y,movingTeam);
						if (check[x][y+1])
						{
							a+=POTENTIAL_POINTS_RATIO;
							//a+=0;
						}
						if(check[x][y-1])
						{
							a+=POTENTIAL_POINTS_RATIO;
							//a+=0;
						}
						if(check[x+1][y])
						{
							a+=POTENTIAL_POINTS_RATIO;
							//a+=0;
						}
						
						if(check[x-1][y])
						{
							a+=POTENTIAL_POINTS_RATIO;
							//a+=0;
						}
						int opponentTeam = movingTeam == 1 ? 2:1; //UPDATE THIS WHEN WE HAVE DIFFERENT PLAYERS!!!
						double b = opponentLosses(grid,check,x,y,opponentTeam,2); //tier 2
						if (a+b>max)
						{
							max=a+b;
							maxX=x;
							maxY=y;
						}
					}
					#pragma warning disable
					catch (Exception INDEXOUTOFBOUNDS) {}
					#pragma warning enable
				}
			}
		}
		grid[maxX][maxY]=1;
		check[maxX][maxY]=false;
		GameStat stat = new GameStat(maxX,maxY,max,grid,check,2); //tier 2
		return stat;
	}
	
	private GameStat tier3(int[][] grid,bool[][] check, int movingTeam)
	{
		double max=0;
		int maxX=0;
		int maxY=0;
		for(int x=0;x<grid.GetLength(0);x++)
		{
			for(int y=0;y<grid[0].GetLength(0);y++)
			{
				double a = aiGaines(grid,x,y,movingTeam);
				if(check[x][y]==true)
				{
					
					try
					{
						for(int row=-1; row<2;row++)
						{
							for(int col=-1;col<2;col++)
							{
								if(row!=0 && col!=0 && check[x+row][y+col]==true && grid[x+row][y+col]!=1)
								{
									a+=POTENTIAL_POINTS_RATIO;
								}
							}
						}
					}
					catch (Exception INDEXOUTOFBOUNDS)
					{
						
					}
					int opponentTeam = movingTeam == 1 ? 2:1; //UPDATE THIS LATER~!!!
					double b = opponentLosses(grid,check,x,y,opponentTeam,3);
					if (a+b>max)
					{
						max=a+b;
						maxX=x;
						maxY=y;
					}
				}
			}
		}
		grid[maxX][maxY]=1;
		check[maxX][maxY]=false;
		GameStat stat = new GameStat(maxX,maxY,max,grid,check,3);
		return stat;
	}
	//returns an arrayList of empty locations which have the most adjacent friendly cubes
	//location is represented as row*numCols + col
	//The list is empty if there are no adjacent friendly locs in the whole grid.
	private List<int> mostAdjacentLocation(int [][] grid, int team)
	{
		int maxCount = 1;  //need to start at 1 so the method returns an empty list if no friendly adjacents are possible.
		int numRows = grid.GetLength(0);
		int numCols = grid[0].GetLength(0);
		List<int> bestLocs = new List<int>();
		
		for (int row = 0; row < numRows; row++)
		{
			for (int col = 0; col < numCols; col++)
			{
				if (grid[row][col] == 0) //Must be an empty location to be considered
				{
					List<int> adjacents = this.getAdjacentLocations(grid, row, col);
					int thisCount = 0;
					foreach (int loc in adjacents)
					{
						if (grid[loc/numCols][loc%numCols] == team)
							thisCount++;
					}
					if (thisCount > maxCount)
					{
						maxCount = thisCount;  //System.out.println("New best loc found at (" + row +", " + col + ") with " + thisCount + " adjacent friendlies.");
						bestLocs = new List<int>();
					}
					if (thisCount == maxCount)
						bestLocs.Add(row*numCols + col);
				}
			}
		}
		return bestLocs;
	}
	/*private int numAdjacentLocations(int [][] grid, int row, int col)
	{
		
	} */
	
	
	private double opponentExpectedGains (int [][] grid, bool[][] check,int opponentTeam)
	{
		GameStat[][] bestMoves = this.bestMovesForEachRoll(grid, check, opponentTeam);
		
		GameStat[] stat3 = bestMoves[0];
		GameStat[] stat6 = bestMoves[1];
		GameStat[] stat9 = bestMoves[2];
		GameStat[] stat12= bestMoves[3];
		
		double roll3=0, roll6=0, roll9=0, roll12=0;
		//GameStat best3 = null, best6 = null, best9 = null, best12 = null;
		foreach(GameStat stat in stat3)
		{
			roll3 += stat.getMax();
		}
		foreach(GameStat stat in stat6)
		{
			roll6 += stat.getMax();
		}
		foreach(GameStat stat in stat9)
		{
			roll9 += stat.getMax();
		}
		foreach(GameStat stat in stat12)
		{
			roll12 += stat.getMax();
		}
		
		double expected = (1.0/4)*roll3 + (4.0/9)*roll6 + (1.0/4)*roll9 + (1.0/36)*roll12;
		return expected;
	}
	
	private GameStat[][] bestMovesForEachRoll(int[][] grid, bool[][] check, int team)
	{
		List<GameStat> stat3 = this.makeMove(grid,check,team,3);
		List<GameStat> stat6 = this.makeMove(grid,check,team,6);
		List<GameStat> stat9 = this.makeMove(grid,check,team,9);
		List<GameStat> stat12 = this.makeMove(grid,check,team,12);
		
		object[][] bestMoves = {stat3.ToArray(), stat6.ToArray(), stat9.ToArray(), stat12.ToArray()};
		return (GameStat[][])bestMoves;
	}
	
	
	
//	private double opponentLosses(int[][] grid, bool[][] check, int x, int y, int opponentTeam, int tierPlaced) 
//	{
//		double opponentOffensiveStrength = this.opponentExpectedGains (grid, check, opponentTeam);
//		return opponentOffensiveStrength;
//	}
	private double opponentLosses(int[][] grid, bool[][] check, int x, int y, int opponentTeam, int tierPlaced)
	{
		double loss = 0;
		if(tierPlaced==1)
		{
			return aiGaines(grid,x,y,opponentTeam);
		}
		else if (tierPlaced==2)
		{
			try
			{
				if (check[x][y+1])
				{
					loss+=POTENTIAL_POINTS_RATIO;
					loss+=aiGaines(grid,x,y+1,opponentTeam);
				}
				if(check[x][y-1])
				{
					loss+=POTENTIAL_POINTS_RATIO;
					loss+=aiGaines(grid,x,y-1,opponentTeam);
				}
				if(check[x+1][y])
				{
					loss+=POTENTIAL_POINTS_RATIO;
					loss+=aiGaines(grid,x+1,y,opponentTeam);
				}
				
				if(check[x-1][y])
				{
					loss+=POTENTIAL_POINTS_RATIO;
					loss+=aiGaines(grid,x-1,y,opponentTeam);
				}
			}
			catch(Exception INDEXOUTOFBOUNDS)
			{
				
			}
			
		}
		else // (tierPlaced==3)
		{
			try
			{
				for(int row=-1; row<2;row++)
				{
					for(int col=-1;col<2;col++)
					{
						if(row!=0 && col!=0 && check[x+row][y+col]==true && grid[x+row][y+col]!=1)
						{
							loss+=POTENTIAL_POINTS_RATIO;
							loss+=aiGaines(grid,x+row,y+col,opponentTeam);
						}
					}
				}
			}
			catch (Exception INDEXOUTOFBOUNDS)
			{
				
			}
		}
		return loss;
	}
	

	private double aiGaines(int[][] grid,int x,int y,int team) 
	{	
		int[,] temp = new int[grid.GetLength(0),grid.GetLength(0)];
		int[,] temp2 = new int[grid.GetLength(0),grid.GetLength(0)];
		//this.transfer(grid,temp);
		//this.transfer(grid,temp2);
		
		
		for(int i = 0; i < grid.GetLength(0); i++)
		{
			for(int j = 0; j < grid[0].GetLength(0); j++)
			{
				temp[i,j] = grid[i][j];
				temp2[i,j] = grid[i][j];
			}
		}
		
		
		
		temp2[x,y]=team;
		int score = GameManager.totalCount(temp,team,0,0);
		////System.out.println(score);
		int score2 = GameManager.totalCount(temp2,team,0,0);
		////System.out.println(score2);
		return score2-score;
	}
	
//	private double gains(int[][] grid, bool [][] check, int x,int y, int tier, int team)
//	{
//		int[][] temp = new int[grid.GetLength(0)][grid[0].GetLength(0)];
//		int[][] temp2 = new int[grid.GetLength(0)][grid[0].GetLength(0)];
//		this.transfer(grid,temp);
//		this.transfer(grid,temp2);
//		if(tier == 1)
//		{
//			temp2[x][y]=team;
//		}
//		else if (tier == 2)
//		{
//			try
//			{
//				if (check[x][y+1])
//				{
//					temp2[x][y+1]=team;
//				}
//				if(check[x][y-1])
//				{
//					temp2[x][y-1]=team;
//				}
//				if(check[x+1][y])
//				{
//					temp2[x+1][y]=team;
//				}
//
//				if(check[x-1][y])
//				{
//					temp2[x-1][y]=team;
//				}
//			}
//			catch(Exception INDEXOUTOFBOUNDS)
//			{
//			}
//		}
//		else // if (tier ==3)
//		{
//			if(check[x][y]==true)
//				{
//					try
//					{
//						for(int row=-1; row<2;row++)
//							{
//								for(int col=-1;col<2;col++)
//									{
//										if(row!=0 && col!=0 && check[x+row][y+col]==true && grid[x+row][y+col]!=1)
//											{
//												temp2[x+row][x+col]=team;
//											}
//									}	
//							}
//					}
//					catch (Exception INDEXOUTOFBOUNDS)
//					{
//
//					}
//				}
//		  }
//		int score = CountScore.totalCount(temp,team,0,0);
//		////System.out.println(score);
//		int score2 = CountScore.totalCount(temp2,team,0,0);
//		////System.out.println(score2);
//		return (double)(score2-score);
//	}
	
	private void transfer(int[][]grid,int[][] temp)
	{
		for (int x=0;x<grid.GetLength(0);x++)
		{
			for(int y=0;y<grid[x].GetLength(0);y++)
			{
				temp[x][y]=grid[x][y];
			}
		}
	}
	
	//Returns an array of all adjacent locations.
	//Each location is expressed as row*numCols + col
	public List<int> getAdjacentLocations(int[][] grid, int fromRow, int fromCol)  //works perfectly.  Use debug line above return statement to check
	{
		List<int> locs = new List<int>();
		int numRows = grid.GetLength(0);
		int numCols = grid[0].GetLength(0);

		for (int row = Math.Max(0, fromRow-1); row <= Math.Min(numRows-1, fromRow+1); row++)
		{
			for (int col = Math.Max(0, fromCol-1); col <= Math.Min(numCols-1, fromCol+1); col++)
			{
				if (!(row == fromRow && col == fromCol))
					locs.Add(row*numCols + col);
			}
		}
		////System.out.println("Locs adjacent to (" + fromRow + ", " + fromCol + "):  " + locs.toString());
		return locs;

	}
}
