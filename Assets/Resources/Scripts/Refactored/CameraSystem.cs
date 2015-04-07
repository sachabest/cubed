using UnityEngine;
using System.Collections;

public class CameraSystem : MonoBehaviour {
	
	private Camera mainCamera;
	private Camera zoomCamera;
	private bool acceptInterrupts;
	
	private Vector2 zoomConstraints, zoomSpeed, targetAngleOffset;
	private Vector3 autoZoomConstratins, autoZoomSpeed, targetAngle;
	
	public delegate void ZoomingHandler(bool start, Vector3 zoomLocaiton);
	public event ZoomingHandler Zooming;
	
	public delegate void CameraChangeHandler(Camera oldCamera, Camera newCamera);
	public event CameraChangeHandler CameraChanged;
	
	public delegate void InterruptHandler(GameObject sender, bool start);
	public event InterruptHandler Interrupting;
	
	
	public CameraSystem(Camera main, Camera zoom, bool acceptInterrupts) {
		this.mainCamera = main;
		this.zoomCamera = zoomCamera;
		this.acceptInterrupts = acceptInterrupts;
	}
	
	private void setActiveCamera(Camera newActiveCamera)
	{
		if (newActiveCamera == zoomCamera)
		{
			zoomCamera.camera.enabled = true;
			mainCamera.camera.enabled = false;
			CameraChanged(mainCamera, zoomCamera);
		}
		else
		{
			zoomCamera.camera.enabled = false;
			mainCamera.camera.enabled = true;
			CameraChanged(zoomCamera, mainCamera);
		}
		
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
