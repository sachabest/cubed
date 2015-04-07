using UnityEngine;
using System.Collections;

public class MoveSystem<T> : MonoBehaviour {
	
	// Ideally this would be stored as a Stack, but the requirement of replaying a game would render that inefficient
	private IList moves;
	
	public MoveSystem() {
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
