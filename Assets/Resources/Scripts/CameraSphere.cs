using UnityEngine;
using System.Collections;

public class CameraSphere : MonoBehaviour 
{
	//Added by Mr. Hazard on May 10, 2013
	private GameSquare squareToZoomTo;
	private float autoZoomSpeed = 1.0f;
	private float manualZoomSpeed = 5.0f;
	private float rotationSpeed = 30.0f;
	
	public Camera mainCamera;
	public Camera zoomCamera;
	
	private float yRot;
	private float xVel;
	private float zVel;
	
	private int maxXVel = 6;
	private int maxZVel = 6;

	// Use this for initialization
	void Start () 
	{
		yRot = 0;
		xVel = 0;
		zVel = 0;
	}
	
	//Mr. Hazard wrote the next two methods on May 10, 2013
	//The private method can only be called from the Update() method
	//Precondition: squareToZoomTo != null
	private void zoomToSquare()
	{
		Vector3 path = new Vector3(squareToZoomTo.transform.position.x - this.transform.position.x,
				                   squareToZoomTo.transform.position.y - this.transform.position.y,
				                   squareToZoomTo.transform.position.z - this.transform.position.z);
			
		this.transform.Translate(Time.deltaTime*autoZoomSpeed*path,Space.World);

		if (path.magnitude < 0.1)
			squareToZoomTo = null;
	}
	public void zoomToSquare(GameSquare square)
	{
		squareToZoomTo = square;
		zoomCamera.GetComponent<CameraZoom>().zoomInOnASquare();
	}
	public GameSquare getTargetSquare()
	{
		return squareToZoomTo;
	}

	// Update is called once per frame
	public void Update () 
	{
		//Added by Mr. Hazard on May 10, 2013
		if (squareToZoomTo != null)
			zoomToSquare();
		
		if(Input.GetKey(KeyCode.LeftArrow)) {
			yRot = rotationSpeed;
		}
		else if(Input.GetKey(KeyCode.RightArrow))
			yRot = -1*rotationSpeed;	
		else
			yRot = 0;

		if(Input.GetKey(KeyCode.D))
			xVel = manualZoomSpeed;
		else if(Input.GetKey(KeyCode.A))
			xVel = -1*manualZoomSpeed;
		else
			xVel = 0;
		
		if(Input.GetKey(KeyCode.S))
			zVel = -1*manualZoomSpeed;
		else if(Input.GetKey(KeyCode.W))
			zVel = manualZoomSpeed;
		else
			zVel = 0;
		
		if (yRot != 0 || xVel != 0 || zVel != 0)
			interruptZoom ();
		
		this.transform.Rotate(   new Vector3(0,                  yRot*Time.deltaTime, 0), Space.World);
		this.transform.Translate(new Vector3(xVel*Time.deltaTime,                  0, zVel*Time.deltaTime), Space.Self);
	}
	
	public void interruptZoom()
	{
		squareToZoomTo = null;
		zoomCamera.GetComponent<CameraZoom>().interruptZoom();
	}
}
