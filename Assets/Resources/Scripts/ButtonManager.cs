using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

	public Text lifeName, lifeScore, industryName, industryScore, rollDisplay, rollButtonText;
	public static ButtonManager instance;
	
	// Use this for initialization
	void Awake () {
		instance = this;
		lifeScore.text = "0" + "/" + GameManager.instance.winningScore;
		industryScore.text = "0" + "/" + GameManager.instance.winningScore;
		rollDisplay.text = "0";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Undo()
	{
		GameManager.instance.Undo();
	}
	public void Tier1()
	{
		GameManager.instance.SetTier(1);
	}
	public void Tier2()
	{
		GameManager.instance.SetTier(2);
	}
	public void Tier3()
	{
		GameManager.instance.SetTier(3);
	}
	public void Turn() 
	{
		GameManager.instance.NextTurn();
	}
}
