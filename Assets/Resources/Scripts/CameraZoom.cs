using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour 
{
	//YOU HAVE TO EDIT THE TBORBIT SCRIPT AS WELL TO GET THE CAMERA ZOOM TO WORK!!  SEE THAT CLASS FOR DETAILS!!
	//private bool isZooming;
    public Vector3 cameraStartLoc;
    public Quaternion cameraStartRotation;

	public Camera mainCamera;
	public Camera thisCamera;
	public GameObject mySphere;
	
	
	private float zoomInDuration = 2.0f;
	private float zoomOutDuration = 4.0f;
	private float zoomPauseDuration = 1.0f; //pause duration must be longer than the rotate duration
	private float rotationDuration = 20.0f;
	private float timeElapsed = 0.0f;
    public float returnToDefaultLocationDuration = 8.0f;
	private Vector3 velocity = Vector3.zero;
	
	
	private float MaxZoomIn = 1.0f;
	private float MaxZoomOut = 15f;
	private float yZoomSpeed = 3.0f;
	private float zZoomSpeed = 3.0f;
	
	private float xMaxAutoZoomIn = 1.0f;
	private float yMaxAutoZoomIn = 1.0f;
	private float zMaxAutoZoomIn = 1.0f;
	
	private float xAutoZoomSpeed = 4.0f;
	private float yAutoZoomSpeed = 4.0f;
	private float zAutoZoomSpeed = 4.0f;
	
	private bool pausing = false;
	private bool zoomingInOnASquare = false;
	private bool zoomingBackToMainCamera = false;
 
	
	private Vector3 targetRotation;
	private float targetAngleX;
	private float targetAngleY;
	private float targetAngleZ;
	
	private float xAngleOffset = -0.2f;
	private float zAngleOffset = 0.2f;

	public void Start () 
	{
		this.transform.position = mainCamera.transform.position;
		this.transform.rotation = mainCamera.transform.rotation;
        cameraStartLoc = mainCamera.transform.position;
        cameraStartRotation = mainCamera.transform.rotation;
	}

	public void Update () 
	{
		if (userZoom()) //userZoom method calls interruptZoom() method (sets elapsed time to 0.0 and returns true) if a user is, indeed, manually zooming
		{
			mainCamera.transform.position = thisCamera.transform.position;
			mainCamera.transform.rotation = thisCamera.transform.rotation;
		}
		else if (zoomingInOnASquare)
			autoZoomIn();
		else if (pausing)
			continuePausing();
        else if (zoomingBackToMainCamera)
            zoomOut();
        else
        {
            setActiveCamera(mainCamera);
            zoomToDefaultAfterLongPause();
        }
	}
	
	private void setActiveCamera(Camera newActiveCamera)
	{
		if (newActiveCamera == thisCamera)
		{
			thisCamera.camera.enabled = true;
			mainCamera.camera.enabled = false;
		}
		else
		{
			thisCamera.camera.enabled = false;
			mainCamera.camera.enabled = true;
		}
	}
	
	public void zoomInOnASquare()
	{
		zoomingInOnASquare = true;
		timeElapsed = 0.0f;
		thisCamera.transform.position = mainCamera.transform.position;
		thisCamera.transform.rotation = mainCamera.transform.rotation;
		setActiveCamera(thisCamera);
	}
	
	private void autoZoomIn() 
	{
		Vector3 targetPosition = Vector3.Lerp (thisCamera.transform.position, mySphere.transform.position, timeElapsed/zoomInDuration);
		
		if (Mathf.Abs(targetPosition.x - mySphere.transform.position.x) < xMaxAutoZoomIn)
			targetPosition.x = thisCamera.transform.position.x;
		
		if (Mathf.Abs(targetPosition.y) < yMaxAutoZoomIn)
			targetPosition.y = thisCamera.transform.position.y;
		
		if (Mathf.Abs(targetPosition.z - mySphere.transform.position.z) < zMaxAutoZoomIn)
			targetPosition.z = thisCamera.transform.position.z;
		
		thisCamera.transform.position = Vector3.SmoothDamp(thisCamera.transform.position, targetPosition, ref velocity, 0.3f);
		//thisCamera.transform.position = targetPosition;

		if (timeElapsed > zoomInDuration)
		{
			if (!pausing)  //if just now starting a new pause
			{
				timeElapsed = 0.0f;  Debug.Log ("Beginning pause sequence.");
				zoomingInOnASquare = false;
				pausing = true;
				calculateAngleToSphere();
				return;
			}
		}
		timeElapsed += Time.deltaTime;
	}	
	
	public bool continuePausing()
	{
		if (pausing && timeElapsed >= zoomPauseDuration)   //if it's time to end a pause
		{
			pausing = false;		     //Debug.Log ("End of pause reached.");
			timeElapsed = 0.0f;
			zoomingBackToMainCamera = true;
			return false;
		}
		//Debug.Log ("Pause time elapsed: " + timeElapsed);
		//target angles are calculated in last phase of zoom in sequence ... see the zoomIn() method			mySphere.GetComponentInChildren<CameraSphere>().isAtDestination() &&

        timeElapsed += Time.deltaTime;  //if pausing, x, y, and z should be zero, which they already are
		return true;
	}

	private void zoomOut()
	{
		Vector3 newPos = Vector3.Lerp (thisCamera.transform.position, mainCamera.transform.position, timeElapsed/zoomOutDuration);
		thisCamera.transform.position = newPos;
		
		thisCamera.transform.rotation = Quaternion.Slerp (thisCamera.transform.rotation,mainCamera.transform.rotation,timeElapsed/zoomOutDuration);

		Vector3 distanceFromMainCamera = mainCamera.transform.position - thisCamera.transform.position;
		if (timeElapsed >= zoomOutDuration)  //distance is < 2.0
		{
			zoomingBackToMainCamera = false;
			setActiveCamera (mainCamera);
			timeElapsed = 0.0f;
		}
		timeElapsed += Time.deltaTime;
	}	
	
	private bool userZoom()
	{
		if(Input.GetKey (KeyCode.I) || Input.GetKey(KeyCode.UpArrow))
		{
			setActiveCamera(thisCamera);
			mySphere.GetComponentInChildren<CameraSphere>().interruptZoom(); //this one calls interruptZoom() on this object
			if (thisCamera.transform.position.y > MaxZoomIn)
			{	thisCamera.transform.Translate (0,-yZoomSpeed*Time.deltaTime,zZoomSpeed*Time.deltaTime); 
				mainCamera.transform.Translate (0,-yZoomSpeed*Time.deltaTime,zZoomSpeed*Time.deltaTime);
			}
			return true;
		}
		else if(Input.GetKey (KeyCode.K) || Input.GetKey (KeyCode.DownArrow))
		{
			setActiveCamera(thisCamera);
			mySphere.GetComponentInChildren<CameraSphere>().interruptZoom(); //this one calls interruptZoom() on this object
			if (thisCamera.transform.position.y < MaxZoomOut)
			{
				thisCamera.transform.Translate (0,yZoomSpeed*Time.deltaTime,-zZoomSpeed*Time.deltaTime); 
				mainCamera.transform.Translate (0,yZoomSpeed*Time.deltaTime,-zZoomSpeed*Time.deltaTime);
			}
			return true;
		}
		return false;
	}
	
	public void interruptZoom()
	{
		zoomingInOnASquare = false;
		zoomingBackToMainCamera = false;
		pausing = false;
		timeElapsed = 0.0f;
	}
	
	private void calculateAngleToSphere()
	{
		targetRotation = mySphere.transform.position - thisCamera.transform.position;
	}
	
	private void rotateTowardTargetRotation()
	{
		/*
		Debug.Log ("Camera rotation start: " + thisCamera.transform.rotation.eulerAngles);
		Debug.Log ("Target rotation: " + targetRotation);
		Quaternion turnAngle = Quaternion.Lerp(thisCamera.transform.rotation, Quaternion.Euler (targetRotation), timeElapsed/rotationDuration); 
		thisCamera.transform.rotation = turnAngle;
		Debug.Log ("Turn angle: " + turnAngle.eulerAngles);
		*/
		Vector3 newDir = Vector3.RotateTowards (thisCamera.transform.forward, targetRotation, timeElapsed/rotationDuration, 0.0f);
		thisCamera.transform.rotation = Quaternion.LookRotation (newDir);
	}

    private void zoomToDefaultAfterLongPause()
    {
        if (timeElapsed > returnToDefaultLocationDuration && timeElapsed < (returnToDefaultLocationDuration + zoomOutDuration))
        {
            float timeZooming = timeElapsed - returnToDefaultLocationDuration;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraStartLoc, timeZooming / zoomOutDuration);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, cameraStartRotation, timeZooming / zoomOutDuration);
        }
        timeElapsed += Time.deltaTime;
    }
}
