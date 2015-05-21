using UnityEngine;
using System.Collections;

public class GameInfo : MonoBehaviour 
{
	private bool singleplayer;
	private int winCondition;
	public string lifeUser, industryUser;
	private PlayerManager.Faction humanFactionChoice = PlayerManager.Faction.Life;

	public static GameInfo instance;

	// Use this for initialization
	void Start () {
		lifeUser = "Local Player";
		industryUser = "Chong's AI";
		Debug.Log("GameInfo start");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Awake() 
	{
		DontDestroyOnLoad(this.gameObject);
		instance = this;

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
		if (inputChoice == 1) {
			humanFactionChoice = PlayerManager.Faction.Life;
			if (singleplayer) {
				lifeUser = "Local Player";
				industryUser = "Chong's AI";
			} else {
				lifeUser = GCCubedListener.instance.localName();
				industryUser = GCCubedListener.instance.opponentName();
			}
		}
		if (inputChoice == 2) {
			humanFactionChoice = PlayerManager.Faction.Industry;
			if (singleplayer) {
				industryUser = "Local Player";
				lifeUser = "Chong's AI";
			} else {
				lifeUser = GCCubedListener.instance.opponentName();
				industryUser = GCCubedListener.instance.localName();
			}
		}
		Debug.Log("InputChoice: " + inputChoice + " --- Life: " + lifeUser + " --- Industry: " + industryUser);
	}
	public PlayerManager.Faction getHumanFactionChoice()
	{
		return humanFactionChoice;
	}
}
