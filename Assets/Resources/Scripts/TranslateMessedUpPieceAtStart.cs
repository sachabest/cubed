using UnityEngine;
using System.Collections;

public class TranslateMessedUpPieceAtStart : MonoBehaviour {
	
	public float xAdjustment;  //for vine: 0.002090
	public float yAdjustment;
	public float zAdjustment;  //for vine: 0.248706
	// Use this for initialization
	void Start () {
		//this.adjust();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void rotateThisWeirdPiece(int degrees)
	{
		Transform pivot = this.transform.FindChild("Rotator Child").transform;
		this.transform.RotateAround(pivot.position, Vector3.up, degrees);
	}
	public void adjust()
	{
		this.transform.Translate (new Vector3(xAdjustment,yAdjustment,zAdjustment));
		//Debug.Log ("Adjust method just got called!");
	}
}
