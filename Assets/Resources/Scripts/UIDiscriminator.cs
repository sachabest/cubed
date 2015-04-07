using UnityEngine;
using System.Collections;

public class UIDiscriminator : MonoBehaviour {
	
	public GameManager manager;
	public GameObject UIRoot;
	// Use this for initialization
	void Start () {
		UIRoot = null;
		if (iPhone.generation == iPhoneGeneration.iPhone4 || iPhone.generation == iPhoneGeneration.iPhone4S) {
			GameObject.Find("UI iPhone 5 / Web").SetActive(false);
			UIRoot = GameObject.Find("UI iPhone 4 / Older");
			
		}
		else {
		//else if (iPhone.generation == iPhoneGeneration.iPhone5) {
			GameObject.Find("UI iPhone 4 / Older").SetActive(false);
			UIRoot = GameObject.Find("UI iPhone 5 / Web");
		}
		
		setupManagerLinks();
	}
	void setupManagerLinks() {
		UILabel[] labels = UIRoot.GetComponentsInChildren<UILabel>();
		foreach (UILabel label in labels) {
			if (label.name.Equals("LeftScore"))
				manager.player1score = label;
			else if (label.name.Equals("RightScore"))
				manager.player2score = label;
			else if (label.name.Equals("RollIndicator"))
				manager.rollDisplay = label;
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
