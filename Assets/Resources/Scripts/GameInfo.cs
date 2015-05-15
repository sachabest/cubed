using UnityEngine;
using System.Collections;

public class GameInfo : MonoBehaviour 
{
	private bool singleplayer;
	private int winCondition;
	private PlayerManager.Faction humanFactionChoice = PlayerManager.Faction.Life;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Awake() 
	{
		DontDestroyOnLoad(this.gameObject);
	}
	public void Singleplayer(bool sp)
	{
		singleplayer = sp;
	}
	public bool getSinglePlayer()
	{
		return singleplayer;
	}
	
	public void setWinCondition(int win)
	{
		winCondition = win;
	}
	public int getWinCondition()
	{
		return winCondition;
	}
	public void setHumanFactionChoice(int inputChoice)
	{
		if (inputChoice == 1)
			humanFactionChoice = PlayerManager.Faction.Life;
		if (inputChoice == 2)
			humanFactionChoice = PlayerManager.Faction.Industry;
	}
	public PlayerManager.Faction getHumanFactionChoice()
	{
		return humanFactionChoice;
	}
}
