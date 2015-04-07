using UnityEngine;
using System;
using System.Collections.Generic;
using Prime31;


public class GameCenterMultiplayerManager : AbstractManager
{
#if UNITY_IPHONE
	// Fired when the matchmaker is cancelled by the user
	public static event Action matchmakerCancelled;
	
	// Fired when the matchmaker fails to find a match
	public static event Action<string> matchmakerFailedEvent;
	
	// Fired when the matchmaker finds a match
	public static event Action<int> matchmakerFoundMatch;
	
	// Fired when the friend request controller is dismissed
	public static event Action friendRequestControllerFinished;
	
	// Fired when a player connects to a match
	public static event Action<string> playerConnected;
	
	// Fired when a player disconnects to a match
	public static event Action<string> playerDisconnected;
	
	// Fired when connected to a player fails
	public static event Action<string> playerConnectionFailed;
	
	// Fired when either an invite request is received either from another player or initiated by the current player via GameCenter.app
	public static event Action inviteRequestWasReceived;
	
	// Fired when a player is speaking if receiveUpdates is true for the channel
	public static event Action<string> playerBeganSpeaking;
	
	// Fired when a player goes silent if receiveUpdates is true for the channel
	public static event Action<string> playerStoppedSpeaking;
	
	// Fired when a programmatic match fails to connect
	public static event Action<string> findMatchFailed;
	
	// Fired when a programmatic find match connects
	public static event Action<int> findMatchFinished;
	
	// Fired when adding players to a current match fails
	public static event Action<string> addPlayersToMatchFailed;
	
	// Fired when finding current activity for your game fails
	public static event Action<string> findActivityFailed;
	
	// Fired when finding activity returns successfully
	public static event Action<int> findActivityFinished;
	
	// Fired when finding current activity for a group fails
	public static event Action<string> findActivityForGroupFailed;
	
	// Fired when finding activity for a group returns successfully
	public static event Action<int> findActivityForGroupFinished;	
	
	// Fired when retrieving scores fails
	public static event Action<string> retrieveMatchesBestScoresFailed;
	
	// Fired when retrieving scores finishes successfully
	public static event Action<List<GameCenterScore>> retrieveMathesBestScoresFinished;
	
	

    static GameCenterMultiplayerManager()
    {
		AbstractManager.initialize( typeof( GameCenterMultiplayerManager ) );
    }
	
	
	
	#region Standard Matchmaking
	
	public void matchmakerWasCancelled( string empty )
	{
		if( matchmakerCancelled != null )
			matchmakerCancelled();
	}
	
	
	public void matchmakerFailed( string error )
	{
		if( matchmakerFailedEvent != null )
			matchmakerFailedEvent( error );
	}
	
	
	public void matchmakerFoundMatchWithExpectedPlayerCount( string playerCount )
	{
		int expectedPlayerCount = int.Parse( playerCount );
		if( matchmakerFoundMatch != null )
			matchmakerFoundMatch( expectedPlayerCount );
	}
	
	
	public void friendRequestComposeViewControllerDidFinish( string empty )
	{
		if( friendRequestControllerFinished != null )
			friendRequestControllerFinished();
	}

		
	public void playerDidConnectToMatch( string playerId )
	{
		if( playerConnected != null )
			playerConnected( playerId );
	}
	
	
	public void playerDidDisconnectFromMatch( string playerId )
	{
		if( playerDisconnected != null )
			playerDisconnected( playerId );
	}
	
	
	public void connectionWithPlayerFailed( string playerId )
	{
		if( playerConnectionFailed != null )
			playerConnectionFailed( playerId );
	}
	
	
	public void inviteRequestReceived( string emtpy )
	{
		if( inviteRequestWasReceived != null )
			inviteRequestWasReceived();
	}
	
	#endregion;

	
	#region Voice Chat Events
	
	public void playerIsSpeaking( string playerId )
	{
		if( playerBeganSpeaking != null )
			playerBeganSpeaking( playerId );
	}
	
	
	public void playerIsSilent( string playerId )
	{
		if( playerStoppedSpeaking != null )
			playerStoppedSpeaking( playerId );
	}
	
	#endregion;
	
	
	#region Programmatic Matchmaking Events
	
	public void findMatchProgramaticallyFailed( string error )
	{
		if( findMatchFailed != null )
			findMatchFailed( error );
	}
	
	
	public void findMatchProgramaticallyFinishedWithExpectedPlayerCount( string playerCount )
	{
		int expectedPlayerCount = int.Parse( playerCount );
		if( findMatchFinished != null )
			findMatchFinished( expectedPlayerCount );
	}
	
	
	public void addPlayersToCurrentMatchFailed( string error )
	{
		if( addPlayersToMatchFailed != null )
			addPlayersToMatchFailed( error );
	}
	
	
	public void findAllActivityFailed( string error )
	{
		if( findActivityFailed != null )
			findActivityFailed( error );
	}
	
	
	public void findAllActivityFinished( string activity )
	{
		int activityCount = int.Parse( activity );
		if( findActivityFinished != null )
			findActivityFinished( activityCount );
	}
	
	
	public void findAllActivityForPlayerGroupFailed( string error )
	{
		if( findActivityForGroupFailed != null )
			findActivityForGroupFailed( error );
	}
	
	
	public void findAllActivityForPlayerGroupFinished( string activity )
	{
		int activityCount = int.Parse( activity );
		if( findActivityForGroupFinished != null )
			findActivityForGroupFinished( activityCount );
	}
	
	
	public void retrieveMatchesBestScoresDidFail( string error )
	{
		if( retrieveMatchesBestScoresFailed != null )
			retrieveMatchesBestScoresFailed( error );
	}
	
	
	public void retrieveMatchesBestScoresDidFinish( string jsonScores )
	{
		List<GameCenterScore> scores = GameCenterScore.fromJSON( jsonScores );
		
		if( retrieveMathesBestScoresFinished != null )
			retrieveMathesBestScoresFinished( scores );
	}
	
	#endregion;

#endif
}
