using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;



public class GameCenterTurnBasedManager : AbstractManager
{
#if UNITY_IPHONE
	// Fired when the match data has been retrieved.  This will happen automatically whenever a new match is chosen from the picker
	public static event Action<byte[]> loadMatchDataEvent;
	
	// Fired when saving match data succeeds
	public static event Action saveCurrentTurnWithMatchDataFinishedEvent;
	
	// Fired when saving match data fails
	public static event Action<string> saveCurrentTurnWithMatchDataFailedEvent;
	
	// Fired when the call to endTurnWithNextParticipant fails.  Could occur due to: Communications problem, it is
	// not current participant's turn, Session is closed or other reasons
	public static event Action<string> endTurnWithNextParticipantFailedEvent;
	
	// Fired when the call to endTurnWithNextParticipantFinished completes successfully
	public static event Action endTurnWithNextParticipantFinishedEvent;
	
	// Fired when the call to endMatchInTurnWithMatchData fails
	public static event Action<string> endMatchInTurnWithMatchDataFailedEvent;
	
	// Fired when the call to endMatchInTurnWithMatchData completes successfully
	public static event Action endMatchInTurnWithMatchDataFinishedEvent;
	
	// Fired when loading the current match's data fails
	public static event Action<string> loadMatchDataFailedEvent;
	
	// Fired when the call to participantQuitInTurn fails
	public static event Action<string> participantQuitInTurnFailedEvent;
	
	// Fired when the call to participantQuitInTurn completes successfully
	public static event Action participantQuitInTurnFinishedEvent;
	
	// Fired when the call to participantQuitOutOfTurn fails
	public static event Action<string> participantQuitOutOfTurnFailedEvent;
	
	// Fired when the call to participantQuitOutOfTurn completes successfully
	public static event Action participantQuitOutOfTurnFinishedEvent;
	
	// Fired when the call to findMatchProgramatically fails
	public static event Action<string> findMatchProgramaticallyFailedEvent;
	
	// Fired when the call to findMatchProgramatically completes successfully
	public static event Action<GKTurnBasedMatch> findMatchProgramaticallyFinishedEvent;
	
	// Fired when the call to loadMatches fails
	public static event Action<string> loadMatchesDidFailEvent;
	
	// Fired when the call to loadMatches completes successfully
	public static event Action<List<GKTurnBasedMatch>> loadMatchesDidFinishEvent;
	
	// Fired when the call to removeCurrentMatch fails
	public static event Action<string> removeMatchFailedEvent;
	
	// Fired when the call to removeCurrentMatch completes successfully
	public static event Action removeMatchFinishedEvent;
	
	// Fired when the matchmaker is cancelled
	public static event Action turnBasedMatchmakerViewControllerWasCancelledEvent;
	
	// Fired when the matchmaker fails to create a match
	public static event Action<string> turnBasedMatchmakerViewControllerFailedEvent;
	
	// Fired when a user quits the match from the matchmaker
	public static event Action<GKTurnBasedMatch> turnBasedMatchmakerViewControllerPlayerQuitEvent;
	
	// Fired when the matchmaker successfully creates a match
	public static event Action<GKTurnBasedMatch> turnBasedMatchmakerViewControllerDidFindMatchEvent;
	
	// Fired when in any of the matches a player is currently partaking in a player completes their turn
	public static event Action<GKTurnBasedMatch> handleTurnEventEvent;
	
	// Fired when a match ends.  When you receive this event you should display the match’s final results to the player and allow
	// the player the option of saving or removing the match data from Game Center
	public static event Action<GKTurnBasedMatch> handleMatchEndedEvent;
	
	// Fired when the handleInviteFromGameCenter delegate method is called signifying an invite was received. Includes all players in the invite
	public static event Action<List<object>> handleInviteFromGameCenterEvent;



	static GameCenterTurnBasedManager()
	{
		AbstractManager.initialize( typeof( GameCenterTurnBasedManager ) );
	}
	
	
	public static void loadMatchDataFinished( byte[] bytes )
	{
		if( loadMatchDataEvent != null )
			loadMatchDataEvent( bytes );
	}
	
	
	public void saveCurrentTurnWithMatchDataFinished( string empty )
	{
		if( saveCurrentTurnWithMatchDataFinishedEvent != null )
			saveCurrentTurnWithMatchDataFinishedEvent();
	}


	public void saveCurrentTurnWithMatchDataFailed( string error )
	{
		if( saveCurrentTurnWithMatchDataFailedEvent != null )
			saveCurrentTurnWithMatchDataFailedEvent( error );
	}
		

	public void endTurnWithNextParticipantFailed( string error )
	{
		if( endTurnWithNextParticipantFailedEvent != null )
			endTurnWithNextParticipantFailedEvent( error );
	}


	public void endTurnWithNextParticipantFinished( string empty )
	{
		if( endTurnWithNextParticipantFinishedEvent != null )
			endTurnWithNextParticipantFinishedEvent();
	}


	public void endMatchInTurnWithMatchDataFailed( string error )
	{
		if( endMatchInTurnWithMatchDataFailedEvent != null )
			endMatchInTurnWithMatchDataFailedEvent( error );
	}


	public void endMatchInTurnWithMatchDataFinished( string empty )
	{
		if( endMatchInTurnWithMatchDataFinishedEvent != null )
			endMatchInTurnWithMatchDataFinishedEvent();
	}


	public void loadMatchDataFailed( string error )
	{
		if( loadMatchDataFailedEvent != null )
			loadMatchDataFailedEvent( error );
	}


	public void participantQuitInTurnFailed( string error )
	{
		if( participantQuitInTurnFailedEvent != null )
			participantQuitInTurnFailedEvent( error );
	}


	public void participantQuitInTurnFinished( string empty )
	{
		if( participantQuitInTurnFinishedEvent != null )
			participantQuitInTurnFinishedEvent();
	}


	public void participantQuitOutOfTurnFailed( string error )
	{
		if( participantQuitOutOfTurnFailedEvent != null )
			participantQuitOutOfTurnFailedEvent( error );
	}


	public void participantQuitOutOfTurnFinished( string empty )
	{
		if( participantQuitOutOfTurnFinishedEvent != null )
			participantQuitOutOfTurnFinishedEvent();
	}


	public void findMatchProgramaticallyFailed( string error )
	{
		if( findMatchProgramaticallyFailedEvent != null )
			findMatchProgramaticallyFailedEvent( error );
	}


	public void findMatchProgramaticallyFinished( string json )
	{
		if( findMatchProgramaticallyFinishedEvent != null )
			findMatchProgramaticallyFinishedEvent( new GKTurnBasedMatch( json.dictionaryFromJson() ) );
	}


	public void loadMatchesDidFail( string error )
	{
		if( loadMatchesDidFailEvent != null )
			loadMatchesDidFailEvent( error );
	}


	public void loadMatchesDidFinish( string json )
	{
		if( loadMatchesDidFinishEvent != null )
			loadMatchesDidFinishEvent( GKTurnBasedMatch.fromJSON( json ) );
	}


	public void removeMatchFailed( string error )
	{
		if( removeMatchFailedEvent != null )
			removeMatchFailedEvent( error );
	}


	public void removeMatchFinished( string empty )
	{
		if( removeMatchFinishedEvent != null )
			removeMatchFinishedEvent();
	}


	public void turnBasedMatchmakerViewControllerWasCancelled( string empty )
	{
		if( turnBasedMatchmakerViewControllerWasCancelledEvent != null )
			turnBasedMatchmakerViewControllerWasCancelledEvent();
	}


	public void turnBasedMatchmakerViewControllerFailed( string error )
	{
		if( turnBasedMatchmakerViewControllerFailedEvent != null )
			turnBasedMatchmakerViewControllerFailedEvent( error );
	}


	public void turnBasedMatchmakerViewControllerPlayerQuit( string json )
	{
		if( turnBasedMatchmakerViewControllerPlayerQuitEvent != null )
			turnBasedMatchmakerViewControllerPlayerQuitEvent( new GKTurnBasedMatch( json.dictionaryFromJson() ) );
	}


	public void turnBasedMatchmakerViewControllerDidFindMatch( string json )
	{
		if( turnBasedMatchmakerViewControllerDidFindMatchEvent != null )
			turnBasedMatchmakerViewControllerDidFindMatchEvent( new GKTurnBasedMatch( json.dictionaryFromJson() ) );
	}


	public void handleTurnEvent( string json )
	{
		if( handleTurnEventEvent != null )
			handleTurnEventEvent( new GKTurnBasedMatch( json.dictionaryFromJson() ) );
	}


	public void handleMatchEnded( string json )
	{
		if( handleMatchEndedEvent != null )
			handleMatchEndedEvent( new GKTurnBasedMatch( json.dictionaryFromJson() ) );
	}
	
	
	public void handleInviteFromGameCenter( string json )
	{
		if( handleInviteFromGameCenterEvent != null )
			handleInviteFromGameCenterEvent( json.listFromJson() );
	}
	
#endif
}

