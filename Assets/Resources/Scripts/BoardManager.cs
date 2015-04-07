using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour 
{
	public GameManager mgr;
	public static Material [] factionEdgeMaterial;
	public readonly GameObject [][] edges;  //not a square ... twice as many rows as cols
	
	//Changes mesh
	public void setMesh(int startCol, int startRow, int squareLength, int faction)
	{
		int numCols = startCol + squareLength;
		int numRows = startRow + squareLength*2;
		
		//Top Row Mesh
		for (int col = startCol; col < numCols; col++)
		{
			Renderer temp = edges[startRow][col].GetComponent<Renderer>();
			temp.material = factionEdgeMaterial[faction];
		}
		
		//Bottom Row Mesh
		for (int col = startCol; col < numCols; col++)
		{
			Renderer temp = edges[numRows][col].GetComponent<Renderer>();
			temp.material = factionEdgeMaterial[faction];
		}
		
		//Left Column
		for (int row = startRow+1; row < numRows; row+=2)
		{
			Renderer temp = edges[row][startCol].GetComponent<Renderer>();
			temp.material = factionEdgeMaterial[faction];
		}
		
		//Right Column
		for (int row  = startRow+1; row < numRows; row+=2)
		{
			Renderer temp = edges[row][numCols].GetComponent<Renderer>();
			temp.material = factionEdgeMaterial[faction];
		}
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
