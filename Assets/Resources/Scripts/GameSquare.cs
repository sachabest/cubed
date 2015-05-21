using UnityEngine;
using System.Collections;
//[SerializeAll]
public class GameSquare : MonoBehaviour 
{
	private GameManager manager;
	public int xval;
	public int yval;
	public int _tier;
	public PlayerManager.Faction faction;
	public float touchTolerance = 10.0f;
	private GameObject piece;
	private Material material;

	// Use this for initialization
	void Start () 
	{
		material = (Material)Resources.Load("Materials/transparent");
		piece = null;
	}
	// void Update() {
	// 	if (Input.touchCount == 1 || Input.GetMouseButtonDown(0)) {
	// 		Debug.Log("touch count " + Input.Input.touchCount);
	// 		Vector3 position = Vector3.zero;
	// 		if (Application.platform == RuntimePlatform.OSXEditor) {
	// 			position = Input.mousePosition;
	// 		}
	// 		else {
	// 			Touch t = Input.touches[0];
	// 			if (t.phase == TouchPhase.Ended) {
	// 				position = t.position;
	// 			}
	// 		}
	// 		if (position != Vector3.zero) {
	// 			Ray cameraRay = GameManager.instance.MainCamera.ScreenPointToRay(position);
	// 			RaycastHit[] hits = Physics.RaycastAll(cameraRay);
	// 			if (hits != null) {
	// 				foreach (RaycastHit hit in hits) {
	// 					if (hit.collider.Equals(collider)) {
	// 						if (!GameManager.instance.mouseUI && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
	// 							manager.HandleClick(xval, yval);
	// 						}
	// 					}
	// 				}
	// 			}
	// 		}
	// 	}
	// }
	// Update is called once per frame
	public void SetManager(GameManager mgr) {
		this.manager = mgr;
	}
	public void setMaterial(Material mat)
	{
		material = mat;
		this.renderer.material = material;
	}
	public bool hasPiece()
	{
		return piece!=null;
	}
	public void DestroyPiece()
	{	
		if (piece == null)
			return;                                                  
		manager.pieceManager.ReturnPiece(piece, faction, _tier);
		piece = null;
		_tier = 0;
		faction = 0;
	}
	/*
	public void OnMouseEnter()
	{
		if (!manager.mouseUI)
			this.renderer.material = (Material)Resources.Load("Materials/highlight");
	}
	public void OnMouseExit()
	{
		this.renderer.material = material;
	}
	*/
	private Vector2 touchLocation;
	public void OnMouseDown() {
		if (Input.touchCount == 1) {
			touchLocation = Input.touches[0].position;
			Debug.Log(touchLocation);
		}
		if (Application.isEditor) {
			manager.HandleClick(xval, yval);
		}
	}
	public void OnMouseUp()
	{
		if (Input.touchCount == 1) {
			Vector2 newTouchLocation = Input.touches[0].position;
			Debug.Log(newTouchLocation);
			if (Vector2.Distance(newTouchLocation, touchLocation) < touchTolerance) {
				if (!GameManager.instance.mouseUI && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
					manager.HandleClick(xval, yval);
				} else {
					Debug.Log("Over UI or dragging");
				}
			}
		}

	}
	public void SetColor(PlayerManager.Faction faction, int tier) //THIS METHOD IS CALLED WITH THE *GAMEMANAGER* DEFINITION FOR PLAYER: 1 -> INDUSTRY
	{																											//   2 -> LIFE
		_tier = tier;
		faction = faction;
		material = (Material)Resources.Load("Materials/transparent");
		this.renderer.material = material;
		//piece = manager.pieceManager.GetPiece(player-1, tier);   //DUDE!!  WE HAVE TO BE CAREFUL ABOUT HOW WE REFER TO THINGS!!!  WHY ARE WE SUBTRACTING 1 HERE?!?!?
		piece = manager.pieceManager.GetPiece(faction, tier);
		piece.SetActive(true);
		piece.transform.position = this.transform.position;
		
		if (faction == PlayerManager.Faction.Life && tier == 1)
		{	
			piece.GetComponent<TranslateMessedUpPieceAtStart>().adjust();  //Debug.Log ("Made the adjustment."); 
		}
		
		if (piece.audio != null)
			piece.audio.Play();
		if(tier==3 && faction == PlayerManager.Faction.Industry) {return;}                   //CHANGED PLAYER TO A 2 ON MAY 28TH
		if (piece.animation != null) {
			piece.animation.PlayQueued("Start",QueueMode.PlayNow);
			piece.animation.PlayQueued("Idle");
			piece.animation["Idle"].wrapMode = WrapMode.Loop;
		}
	}
}