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
			// LoadEvents();
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
	/*
	void LoadEvents() {
		// Listens to all the GameCenter events.  All event listeners MUST be removed before this object is disposed!
		// Player events
		GameCenterManager.loadPlayerDataFailedEvent += loadPlayerDataFailed;
		GameCenterManager.playerDataLoadedEvent += playerDataLoaded;
		GameCenterManager.playerAuthenticatedEvent += playerAuthenticated;
		GameCenterManager.playerFailedToAuthenticateEvent += playerFailedToAuthenticate;
		GameCenterManager.playerLoggedOutEvent += playerLoggedOut;
		GameCenterManager.profilePhotoLoadedEvent += profilePhotoLoaded;
		GameCenterManager.profilePhotoFailedEvent += profilePhotoFailed;
		
		// Leaderboards and scores
		GameCenterManager.loadCategoryTitlesFailedEvent += loadCategoryTitlesFailed;
		GameCenterManager.categoriesLoadedEvent += categoriesLoaded;
		GameCenterManager.reportScoreFailedEvent += reportScoreFailed;
		GameCenterManager.reportScoreFinishedEvent += reportScoreFinished;
		GameCenterManager.retrieveScoresFailedEvent += retrieveScoresFailed;
		GameCenterManager.scoresLoadedEvent += scoresLoaded;
		GameCenterManager.retrieveScoresForPlayerIdFailedEvent += retrieveScoresForPlayerIdFailed;
		GameCenterManager.scoresForPlayerIdLoadedEvent += scoresForPlayerIdLoaded;
		
		// Achievements
		GameCenterManager.reportAchievementFailedEvent += reportAchievementFailed;
		GameCenterManager.reportAchievementFinishedEvent += reportAchievementFinished;
		GameCenterManager.loadAchievementsFailedEvent += loadAchievementsFailed;
		GameCenterManager.achievementsLoadedEvent += achievementsLoaded;
		GameCenterManager.resetAchievementsFailedEvent += resetAchievementsFailed;
		GameCenterManager.resetAchievementsFinishedEvent += resetAchievementsFinished;
		GameCenterManager.retrieveAchievementMetadataFailedEvent += retrieveAchievementMetadataFailed;
		GameCenterManager.achievementMetadataLoadedEvent += achievementMetadataLoaded;

		/*
		// Challenges
		GameCenterManager.localPlayerDidSelectChallengeEventEvent += localPlayerDidSelectChallengeEvent;
		GameCenterManager.localPlayerDidCompleteChallengeEventEvent += localPlayerDidCompleteChallengeEvent;
		GameCenterManager.remotePlayerDidCompleteChallengeEventEvent += remotePlayerDidCompleteChallengeEvent;
		GameCenterManager.challengesLoadedEventEvent += challengesLoadedEvent;
		GameCenterManager.challengesFailedToLoadEventEvent += challengesFailedToLoadEvent;
		
		GameCenterTurnBasedManager.loadMatchDataEventEvent += loadMatchDataEvent;
		GameCenterTurnBasedManager.saveCurrentTurnWithMatchDataFinishedEventEvent += saveCurrentTurnWithMatchDataFinishedEvent;
		GameCenterTurnBasedManager.saveCurrentTurnWithMatchDataFailedEventEvent += saveCurrentTurnWithMatchDataFailedEvent;
		GameCenterTurnBasedManager.endTurnWithNextParticipantFailedEventEvent += endTurnWithNextParticipantFailedEvent;
		GameCenterTurnBasedManager.endTurnWithNextParticipantFinishedEventEvent += endTurnWithNextParticipantFinishedEvent;
		GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFailedEventEvent += endMatchInTurnWithMatchDataFailedEvent;
		GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFinishedEventEvent += endMatchInTurnWithMatchDataFinishedEvent;
		GameCenterTurnBasedManager.loadMatchDataFailedEventEvent += loadMatchDataFailedEvent;
		GameCenterTurnBasedManager.participantQuitInTurnFailedEventEvent += participantQuitInTurnFailedEvent;
		GameCenterTurnBasedManager.participantQuitInTurnFinishedEventEvent += participantQuitInTurnFinishedEvent;
		GameCenterTurnBasedManager.participantQuitOutOfTurnFailedEventEvent += participantQuitOutOfTurnFailedEvent;
		GameCenterTurnBasedManager.participantQuitOutOfTurnFinishedEventEvent += participantQuitOutOfTurnFinishedEvent;
		GameCenterTurnBasedManager.findMatchProgramaticallyFailedEventEvent += findMatchProgramaticallyFailedEvent;
		GameCenterTurnBasedManager.findMatchProgramaticallyFinishedEventEvent += findMatchProgramaticallyFinishedEvent;
		GameCenterTurnBasedManager.loadMatchesDidFailEventEvent += loadMatchesDidFailEvent;
		GameCenterTurnBasedManager.loadMatchesDidFinishEventEvent += loadMatchesDidFinishEvent;
		GameCenterTurnBasedManager.removeMatchFailedEventEvent += removeMatchFailedEvent;
		GameCenterTurnBasedManager.removeMatchFinishedEventEvent += removeMatchFinishedEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerWasCancelledEventEvent += turnBasedMatchmakerViewControllerWasCancelledEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerFailedEventEvent += turnBasedMatchmakerViewControllerFailedEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerPlayerQuitEventEvent += turnBasedMatchmakerViewControllerPlayerQuitEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerDidFindMatchEventEvent += turnBasedMatchmakerViewControllerDidFindMatchEvent;
		GameCenterTurnBasedManager.handleTurnEventEventEvent += handleTurnEventEvent;
		GameCenterTurnBasedManager.handleMatchEndedEventEvent += handleMatchEndedEvent;
		GameCenterTurnBasedManager.handleInviteFromGameCenterEventEvent += handleInviteFromGameCenterEvent;
	}

	void OnDisable() {
		if (eventsLoaded) {
			// Remove all the event handlers
			// Player events
			GameCenterManager.loadPlayerDataFailedEvent -= loadPlayerDataFailed;
			GameCenterManager.playerDataLoadedEvent -= playerDataLoaded;
			GameCenterManager.playerAuthenticatedEvent -= playerAuthenticated;
			GameCenterManager.playerLoggedOutEvent -= playerLoggedOut;
			GameCenterManager.profilePhotoLoadedEvent -= profilePhotoLoaded;
			GameCenterManager.profilePhotoFailedEvent -= profilePhotoFailed;
			
			// Leaderboards and scores
			GameCenterManager.loadCategoryTitlesFailedEvent -= loadCategoryTitlesFailed;
			GameCenterManager.categoriesLoadedEvent -= categoriesLoaded;
			GameCenterManager.reportScoreFailedEvent -= reportScoreFailed;
			GameCenterManager.reportScoreFinishedEvent -= reportScoreFinished;
			GameCenterManager.retrieveScoresFailedEvent -= retrieveScoresFailed;
			GameCenterManager.scoresLoadedEvent -= scoresLoaded;
			GameCenterManager.retrieveScoresForPlayerIdFailedEvent -= retrieveScoresForPlayerIdFailed;
			GameCenterManager.scoresForPlayerIdLoadedEvent -= scoresForPlayerIdLoaded;
			
			// Achievements
			GameCenterManager.reportAchievementFailedEvent -= reportAchievementFailed;
			GameCenterManager.reportAchievementFinishedEvent -= reportAchievementFinished;
			GameCenterManager.loadAchievementsFailedEvent -= loadAchievementsFailed;
			GameCenterManager.achievementsLoadedEvent -= achievementsLoaded;
			GameCenterManager.resetAchievementsFailedEvent -= resetAchievementsFailed;
			GameCenterManager.resetAchievementsFinishedEvent -= resetAchievementsFinished;
			GameCenterManager.retrieveAchievementMetadataFailedEvent -= retrieveAchievementMetadataFailed;
			GameCenterManager.achievementMetadataLoadedEvent -= achievementMetadataLoaded;
			/*
			// Challenges
			GameCenterManager.localPlayerDidSelectChallengeEventEvent -= localPlayerDidSelectChallengeEvent;
			GameCenterManager.localPlayerDidCompleteChallengeEventEvent -= localPlayerDidCompleteChallengeEvent;
			GameCenterManager.remotePlayerDidCompleteChallengeEventEvent -= remotePlayerDidCompleteChallengeEvent;
			GameCenterManager.challengesLoadedEventEvent -= challengesLoadedEvent;
			GameCenterManager.challengesFailedToLoadEventEvent -= challengesFailedToLoadEvent;
			
			GameCenterTurnBasedManager.loadMatchDataEventEvent -= loadMatchDataEvent;
			GameCenterTurnBasedManager.saveCurrentTurnWithMatchDataFinishedEventEvent -= saveCurrentTurnWithMatchDataFinishedEvent;
			GameCenterTurnBasedManager.saveCurrentTurnWithMatchDataFailedEventEvent -= saveCurrentTurnWithMatchDataFailedEvent;
			GameCenterTurnBasedManager.endTurnWithNextParticipantFailedEventEvent -= endTurnWithNextParticipantFailedEvent;
			GameCenterTurnBasedManager.endTurnWithNextParticipantFinishedEventEvent -= endTurnWithNextParticipantFinishedEvent;
			GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFailedEventEvent -= endMatchInTurnWithMatchDataFailedEvent;
			GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFinishedEventEvent -= endMatchInTurnWithMatchDataFinishedEvent;
			GameCenterTurnBasedManager.loadMatchDataFailedEventEvent -= loadMatchDataFailedEvent;
			GameCenterTurnBasedManager.participantQuitInTurnFailedEventEvent -= participantQuitInTurnFailedEvent;
			GameCenterTurnBasedManager.participantQuitInTurnFinishedEventEvent -= participantQuitInTurnFinishedEvent;
			GameCenterTurnBasedManager.participantQuitOutOfTurnFailedEventEvent -= participantQuitOutOfTurnFailedEvent;
			GameCenterTurnBasedManager.participantQuitOutOfTurnFinishedEventEvent -= participantQuitOutOfTurnFinishedEvent;
			GameCenterTurnBasedManager.findMatchProgramaticallyFailedEventEvent -= findMatchProgramaticallyFailedEvent;
			GameCenterTurnBasedManager.findMatchProgramaticallyFinishedEventEvent -= findMatchProgramaticallyFinishedEvent;
			GameCenterTurnBasedManager.loadMatchesDidFailEventEvent -= loadMatchesDidFailEvent;
			GameCenterTurnBasedManager.loadMatchesDidFinishEventEvent -= loadMatchesDidFinishEvent;
			GameCenterTurnBasedManager.removeMatchFailedEventEvent -= removeMatchFailedEvent;
			GameCenterTurnBasedManager.removeMatchFinishedEventEvent -= removeMatchFinishedEvent;
			GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerWasCancelledEventEvent -= turnBasedMatchmakerViewControllerWasCancelledEvent;
			GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerFailedEventEvent -= turnBasedMatchmakerViewControllerFailedEvent;
			GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerPlayerQuitEventEvent -= turnBasedMatchmakerViewControllerPlayerQuitEvent;
			GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerDidFindMatchEventEvent -= turnBasedMatchmakerViewControllerDidFindMatchEvent;
			GameCenterTurnBasedManager.handleTurnEventEventEvent -= handleTurnEventEvent;
			GameCenterTurnBasedManager.handleMatchEndedEventEvent -= handleMatchEndedEvent;
			GameCenterTurnBasedManager.handleInviteFromGameCenterEventEvent -= handleInviteFromGameCenterEvent;

		}
	} */
	#region Player Events
	
	void playerAuthenticated()
	{
		Debug.Log( "playerAuthenticated" );
	}
	
	
	void playerFailedToAuthenticate( string error )
	{
		Debug.Log( "playerFailedToAuthenticate: " + error );
	}
	
	
	void playerLoggedOut()
	{
		Debug.Log( "playerLoggedOut" );
	}
	

	void playerDataLoaded( List<GameCenterPlayer> players )
	{
		Debug.Log( "playerDataLoaded" );
		foreach( GameCenterPlayer p in players )
			Debug.Log( p );
	}
	
	
	void loadPlayerDataFailed( string error )
	{
		Debug.Log( "loadPlayerDataFailed: " + error );
	}
	
	
	void profilePhotoLoaded( string path )
	{
		Debug.Log( "profilePhotoLoaded: " + path );
	}
	
	
	void profilePhotoFailed( string error )
	{
		Debug.Log( "profilePhotoFailed: " + error );
	}
	
	#endregion
	#region Leaderboard Events
	
	void categoriesLoaded( List<GameCenterLeaderboard> leaderboards )
	{
		Debug.Log( "categoriesLoaded" );
		foreach( GameCenterLeaderboard l in leaderboards )
			Debug.Log( l );
	}
	
	
	void loadCategoryTitlesFailed( string error )
	{
		Debug.Log( "loadCategoryTitlesFailed: " + error );
	}
	
	#endregion
	#region Score Events
	void scoresLoaded( List<GameCenterScore> scores )
	{
		Debug.Log( "scoresLoaded" );
		foreach( GameCenterScore s in scores )
			Debug.Log( s );
	}
	
	
	void retrieveScoresFailed( string error )
	{
		Debug.Log( "retrieveScoresFailed: " + error );
	}
	
	
	void retrieveScoresForPlayerIdFailed( string error )
	{
		Debug.Log( "retrieveScoresForPlayerIdFailed: " + error );
	}
	
	
	void scoresForPlayerIdLoaded( List<GameCenterScore> scores )
	{
		Debug.Log( "scoresForPlayerIdLoaded" );
		foreach( GameCenterScore s in scores )
			Debug.Log( s );
	}
	
	
	void reportScoreFinished( string category )
	{
		Debug.Log( "reportScoreFinished for category: " + category );
	}
	

	void reportScoreFailed( string error )
	{
		Debug.Log( "reportScoreFailed: " + error );
	}
	
	#endregion
	#region Achievement Events

	void achievementMetadataLoaded( List<GameCenterAchievementMetadata> achievementMetadata )
	{
		Debug.Log( "achievementMetadatLoaded" );
		foreach( GameCenterAchievementMetadata s in achievementMetadata )
			Debug.Log( s );
	}
	
	
	void retrieveAchievementMetadataFailed( string error )
	{
		Debug.Log( "retrieveAchievementMetadataFailed: " + error );
	}
	
	
	void resetAchievementsFinished()
	{
		Debug.Log( "resetAchievmenetsFinished" );
	}
	
	
	void resetAchievementsFailed( string error )
	{
		Debug.Log( "resetAchievementsFailed: " + error );
	}
	
	
	void achievementsLoaded( List<GameCenterAchievement> achievements )
	{
		Debug.Log( "achievementsLoaded" );
		foreach( GameCenterAchievement s in achievements )
			Debug.Log( s );
	}
	

	void loadAchievementsFailed( string error )
	{
		Debug.Log( "loadAchievementsFailed: " + error );
	}
	
	
	void reportAchievementFinished( string identifier )
	{
		Debug.Log( "reportAchievementFinished: " + identifier );
	}
	
	
	void reportAchievementFailed( string error )
	{
		Debug.Log( "reportAchievementFailed: " + error );
	}
	
	#endregion
	#region Challenges
	
	public void localPlayerDidSelectChallengeEvent( GameCenterChallenge challenge )
	{
		Debug.Log( "localPlayerDidSelectChallengeEvent : " + challenge );
	}
	
	
	public void localPlayerDidCompleteChallengeEvent( GameCenterChallenge challenge )
	{
		Debug.Log( "localPlayerDidCompleteChallengeEvent : " + challenge );
	}
	
	
	public void remotePlayerDidCompleteChallengeEvent( GameCenterChallenge challenge )
	{
		Debug.Log( "remotePlayerDidCompleteChallengeEvent : " + challenge );
	}
	
	
	void challengesLoadedEvent( List<GameCenterChallenge> list )
	{
		Debug.Log( "challengesLoadedEvent" );
		Prime31.Utils.logObject( list );
	}
	
	
	void challengesFailedToLoadEvent( string error )
	{
		Debug.Log( "challengesFailedToLoadEvent: " + error );
	}
	
	#endregion
	#region TurnBasedEvents
	void loadMatchDataEvent(byte[] bytes )
	{
		currentMatchData = System.Text.UTF8Encoding.UTF8.GetString( bytes );
		SaveLoadManager.instance.ParseJSONGameStateString(currentMatchData);
		LoadLevel("SinglePlayerOptions");
	}


	void saveCurrentTurnWithMatchDataFinishedEvent()
	{
		Debug.Log( "saveCurrentTurnWithMatchDataFinishedEvent" );
	}
	
	
	void saveCurrentTurnWithMatchDataFailedEvent( string error )
	{
		Debug.Log( "saveCurrentTurnWithMatchDataFailedEvent: " + error );
	}
		

	void endTurnWithNextParticipantFailedEvent( string error )
	{
		Debug.Log( "endTurnWithNextParticipantFailedEvent: " + error );
	}


	void endTurnWithNextParticipantFinishedEvent()
	{
		Debug.Log( "endTurnWithNextParticipantFinishedEvent" );
	}


	void endMatchInTurnWithMatchDataFailedEvent( string error )
	{
		Debug.Log( "endMatchInTurnWithMatchDataFailedEvent: " + error );
	}


	void endMatchInTurnWithMatchDataFinishedEvent()
	{
		Debug.Log( "endMatchInTurnWithMatchDataFinishedEvent" );
		LoadLevel("MainMenuV2");
		currentMatch = null;
		// show some kind of victory screen
	}


	void loadMatchDataFailedEvent( string error )
	{
		Debug.Log( "loadMatchDataFailedEvent: " + error );
	}


	void participantQuitInTurnFailedEvent( string error )
	{
		Debug.Log( "participantQuitInTurnFailedEvent: " + error );
	}


	void participantQuitInTurnFinishedEvent()
	{
		Debug.Log( "participantQuitInTurnFinishedEvent" );
	}


	void participantQuitOutOfTurnFailedEvent( string error )
	{
		Debug.Log( "participantQuitOutOfTurnFailedEvent: " + error );
	}


	void participantQuitOutOfTurnFinishedEvent()
	{
		Debug.Log( "participantQuitOutOfTurnFinishedEvent" );
	}


	void findMatchProgramaticallyFailedEvent( string error )
	{
		Debug.Log( "findMatchProgramaticallyFailedEvent: " + error );
	}


	void findMatchProgramaticallyFinishedEvent( GKTurnBasedMatch match )
	{
		Debug.Log( "findMatchProgramaticallyFinishedEvent: " + match );
	}


	void loadMatchesDidFailEvent( string error )
	{
		Debug.Log( "loadMatchesDidFailEvent: " + error );
	}


	void loadMatchesDidFinishEvent( List<GKTurnBasedMatch> list )
	{
		Debug.Log( "loadMatchesDidFinishEvent" );
		foreach( var item in list )
			Debug.Log( item );
	}


	void removeMatchFailedEvent( string error )
	{
		Debug.Log( "removeMatchFailedEvent: " + error );
	}


	void removeMatchFinishedEvent()
	{
		Debug.Log( "removeMatchFinishedEvent" );
	}


	void turnBasedMatchmakerViewControllerWasCancelledEvent()
	{
		Debug.Log( "turnBasedMatchmakerViewControllerWasCancelledEvent" );
	}


	void turnBasedMatchmakerViewControllerFailedEvent( string error )
	{
		Debug.Log( "turnBasedMatchmakerViewControllerFailedEvent: " + error );
	}


	void turnBasedMatchmakerViewControllerPlayerQuitEvent( GKTurnBasedMatch match )
	{
		Debug.Log( "turnBasedMatchmakerViewControllerPlayerQuitEvent: " + match );
	}


	void turnBasedMatchmakerViewControllerDidFindMatchEvent( GKTurnBasedMatch match )
	{
		Debug.Log( "turnBasedMatchmakerViewControllerDidFindMatchEvent: " + match );
		currentMatch = match;
		GameCenterTurnBasedBinding.changeCurrentMatch(match.matchId);
		LoadLevel("cubed");
	}

	void handleTurnEventEvent( GKTurnBasedMatch match )
	{
		
		Debug.Log( "handleTurnEventEvent: " + match );
	}


	void handleMatchEndedEvent( GKTurnBasedMatch match )
	{
		Debug.Log( "handleMatchEndedEvent: " + match );
	}
	
	
	void handleInviteFromGameCenterEvent( List<object> players )
	{
		Debug.Log( "handleInviteFromGameCenterEvent" );
		Prime31.Utils.logObject( players );
	}
	#endregion
#endif
}