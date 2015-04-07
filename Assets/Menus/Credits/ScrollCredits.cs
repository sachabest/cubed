using UnityEngine;
using System.Collections;

public class ScrollCredits : MonoBehaviour {
	
	public float scrollSpeed = 0.7f;
	public float zoomSpeed = 1.0f;
	
	private float yLimitToStartZoomingIn = 17.2f;
	private Vector3 startLocation = new Vector3(0f,-16.8f,0f);   //the start position for the credits rectangle
	private Vector3 endLocation = new Vector3(0f,18.0f,2.1f);  //the end position for the credits rectangle -- position with the bottom icon centered on the camera;
	private bool isZoomingIntoBottomImage = false;
	private bool creditsFinished = false;
	
	
	// Use this for initialization
	void Start ()
	{
		this.transform.position = startLocation; //new Vector3(0,-16.8f,0);
		isZoomingIntoBottomImage = false;
		creditsFinished = false;
	}
	// Update is called once per frame
	void Update () 
	{
		if (!creditsFinished)
		{
			if (!isZoomingIntoBottomImage)
			{
				this.transform.Translate (0,0,-scrollSpeed*Time.deltaTime);
				if (this.transform.position.y >= yLimitToStartZoomingIn)
					isZoomingIntoBottomImage = true;
			}
			else
			{
				this.transform.position = Vector3.Lerp (this.transform.position, endLocation, Time.deltaTime/zoomSpeed);
				if (Vector3.Magnitude(this.transform.position - endLocation) < 0.01f)
					creditsFinished = true;
			}
		}
		if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.Escape))
			if (this.transform.position.y > -16.0f)
				Application.LoadLevel ("MainMenuV2");
	}
}
