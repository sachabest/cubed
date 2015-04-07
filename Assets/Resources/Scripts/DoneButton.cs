using UnityEngine;
using System.Collections;

public class DoneButton : MonoBehaviour 
{
	public GameManager mgr;
	bool fmrToggle = false;
	Vector3 screenPosition;
	public GUIStyle customStyle = new GUIStyle();

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
		mgr.NextTurn();
		Debug.Log ("Next Turn");
		
	}
	
	public void OnHover(bool isOver)
	{
		fmrToggle = isOver;
	}
	public void OnGUI()
	{
		customStyle.fontSize = 24;
		customStyle.normal.textColor = Color.red;
		
		
		if (fmrToggle == true)
		{
			GUI.Label (new Rect (screenPosition.x,screenPosition.y, 100, 100), "ROLL", customStyle);
		}
	}
}
