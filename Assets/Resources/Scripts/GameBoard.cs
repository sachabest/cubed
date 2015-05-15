using UnityEngine;
using System.Collections;
using System;

public class GameBoard : MonoBehaviour 
{
	public int[,] Board;
	private PlayerManager.Faction[,,] PlayableArea;
	public GameSquare[,] GSArray;
	public GameSquare sqr;
	
	public GameManager m;

	// Use this for initialization
	void Start () 
	{
		Board = new int[12,12];
		PlayableArea = new PlayerManager.Faction[12,12,2];
		GSArray = new GameSquare[12,12];
		for(int x = 0; x < 12; x++)
		{
			for(int y = 0; y < 12; y++)
			{
				GameSquare square = (GameSquare)Instantiate(sqr, new Vector3((x*1f)+this.transform.position.x, 0f, (y*1f)+this.transform.position.z), this.transform.rotation);
				square.xval = x;
				square.yval = y;
				square.SetManager(m);
				square.transform.parent = this.transform;
				GSArray[x,y] = square;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void Set(int x, int y, int value)
	{
		Board[x,y] = value;
	}
	public bool Remove(int x, int y, int _tier, PlayerManager.Faction factionRemovingPiece)
	{
		PlayerManager.Faction faction = PlayerManager.SwitchFaction (factionRemovingPiece);  
		if(Board[x,y] != 0)
		{
			Board[x,y] = 0;
			if(_tier==2)
			{
				try
				{
					UnsetPlayable(x+1, y, faction);
					Reserve(x+1, y);                 //just sets the *image* based on the data in PlayableArea
					UnsetPlayable(x-1, y, faction);
					Reserve(x-1, y);
					UnsetPlayable(x, y+1, faction);
					Reserve(x, y+1);
					UnsetPlayable(x, y-1, faction);
					Reserve(x, y-1);
				}
				#pragma warning disable
				catch(Exception e){}
				#pragma warning enable
			
			}
			if(_tier == 3)
			{
				for(int i = x-1; i <= x+1; i++)
				{
					for(int j = y-1; j <= y+1; j++)
					{
						try
						{
							UnsetPlayable(i,j,faction);
							Reserve(i, j);             //just sets the *image* based on the data in PlayableArea
						}
						catch(Exception e)
						{
							continue;
						}
					}
				}
			}
			Debug.Log ("Board.Remove TRUE");
			return true;
		}
		Debug.Log ("Board.Remove FALSE");
			return false;
	}
	public void SetPlayable(int x, int y, PlayerManager.Faction faction)
	{
		PlayableArea[x,y,(int) faction-1]++;
	}
	public void UnsetPlayable(int x, int y, PlayerManager.Faction faction)
	{
		PlayableArea[x,y,(int) faction-1]--;
	}
	public bool GetPlayable(int x, int y, PlayerManager.Faction faction)
	{
		return (PlayableArea[x,y,(int) faction - 1] >= PlayableArea[x,y,(int) PlayerManager.SwitchFaction(faction) - 1])&&Board[x,y]==0;
	}
	public bool[][] PlayableToBoolArray(PlayerManager.Faction faction)
	{
		bool[][] output = new bool[PlayableArea.GetLength(0)][];
		for(int i = 0; i < PlayableArea.GetLength(0); i++)
		{
			output[i] = new bool[PlayableArea.GetLength(1)];
		}
		for(int x = 0; x < PlayableArea.GetLength(0); x++)
		{
			for(int y = 0; y < PlayableArea.GetLength(1); y++)
			{
				output[x][y] = GetPlayable(x, y, faction);
			}
		}
		return output;
	}
	
	//This method actually just sets the *image* for a given square based upon the data in the PlayableArea arrays.
	public void Reserve(int x, int y)
	{
		if(/*!GSArray[x,y].hasPiece() &&*/ Board[x,y] == 0)
		{
			if(PlayableArea[x,y,0] > PlayableArea[x,y,1])
			{
				GSArray[x,y].setMaterial((Material)Resources.Load ("Materials/"+ PlayerManager.Faction.Life +"Reserve"));
			}
			else if(PlayableArea[x,y,0] < PlayableArea[x,y,1])
			{
				GSArray[x,y].setMaterial((Material)Resources.Load ("Materials/"+ PlayerManager.Faction.Industry +"Reserve"));
			}
			else
			{
				GSArray[x,y].setMaterial((Material)Resources.Load ("Materials/transparent"));
			}
		}
	}
}
