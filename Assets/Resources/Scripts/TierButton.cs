using UnityEngine;
using System.Collections;

public class TierButton : MonoBehaviour 
{
	public GameManager manager;
	public int tiersetting;
	#pragma warning disable
	bool fmrToggle = false;
	#pragma warning enable
	
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
		manager.SetTier(tiersetting);
	}
	public void OnHover(bool isOver)
	{
		fmrToggle = isOver;
	//	screenPosition = Event.current.mousePosition;
	}
}
