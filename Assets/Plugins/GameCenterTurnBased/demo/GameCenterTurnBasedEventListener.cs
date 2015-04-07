using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class GameCenterTurnBasedEventListener : MonoBehaviour
{
#if UNITY_IPHONE
	void OnEnable()
	{
		// Listen to all events for illustration purposes
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


	void OnDisable()
	{
		// Remove all event handlers
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

	
	
	void loadMatchDataEvent( Byte[] bytes )
	{
		var theStr = System.Text.UTF8Encoding.UTF8.GetString( bytes );
		Debug.Log( "loadMatchDataEvent: " + theStr );
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
#endif
}


