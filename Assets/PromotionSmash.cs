using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PromotionSmash : MonoBehaviour {

	private Vector2 originalScale;
	private bool shrink, fadeOut;
	public Image selfImage;
	public AudioSource audio;
	private float multiplier = 7.0f;
	public float equalityTolerance = 2f;
	private float fadeSpeed = 10.0f;

	// Use this for initialization
	void Start () {
		selfImage.rectTransform.rect.Set(0, 0, 400, 400);
	}

	void Awake() {
		this.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		if (shrink) {
			Debug.Log("shrinking");
			selfImage.rectTransform.sizeDelta = Vector2.Lerp(selfImage.rectTransform.sizeDelta, originalScale, multiplier * Time.deltaTime);
		}
		if (Vector3.Distance(originalScale, selfImage.rectTransform.sizeDelta) < 20 * equalityTolerance) {
			shrink = false;
			fadeOut = true;
		}
		if (fadeOut) {
			Debug.Log("fading");
            selfImage.color = new Color(1f,1f,1f, Mathf.Lerp(selfImage.color.a, 0.0f, fadeSpeed * Time.deltaTime));
		}
		if (selfImage.color.a < equalityTolerance) {
			fadeOut = false;
			this.gameObject.SetActive(false);
		}
	}

	public void smash() {
		selfImage.color = new Color(1f,1f,1f, 1f);
		originalScale = new Vector2(400, 400);
		this.gameObject.SetActive(true);
		//selfImage.rectTransform.rect.Set(0, 0, 400, 400);
		selfImage.rectTransform.sizeDelta = new Vector2(1000, 1000);
		//selfImage.rectTransform.sizeDelta = new Vector2(5, 5);
		shrink = true;
		audio.Play();
		Debug.Log("smashing");
	}
}
