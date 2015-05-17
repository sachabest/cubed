using UnityEngine;
using System.Collections;

public class PieceManager : MonoBehaviour {
	
	public Stack life3, life6, life9, industry3, industry6, industry9, lifeSuper, industrySuper;
	public int cMax;
	public bool visibilityDone;
	
	// Use this for initialization
	void Start () {													//According to the GameManager:  1 --> Life
		
		visibilityDone = false;														//				 2 --> Industry
		life3 = new Stack(2*cMax);
		life6 = new Stack(cMax);
		life9 = new Stack(cMax);
		lifeSuper = new Stack(cMax);
		industry3 = new Stack(cMax);
		industry6 = new Stack(cMax);
		industry9 = new Stack(cMax);
		industrySuper = new Stack(cMax);
		setupArrays(cMax);
	}
	void setupArrays(int cMax) {
		string loadName = null;
		for (int c = 0; c < cMax; c++) {
			//loadName = "Prefabs/Life3_1";
			//life3.Push(createStartObject(loadName));
			loadName = "Prefabs/Life3_0";
			life3.Push(createStartObject(loadName));
			loadName = "Prefabs/Industry3_0";
			industry3.Push(createStartObject(loadName));
			loadName = "Prefabs/Industry4_" + UnityEngine.Random.Range(0,4).ToString();
			industrySuper.Push(createStartObject(loadName));
			industrySuper.Push(createStartObject(loadName));
			loadName = "Prefabs/Life4_" + UnityEngine.Random.Range(0,4).ToString();
			lifeSuper.Push(createStartObject(loadName)); 
			lifeSuper.Push(createStartObject(loadName)); 
			if (c < (cMax * 2/3)) {
				loadName = "Prefabs/Life6_0";
				life6.Push(createStartObject(loadName));
				loadName = "Prefabs/Industry6_0";
				industry6.Push(createStartObject(loadName));
			}
			if (c < (cMax * 1/3)) {
				loadName = "Prefabs/Life9_0";
				life9.Push(createStartObject(loadName));
				loadName = "Prefabs/Industry9_0";
				industry9.Push(createStartObject(loadName));
			}
		}
	}
	
	
	GameObject createStartObject(string loadName) {
		Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90, 0);
		GameObject created = null;
		if (loadName.Equals ("Prefabs/Life3_0"))
		{
			created = (GameObject) Instantiate(Resources.Load(loadName), new Vector3(0, 0, 0), Quaternion.identity);
			created.GetComponent<TranslateMessedUpPieceAtStart>().rotateThisWeirdPiece(UnityEngine.Random.Range(0, 4) * 90);
		}
		else
			created = (GameObject) Instantiate(Resources.Load(loadName), new Vector3(0, 0, 0), rotation);
		created.SetActive(false);
		return created;
	}
	
	
	public GameObject GetPiece(PlayerManager.Faction faction, int tier) {
		GameObject returned = null;
		if (faction == PlayerManager.Faction.Life) { //Life is one!!!
			if (tier == 1)
				returned = (GameObject) life3.Pop();
			else if (tier == 2)
				returned = (GameObject) life6.Pop();
			else if (tier == 3)
				returned = (GameObject) life9.Pop();
			else
				returned = (GameObject) lifeSuper.Pop();
		}
		else if (faction == PlayerManager.Faction.Industry) {  //Industry is 2!!!
			if (tier == 1)
				returned = (GameObject) industry3.Pop();
			else if (tier == 2)
				returned = (GameObject) industry6.Pop();
			else if (tier == 3)
				returned = (GameObject) industry9.Pop();
			else
				returned = (GameObject) industrySuper.Pop();
		}
		return returned;
	}
	public void ReturnPiece(GameObject piece, PlayerManager.Faction faction, int tier) {
		if (piece == null)
			return;
		piece.SetActive (false);
		piece.transform.position = new Vector3(-1000,-1000,-1000);
		if (faction == PlayerManager.Faction.Life) {  //Life is one!!!
			if (tier == 1)
				life3.Push (piece);
			else if (tier == 2)
				life6.Push (piece);
			else if (tier == 3)
				life9.Push (piece);
			else if (tier == 4)
				lifeSuper.Push(piece);
		}
		else if (faction == PlayerManager.Faction.Industry) { //Industry is two!!!!
			if (tier == 1)
				industry3.Push (piece);
			else if (tier == 2)
				industry6.Push (piece);
			else if (tier == 3)
				industry9.Push (piece);
			else if (tier == 4)
				industrySuper.Push(piece);
		}
	}
	// Update is called once per frame
	void Update () {
	}
}
