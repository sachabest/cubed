using UnityEngine;
using System.Collections;

public class WinScreenCube : MonoBehaviour 
{
	public Camera WinCamera;
	public Camera MainCamera;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnMouseDown()
	{
		WinCamera.enabled = false;
		MainCamera.enabled = true;
	}
}
