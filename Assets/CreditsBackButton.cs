using UnityEngine;
using System.Collections;

public class CreditsBackButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Return() {
		if (GCCubedListener.instance != null) {
//			GCCubedListener.instance.LoadLevel("MainMenuV2");
		} else {
			Application.LoadLevel("MainMenuV2");
		}
	}
}
