using UnityEngine;
using System.Collections;


public class GCTestObject : MonoBehaviour
{
	public GUIText statusText;
	private static GCTestObject _instance;
	
	
    void Awake()
    {
		// Ensure the name of the GameObject matches the class name so we can send messages remotely to a known GO
		gameObject.name = this.GetType().ToString();
		_instance = this;
    }
	
	
	public void receiveTime( string data )
	{
		statusText.text = data;
	}
	
	
	// The following methods is used in the GameCenterMultiplayerTestSceneThree scene.
	// This demonstrates how to send and receive a raw byte[]
	public void sendRawBytes()
	{
		// we will just send some text across the wire encoded into a byte array for demonstration purposes
		var theStr = "im a string that will be encoded to a byte array and sent across the wire using GameCenter";
		var bytes = System.Text.UTF8Encoding.UTF8.GetBytes( theStr );
		
		GameCenterMultiplayerBinding.sendRawMessageToAllPeers( "GCTestObject", "receivedRawBytes", bytes, true );
	}
	

	// Notice that this method is static.  It MUST be a static method in order for the plugin to locate the method
	// from outside of the Mono runtime in Obj-C
	public static void receivedRawBytes( byte[] bytes )
	{
		var theStr = System.Text.UTF8Encoding.UTF8.GetString( bytes );
		Debug.Log( "receivedRawBytes: " + theStr );
		
		_instance.statusText.text = theStr;
	}

}
