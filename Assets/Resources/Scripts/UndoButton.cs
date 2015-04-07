using UnityEngine;
using System.Collections;

public class UndoButton : MonoBehaviour 
{
	public GameManager mgr;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	public void OnClick()
	{
		mgr.Undo();
	}
}
