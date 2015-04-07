using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	static string desiredScore = "Enter Score Here";
	public GameManager mgr;
	public GUISkin skin = new GUISkin();
	
    void OnGUI() 
	{
		// Background Box
		GUILayout.BeginArea(new Rect(Screen.width/2-75,Screen.height/2-75,150,150));
		GUILayout.BeginVertical();
		GUILayout.Box("Main Menu");
		
		// First Button

		if(GUILayout.Button("Play Online"))
		{
			Application.LoadLevel("NetMenu");
		}
		
		if (GUILayout.Button ("Play Local"))
		{
			Application.LoadLevel("cubed");
		}
		
		// Second Button
		if(GUILayout.Button("Options")) 
		{
			Application.LoadLevel("OptionsMenu");
		}
		
		// Third Button
		if (GUILayout.Button ("Tutorial"))
		{
			Application.LoadLevel ("tutorial");
		}
		
		//Desired Score
		desiredScore = GUILayout.TextField (desiredScore,100);
		Debug.Log (desiredScore);
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	public static string setScore()
	{
		return desiredScore;
	}
	
}