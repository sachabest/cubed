using UnityEngine;
using System.Collections;

public class GestureTrigger : MonoBehaviour {

	// Use this for initialization
	public GameManager manager;
	void Start () {
	
	}
	void OnDrag(DragGesture gesture) { 
		if (gesture.Phase == ContinuousGesturePhase.Started)
			manager.mouseUI = true;
		else if (gesture.Phase == ContinuousGesturePhase.Ended)
			manager.mouseUI = false;
	}
	void OnPinch(PinchGesture gesture) { 
		if (gesture.Phase == ContinuousGesturePhase.Started)
			manager.mouseUI = true;
		else if (gesture.Phase == ContinuousGesturePhase.Ended)
			manager.mouseUI = false;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
