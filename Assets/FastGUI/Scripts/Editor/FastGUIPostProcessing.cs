using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class FastGUIPostProcessing : Editor
{
	
	public FastGUI monsterImporter;
	
	private struct ImageButtonSettings
	{
		string hovername;
		string pressedname;
		string idlename;
		string disabledname;
		
	}
	
	public void SetImporterLink(FastGUI pLink)
	{
		monsterImporter = pLink;
	}
	
	static public Vector3 AdjustPosition(float pX, float pY)
	{
		Vector3 tTargetPos = new Vector3(pX-(FastGUI.targetWidht/2) , FastGUI.targetHeight/2-pY, 0);
		tTargetPos.x = tTargetPos.x/(FastGUI.targetHeight/2);
		tTargetPos.y = tTargetPos.y/(FastGUI.targetHeight/2);
		
		
		tTargetPos.x += FastGUI.actualRoot.transform.position.x;
		tTargetPos.y += FastGUI.actualRoot.transform.position.y;
		
		
		return tTargetPos;
	}
	
	public static Transform CreateAnchor(XMLNode pNode, UIPanel pTargetRootPannel, Transform pLastAnchor, string pLastAnchorPath)
	{
		string 	tName		= pNode.GetNode ("name>0")["_text"].ToString();
		string 	tParentName	= pNode.GetNode ("path>0")["_text"].ToString();
		float 	tPosX		= float.Parse(pNode.GetNode ("posX>0")["_text"].ToString());
		float 	tPosY		= float.Parse(pNode.GetNode ("posY>0")["_text"].ToString());
		int 	tLayerID	= int.Parse(pNode.GetNode ("layerID>0")["_text"].ToString());
		Transform lastRoot = pTargetRootPannel.transform;
		
		if(tParentName == pLastAnchorPath)
		{
			lastRoot = pLastAnchor;
		}
		else
		{
			if(pTargetRootPannel.transform.FindChild(tParentName))
				lastRoot = pTargetRootPannel.transform.FindChild(tParentName);
		}
		
		GameObject tNewGo;
		if(FastGUI.actualFastGUIOutput.references.ContainsKey(tLayerID))
		{
			tNewGo = EditorUtility.InstanceIDToObject(FastGUI.actualFastGUIOutput.references[tLayerID]) as GameObject;
			if(tNewGo == null)
			{
				FastGUI.actualFastGUIOutput.references.Remove(tLayerID);	
				return CreateAnchor(pNode, pTargetRootPannel, pLastAnchor, pLastAnchorPath);
			}
		}
		else
		{
			tNewGo	= new GameObject(tName);
			FastGUI.actualFastGUIOutput.references.Add(tLayerID, tNewGo.GetInstanceID());
		}
		
		

		tNewGo.transform.parent 	= lastRoot;
		tNewGo.transform.position 	= AdjustPosition(tPosX,tPosY);
		tNewGo.transform.localScale = Vector3.one;
		
		return tNewGo.transform;
	}
	
	public static Transform CreateClippingPanel(XMLNode pNode, UIPanel pTargetRootPannel, Transform pLastAnchor, string pLastAnchorPath)
	{
		//float 	tPosX		= float.Parse(pNode.GetNode ("posX>0")["_text"].ToString());
		//float 	tPosY		= float.Parse(pNode.GetNode ("posY>0")["_text"].ToString());
		float 	tClipX		= float.Parse(pNode.GetNode ("clippingX>0")["_text"].ToString());
		float 	tClipY		= float.Parse(pNode.GetNode ("clippingY>0")["_text"].ToString());
		Transform tContainer = CreateAnchor(pNode, pTargetRootPannel, pLastAnchor, pLastAnchorPath );
		
		UIPanel tPanel		= tContainer.gameObject.AddComponent<UIPanel>();
		tPanel.clipping		= UIDrawCall.Clipping.SoftClip;
		tPanel.clipRange	= new Vector4(0,0,tClipX,tClipY);
		tPanel.clipSoftness	= new Vector2(5, 5);
		
		return tContainer;
	}
	
	public static Transform CreateSlicedSprite(XMLNode pNode, UIPanel pTargetRootPannel, Transform pLastAnchor, string pLastAnchorPath )
	{	string 	tName		= pNode.GetNode ("name>0")["_text"].ToString();
		string 	tParentName	= pNode.GetNode ("path>0")["_text"].ToString();
		float 	tPosX		= float.Parse(pNode.GetNode ("posX>0")["_text"].ToString());
		float 	tPosY		= float.Parse(pNode.GetNode ("posY>0")["_text"].ToString());
		float 	tWidth		= float.Parse(pNode.GetNode ("width>0")["_text"].ToString());
		float 	tHeight		= float.Parse(pNode.GetNode ("height>0")["_text"].ToString());
		string	tSource		= pNode.GetNode ("source>0")["_text"].ToString();
		int 	tLayerID	= int.Parse(pNode.GetNode ("layerID>0")["_text"].ToString());
		Transform lastRoot = pTargetRootPannel.transform;
		
		if(tParentName == pLastAnchorPath)
		{
			lastRoot = pLastAnchor;
		}
		else
		{
			if(pTargetRootPannel.transform.FindChild(tParentName))
				lastRoot = pTargetRootPannel.transform.FindChild(tParentName);
		}
		
		Texture2D tTexture 			= AssetDatabase.LoadAssetAtPath(FastGUI.assetFolderToBeParseed+"/Images/"+tSource, typeof(Texture2D)) as Texture2D;
		if(tTexture == null)
			return null;
		
		if(NGUISettings.atlas == null)
			NGUISettings.atlas = FastGUIAtlasManager.CreateNewAtlas();
		
		UIAtlas.Sprite tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
		if(tSprite == null)
		{
			UIAtlasMaker.AddOrUpdate(NGUISettings.atlas, tTexture);
			tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
		}
		
		UISlicedSprite tSpriteWidget = null;
		GameObject tNewGo = null;
		if(FastGUI.actualFastGUIOutput.references.ContainsKey(tLayerID))
		{
			tNewGo 			= EditorUtility.InstanceIDToObject(FastGUI.actualFastGUIOutput.references[tLayerID]) as GameObject;
			if(tNewGo == null)
			{
				FastGUI.actualFastGUIOutput.references.Remove(tLayerID);	
				//CreateSprite(pNode, pTargetRootPannel, pLastAnchor, pLastAnchorPath);
				tSpriteWidget 						= NGUITools.AddWidget<UISlicedSprite>(lastRoot.gameObject);
				FastGUI.actualFastGUIOutput.references.Add(tLayerID, tSpriteWidget.gameObject.GetInstanceID());
			}
			else
			{
				tSpriteWidget 	= tNewGo.GetComponent<UISlicedSprite>();
			}
		}
		else
		{
			tSpriteWidget 						= NGUITools.AddWidget<UISlicedSprite>(lastRoot.gameObject);
			FastGUI.actualFastGUIOutput.references.Add(tLayerID, tSpriteWidget.gameObject.GetInstanceID());
		}
		tSprite.inner.xMin = tSprite.outer.xMin + Mathf.Floor( tWidth / 3 );
		tSprite.inner.yMin = tSprite.outer.yMin + Mathf.Floor( tHeight / 3 );
		tSprite.inner.xMax = tSprite.outer.xMax - Mathf.Floor( tWidth / 3 );
		tSprite.inner.yMax = tSprite.outer.yMax - Mathf.Floor( tHeight / 3 );
		
		tSpriteWidget.name 					= tName + " ( Sprite )";
		tSpriteWidget.atlas 				= NGUISettings.atlas;
		tSpriteWidget.spriteName 			= tTexture.name;
		tSpriteWidget.pivot 				= NGUISettings.pivot;
		tSpriteWidget.depth					= NGUITools.CalculateNextDepth(pTargetRootPannel.gameObject);
		tSpriteWidget.transform.localScale 	= new Vector3(tWidth, tHeight, 1);
		tSpriteWidget.transform.position 	= AdjustPosition( tPosX, tPosY );
		tSpriteWidget.MakePixelPerfect();
		
		return lastRoot;
	}
	
	public static Transform CreateSprite(XMLNode pNode, UIPanel pTargetRootPannel, Transform pLastAnchor, string pLastAnchorPath )
	{	
		string 	tName		= pNode.GetNode ("name>0")["_text"].ToString();
		string 	tParentName	= pNode.GetNode ("path>0")["_text"].ToString();
		float 	tPosX		= float.Parse(pNode.GetNode ("posX>0")["_text"].ToString());
		float 	tPosY		= float.Parse(pNode.GetNode ("posY>0")["_text"].ToString());
		string	tSource		= pNode.GetNode ("source>0")["_text"].ToString();
		int 	tLayerID	= int.Parse(pNode.GetNode ("layerID>0")["_text"].ToString());
		Transform lastRoot = pTargetRootPannel.transform;
		
		
		
		if(tParentName == pLastAnchorPath)
		{
			lastRoot = pLastAnchor;
		}
		else
		{
			if(pTargetRootPannel.transform.FindChild(tParentName))
				lastRoot = pTargetRootPannel.transform.FindChild(tParentName);
		}
		
		Texture2D tTexture 			= AssetDatabase.LoadAssetAtPath(FastGUI.assetFolderToBeParseed+"/Images/"+tSource, typeof(Texture2D)) as Texture2D;
		if(tTexture == null)
			return null;
		
		if(NGUISettings.atlas == null)
			NGUISettings.atlas = FastGUIAtlasManager.CreateNewAtlas();
		
		UIAtlas.Sprite tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
		if(tSprite == null)
		{
			UIAtlasMaker.AddOrUpdate(NGUISettings.atlas, tTexture);
			tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
		}
		
		UISprite tSpriteWidget = null;
		GameObject tNewGo = null;
		if(FastGUI.actualFastGUIOutput.references.ContainsKey(tLayerID))
		{
			tNewGo 			= EditorUtility.InstanceIDToObject(FastGUI.actualFastGUIOutput.references[tLayerID]) as GameObject;
			if(tNewGo == null)
			{
				FastGUI.actualFastGUIOutput.references.Remove(tLayerID);	
				//CreateSprite(pNode, pTargetRootPannel, pLastAnchor, pLastAnchorPath);
				tSpriteWidget 						= NGUITools.AddWidget<UISprite>(lastRoot.gameObject);
				FastGUI.actualFastGUIOutput.references.Add(tLayerID, tSpriteWidget.gameObject.GetInstanceID());
			}
			else
			{
				tSpriteWidget 	= tNewGo.GetComponent<UISprite>();
			}
		}
		else
		{
			tSpriteWidget 						= NGUITools.AddWidget<UISprite>(lastRoot.gameObject);
			FastGUI.actualFastGUIOutput.references.Add(tLayerID, tSpriteWidget.gameObject.GetInstanceID());
		}
		
		tSpriteWidget.name 					= tName + " ( Sprite )";
		tSpriteWidget.atlas 				= NGUISettings.atlas;
		tSpriteWidget.spriteName 			= tTexture.name;
		tSpriteWidget.pivot 				= NGUISettings.pivot;
		tSpriteWidget.depth					= NGUITools.CalculateNextDepth(pTargetRootPannel.gameObject);
		tSpriteWidget.transform.localScale 	= Vector3.one;
		tSpriteWidget.transform.position 	= AdjustPosition( tPosX, tPosY );
		tSpriteWidget.MakePixelPerfect();
		
		return lastRoot;
	}
	public static Transform CreateCheckBox(XMLNode pNode, UIPanel pTargetRootPannel,Transform pLastAnchor, string pLastAnchorPath )
	{	
		string 	tName		= pNode.GetNode ("name>0")["_text"].ToString();
		string 	tParentName	= pNode.GetNode ("path>0")["_text"].ToString();
		float 	tPosX		= float.Parse(pNode.GetNode ("posX>0")["_text"].ToString());
		float 	tPosY		= float.Parse(pNode.GetNode ("posY>0")["_text"].ToString());
		int 	tLayerID	= int.Parse(pNode.GetNode ("layerID>0")["_text"].ToString());
		
		Transform lastRoot = pTargetRootPannel.transform;
		
		
		if(tParentName == pLastAnchorPath)
		{
			lastRoot = pLastAnchor;
		}
		else
		{
			if(pTargetRootPannel.transform.FindChild(tParentName))
				lastRoot = pTargetRootPannel.transform.FindChild(tParentName);
		}
		
		if(NGUISettings.atlas == null)
			NGUISettings.atlas = FastGUIAtlasManager.CreateNewAtlas();
		
		Dictionary<string,string> arrStates = new Dictionary<string, string>();
		if(pNode.GetNode ("states>0").GetNode("background>0")!=null)
		{
			arrStates.Add("background", pNode.GetNode("states>0").GetNode("background>0")["_text"].ToString());
		}
		if(pNode.GetNode ("states>0").GetNode("checkmark>0")!=null)
		{
			arrStates.Add("checkmark", pNode.GetNode("states>0").GetNode("checkmark>0")["_text"].ToString());
		}
		
		
		
		
		UICheckbox tCheckBox = null;
		GameObject tNewGo = null;
		if(FastGUI.actualFastGUIOutput.references.ContainsKey(tLayerID))
		{
			tNewGo 		= EditorUtility.InstanceIDToObject(FastGUI.actualFastGUIOutput.references[tLayerID]) as GameObject;
			if(tNewGo == null)
			{
				FastGUI.actualFastGUIOutput.references.Remove(tLayerID);	
				return CreateCheckBox(pNode, pTargetRootPannel, pLastAnchor, pLastAnchorPath);
			}
			else
			{
				tCheckBox 	= tNewGo.GetComponent<UICheckbox>();
				
				foreach(Transform tTrans in tNewGo.transform)
				{
					DestroyImmediate(tTrans.gameObject);
				}
			}
		}
		else
		{
			tNewGo 						= new GameObject(tName);
			tNewGo.transform.parent 	= lastRoot;
			tNewGo.transform.position 	= AdjustPosition(tPosX, tPosY);
			tNewGo.transform.localScale = Vector3.one;
			
			tCheckBox					= tNewGo.AddComponent<UICheckbox>();
			
			FastGUI.actualFastGUIOutput.references.Add(tLayerID, tNewGo.GetInstanceID());
		}
		
		
		UIAtlas.Sprite tSprite = null;
		foreach(KeyValuePair<string, string> tStatesSource in arrStates)
		{
			Texture2D tTexture 			= AssetDatabase.LoadAssetAtPath(FastGUI.assetFolderToBeParseed+"/Images/"+tStatesSource.Value, typeof(Texture2D)) as Texture2D;
			if(tTexture == null)
				continue;
			
			tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
			if(tSprite == null)
			{
				UIAtlasMaker.AddOrUpdate(NGUISettings.atlas, tTexture);
				tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
			}
			
			if(tStatesSource.Key == "background")
			{
				UISlicedSprite tBackground			= NGUITools.AddWidget<UISlicedSprite>(tNewGo);
				tBackground.fillCenter				= true;
				tBackground.name 					= tSprite.name + " ( Background )";
				tBackground.atlas 					= NGUISettings.atlas;
				tBackground.spriteName 				= tSprite.name;
				tBackground.pivot 					= NGUISettings.pivot;
				tBackground.depth					= NGUITools.CalculateNextDepth(pTargetRootPannel.gameObject);
				tBackground.transform.position 		= AdjustPosition( tPosX, tPosY );
				tBackground.transform.localScale	= new Vector3(tSprite.inner.width, tSprite.inner.height, 0);
			}
			else if(tStatesSource.Key == "checkmark")
			{
				UISprite tSpriteWidget 				= NGUITools.AddWidget<UISprite>(tNewGo);
				tSpriteWidget.name 					= tSprite.name + " ( Checkmark )";
				tSpriteWidget.atlas 				= NGUISettings.atlas;
				tSpriteWidget.spriteName 			= tSprite.name;
				tSpriteWidget.pivot 				= NGUISettings.pivot;
				tSpriteWidget.depth					= NGUITools.CalculateNextDepth(pTargetRootPannel.gameObject);
				tSpriteWidget.transform.localScale 	= Vector3.one;
				tSpriteWidget.transform.position 	= AdjustPosition( tPosX, tPosY );
				tSpriteWidget.MakePixelPerfect();
				
				tCheckBox.checkSprite				= tSpriteWidget;
			}
		}
		
		NGUITools.AddWidgetCollider(tCheckBox.gameObject);
		
		return tCheckBox.transform;
	}
	public static Transform CreateSlider(XMLNode pNode, UIPanel pTargetRootPannel, Transform pLastAnchor, string pLastAnchorPath )
	{	
		string 	tName		= pNode.GetNode ("name>0")["_text"].ToString();
		string 	tParentName	= pNode.GetNode ("path>0")["_text"].ToString();
		float 	tPosX		= float.Parse(pNode.GetNode ("posX>0")["_text"].ToString());
		float 	tPosY		= float.Parse(pNode.GetNode ("posY>0")["_text"].ToString());
		int 	tLayerID	= int.Parse(pNode.GetNode ("layerID>0")["_text"].ToString());
		
		Transform lastRoot = pTargetRootPannel.transform;
		
		
		if(tParentName == pLastAnchorPath)
		{
			lastRoot = pLastAnchor;
		}
		else
		{
			if(pTargetRootPannel.transform.FindChild(tParentName))
				lastRoot = pTargetRootPannel.transform.FindChild(tParentName);
		}
		
		
		if(NGUISettings.atlas == null)
			NGUISettings.atlas = FastGUIAtlasManager.CreateNewAtlas();
		
		Dictionary<string,string> arrStates = new Dictionary<string, string>();
		if(pNode.GetNode ("states>0").GetNode("background>0")!=null)
		{
			arrStates.Add("background", pNode.GetNode("states>0").GetNode("background>0")["_text"].ToString());
		}
		if(pNode.GetNode ("states>0").GetNode("foreground>0")!=null)
		{
			arrStates.Add("foreground", pNode.GetNode("states>0").GetNode("foreground>0")["_text"].ToString());
		}
		if(pNode.GetNode ("states>0").GetNode("thumb>0")!=null)
		{
			arrStates.Add("thumb", pNode.GetNode("states>0").GetNode("thumb>0")["_text"].ToString());
		}
		
		
		
		
		UISlider tSlider = null;
		GameObject tNewGo = null;
		if(FastGUI.actualFastGUIOutput.references.ContainsKey(tLayerID))
		{
			tNewGo 			= EditorUtility.InstanceIDToObject(FastGUI.actualFastGUIOutput.references[tLayerID]) as GameObject;
			if(tNewGo == null)
			{
				FastGUI.actualFastGUIOutput.references.Remove(tLayerID);	
				return CreateCheckBox(pNode, pTargetRootPannel, pLastAnchor, pLastAnchorPath);
			}
			else
			{
				tSlider 		= tNewGo.GetComponent<UISlider>();
				
				foreach(Transform tTrans in tNewGo.transform)
				{
					DestroyImmediate(tTrans.gameObject);
				}
			}
		}
		else
		{
			tNewGo 						= new GameObject(tName);
			tNewGo.transform.parent 	= lastRoot;
			tNewGo.transform.position 	= AdjustPosition(tPosX, tPosY);
			tNewGo.transform.localScale = Vector3.one;
			
			tSlider						= tNewGo.AddComponent<UISlider>();
			
			FastGUI.actualFastGUIOutput.references.Add(tLayerID, tNewGo.GetInstanceID());
		}
		
		UIAtlas.Sprite tSprite = null;
		
		foreach(KeyValuePair<string, string> tStatesSource in arrStates)
		{
			
			Texture2D tTexture 			= AssetDatabase.LoadAssetAtPath(FastGUI.assetFolderToBeParseed+"/Images/"+tStatesSource.Value, typeof(Texture2D)) as Texture2D;
			if(tTexture == null)
				continue;
			
			tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
			if(tSprite == null)
			{
				UIAtlasMaker.AddOrUpdate(NGUISettings.atlas, tTexture);
				tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
			}
			
			if(tStatesSource.Key == "foreground")
			{
				UIFilledSprite tForeground			= NGUITools.AddWidget<UIFilledSprite>(tNewGo);
				tForeground.fillDirection			= UIFilledSprite.FillDirection.Horizontal;
				tForeground.fillAmount				= 1;
				tForeground.name 					= tSprite.name + " ( Foreground )";
				tForeground.atlas 					= NGUISettings.atlas;
				tForeground.spriteName 				= tSprite.name;
				tForeground.pivot 					= UIWidget.Pivot.Left;
				tForeground.depth					= NGUITools.CalculateNextDepth(pTargetRootPannel.gameObject);
				tForeground.transform.localScale	= Vector3.one;
				tForeground.transform.position 		= AdjustPosition( tPosX, tPosY );
				
				tForeground.MakePixelPerfect();
				
				tSlider.foreground				= tForeground.transform;
				
				
				tForeground.transform.parent.localPosition -= new Vector3(tForeground.transform.localScale.x/2f,0,0);
			}
			else if(tStatesSource.Key == "background")
			{
				UISprite tSpriteWidget 				= NGUITools.AddWidget<UISprite>(tNewGo);
				tSpriteWidget.name 					= tSprite.name + " ( Background )";
				tSpriteWidget.atlas 				= NGUISettings.atlas;
				tSpriteWidget.spriteName 			= tSprite.name;
				tSpriteWidget.pivot 				= UIWidget.Pivot.Left;
				tSpriteWidget.depth					= NGUITools.CalculateNextDepth(pTargetRootPannel.gameObject);
				tSpriteWidget.transform.localScale 	= Vector3.one;
				tSpriteWidget.transform.position 	= AdjustPosition( tPosX, tPosY );
				tSpriteWidget.MakePixelPerfect();
			}
			else if(tStatesSource.Key == "thumb")
			{	
				UISprite tSpriteWidget 				= NGUITools.AddWidget<UISprite>(tNewGo);
				tSpriteWidget.name 					= tSprite.name + " ( Thumb )";
				tSpriteWidget.atlas 				= NGUISettings.atlas;
				tSpriteWidget.spriteName 			= tSprite.name;
				tSpriteWidget.pivot 				= NGUISettings.pivot;
				tSpriteWidget.depth					= NGUITools.CalculateNextDepth(pTargetRootPannel.gameObject);
				tSpriteWidget.transform.localScale 	= Vector3.one;
				tSpriteWidget.transform.position 	= AdjustPosition( tPosX, tPosY );
				tSlider.thumb						= tSpriteWidget.transform;
				
				tSpriteWidget.MakePixelPerfect();
			}
		}
		
		NGUITools.AddWidgetCollider(tSlider.gameObject);
		
		return tSlider.transform;
	}
	public static Transform CreateProgressBar(XMLNode pNode, UIPanel pTargetRootPannel, Transform pLastAnchor, string pLastAnchorPath )
	{	
		string 	tName		= pNode.GetNode ("name>0")["_text"].ToString();
		string 	tParentName	= pNode.GetNode ("path>0")["_text"].ToString();
		float 	tPosX		= float.Parse(pNode.GetNode ("posX>0")["_text"].ToString());
		float 	tPosY		= float.Parse(pNode.GetNode ("posY>0")["_text"].ToString());
		int 	tLayerID	= int.Parse(pNode.GetNode ("layerID>0")["_text"].ToString());
		
		Transform lastRoot = pTargetRootPannel.transform;
		
		
		if(pTargetRootPannel.transform.FindChild(tParentName))
			lastRoot = pTargetRootPannel.transform.FindChild(tParentName);
		
		
		if(NGUISettings.atlas == null)
			NGUISettings.atlas = FastGUIAtlasManager.CreateNewAtlas();
		
		Dictionary<string,string> arrStates = new Dictionary<string, string>();
		
		if(pNode.GetNode ("states>0").GetNode("background>0")!=null)
		{
			arrStates.Add("background", pNode.GetNode("states>0").GetNode("background>0")["_text"].ToString());
		}
		if(pNode.GetNode ("states>0").GetNode("foreground>0")!=null)
		{
			arrStates.Add("foreground", pNode.GetNode("states>0").GetNode("foreground>0")["_text"].ToString());
		}
		
		
		UISlider tSlider = null;
		GameObject tNewGo = null;
		if(FastGUI.actualFastGUIOutput.references.ContainsKey(tLayerID))
		{
			tNewGo = EditorUtility.InstanceIDToObject(FastGUI.actualFastGUIOutput.references[tLayerID]) as GameObject;
			if(tNewGo == null)
			{
				FastGUI.actualFastGUIOutput.references.Remove(tLayerID);	
				return CreateCheckBox(pNode, pTargetRootPannel, pLastAnchor, pLastAnchorPath);
			}
			else
			{
				tSlider = tNewGo.GetComponent<UISlider>();
				
				foreach(Transform tTrans in tNewGo.transform)
				{
					DestroyImmediate(tTrans.gameObject);
				}
			}
		}
		else
		{
			tNewGo 						= new GameObject(tName);
			tNewGo.transform.parent 	= lastRoot;
			tNewGo.transform.position 	= AdjustPosition(tPosX, tPosY);
			tNewGo.transform.localScale = Vector3.one;
			
			tSlider						= tNewGo.AddComponent<UISlider>();
			
			FastGUI.actualFastGUIOutput.references.Add(tLayerID, tNewGo.GetInstanceID());
		}
		
		UIAtlas.Sprite tSprite = null;
		
		foreach(KeyValuePair<string, string> tStatesSource in arrStates)
		{
			
			Texture2D tTexture 			= AssetDatabase.LoadAssetAtPath(FastGUI.assetFolderToBeParseed+"/Images/"+tStatesSource.Value, typeof(Texture2D)) as Texture2D;
			if(tTexture==null)
				continue;
			
			tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
			if(tSprite == null)
			{
				UIAtlasMaker.AddOrUpdate(NGUISettings.atlas, tTexture);
				tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
			}
			
			if(tStatesSource.Key == "foreground")
			{
				UIFilledSprite tForeground			= NGUITools.AddWidget<UIFilledSprite>(tNewGo);
				tForeground.fillDirection			= UIFilledSprite.FillDirection.Horizontal;
				tForeground.fillAmount				= 1;
				tForeground.name 					= tSprite.name + " ( Foreground )";
				tForeground.atlas 					= NGUISettings.atlas;
				tForeground.spriteName 				= tSprite.name;
				tForeground.pivot 					= UIWidget.Pivot.Left;
				tForeground.depth					= NGUITools.CalculateNextDepth(pTargetRootPannel.gameObject);
				tForeground.transform.localScale	= Vector3.one;
				tForeground.transform.position 		= AdjustPosition( tPosX, tPosY );
				
				tForeground.MakePixelPerfect();
				
				tSlider.foreground					= tForeground.transform;
				
				
				tForeground.transform.parent.localPosition -= new Vector3(tForeground.transform.localScale.x/2f,0,0);
			}
			else if(tStatesSource.Key == "background")
			{
				UISprite tSpriteWidget 				= NGUITools.AddWidget<UISprite>(tNewGo);
				tSpriteWidget.name 					= tSprite.name + " ( Background )";
				tSpriteWidget.atlas 				= NGUISettings.atlas;
				tSpriteWidget.spriteName 			= tSprite.name;
				tSpriteWidget.pivot 				= UIWidget.Pivot.Left;
				tSpriteWidget.depth					= NGUITools.CalculateNextDepth(pTargetRootPannel.gameObject);
				tSpriteWidget.transform.localScale 	= Vector3.one;
				tSpriteWidget.transform.position 	= AdjustPosition( tPosX, tPosY );
				tSpriteWidget.MakePixelPerfect();
			}
		}
		
		return tSlider.transform;
	}
	public static Transform CreateImageButton(XMLNode pNode, UIPanel pTargetRootPannel, Transform pLastAnchor, string pLastAnchorPath )
	{	
		string 	tName		= pNode.GetNode ("name>0")["_text"].ToString();
		string 	tParentName	= pNode.GetNode ("path>0")["_text"].ToString();
		float 	tPosX		= float.Parse(pNode.GetNode ("posX>0")["_text"].ToString());
		float 	tPosY		= float.Parse(pNode.GetNode ("posY>0")["_text"].ToString());
		int 	tLayerID	= int.Parse(pNode.GetNode ("layerID>0")["_text"].ToString());
		
		
		Transform lastRoot = pTargetRootPannel.transform;
		
		
		if(tParentName == pLastAnchorPath)
		{
			lastRoot = pLastAnchor;
		}
		else
		{
			if(pTargetRootPannel.transform.FindChild(tParentName))
				lastRoot = pTargetRootPannel.transform.FindChild(tParentName);
		}
		
		if(NGUISettings.atlas == null)
			NGUISettings.atlas = FastGUIAtlasManager.CreateNewAtlas();
		
		Dictionary<string,string> arrStates = new Dictionary<string, string>();
		if(pNode.GetNode ("states>0").GetNode("pressed>0")!=null)
		{
			arrStates.Add("pressed", pNode.GetNode("states>0").GetNode("pressed>0")["_text"].ToString());
		}
		if(pNode.GetNode ("states>0").GetNode("idle>0")!=null)
		{
			arrStates.Add("idle", pNode.GetNode("states>0").GetNode("idle>0")["_text"].ToString());
		}
		if(pNode.GetNode ("states>0").GetNode("hover>0")!=null)
		{
			arrStates.Add("hover", pNode.GetNode("states>0").GetNode("hover>0")["_text"].ToString());
		}
		if(pNode.GetNode ("states>0").GetNode("disabled>0") != null)
		{
			arrStates.Add("disabled", pNode.GetNode("states>0").GetNode("disabled>0")["_text"].ToString());
		}
		
		
		
		UIImageButton tImageButton = null;
		GameObject tNewGo = null;
		if(FastGUI.actualFastGUIOutput.references.ContainsKey(tLayerID))
		{
			tNewGo 	= EditorUtility.InstanceIDToObject(FastGUI.actualFastGUIOutput.references[tLayerID]) as GameObject;
			if(tNewGo == null)
			{
				FastGUI.actualFastGUIOutput.references.Remove(tLayerID);	
				return CreateImageButton(pNode, pTargetRootPannel, pLastAnchor, pLastAnchorPath);
			}
			else
			{
				tImageButton 	= tNewGo.GetComponent<UIImageButton>();
				foreach(Transform tTrans in tNewGo.transform)
				{
					DestroyImmediate(tTrans.gameObject);
				}
			}
		}
		else
		{
			tNewGo 						= new GameObject(tName);
			tNewGo.transform.parent 	= lastRoot;
			tNewGo.transform.position 	= AdjustPosition(tPosX, tPosY);
			tNewGo.transform.localScale = Vector3.one;
			
			tImageButton				= tNewGo.AddComponent<UIImageButton>();
			
			FastGUI.actualFastGUIOutput.references.Add(tLayerID, tNewGo.GetInstanceID());
		}
				
		UIAtlas.Sprite tSprite = null;
		foreach(KeyValuePair<string, string> tStatesSource in arrStates)
		{
			Texture2D tTexture 			= AssetDatabase.LoadAssetAtPath(FastGUI.assetFolderToBeParseed+"/Images/"+tStatesSource.Value, typeof(Texture2D)) as Texture2D;
			if(tTexture==null)
				continue;
			
			tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
			if(tSprite == null)
			{
				UIAtlasMaker.AddOrUpdate(NGUISettings.atlas, tTexture);
				tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
				
				if(tSprite==null)
					continue;
			}
			
			if(tStatesSource.Key == "idle")
			{
				tImageButton.normalSprite = tSprite.name;
			}
			else if(tStatesSource.Key == "pressed")
			{
				tImageButton.pressedSprite = tSprite.name;
			}
			else if(tStatesSource.Key == "hover")
			{
				tImageButton.hoverSprite = tSprite.name;
			}
		}
		
		UISprite tSpriteWidget 				= NGUITools.AddWidget<UISprite>(tNewGo);
		tSpriteWidget.name 					= tName + " ( Button States )";
		tSpriteWidget.atlas 				= NGUISettings.atlas;
		tSpriteWidget.spriteName 			= tImageButton.normalSprite;
		tSpriteWidget.pivot 				= NGUISettings.pivot;
		tSpriteWidget.depth					= NGUITools.CalculateNextDepth(pTargetRootPannel.gameObject);
		tSpriteWidget.transform.localScale 	= Vector3.one;
		tSpriteWidget.transform.position 	= AdjustPosition( tPosX, tPosY );
		tSpriteWidget.MakePixelPerfect();
		
		
		NGUITools.AddWidgetCollider(tImageButton.gameObject);
		
		tImageButton.target = tSpriteWidget;
		
		return tImageButton.transform;
	}
	
	public static Transform CreateSlicedImageButton(XMLNode pNode, UIPanel pTargetRootPannel, Transform pLastAnchor, string pLastAnchorPath )
	{	
		string 	tName		= pNode.GetNode ("name>0")["_text"].ToString();
		string 	tParentName	= pNode.GetNode ("path>0")["_text"].ToString();
		float 	tPosX		= float.Parse(pNode.GetNode ("posX>0")["_text"].ToString());
		float 	tPosY		= float.Parse(pNode.GetNode ("posY>0")["_text"].ToString());
		float 	tWidth		= float.Parse(pNode.GetNode ("width>0")["_text"].ToString());
		float 	tHeight		= float.Parse(pNode.GetNode ("height>0")["_text"].ToString());
		int 	tLayerID	= int.Parse(pNode.GetNode ("layerID>0")["_text"].ToString());
		
		
		Transform lastRoot = pTargetRootPannel.transform;
		
		
		if(tParentName == pLastAnchorPath)
		{
			lastRoot = pLastAnchor;
		}
		else
		{
			if(pTargetRootPannel.transform.FindChild(tParentName))
				lastRoot = pTargetRootPannel.transform.FindChild(tParentName);
		}
		
		if(NGUISettings.atlas == null)
			NGUISettings.atlas = FastGUIAtlasManager.CreateNewAtlas();
		
		Dictionary<string,string> arrStates = new Dictionary<string, string>();
		if(pNode.GetNode ("states>0").GetNode("pressed>0")!=null)
		{
			arrStates.Add("pressed", pNode.GetNode("states>0").GetNode("pressed>0")["_text"].ToString());
		}
		if(pNode.GetNode ("states>0").GetNode("idle>0")!=null)
		{
			arrStates.Add("idle", pNode.GetNode("states>0").GetNode("idle>0")["_text"].ToString());
		}
		if(pNode.GetNode ("states>0").GetNode("hover>0")!=null)
		{
			arrStates.Add("hover", pNode.GetNode("states>0").GetNode("hover>0")["_text"].ToString());
		}
		if(pNode.GetNode ("states>0").GetNode("disabled>0") != null)
		{
			arrStates.Add("disabled", pNode.GetNode("states>0").GetNode("disabled>0")["_text"].ToString());
		}
		
		
		
		UIImageButton tImageButton = null;
		GameObject tNewGo = null;
		if(FastGUI.actualFastGUIOutput.references.ContainsKey(tLayerID))
		{
			tNewGo 	= EditorUtility.InstanceIDToObject(FastGUI.actualFastGUIOutput.references[tLayerID]) as GameObject;
			if(tNewGo == null)
			{
				FastGUI.actualFastGUIOutput.references.Remove(tLayerID);	
				return CreateSlicedImageButton(pNode, pTargetRootPannel, pLastAnchor, pLastAnchorPath);
			}
			else
			{
				tImageButton 	= tNewGo.GetComponent<UIImageButton>();
				foreach(Transform tTrans in tNewGo.transform)
				{
					DestroyImmediate(tTrans.gameObject);
				}
			}
		}
		else
		{
			tNewGo 						= new GameObject(tName);
			tNewGo.transform.parent 	= lastRoot;
			tNewGo.transform.position 	= AdjustPosition(tPosX, tPosY);
			tNewGo.transform.localScale = Vector3.one;
			
			tImageButton				= tNewGo.AddComponent<UIImageButton>();
			
			FastGUI.actualFastGUIOutput.references.Add(tLayerID, tNewGo.GetInstanceID());
		}
				
		UIAtlas.Sprite tSprite = null;
		foreach(KeyValuePair<string, string> tStatesSource in arrStates)
		{
			Texture2D tTexture 			= AssetDatabase.LoadAssetAtPath(FastGUI.assetFolderToBeParseed+"/Images/"+tStatesSource.Value, typeof(Texture2D)) as Texture2D;
			if(tTexture == null)
				continue;
			
			tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
			if(tSprite == null)
			{
				UIAtlasMaker.AddOrUpdate(NGUISettings.atlas, tTexture);
				tSprite = NGUISettings.atlas.GetSprite(tTexture.name);
				
				tSprite.inner.xMin = tSprite.outer.xMin + Mathf.Floor( tWidth / 3 );
				tSprite.inner.yMin = tSprite.outer.yMin + Mathf.Floor( tHeight / 3 );
				tSprite.inner.xMax = tSprite.outer.xMax - Mathf.Floor( tWidth / 3 );
				tSprite.inner.yMax = tSprite.outer.yMax - Mathf.Floor( tHeight / 3 );
			}
			
			if(tStatesSource.Key == "idle")
			{
				tImageButton.normalSprite = tSprite.name;
			}
			else if(tStatesSource.Key == "pressed")
			{
				tImageButton.pressedSprite = tSprite.name;
			}
			else if(tStatesSource.Key == "hover")
			{
				tImageButton.hoverSprite = tSprite.name;
			}
		}
		
		UISlicedSprite tSpriteWidget 		= NGUITools.AddWidget<UISlicedSprite>(tNewGo);		
		tSpriteWidget.name 					= tName + " ( Button States )";
		tSpriteWidget.atlas 				= NGUISettings.atlas;
		tSpriteWidget.spriteName 			= tImageButton.normalSprite;
		tSpriteWidget.pivot 				= NGUISettings.pivot;
		tSpriteWidget.depth					= NGUITools.CalculateNextDepth(pTargetRootPannel.gameObject);
		tSpriteWidget.transform.localScale 	= new Vector3(tWidth,tHeight,1);
		tSpriteWidget.transform.position 	= AdjustPosition( tPosX, tPosY );
		tSpriteWidget.MakePixelPerfect();
		
		
		NGUITools.AddWidgetCollider(tImageButton.gameObject);
		
		tImageButton.target = tSpriteWidget;
		
		return tImageButton.transform;
	}
	
	public static Transform CreateTextLabel( XMLNode pNode, UIPanel pTargetRootPannel, Transform pLastAnchor, string pLastAnchorPath )
	{
		// Node parameters:
		string	tFontName	= pNode.GetNode ("fontName>0")["_text"].ToString();
		string	tFontColor	= pNode.GetNode ("fontColor>0")["_text"].ToString(); 
		string	tContent	= pNode.GetNode ("contents>0")["_text"].ToString();
		
		UILabel tLabel;
		Transform tContainer = CreateAnchor( pNode, pTargetRootPannel, pLastAnchor, pLastAnchorPath );
		
		// Process:
		UIFont tPrefabFont = GetFont( tFontName );
		if(tPrefabFont == null)
		{
			tPrefabFont = FastGUIAtlasManager.CreateNewFont(tFontName);
		}
		
		// Create Label:
		tLabel = tContainer.gameObject.AddComponent<UILabel>();
		
		// Set Position:
		tLabel.transform.localPosition	= tContainer.localPosition;

		string[] tColor = tFontColor.Split(';');
		tLabel.color	= new Color(float.Parse(tColor[0])/255f,float.Parse(tColor[1])/255f,float.Parse(tColor[2])/255f);
		tLabel.font 	= tPrefabFont;
		tLabel.text		= tContent;
		tLabel.depth	= NGUITools.CalculateNextDepth(pTargetRootPannel.gameObject);;
		tLabel.pivot	= UIWidget.Pivot.Center;
		
		tLabel.MakePixelPerfect();
		tLabel.MakePositionPerfect();
		
		return tContainer;
	}
	
	static public Transform CreateTextInput( XMLNode pNode, UIPanel pTargetRootPannel, Transform pLastAnchor, string pLastAnchorPath )
	{
		// Node parameters:
		string	tFontColor	= pNode.GetNode ("fontColor>0")["_text"].ToString();

		Transform 	tContainer  = CreateTextLabel( pNode, pTargetRootPannel, pLastAnchor, pLastAnchorPath );
		UILabel		tLabel		= tContainer.GetComponent<UILabel>();
	
		// Create Input:
		GameObject	tInputContainer		= new GameObject();
		tInputContainer.transform.parent = tContainer.parent;
		UIInput tInput 					= tInputContainer.AddComponent<UIInput>();
		BoxCollider tBox 				= tInputContainer.AddComponent<BoxCollider>();
		tBox.size 						= new Vector3( tLabel.relativeSize.x * tContainer.localScale.x, tContainer.localScale.y, 1 );;
		tInput.transform.localScale		= Vector3.one;
		tInput.transform.localPosition	= tContainer.localPosition;
		tInput.name						= tContainer.name;
		tContainer.name					= "label";
		tContainer.parent 				= tInputContainer.transform;
		tContainer.localPosition		= Vector3.zero;
		tInput.label					= tLabel;
		
		string[] tColor = tFontColor.Split(';');
		tInput.activeColor = new Color(float.Parse(tColor[0])/255f,float.Parse(tColor[1])/255f,float.Parse(tColor[2])/255f);
		
		return tContainer;
	}
	
	static public UIFont GetFont(string pFontName)
	{
		string[] tPaths = Directory.GetFiles(Application.dataPath+"/", pFontName + ".prefab", SearchOption.AllDirectories );
		
		
		foreach ( var tPath in tPaths )
		{
			string projectRelativePath = tPath.Substring(tPath.ToString().IndexOf("Assets/"));
			UIFont tGO = AssetDatabase.LoadAssetAtPath(projectRelativePath, typeof(UIFont)) as UIFont;
			
			return tGO;
		}
		return null;
	}
}
