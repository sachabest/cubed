using UnityEngine;
using System.Collections;

public class FactionOptionSquare : MonoBehaviour 
{
	public int humanFactionChoice;
	public Vector3 startLocation;
	public Vector3 activeLocation;
	public bool isSelected = false;
	public FactionOptionSquare theOtherOption;
	public int duration;
	
	private GameInfo theGameInfo;
	public Material material;
	public Material highlighted;
	// Use this for initialization
	void Start () 
	{
	 	theGameInfo = GameObject.Find ("GameInfo").GetComponent<GameInfo>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (isSelected && !this.transform.position.Equals(activeLocation))
			this.transform.position = Vector3.Lerp(this.transform.position, activeLocation, Time.deltaTime/duration);
		else if (!isSelected && !this.transform.position.Equals (startLocation))
			this.transform.position = Vector3.Lerp (this.transform.position, startLocation, Time.deltaTime/duration);
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
		theOtherOption.isSelected = false;
		this.isSelected = true;
		theGameInfo.setHumanFactionChoice(humanFactionChoice);
		//theGameInfo.setHumanFactionChoice(myWinCondtion);
	}
}