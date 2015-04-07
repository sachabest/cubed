using UnityEngine;
using System.Collections;

public class PlayerIndicator : MonoBehaviour 
{
	public int player;
	private Vector3 Player1pos;
	private Vector3 Player2pos;
	private Material p1Material;
	private Material p2Material;
	public GameManager m;
	// Use this for initialization
	void Start () 
	{
		//p1Material = (Material)Resources.Load ("Materials/"+m.getPlayerFaction(1)+"Reserve");
		//p2Material = (Material)Resources.Load ("Materials/"+m.getPlayerFaction(2)+"Reserve");
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	public void Set(int p)
	{
		player = p;
		if(p==1)
		{
			this.renderer.material = (Material)Resources.Load ("Materials/"+m.getPlayerFaction(1)+"Reserve");
		}
		else if(p==2)
		{
			this.renderer.material = (Material)Resources.Load ("Materials/"+m.getPlayerFaction(2)+"Reserve");
		}
	}
}
