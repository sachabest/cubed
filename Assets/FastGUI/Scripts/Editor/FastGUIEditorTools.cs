using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class FastGUIEditorTools : MonoBehaviour 
{
	static Texture2D mGradientTex;
	
	
	
	static public Texture2D blankTexture
	{
		get
		{
			return EditorGUIUtility.whiteTexture;
		}
	}
	static public Texture2D gradientTexture
	{
		get
		{
			if (mGradientTex == null) mGradientTex = CreateGradientTex();
			return mGradientTex;
		}
	}
	static Texture2D CreateGradientTex ()
	{
		Texture2D tex = new Texture2D(1, 16);
		tex.name = "[Generated] Gradient Texture";
		tex.hideFlags = HideFlags.DontSave;

		Color c0 = new Color(1f, 1f, 1f, 0f);
		Color c1 = new Color(1f, 1f, 1f, 0.4f);

		for (int i = 0; i < 16; ++i)
		{
			float f = Mathf.Abs((i / 15f) * 2f - 1f);
			f *= f;
			tex.SetPixel(0, i, Color.Lerp(c0, c1, f));
		}

		tex.Apply();
		tex.filterMode = FilterMode.Bilinear;
		return tex;
	}
	
	static public Rect DrawHeader (string text)
	{
		GUILayout.Space(28f);
		Rect rect = GUILayoutUtility.GetLastRect();
		rect.yMin += 5f;
		rect.yMax -= 4f;
		rect.width = Screen.width;

		if (Event.current.type == EventType.Repaint)
		{
			GUI.color = Color.black;
			GUI.DrawTexture(new Rect(0f, rect.yMin, Screen.width, rect.yMax - rect.yMin), gradientTexture);
			GUI.color = new Color(0f, 0f, 0f, 0.25f);
			GUI.DrawTexture(new Rect(0f, rect.yMin, Screen.width, 1f), blankTexture);
			GUI.DrawTexture(new Rect(0f, rect.yMax - 1, Screen.width, 1f), blankTexture);
			GUI.color = Color.white;
			GUI.Label(new Rect(rect.x + 4f, rect.y, rect.width - 4, rect.height), text, EditorStyles.boldLabel);
		}
		return rect;
	}
	static public void DrawSeparator ()
	{
		GUILayout.Space(12f);

		if (Event.current.type == EventType.Repaint)
		{
			Texture2D tex = blankTexture;
			Rect rect = GUILayoutUtility.GetLastRect();
			GUI.color = new Color(0f, 0f, 0f, 0.25f);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
			GUI.color = Color.white;
		}
	}
}
