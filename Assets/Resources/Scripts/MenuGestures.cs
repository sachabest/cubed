using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuGestures : MonoBehaviour {
	
	private bool dragging;
	public MenuCube thisCube;
	public GameInfo gameInfo;
	public Image topArrow, bottomArrow, rightArrow, leftArrow;
	
	// Use this for initialization
	void Start () {
		Debug.Log("MenuGestures loaded");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTap(TapGesture gesture) {
		int side = thisCube.CurrentSide();
		if (thisCube.animation.isPlaying || dragging)
			return;
		switch (side) {
		case (int) MenuCube.Sides.SinglePlayer:
			thisCube.singleplayer();
			gameInfo.Singleplayer(true);
			thisCube.gameCenter.LoadLevel("SinglePlayerOptions");
			break;
		case (int) MenuCube.Sides.MultiPlayer:
			gameInfo.Singleplayer(false);
			gameInfo.setWinCondition(100);
			thisCube.multiplayer();
			thisCube.gameCenter.ShowMatchmaking();
			break;
		case (int) MenuCube.Sides.Options:
			//Application.LoadLevel("OptionsMenu");
			break;
		case (int) MenuCube.Sides.Credits:
			thisCube.gameCenter.LoadLevel("Credits");
			break;
		default:
			break;
		}
	}
	void OnDrag(DragGesture gesture) {
		//Debug.Log (gesture);
		if (gesture.Fingers.Count != 1)
			return;
		if (gesture.Phase == ContinuousGesturePhase.Started)
			dragging = true;
		else if (gesture.Phase == ContinuousGesturePhase.Ended)
			dragging = false;
		if (dragging) {
			// figure out our previous screen space finger position
			Vector3 fingerPos3d, prevFingerPos3d;
			fingerPos3d = gesture.Fingers[0].Position;
			prevFingerPos3d = gesture.Fingers[0].PreviousPosition;
			// convert these to world-space coordinates, and compute the amount of motion we need to apply to the object
			Vector3 move = fingerPos3d - prevFingerPos3d;



            //Mr. Hazard started coding here ... this is for simple, swipe-type of gestures
            int rotationDirection = getMenuCubeRotationDirection(move);
            hideArrows(rotationDirection);
            this.rotateCubeInDirection(rotationDirection);
            //Mr. Hazard thinks he's done now.


            // Sacha's code that Mr. Hazard isn't using ...
			/*int angle = (int) Vector3.Angle(fingerPos3d, prevFingerPos3d);  
			if (angle < 10) {
				if (Vector3.Angle(move, this.transform.position) < 5) {
					//do something
				}
			} 
             */
		}
	}


	/// Please excuse the inverted if statements
	/// I did it wrong once and am too lazy to fix it
	private void hideArrows(int rotationDirection) {
		switch (rotationDirection) {
			case 1: // NORTH
				if (!thisCube.onS) {
					topArrow.gameObject.SetActive(true);
					bottomArrow.gameObject.SetActive(true);
					rightArrow.gameObject.SetActive(true);
					leftArrow.gameObject.SetActive(true);
				} else {
					topArrow.gameObject.SetActive(false);
					bottomArrow.gameObject.SetActive(true);
					rightArrow.gameObject.SetActive(false);
					leftArrow.gameObject.SetActive(false);
				}
				break;
			case 2: // EAST
				if (!thisCube.onA) {
					topArrow.gameObject.SetActive(true);
					bottomArrow.gameObject.SetActive(true);
					rightArrow.gameObject.SetActive(true);
					leftArrow.gameObject.SetActive(true);
				} else {
					topArrow.gameObject.SetActive(false);
					bottomArrow.gameObject.SetActive(false);
					rightArrow.gameObject.SetActive(true);
					leftArrow.gameObject.SetActive(false);
				}
				break;
			case 3: // SOUTH
				if (!thisCube.onW) {
					topArrow.gameObject.SetActive(true);
					bottomArrow.gameObject.SetActive(true);
					rightArrow.gameObject.SetActive(true);
					leftArrow.gameObject.SetActive(true);
				} else {
					topArrow.gameObject.SetActive(false);
					bottomArrow.gameObject.SetActive(true);
					rightArrow.gameObject.SetActive(false);
					leftArrow.gameObject.SetActive(false);
				}
				break;
			case 4: // WEST
				if (!thisCube.onD) {
					topArrow.gameObject.SetActive(true);
					bottomArrow.gameObject.SetActive(true);
					rightArrow.gameObject.SetActive(true);
					leftArrow.gameObject.SetActive(true);
				} else {
					topArrow.gameObject.SetActive(false);
					bottomArrow.gameObject.SetActive(true);
					rightArrow.gameObject.SetActive(false);
					leftArrow.gameObject.SetActive(false);
				}
				break;
		}
	}

    //Converts a Vector3 direction into an integer ranging from 0 to 4.
    //returns 0 if Vector3 is indeterminate and the Menu Cube should remain still.
    //returns 1 if the MenuCube should rotate "North"
    //returns 2 if the MenuCube should rotate "East"
    //returns 3 if the MenuCube should rotate "South"
    //returns 4 if the MenuCube should rotate "West"
    private int getMenuCubeRotationDirection(Vector3 inputVector)
    {
        int MAX_TOLERANCE = 10;
        int MIN_TOLERANCE = 20;
        
        float dx = inputVector.x;
        float dy = inputVector.y;

        if (Mathf.Abs(dx) < MAX_TOLERANCE)
        {
            if (dy > MIN_TOLERANCE)
                return 1;
            if (dy < -MIN_TOLERANCE)
                return 3;
        }
        if (Mathf.Abs(dy) < MAX_TOLERANCE)
        {
            if (dx > MIN_TOLERANCE)
                return 2;
            if (dx < -MIN_TOLERANCE)
                return 4;
        }
        return 0;
    }

    //Rotates the MenuCube in the direction specified by the parameter.
    //If direction parameter is 1, the MenuCube should rotate "North"
    //If direction parameter is 2, the MenuCube should rotate "East"
    //If direction parameter is 3, the MenuCube should rotate "South"
    //If direction parameter is 4, the MenuCube should rotate "West"
    //If direction parameter is any other int value, this method does nothing.
    private void rotateCubeInDirection(int direction)
    {
        switch (direction)
        {
            case 1: //NORTH
				if (thisCube.NoKeyPressed() && !thisCube.animation.isPlaying) {
					thisCube.CubeAnimation("SinglePlayer", false);
					thisCube.onW = true;
					thisCube.onS = false;
					thisCube.onA = false;
					thisCube.onD = false;
					break;
				}
				else if (thisCube.onS) {
					thisCube.CubeAnimation("Credits", true);
					thisCube.onW = false;
					thisCube.onS = false;
					thisCube.onA = false;
					thisCube.onD = false;
				}
                break;
            case 2: //EAST
                if (thisCube.NoKeyPressed() && !thisCube.animation.isPlaying) {
					thisCube.CubeAnimation("MultiPlayer", false);
					thisCube.onW = false;
					thisCube.onS = false;
					thisCube.onA = false;
					thisCube.onD = true;
					break;
				}
				else if (thisCube.onA) {
					thisCube.CubeAnimation("Options", true);
					thisCube.onW = false;
					thisCube.onS = false;
					thisCube.onA = false;
					thisCube.onD = false;			
				}
                break;
            case 3: //SOUTH
                if (thisCube.NoKeyPressed() && !thisCube.animation.isPlaying) {
					thisCube.CubeAnimation("Credits", false);
					thisCube.onW = false;
					thisCube.onS = true;
					thisCube.onA = false;
					thisCube.onD = false;
					break;
				}
				else if (thisCube.onW) {
					thisCube.CubeAnimation("SinglePlayer", true);
					thisCube.onW = false;
					thisCube.onS = false;
					thisCube.onA = false;
					thisCube.onD = false;
				}
                break;
            case 4: //WEST
                if (thisCube.NoKeyPressed() && !thisCube.animation.isPlaying) {
					thisCube.CubeAnimation("Options", false);
					thisCube.onW = false;
					thisCube.onS = false;
					thisCube.onA = true;
					thisCube.onD = false;
					break;
				}
				else if (thisCube.onD) {
					thisCube.CubeAnimation("MultiPlayer", true);
					thisCube.onW = false;
					thisCube.onS = false;
					thisCube.onA = false;
					thisCube.onD = false;
				}
                break;
        }
    }
}