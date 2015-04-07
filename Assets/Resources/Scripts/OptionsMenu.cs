using UnityEngine;
using System.Collections;

public class OptionsMenu : MonoBehaviour 
{
	void OnGUI() 
	{
			// Background Box
			GUILayout.BeginArea(new Rect(Screen.width/2-75,Screen.height/2-75,150,150));
			GUILayout.Box ("Options", GUILayout.Width(150), GUILayout.Height(150));
			GUILayout.EndArea();
			
			// First Button
			GUILayout.BeginArea(new Rect(Screen.width/2-40,Screen.height/2-45,80,20));
			GUILayout.HorizontalSlider(100,0,200);
			GUILayout.EndArea();
			
			
			GUILayout.BeginArea(new Rect (Screen.width/2-40, Screen.height/2-15, 80, 20));
			if (GUILayout.Button ("Play Local"))
			{
				Application.LoadLevel("cubed");
			}
			GUILayout.EndArea();
			
			// Second Button
			GUILayout.BeginArea (new Rect (Screen.width/2-40, Screen.height/2+15, 80,20));
			if(GUILayout.Button("Options")) 
			{
				Application.LoadLevel("options");
			}
			GUILayout.EndArea();
			
			// Third Button
			GUILayout.BeginArea (new Rect (Screen.width/2-40, Screen.height/2+45, 80, 20));
			if (GUILayout.Button ("Tutorial"))
			{
				Application.LoadLevel ("tutorial");
			}
			GUILayout.EndArea();
	
		}
}