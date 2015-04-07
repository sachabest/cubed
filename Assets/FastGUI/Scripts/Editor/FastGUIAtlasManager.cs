using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class FastGUIAtlasManager: EditorWindow 
{
	public static void CheckSourceFolder()
	{
		if(FastGUI.sourceFolder == "")
		{
			AssetDatabase.CreateFolder(FastGUI.assetFolderToBeParseed,"Source");
			FastGUI.sourceFolder = FastGUI.assetFolderToBeParseed+"/Source/";
		}
	}
	public static UIAtlas CreateNewAtlas()
	{
		CheckSourceFolder();
		string prefabPath = "";
		string matPath = "";
		
		// If we have an atlas to work with, see if we can figure out the path for it and its material
		if (NGUISettings.atlas != null && NGUISettings.atlas.name == NGUISettings.atlasName)
		{
			prefabPath = AssetDatabase.GetAssetPath(NGUISettings.atlas.gameObject.GetInstanceID());
			if (NGUISettings.atlas.spriteMaterial != null) matPath = AssetDatabase.GetAssetPath(NGUISettings.atlas.spriteMaterial.GetInstanceID());
		}

		// Assume default values if needed
		NGUISettings.atlasName 	= FastGUI.objectToBeLoaded.name;
		if (string.IsNullOrEmpty(prefabPath)) prefabPath = FastGUI.sourceFolder + NGUISettings.atlasName + ".prefab";
		if (string.IsNullOrEmpty(matPath)) matPath = FastGUI.sourceFolder + NGUISettings.atlasName + ".mat";

		// Try to load the prefab
		GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
		if (NGUISettings.atlas == null && go != null) NGUISettings.atlas = go.GetComponent<UIAtlas>();

		

		// Try to load the material
		Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;

		// If the material doesn't exist, create it
		if (mat == null)
		{
			Shader shader = Shader.Find("Unlit/Transparent Colored");
			mat = new Material(shader);

			// Save the material
			AssetDatabase.CreateAsset(mat, matPath);
			AssetDatabase.Refresh();

			// Load the material so it's usable
			mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
		}

		if (NGUISettings.atlas == null || NGUISettings.atlas.name != NGUISettings.atlasName)
		{
			// Create a new prefab for the atlas
#if UNITY_3_4
			Object prefab = (go != null) ? go : EditorUtility.CreateEmptyPrefab(prefabPath); 
#else
			Object prefab = (go != null) ? go : PrefabUtility.CreateEmptyPrefab(prefabPath);
#endif
			// Create a new game object for the atlas
			go = new GameObject(NGUISettings.atlasName);
			go.AddComponent<UIAtlas>().spriteMaterial = mat;

			// Update the prefab
#if UNITY_3_4
			EditorUtility.ReplacePrefab(go, prefab);
#else
			PrefabUtility.ReplacePrefab(go, prefab);
#endif
			DestroyImmediate(go);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			// Select the atlas
			go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
			NGUISettings.atlas = go.GetComponent<UIAtlas>();
		}
		return go.GetComponent<UIAtlas>();
	}
	
	public static UIFont CreateNewFont(string pTagetFontName)
	{	
		string[] directorys 			= Directory.GetDirectories(Application.dataPath+"/", "Typeface", SearchOption.AllDirectories);	
		string targetFolder		 		= "";
		string prefabPath 				= "";
		string matPath = "";
		
		if(directorys.Length == 0)
		{
			Debug.LogWarning("Can't find the Typeface directory for the font creation, this name is Case Sensetive and must be 'TypeFace' ");
		}
		else
		{
			string[] tFiles = Directory.GetFiles(directorys[0], "*", SearchOption.AllDirectories );
			foreach(string tFile in tFiles)
			{
				if(tFile.IndexOf(".png") >= 0 && tFile.IndexOf(".meta") < 0)
				{
					NGUISettings.fontTexture 	= AssetDatabase.LoadAssetAtPath(FastGUIUtils.GetProjectRelativePath(tFile), typeof(Texture2D)) as Texture2D;
				}
				else if(tFile.IndexOf(".txt") >= 0 && tFile.IndexOf(".meta") < 0)
				{
					NGUISettings.fontData 		= AssetDatabase.LoadAssetAtPath(FastGUIUtils.GetProjectRelativePath(tFile), typeof(TextAsset)) as TextAsset;
					string tProjectRelative 	= FastGUIUtils.GetProjectRelativePath(tFile);
					targetFolder 				= FastGUIUtils.GetParentFolderPath(tProjectRelative);
				}
			}
	
			// Assume default values if needed
			
			if (string.IsNullOrEmpty(NGUISettings.fontName)) NGUISettings.fontName = pTagetFontName;
			if (string.IsNullOrEmpty(prefabPath)) prefabPath = targetFolder + NGUISettings.fontName + ".prefab";
			if (string.IsNullOrEmpty(matPath)) matPath = targetFolder + NGUISettings.fontName + ".mat";
			
	
			// Draw the atlas selection only if we have the font data and texture specified, just to make it easier
			if (NGUISettings.fontData == null && NGUISettings.fontTexture == null)
			{
				Debug.LogError("NGUISettings.fontData is "+NGUISettings.fontData+ " and NGUISettings.fontTexture is" +NGUISettings.fontTexture);
				return null;
			}
			
			GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
			int create = 2;
			
			
			if (go == null || EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to replace the contents of the " +
				NGUISettings.fontName + " font with the currently selected values? This action can't be undone.", "Yes", "No"))
			{
				// Try to load the material
				Material mat = null;
				
				// Non-atlased font
				if (create == 2)
				{
					mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;

					// If the material doesn't exist, create it
					if (mat == null)
					{
						Shader shader = Shader.Find("Unlit/Transparent Colored");
						mat = new Material(shader);

						// Save the material
						AssetDatabase.CreateAsset(mat, matPath);
						AssetDatabase.Refresh();

						// Load the material so it's usable
						mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
					}
					mat.mainTexture = NGUISettings.fontTexture;
				}

				// Font doesn't exist yet
				if (go == null || go.GetComponent<UIFont>() == null)
				{
					// Create a new prefab for the atlas
					Object prefab = CreateEmptyPrefab(prefabPath);

					// Create a new game object for the font
					go = new GameObject(NGUISettings.fontName);
					NGUISettings.font = go.AddComponent<UIFont>();
					CreateFont(NGUISettings.font, create, mat);

					// Update the prefab
					ReplacePrefab(go, prefab);
					DestroyImmediate(go);
					AssetDatabase.Refresh();

					// Select the atlas
					go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
					NGUISettings.font = go.GetComponent<UIFont>();
				}
				else
				{
					NGUISettings.font = go.GetComponent<UIFont>();
					CreateFont(NGUISettings.font, create, mat);
				}
				MarkAsChanged();
				return NGUISettings.font;
			}
		}
		return null;
	}
	
	static void MarkAsChanged ()
	{
		if (NGUISettings.font != null)
		{
			List<UILabel> labels = NGUIEditorTools.FindInScene<UILabel>();

			foreach (UILabel lbl in labels)
			{
				if (lbl.font == NGUISettings.font)
				{
					lbl.font = null;
					lbl.font = NGUISettings.font;
				}
			}
		}
	}
	
	
	static void CreateFont (UIFont font, int create, Material mat)
	{
		if (create == 1)
		{
			// New dynamic font
			font.atlas = null;
			font.dynamicFont = NGUISettings.dynamicFont;
			font.dynamicFontSize = NGUISettings.dynamicFontSize;
			font.dynamicFontStyle = NGUISettings.dynamicFontStyle;
		}
		else
		{
			// New bitmap font
			font.dynamicFont = null;
			BMFontReader.Load(font.bmFont, NGUITools.GetHierarchy(font.gameObject), NGUISettings.fontData.bytes);

			if (create == 2)
			{
				font.atlas = null;
				font.material = mat;
			}
			else if (create == 3)
			{
				font.spriteName = NGUISettings.fontTexture.name;
				font.atlas = NGUISettings.atlas;
			}
		}
	}
	
	static Object CreateEmptyPrefab (string prefabPath)
	{
#if UNITY_3_4
		return EditorUtility.CreateEmptyPrefab(prefabPath);
#else
		return PrefabUtility.CreateEmptyPrefab(prefabPath);
#endif
	}

	static void ReplacePrefab (GameObject go, Object prefab)
	{
#if UNITY_3_4
		// Update the prefab
		EditorUtility.ReplacePrefab(go, prefab);
#else
		// Update the prefab
		PrefabUtility.ReplacePrefab(go, prefab);
#endif
	}
}
