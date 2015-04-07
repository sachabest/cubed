using UnityEngine;
using System.Collections;

public class WinConditionSelection : MonoBehaviour {
	
	public int myWinCondtion;
	private GameInfo theGameInfo;
	public Material material;
	public Material highlighted;
	// Use this for initialization
	void Start () 
	{
	 	theGameInfo = GameObject.Find ("GameInfo").GetComponent<GameInfo>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	
	public void OnMouseEnter()
	{
		this.renderer.material = highlighted; //(Material)Resources.Load("Materials/Unused Material 1"); ///("Materials/highlight");
	}
	public void OnMouseExit()
	{
		this.renderer.material = material;
	}
	public void OnMouseDown()
	{
		theGameInfo.setWinCondition(myWinCondtion);
		GameObject gameCenter = GameObject.Find ("GameCenter");
		if (gameCenter != null) {
			gameCenter.GetComponent<GCCubedListener>().LoadLevel("cubed");
			return;
		}
		Application.LoadLevel("cubed");
	}
}