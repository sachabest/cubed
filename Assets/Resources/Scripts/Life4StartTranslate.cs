using UnityEngine;
using System.Collections;

public class Life4StartTranslate : MonoBehaviour {

	//This start calculations adjust for the offset for the Life4 model initial location.
	void Start () {
		this.transform.localScale = new Vector3(24.9f,23.0f,22.0f);
		int random = UnityEngine.Random.Range (0,4);
		this.transform.rotation = Quaternion.Euler (0,90*random,0);
		//this.transform.localPosition = new Vector3(-3.005f,0.0f,-0.75f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
