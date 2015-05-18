using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;

public class GCCubedListener : MonoBehaviour {
#if UNITY_IPHONE
	
	public static string UNDEF_OPPONENT = "Unknown Opponent";
	public SaveLoadManager saveLoad;
	private bool eventsLoaded;
	public bool gcInUse;
	public GKTurnBasedMatch currentMatch;
	public bool singleplayer;
	public string currentMatchData;

	public static GCCubedListener instance;

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
		instance = this;
	}
	void Start() {
		GameCenterBinding.authenticateLocalPlayer();
		if (GameCenterTurnBasedBinding.isTurnBasedMultiplayerAvailable()) {
			Debug.Log("Listener working");
			Debug.Log("GameCenter working");
			LoadEvents();
			eventsLoaded = true;
			gcInUse = true;
			Handheld.SetActivityIndicatorStyle(iOSActivityIndicatorStyle.Gray);
		}
		else {
			//Debug.Log("Fuck Game Center");
		}
	}

	public PlayerManager.Faction getLocalFaction() {
		if (GameInfo.instance.lifeUser.Equals(localName())) {
			return PlayerManager.Faction.Life;
		} else if (GameInfo.instance.industryUser.Equals(localName())) {
			return PlayerManager.Faction.Industry;
		}
		return PlayerManager.Faction.Uninitialized;
	}
	public string localName() {
		return GameCenterBinding.playerAlias();
	}
	public string opponentName() {
		foreach (GKTurnBasedParticipant p in currentMatch.participants) {
			if (!p.player.alias.Equals(localName())) {
				return p.player.alias;
			}
		}
		return UNDEF_OPPONENT;
	}
	public bool loggedIn() {
		return GameCenterBinding.isPlayerAuthenticated ();
	}
	public bool myTurn() {
		return GameCenterTurnBasedBinding.isCurrentPlayersTurn();
	}
	void OnLevelWasLoaded(int level) {
		if (level == 1 || level == 2) {
			Screen.autorotateToPortrait = false;
			Screen.autorotateToPortraitUpsideDown = false;
		}
		else {
			Screen.autorotateToPortrait = true;
		}
	}
	#region Delegates
	// OnAppllicationQuit() should call this from manager
	public void SaveTurn(string json) {
		GameCenterTurnBasedBinding.saveCurrentTurnWithMatchData(saveLoad.Encode(json));
	}
	public void EndTurn(string json) {
		byte[] submitted = saveLoad.Encode(json);
		string enemy = null;
		for (int count = 0; count < currentMatch.participants.Count; count++) {
			GKTurnBasedParticipant loopPlayer = currentMatch.participants[count];
			if (loopPlayer != null && loopPlayer.playerId != null && currentMatch.currentParticipant != null && currentMatch.currentParticipant.playerId != null
			&& !loopPlayer.playerId.Equals(currentMatch.currentParticipant.playerId)) {
				enemy = loopPlayer.playerId;
				break;
			}
		}
		GameCenterTurnBasedBinding.endTurnWithNextParticipant(enemy, submitted);
		// clear the moves stack so that moves don't bleed over from game to game
		SaveLoadManager.instance.moves = null;
		LoadLevel("MainMenuV2");
		GameCenterTurnBasedBinding.loadMatches();
	}
	public void Win() {
		foreach (GKTurnBasedParticipant p in currentMatch.participants) {
			if (p != null && !p.Equals(GameCenterBinding.playerIdentifier())) {
				GameCenterTurnBasedBinding.setMatchOutcomeForParticipant(GKTurnBasedMatchOutcome.Lost, p.playerId);
			}
		}
		GameCenterTurnBasedBinding.setMatchOutcomeForParticipant(GKTurnBasedMatchOutcome.Won, GameCenterBinding.playerIdentifier());
	}
	public void ShowMatchmaking() {
		if (GameCenterBinding.isPlayerAuthenticated()) {
			GameCenterTurnBasedBinding.loadMatches();
			GameCenterTurnBasedBinding.findMatch(2, 2, true);
		}
		else {
			Debug.Log ("Player not authenticated");
		}
	}
	#endregion
	public void LoadLevel(string level) {
		StartCoroutine(Level(level));
	}
	public IEnumerator Level(string level) {
		EtceteraBinding.showBezelActivityViewWithLabel("Reticulating splines...");
        AsyncOperation load = Application.LoadLevelAsync(level);
		yield return load;
		EtceteraBinding.hideActivityView();
	}
	
	void LoadEvents() {	
		GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFinishedEvent += endMatchInTurnWithMatchDataFinishedEvent;
		GameCenterTurnBasedManager.matchDataLoadedEvent += loadMatchDataEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerDidFindMatchEvent += turnBasedMatchmakerViewControllerDidFindMatchEvent;
	}

	
	#region TurnBasedEvents
	void loadMatchDataEvent(byte[] bytes )
	{
		currentMatchData = System.Text.UTF8Encoding.UTF8.GetString( bytes );
		SaveLoadManager.instance.ParseJSONGameStateString(currentMatchData);
		if (SaveLoadManager.instance.moves.Count > 0) {
			LoadLevel("cubed");
		} else {
			LoadLevel("SinglePlayerOptions");
		}
	}

	void endMatchInTurnWithMatchDataFinishedEvent()
	{
		Debug.Log( "endMatchInTurnWithMatchDataFinishedEvent" );
		LoadLevel("MainMenuV2");
		currentMatch = null;
		// show some kind of victory screen
	}

	void turnBasedMatchmakerViewControllerDidFindMatchEvent( GKTurnBasedMatch match )
	{
		Debug.Log( "turnBasedMatchmakerViewControllerDidFindMatchEvent: " + match );
		currentMatch = match;
		GameCenterTurnBasedBinding.changeCurrentMatch(match.matchId);
		LoadLevel("SinglePlayerOptions");
	}
	#endregion
#endif
}