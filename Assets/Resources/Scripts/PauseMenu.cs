using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {
	
	public bool paused;
	public GUISkin skin;
	public GameManager manager;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.P)) { 
	       	paused = !paused;
		    if (paused) {
		        Time.timeScale = 0.0f;
		    } else {
		        Time.timeScale = 1.0f;
		    }
		}
	}
	void OnGUI() {
		if(!paused)
		{
		   GUILayout.BeginArea(new Rect(200,10,400,20));
		   GUILayout.BeginVertical();
		   GUILayout.BeginHorizontal();
		   GUILayout.FlexibleSpace();
		   GUILayout.Label("Press P to Pause");
		   GUILayout.FlexibleSpace();
		   GUILayout.EndHorizontal();
		   GUILayout.EndVertical();
		   GUILayout.EndArea();
		   return;
		}
		   
		GUIStyle box = "box";   
	    GUILayout.BeginArea(new Rect( Screen.width/2 - 200,Screen.height/2 - 300, 400, 600), box);
	    GUILayout.BeginVertical(); 
	    GUILayout.FlexibleSpace();
	    if(GUILayout.Button("Save Game"))
	    {
	       //manager.saveQuit();
	    }
	    GUILayout.Space(60);
		if (GUILayout.Button ("Load Last Save")) {
			//manager.loadLastSave();
	    } 
	    GUILayout.FlexibleSpace();
	    GUILayout.EndVertical();
	    GUILayout.EndArea();
	}
}
