using UnityEngine;
using System.Collections;

public class RollDisplay : MonoBehaviour 
{
	private int number;

	// Use this for initialization
	void Start () 
	{
		number = 0;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.renderer.material = (Material)Resources.Load("Materials/" + number);
	
	}
	public void Set(int n)
	{
		number = n;
	}
}
