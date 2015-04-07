using UnityEngine;
using System.Collections;
using Prime31;


public class GameCenterMultiplayerGUIManager : MonoBehaviourGUI
{
	public GUIText statusText;

#if UNITY_IPHONE	
	void Start()
	{
		// always authenticate at every launch
		GameCenterBinding.authenticateLocalPlayer();
		
		// Listen to a few events to help update the GUIText
		GameCenterMultiplayerManager.playerConnected += playerConnected;
		GameCenterMultiplayerManager.playerDisconnected += playerDisconnected;
	}
	
	
	void OnDisable()
	{
		GameCenterMultiplayerManager.playerConnected -= playerConnected;
		GameCenterMultiplayerManager.playerDisconnected -= playerDisconnected;
	}
	
	

	#region Event listeners
	
	public void playerConnected( string playerId )
	{
		statusText.text = "player connected: " + playerId;
	}
	
	
	void playerDisconnected( string playerId )
	{
		statusText.text = "player disconnected: " + playerId;
	}
	
	#endregion;
	
	
	void OnGUI()
	{
		beginColumn();

		if( GUILayout.Button( "Show Matchmaker" ) )
		{
			GameCenterMultiplayerBinding.showMatchmakerWithMinMaxPlayers( 2, 4 );
		}
		
		
		if( GUILayout.Button( "Send To All" ) )
		{
			GameCenterMultiplayerBinding.sendMessageToAllPeers( "GCTestObject", "receiveTime", Time.timeSinceLevelLoad.ToString(), false );
		}
		
		
		if( GUILayout.Button( "Disconnect" ) )
		{
			GameCenterMultiplayerBinding.disconnectFromMatch();
		}
		
		
		if( GUILayout.Button( "Show Friend Request (iOS 4.2+)" ) )
		{
			GameCenterMultiplayerBinding.showFriendRequestController();
		}
	
	
		endColumn( true );
		
		
		if( GUILayout.Button( "Find Match (no GUI)" ) )
		{
			GameCenterMultiplayerBinding.findMatchProgrammaticallyWithMinMaxPlayers( 2, 4 );
		}
		
		
		
		if( GUILayout.Button( "Cancel Find Match" ) )
		{
			GameCenterMultiplayerBinding.cancelProgrammaticMatchRequest();
		}
		
		
		if( GUILayout.Button( "Find Activity" ) )
		{
			GameCenterMultiplayerBinding.findAllActivity();
		}
		
		
		if( GUILayout.Button( "Enable Voicechat" ) )
		{
			GameCenterMultiplayerBinding.enableVoiceChat( true );
		}
		
		
		if( GUILayout.Button( "Start Voicechat" ) )
		{
			GameCenterMultiplayerBinding.addAndStartVoiceChatChannel( "testChannel" );
		}
		
		
		if( GUILayout.Button( "Receive VC Updates" ) )
		{
			GameCenterMultiplayerBinding.receiveUpdates( "testChannel", true );
		}
		
		endColumn();
		
		
		if( bottomLeftButton( "Advanced Multiplayer" ) )
		{
			GameCenterMultiplayerBinding.disconnectFromMatch();
			Application.LoadLevel( "GameCenterMultiplayerTestSceneTwo" );
		}
	}
#endif
}
