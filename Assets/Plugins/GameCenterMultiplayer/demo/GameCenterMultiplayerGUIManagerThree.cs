using UnityEngine;
using System.Collections;
using Prime31;


public class GameCenterMultiplayerGUIManagerThree : MonoBehaviourGUI
{
	public GUIText statusText;
	private GCTestObject _testGameObject;
	
#if UNITY_IPHONE
	void Start()
	{
		// always authenticate at every launch
		GameCenterBinding.authenticateLocalPlayer();
		
		// Listen to a few events to help update the GUIText
		GameCenterMultiplayerManager.playerConnected += playerConnected;
		GameCenterMultiplayerManager.playerDisconnected += playerDisconnected;
		
		// grab a reference to our GCTestObject
		var testObjects = FindSceneObjectsOfType( typeof( GCTestObject ) ) as GCTestObject[];
		if( testObjects.Length == 0 )
		{
			Debug.Log( "could not find any GCTestObjects in the scene!" );
			return;
		}
		
		_testGameObject = testObjects[0];
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
			_testGameObject.sendRawBytes();
		}
	
	
		endColumn( true );
		
		
		if( GUILayout.Button( "Find Match (no GUI)" ) )
		{
			GameCenterMultiplayerBinding.findMatchProgrammaticallyWithMinMaxPlayers( 2, 4 );
		}
		
		endColumn();
		
		
		if( bottomLeftButton( "Leaderboard Scene" ) )
		{
			GameCenterMultiplayerBinding.disconnectFromMatch();
			Application.LoadLevel( "GameCenterTestScene" );
		}
	}
#endif
}
