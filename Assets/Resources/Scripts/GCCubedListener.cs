using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GCCubedListener : MonoBehaviour
{
#if UNITY_IPHONE
	
	public SaveLoadManager saveLoad;
	private bool eventsLoaded;
	public bool gcInUse;
	public GKTurnBasedMatch currentMatch;
	public bool singleplayer;
	public string currentMatchData;
	
	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}
	void Start() {
		Debug.Log("Listener working");
		if (GameCenterBinding.isGameCenterAvailable()) {
			Debug.Log("GameCenter working");
			GameCenterBinding.authenticateLocalPlayer();
			LoadEvents();
			eventsLoaded = true;
			gcInUse = true;
		}
		Handheld.SetActivityIndicatorStyle(iOSActivityIndicatorStyle.Gray);
	//	else
			//Debug.Log("Fuck Game Center");
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
		EtceteraBinding.showBezelActivityViewWithLabel("Loading..." + level);
        AsyncOperation load = Application.LoadLevelAsync(level);
		yield return load;
		EtceteraBinding.hideActivityView();
	}
	void LoadEvents() {
		// Listens to all the GameCenter events.  All event listeners MUST be removed before this object is disposed!
		// Player events
		GameCenterManager.loadPlayerDataFailed += loadPlayerDataFailed;
		GameCenterManager.playerDataLoaded += playerDataLoaded;
		GameCenterManager.playerAuthenticated += playerAuthenticated;
		GameCenterManager.playerFailedToAuthenticate += playerFailedToAuthenticate;
		GameCenterManager.playerLoggedOut += playerLoggedOut;
		GameCenterManager.profilePhotoLoaded += profilePhotoLoaded;
		GameCenterManager.profilePhotoFailed += profilePhotoFailed;
		
		// Leaderboards and scores
		GameCenterManager.loadCategoryTitlesFailed += loadCategoryTitlesFailed;
		GameCenterManager.categoriesLoaded += categoriesLoaded;
		GameCenterManager.reportScoreFailed += reportScoreFailed;
		GameCenterManager.reportScoreFinished += reportScoreFinished;
		GameCenterManager.retrieveScoresFailed += retrieveScoresFailed;
		GameCenterManager.scoresLoaded += scoresLoaded;
		GameCenterManager.retrieveScoresForPlayerIdFailed += retrieveScoresForPlayerIdFailed;
		GameCenterManager.scoresForPlayerIdLoaded += scoresForPlayerIdLoaded;
		
		// Achievements
		GameCenterManager.reportAchievementFailed += reportAchievementFailed;
		GameCenterManager.reportAchievementFinished += reportAchievementFinished;
		GameCenterManager.loadAchievementsFailed += loadAchievementsFailed;
		GameCenterManager.achievementsLoaded += achievementsLoaded;
		GameCenterManager.resetAchievementsFailed += resetAchievementsFailed;
		GameCenterManager.resetAchievementsFinished += resetAchievementsFinished;
		GameCenterManager.retrieveAchievementMetadataFailed += retrieveAchievementMetadataFailed;
		GameCenterManager.achievementMetadataLoaded += achievementMetadataLoaded;
		
		// Challenges
		GameCenterManager.localPlayerDidSelectChallengeEvent += localPlayerDidSelectChallengeEvent;
		GameCenterManager.localPlayerDidCompleteChallengeEvent += localPlayerDidCompleteChallengeEvent;
		GameCenterManager.remotePlayerDidCompleteChallengeEvent += remotePlayerDidCompleteChallengeEvent;
		GameCenterManager.challengesLoadedEvent += challengesLoadedEvent;
		GameCenterManager.challengesFailedToLoadEvent += challengesFailedToLoadEvent;
		
		GameCenterTurnBasedManager.loadMatchDataEvent += loadMatchDataEvent;
		GameCenterTurnBasedManager.saveCurrentTurnWithMatchDataFinishedEvent += saveCurrentTurnWithMatchDataFinishedEvent;
		GameCenterTurnBasedManager.saveCurrentTurnWithMatchDataFailedEvent += saveCurrentTurnWithMatchDataFailedEvent;
		GameCenterTurnBasedManager.endTurnWithNextParticipantFailedEvent += endTurnWithNextParticipantFailedEvent;
		GameCenterTurnBasedManager.endTurnWithNextParticipantFinishedEvent += endTurnWithNextParticipantFinishedEvent;
		GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFailedEvent += endMatchInTurnWithMatchDataFailedEvent;
		GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFinishedEvent += endMatchInTurnWithMatchDataFinishedEvent;
		GameCenterTurnBasedManager.loadMatchDataFailedEvent += loadMatchDataFailedEvent;
		GameCenterTurnBasedManager.participantQuitInTurnFailedEvent += participantQuitInTurnFailedEvent;
		GameCenterTurnBasedManager.participantQuitInTurnFinishedEvent += participantQuitInTurnFinishedEvent;
		GameCenterTurnBasedManager.participantQuitOutOfTurnFailedEvent += participantQuitOutOfTurnFailedEvent;
		GameCenterTurnBasedManager.participantQuitOutOfTurnFinishedEvent += participantQuitOutOfTurnFinishedEvent;
		GameCenterTurnBasedManager.findMatchProgramaticallyFailedEvent += findMatchProgramaticallyFailedEvent;
		GameCenterTurnBasedManager.findMatchProgramaticallyFinishedEvent += findMatchProgramaticallyFinishedEvent;
		GameCenterTurnBasedManager.loadMatchesDidFailEvent += loadMatchesDidFailEvent;
		GameCenterTurnBasedManager.loadMatchesDidFinishEvent += loadMatchesDidFinishEvent;
		GameCenterTurnBasedManager.removeMatchFailedEvent += removeMatchFailedEvent;
		GameCenterTurnBasedManager.removeMatchFinishedEvent += removeMatchFinishedEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerWasCancelledEvent += turnBasedMatchmakerViewControllerWasCancelledEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerFailedEvent += turnBasedMatchmakerViewControllerFailedEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerPlayerQuitEvent += turnBasedMatchmakerViewControllerPlayerQuitEvent;
		GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerDidFindMatchEvent += turnBasedMatchmakerViewControllerDidFindMatchEvent;
		GameCenterTurnBasedManager.handleTurnEventEvent += handleTurnEventEvent;
		GameCenterTurnBasedManager.handleMatchEndedEvent += handleMatchEndedEvent;
		GameCenterTurnBasedManager.handleInviteFromGameCenterEvent += handleInviteFromGameCenterEvent;
	}
	void OnDisable() {
		if (eventsLoaded) {
			// Remove all the event handlers
			// Player events
			GameCenterManager.loadPlayerDataFailed -= loadPlayerDataFailed;
			GameCenterManager.playerDataLoaded -= playerDataLoaded;
			GameCenterManager.playerAuthenticated -= playerAuthenticated;
			GameCenterManager.playerLoggedOut -= playerLoggedOut;
			GameCenterManager.profilePhotoLoaded -= profilePhotoLoaded;
			GameCenterManager.profilePhotoFailed -= profilePhotoFailed;
			
			// Leaderboards and scores
			GameCenterManager.loadCategoryTitlesFailed -= loadCategoryTitlesFailed;
			GameCenterManager.categoriesLoaded -= categoriesLoaded;
			GameCenterManager.reportScoreFailed -= reportScoreFailed;
			GameCenterManager.reportScoreFinished -= reportScoreFinished;
			GameCenterManager.retrieveScoresFailed -= retrieveScoresFailed;
			GameCenterManager.scoresLoaded -= scoresLoaded;
			GameCenterManager.retrieveScoresForPlayerIdFailed -= retrieveScoresForPlayerIdFailed;
			GameCenterManager.scoresForPlayerIdLoaded -= scoresForPlayerIdLoaded;
			
			// Achievements
			GameCenterManager.reportAchievementFailed -= reportAchievementFailed;
			GameCenterManager.reportAchievementFinished -= reportAchievementFinished;
			GameCenterManager.loadAchievementsFailed -= loadAchievementsFailed;
			GameCenterManager.achievementsLoaded -= achievementsLoaded;
			GameCenterManager.resetAchievementsFailed -= resetAchievementsFailed;
			GameCenterManager.resetAchievementsFinished -= resetAchievementsFinished;
			GameCenterManager.retrieveAchievementMetadataFailed -= retrieveAchievementMetadataFailed;
			GameCenterManager.achievementMetadataLoaded -= achievementMetadataLoaded;
			
			// Challenges
			GameCenterManager.localPlayerDidSelectChallengeEvent -= localPlayerDidSelectChallengeEvent;
			GameCenterManager.localPlayerDidCompleteChallengeEvent -= localPlayerDidCompleteChallengeEvent;
			GameCenterManager.remotePlayerDidCompleteChallengeEvent -= remotePlayerDidCompleteChallengeEvent;
			GameCenterManager.challengesLoadedEvent -= challengesLoadedEvent;
			GameCenterManager.challengesFailedToLoadEvent -= challengesFailedToLoadEvent;
			
			GameCenterTurnBasedManager.loadMatchDataEvent -= loadMatchDataEvent;
			GameCenterTurnBasedManager.saveCurrentTurnWithMatchDataFinishedEvent -= saveCurrentTurnWithMatchDataFinishedEvent;
			GameCenterTurnBasedManager.saveCurrentTurnWithMatchDataFailedEvent -= saveCurrentTurnWithMatchDataFailedEvent;
			GameCenterTurnBasedManager.endTurnWithNextParticipantFailedEvent -= endTurnWithNextParticipantFailedEvent;
			GameCenterTurnBasedManager.endTurnWithNextParticipantFinishedEvent -= endTurnWithNextParticipantFinishedEvent;
			GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFailedEvent -= endMatchInTurnWithMatchDataFailedEvent;
			GameCenterTurnBasedManager.endMatchInTurnWithMatchDataFinishedEvent -= endMatchInTurnWithMatchDataFinishedEvent;
			GameCenterTurnBasedManager.loadMatchDataFailedEvent -= loadMatchDataFailedEvent;
			GameCenterTurnBasedManager.participantQuitInTurnFailedEvent -= participantQuitInTurnFailedEvent;
			GameCenterTurnBasedManager.participantQuitInTurnFinishedEvent -= participantQuitInTurnFinishedEvent;
			GameCenterTurnBasedManager.participantQuitOutOfTurnFailedEvent -= participantQuitOutOfTurnFailedEvent;
			GameCenterTurnBasedManager.participantQuitOutOfTurnFinishedEvent -= participantQuitOutOfTurnFinishedEvent;
			GameCenterTurnBasedManager.findMatchProgramaticallyFailedEvent -= findMatchProgramaticallyFailedEvent;
			GameCenterTurnBasedManager.findMatchProgramaticallyFinishedEvent -= findMatchProgramaticallyFinishedEvent;
			GameCenterTurnBasedManager.loadMatchesDidFailEvent -= loadMatchesDidFailEvent;
			GameCenterTurnBasedManager.loadMatchesDidFinishEvent -= loadMatchesDidFinishEvent;
			GameCenterTurnBasedManager.removeMatchFailedEvent -= removeMatchFailedEvent;
			GameCenterTurnBasedManager.removeMatchFinishedEvent -= removeMatchFinishedEvent;
			GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerWasCancelledEvent -= turnBasedMatchmakerViewControllerWasCancelledEvent;
			GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerFailedEvent -= turnBasedMatchmakerViewControllerFailedEvent;
			GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerPlayerQuitEvent -= turnBasedMatchmakerViewControllerPlayerQuitEvent;
			GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerDidFindMatchEvent -= turnBasedMatchmakerViewControllerDidFindMatchEvent;
			GameCenterTurnBasedManager.handleTurnEventEvent -= handleTurnEventEvent;
			GameCenterTurnBasedManager.handleMatchEndedEvent -= handleMatchEndedEvent;
			GameCenterTurnBasedManager.handleInviteFromGameCenterEvent -= handleInviteFromGameCenterEvent;
		}
	}
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
		
		LoadLevel("cubed");
		Debug.Log( "loadMatchDataEvent: " + currentMatchData );
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