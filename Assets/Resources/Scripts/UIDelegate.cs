using UnityEngine;
using System.Collections;

public class UIDelegate : MonoBehaviour {

	public GameManager manager;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	void OnHover(bool isOver) {
		manager.mouseUI = isOver;
		
	}
	void OnPress(bool isDown) {
		manager.mouseUI = isDown;
	}
	void OnMouseDown() {
		manager.mouseUI = true;
	}
	void OnMouseUp() {
		manager.mouseUI = false;	
	}
}
