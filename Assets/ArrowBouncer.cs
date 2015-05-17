using UnityEngine;
using System.Collections;

public class ArrowBouncer : MonoBehaviour {

	public float maxDistance = 6.0f;
	public Vector3 direction;
	public float relativeSpeed = 2.0f;
	public float returnSpeed = 4.0f;
	private Vector3 originalPosition;
	public bool returning;
	
	// Use this for initialization
	void Start () {
		originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!returning) {
			transform.position = Vector3.Slerp(transform.position, transform.position + (maxDistance * direction), Time.deltaTime * relativeSpeed);
			if (Vector3.Distance(transform.position, originalPosition) >= maxDistance) {
				returning = true;
			}
		} else {
			transform.position = Vector3.Slerp(transform.position, originalPosition, returnSpeed * Time.deltaTime);
			if (Vector3.Distance(transform.position, originalPosition) <= 0.05f) {
				returning = false;
			}
		}
	}
}
