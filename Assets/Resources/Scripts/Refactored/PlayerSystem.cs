using UnityEngine;
using System.Collections;

public class PlayerSystem : MonoBehaviour {
	
	private string[] playerNames;
	private int[] playerScores;
	private int numPlayers, winningScore;
	
	public delegate void ScoreChangeEventHandler(GameObject sender, int player, int newScore);
	public event ScoreChangeEventHandler ScoreIncremented;
	public delegate void GameWonEventHandler(int player);
	public event GameWonEventHandler GameWon;
	// Use this for initialization
	void Start () {
	
	}
	public PlayerSystem(int winningScore, int numPlayers, string[] playerNames) {
		if (playerNames.Length != numPlayers)
			throw new UnityException("The number of players and number of names do not match.");
		this.playerNames = playerNames;
		this.numPlayers = numPlayers;
		this.winningScore = winningScore;
		playerScores = new int[numPlayers];
	}
	public PlayerSystem() {
		this.numPlayers = 2;
		this.playerNames = new string[2];
		this.playerNames[0] = "Player 1";
		this.playerNames[1] = "Player 2";
		this.playerScores = new int[2];
		this.winningScore = winningScore;
	}
	public void IncrementeScore(GameObject self, int player, int increment) {
		try {
			playerScores[player] += increment;
		}
		catch (UnityException e) {
			Debug.Log("That player has not been created yet or the value was entered incorrectly");
			throw e;
		}
		ScoreIncremented(self, player, playerScores[player]);
		if (playerScores[player] > winningScore)
			GameWon(player);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
