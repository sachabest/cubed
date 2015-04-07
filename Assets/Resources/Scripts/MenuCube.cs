using UnityEngine;
using System.Collections;

public class MenuCube : MonoBehaviour {
	
	public Animation animation;
	public Camera camera;
	public MenuGestures gestures;
	public GCCubedListener gameCenter;
	public bool onW, onD, onS, onA;
	private float yRot;
	public enum Sides {SinglePlayer = 1, MultiPlayer, Credits, Options};
	// Use this for initialization
	void Start () {
		yRot = 0.1f;
		onA = false;
		onD = false;
		onS = false;
		onW = false;
	}
#if UNITY_WEBPLAYER
	void () {
		int side = CurrentSide();
		if (animation.isPlaying || gestures.dragging)
			return;
		switch (side) {
		case 0:
			break;
		case (int) Sides.SinglePlayer:
			Application.LoadLevel("cubed");
			break;
		case (int) Sides.MultiPlayer:
			gameCenter.ShowMatchmaking();
			break;
		case (int) Sides.Options:
			Application.LoadLevel("OptionsMenu");
			break;
		case (int) Sides.Credits:
			Application.LoadLevel("Credits");
			break;
		default:
			break;

		}
	}
#endif
	public void singleplayer() {
		gameCenter.singleplayer = true;
	}
	public void multiplayer() {
		gameCenter.singleplayer = false;
	}
	// Update is called once per frame
	void LateUpdate () {
		camera.transform.Rotate(new Vector3(0, yRot, 0), Space.World);
		camera.transform.Translate(new Vector3(0, 0, 0), Space.Self);
	}
	public void CubeAnimation(string animationName, bool up) {
		if (up) {
			if (animation.IsPlaying(animationName + "Backward"))
				animation.Rewind();
			else
				animation.Play(animationName + "Backward");
		}
		else {
			animation.Play(animationName + "Forward");
		}
	}
	void Update() {
		if (!OnOtherAnimation(KeyCode.W) && Input.GetKeyDown(KeyCode.W)) {
			onW = true;
			CubeAnimation("SinglePlayer", false);
		}
		if (!OnOtherAnimation(KeyCode.W) && Input.GetKeyUp(KeyCode.W)) {
			onW = false;
			CubeAnimation("SinglePlayer", true);
		}
		if (!OnOtherAnimation(KeyCode.D) && Input.GetKeyDown(KeyCode.D)) {
			onD = true;
			CubeAnimation("MultiPlayer", false);
		}
		if (!OnOtherAnimation(KeyCode.D) && Input.GetKeyUp(KeyCode.D)) {
			onD = false;
			CubeAnimation("MultiPlayer", true);
		}
		if (!OnOtherAnimation(KeyCode.A) && Input.GetKeyDown(KeyCode.A)) {
			onA = true;
			CubeAnimation("Options", false);
		}
		if (!OnOtherAnimation(KeyCode.A) && Input.GetKeyUp(KeyCode.A)) {
			onA = false;
			CubeAnimation("Options", true);
		}
		if (!OnOtherAnimation(KeyCode.S) && Input.GetKeyDown(KeyCode.S)) {
			onS = true;
			CubeAnimation("Credits", false);
		}
		if (!OnOtherAnimation(KeyCode.S) && Input.GetKeyUp(KeyCode.S)) {
			onS = false;
			CubeAnimation("Credits", true);
		}
	}
	public int CurrentSide() {
		if (animation.isPlaying)
			return 0;
		else {
			if (onA)
				return (int) Sides.Options;
			else if (onW)
				return (int) Sides.SinglePlayer;
			else if (onS)
				return (int) Sides.Credits;
			else if (onD)
				return (int) Sides.MultiPlayer;
			else
				return 0;
		}
	}
	public bool OnOtherAnimation(KeyCode current) {
		if (current == KeyCode.S)
			return onA || onD || onW;
		else if (current == KeyCode.W)
			return onA || onD || onS;
		else if (current == KeyCode.D)
			return onA || onS || onW;
		else if (current == KeyCode.A)
			return onD || onS || onW;
		return false;
	}
	public bool NoKeyPressed() {
		if (onA)
			return false;
		if (onS)
			return false;
		if (onD)
			return false;
		if (onW)
			return false;
		return true;
	}
}
