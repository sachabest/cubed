using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Prime31;

public class GameManager : MonoBehaviour 
{
	public GameBoard board;
	public GameObject myBoardSphere;
	public bool rolledATwo, mouseUI;
	public int tier, roll, winningScore;
	public bool singleplayer = true;
	private PlayerManager.Faction humanFactionChoice;
	public AI AI;
	private int[] playerScores;
	private Stack<Move> moves;
	public PieceManager pieceManager;
	public GCCubedListener gameCenter;
	public SaveLoadManager saveLoad;
	private bool loadingMoves, previousTurnEnded, hasMovedThisTurn, hasRolled;
	private Move topOfStack;
	private bool escapeAttempt = false;
	private float gameTimer = 0.0f;
	private string tempEscapeText = "";
	private bool inWinningSequence = false;
	public Camera WinCamera, MainCamera;
	public GameInfo gameInfo;
	public bool GameIsOver;
	public GameObject WinScreenCube;
	private List<int> territoriesScoringPointsForCurrentFaction; //row,col,length
	private int[,] promotedTerritoryFormerTierValues; //faction *10 + tier value
	public Light winCameraLight;
	public float lifeWinsIntensity = 0.5f, industryWinsIntensity = 0.7f;
	
	public PromotionSmash promotionSmash;

	public AudioSource industry, life;

	public PlayerManager.Faction currentFaction;

	public static GameManager instance;

	// Use this for initialization
	void Awake () 
	{
		// set for singleton pattern
		instance = this;

		territoriesScoringPointsForCurrentFaction = new List<int>();

		// for undo capability
		promotedTerritoryFormerTierValues = new int[12,12]; //faction *10 + tier value

		// disable the win camera at start
		WinCamera.camera.enabled = false;

		SetTier(1);

		// create array to hold player scores
		playerScores = new int[3];

		// to hold moves for undo, sserialize, etc.
		moves = new Stack<Move>();

		gameInfo = GameInfo.instance;
		singleplayer = gameInfo.getSinglePlayer();
		winningScore = gameInfo.getWinCondition();

		// if there is an AI playing, make it
		if (singleplayer) {
			AI = new AI(winningScore);

			humanFactionChoice = gameInfo.getHumanFactionChoice();

			// set the current player to the human
			currentFaction = PlayerManager.Faction.Uninitialized;
		}

		gameCenter = GCCubedListener.instance;
		saveLoad = SaveLoadManager.instance;
		if (gameCenter != null) {
			GameCenterAwake();
		}

		playAudio (humanFactionChoice);
		// log human faction for testing
		Debug.Log (humanFactionChoice);
	}

	// Refactored from Awake
	void GameCenterAwake() {
		if (!singleplayer) {
			moves = saveLoad.moves;
			humanFactionChoice = gameCenter.getLocalFaction();
			if (moves != null && moves.Count > 0) {
				if (gameCenter.myTurn()) {
					currentFaction = humanFactionChoice;
				} else {
					ButtonManager.instance.rollButtonText.text = "Back";
					currentFaction = PlayerManager.SwitchFaction(humanFactionChoice);
				}
				topOfStack = (Move) moves.Peek();
				previousTurnEnded = (topOfStack.getCol() == -1) && (topOfStack.getRow() == -1) && (topOfStack.getTier() == -1);
				hasMovedThisTurn = false;
				hasRolled = false;
				Debug.Log("Faction that last ended turn (Life/Industry) (1/2): " + (int) PlayerManager.SwitchFaction(currentFaction));
			}
			else { // first turn
				Debug.Log("No move stack. Assuming first turn/");
				currentFaction = PlayerManager.Faction.Uninitialized;
			}
		}
	}

	void Start () {
		Debug.Log("Setting player names on UI");
		// this needs to be here or the buttons won't have been created yet
		ButtonManager.instance.lifeName.text = gameInfo.lifeUser;
		ButtonManager.instance.industryName.text = gameInfo.industryUser;
	}
	void playAudio(PlayerManager.Faction faction) {
		if (faction == PlayerManager.Faction.Life) 
			life.Play ();
		else if (faction == PlayerManager.Faction.Industry)
			industry.Play ();
	}
	// This prevents the game from misintrepreting drag motions as clicks on the board
	void OnDrag(DragGesture gesture) { 
		if (gesture.Phase == ContinuousGesturePhase.Started)
			mouseUI = true;
		else if (gesture.Phase == ContinuousGesturePhase.Ended)
			mouseUI = false;
		Debug.Log(mouseUI);
	}
	public Stack<Move> GetMoves() {
		return moves;
	}
	
	void Update () 
	{
		if (inWinningSequence)
		{	
			gameTimer += Time.deltaTime;
			if (gameTimer > 3.0f)
			{
				if(playerScores[(int) PlayerManager.Faction.Life] > winningScore)
				{
					winCameraLight.intensity = lifeWinsIntensity;
					WinScreenCube.renderer.material = (Material)Resources.Load ("Materials/LifeWIN");

				}
				else if(playerScores[(int) PlayerManager.Faction.Industry] > winningScore)
				{
					winCameraLight.intensity = industryWinsIntensity;
					WinScreenCube.renderer.material = (Material)Resources.Load ("Materials/IndustryWIN");	
				}
				if (!singleplayer) {
					GCCubedListener.instance.Win();
				}

		//		PlayerIndicator.text = "Game Over.  Press Esc for Menu.";
				GameIsOver = true;
				WinCamera.enabled = true;
				WinScreenCube.audio.Play ();
				inWinningSequence = false;
				gameTimer = 0.0f;
			}
		}
	}
	/*void OnApplicationQuit() {
		if (gameCenter != null)
			gameCenter.SaveTurn(saveLoad.CreateJSONGameStateString(moves));
	}*/
	public bool Undo ()
	{		
		if(GameIsOver)
		{
			return false;
		}
		if(moves.Count == 0)
		{
			return false;
		}
		Move move = (Move)moves.Pop();
		PlayerManager.Faction faction = move.getFaction();
		if(faction != currentFaction)
		{
			moves.Push(move);
			Debug.Log ("Not your move to undo BITCH");
			return false;
		}
		int tier = move.getTier();
		if (faction == currentFaction && tier == 0) // tier == 0, thus I need to undo a remover -> replace
		{
			int row = move.getRow();
			int col = move.getCol();
			int removedTier = move.getRemovedTier();
			
			//GameSquare square = board.GSArray[row,col];
			
			this.currentFaction = PlayerManager.SwitchFaction(this.currentFaction);
			roll += removedTier*3;
			this.tier = removedTier;
			HandleClick(row, col);
			this.tier = 0;
			rolledATwo = true;
			this.currentFaction = PlayerManager.SwitchFaction(this.currentFaction);
			
			Debug.Log("Undo TRUE");
			return true;
		}
		else if (faction == currentFaction && tier !=0) 
		{
			int row = move.getRow();
			int col = move.getCol();
			GameSquare square = board.GSArray[row,col]; 
			
			
			if(board.Remove (row, col, tier,PlayerManager.SwitchFaction(faction)))
			{	
				
				square.DestroyPiece();

				board.Reserve(square.xval, square.yval);
				square._tier = 0;
				roll += 3*tier;
				ButtonManager.instance.rollDisplay.text = roll.ToString();
				Debug.Log("Remove Successful");
				Debug.Log("Undo TRUE");
				return true;
			}
			Debug.Log ("Undo FALSE");
			return false;
			
			
		}
		else
		{
			Debug.Log ("Undo FALSE");
			return false;		
		}
	}
	public void HandleClick(int xval, int yval)
	{
		if (GameIsOver)
		{
			return;
		}
		GameSquare square = board.GSArray[xval,yval];
		//gamecenter
		hasMovedThisTurn = true;
		
		if(tier == 0 || (rolledATwo && square.faction != currentFaction))
		{
			if(Remove(xval, yval, square._tier))
			{
				roll = roll - 2;
				square.DestroyPiece();
				board.Reserve(square.xval, square.yval);  //sets the image for what level of reservation should be displayed
				square._tier = 0;
				updateRemoveModels(xval,yval,square._tier);
			}
			
		}
		else if(PurchasePiece(xval, yval))    
		{
			square.SetColor(currentFaction, tier);                                                   
			Move move = new Move(currentFaction, xval, yval, tier);
			moves.Push(move);
			Debug.Log("Promoting for " + currentFaction);
			PromoteNewPieces(currentFaction);                              
			myBoardSphere.GetComponent<CameraSphere>().zoomToSquare(square);
		}
		ButtonManager.instance.rollDisplay.text = roll.ToString();
	}
	private void updateRemoveModels(int row, int col, int removedTier)
	{
		if (removedTier == 4)
			promotedTerritoryFormerTierValues[row,col] = 0;
		
		//Make sure opponent's tier 4 pieces are removed appropriately
		int[,] gridCopy = new int[board.Board.GetLength(0),board.Board.GetLength(1)];
		for(int i = 0; i < board.Board.GetLength(0); i++)
			for(int j = 0; j < board.Board.GetLength(1); j++)
				gridCopy[i,j] = (int) board.Board[i,j];
		PlayerManager.Faction opponent = PlayerManager.SwitchFaction (currentFaction);
		int newOpponentScore = totalCount ( gridCopy, (int) opponent,0,0,new List<int>(30));
		Debug.Log ("promoting for opponent");
		PromoteNewPieces (opponent);
		if (opponent == PlayerManager.Faction.Life)
			ButtonManager.instance.lifeScore.text = "" + newOpponentScore;
		else
			ButtonManager.instance.industryScore.text = "" + newOpponentScore;
	}
	public void NextTurn()
	{
		// promotionSmash.smash();
		Debug.Log ("Next Turn Clicked");
		Debug.Log ("CurrentPlayer: " + currentFaction + ". Has moved? " + hasMovedThisTurn + ". Has Rolled? " + hasRolled);
		if (GameIsOver)
			return;
		// first turn
		if (currentFaction == PlayerManager.Faction.Uninitialized) {
			hasRolled = true;
			roll = (UnityEngine.Random.Range(1, 7) + UnityEngine.Random.Range(1,7));
			currentFaction = humanFactionChoice;
			Debug.Log("Returning, first turn - adding roll");
			ButtonManager.instance.rollDisplay.text = roll.ToString();
			ButtonManager.instance.rollButtonText.text = "Lock";
			return;
		}
		// pushes empty move to the stack to signify the end of a turn - player references the player that just played
		// This is the case of GameCenter Multiplayer
		if (gameCenter != null  && !singleplayer && !loadingMoves) {
			if (!gameCenter.myTurn()) {
				gameCenter.LoadLevel("MainMenuV2");
				return;
			}
			else if (hasRolled) {
				if (hasMovedThisTurn) {
					moves.Push(new Move(currentFaction, -1, -1, -1));
					Debug.Log("GameCenter " + gameCenter);
					Debug.Log("SaveLoad " + saveLoad);
					hasMovedThisTurn = false;
					hasRolled = false;
					gameCenter.EndTurn(saveLoad.CreateJSONGameStateString(GetMoves()));
					gameCenter.LoadLevel("MainMenuV2");
					GameCenterTurnBasedBinding.loadMatches();
					return;
				}
				else {
					string[] options = { "OK" };
					EtceteraBinding.showAlertWithTitleMessageAndButtons("Cannot End Turn...", "You diddn't move!", options);
					return;
				}
			}
		}

		// handle AI roll (cannot remove)
		if (singleplayer && currentFaction == humanFactionChoice) {
			roll = (UnityEngine.Random.Range(2, 7) + UnityEngine.Random.Range(1,7));
		} else {
			roll = (UnityEngine.Random.Range(1, 7) + UnityEngine.Random.Range(1,7));
		}

		hasRolled = true;

		// show a roll on the screen
		ButtonManager.instance.rollDisplay.text = roll.ToString();

		if (roll == 2)
			rolledATwo = true;
		else
			rolledATwo = false;

		// reset current tier selection
		tier = 1;

		// change turns
		currentFaction = PlayerManager.SwitchFaction (currentFaction);

		// let the AI play if it should
		if (singleplayer && currentFaction != humanFactionChoice) {
			AI.Play(currentFaction, board, roll);
			ButtonManager.instance.rollButtonText.text = "Roll";
		} else if (singleplayer) {
			ButtonManager.instance.rollButtonText.text = "Lock";

		}
	}

	public bool PurchasePiece(int x, int y)
	{
		bool output = false;
		roll -= (3*tier);
		if(roll < 0 || !board.GetPlayable(x, y, currentFaction))
		{
			roll += (3*tier);
			Debug.Log ("Nope");
			return false;
		}
		if(board.GetPlayable(x, y, currentFaction))
		{
			board.Set(x, y, (int) currentFaction);
			output = true;
		}
		if(tier==2)
		{
			/*
			try
			{
				board.SetPlayable(x+1, y, player);
				board.Reserve(x+1, y);
				board.SetPlayable(x-1, y, player);
				board.Reserve(x-1, y);
				board.SetPlayable(x, y+1, player);
				board.Reserve(x, y+1);
				board.SetPlayable(x, y-1, player);
				board.Reserve(x, y-1);
			}
			catch(Exception e){}
			*/
			#pragma warning disable
			try
			{
				board.SetPlayable(x+1, y, currentFaction);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x+1, y);
			}
			catch(Exception e){}
			try
			{
				board.SetPlayable(x-1, y, currentFaction);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x-1, y);
			}
			catch(Exception e){}
			try
			{
				board.SetPlayable(x, y+1, currentFaction);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x, y+1);
			}
			catch(Exception e){}
			try
			{
				board.SetPlayable(x, y-1, currentFaction);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x, y-1);
			}
			catch(Exception e){}	
			#pragma warning enable
		}
		else if(tier == 3)
		{
			for(int i = x-1; i <= x+1; i++)
			{
				for(int j = y-1; j <= y+1; j++)
				{
					try
					{
						board.SetPlayable(i,j,currentFaction);
						board.Reserve (i, j);
					}
					catch(Exception e)
					{
						continue;
					}
				}
			}
		}
		Debug.Log("Can purchase piece: tier " +tier+", player "+currentFaction+": "+output);
		
		int[,] boardcopy = new int[12,12];
		Array.Copy(board.Board, boardcopy, 144);
		//Debug.Log ("Score" + totalCount(boardcopy, player, 0, 0));
		
		int newScore = totalCount(boardcopy, (int) currentFaction, 0, 0, new List<int>(30));
		Debug.Log ("Score: " + newScore);
		playerScores[(int) currentFaction] = newScore;
		if(newScore >= winningScore)
		{
			Win();
		}
		ButtonManager.instance.lifeScore.text =  playerScores[(int)PlayerManager.Faction.Life].ToString() + "/" + winningScore;
		ButtonManager.instance.industryScore.text =  playerScores[(int)PlayerManager.Faction.Industry].ToString() + "/" + winningScore;
		return output;
	}
	public void SetTier(int newTier)
	{
		this.tier = newTier;
	}
	public bool Remove(int x, int y, int _tier)
	{
		
		if(rolledATwo)
		{
			if (_tier == 4)
			{
				PlayerManager.Faction opponent = PlayerManager.SwitchFaction(currentFaction);
				board.Remove (x,y,_tier,currentFaction); //pass in the player who is removing the piece
				board.GSArray[x,y].DestroyPiece();
				_tier = promotedTerritoryFormerTierValues[x,y]%10;
				promotedTerritoryFormerTierValues[x,y] = 0;
				board.Set (x,y,_tier);
				board.GSArray[x,y].SetColor(opponent, _tier);
			}
	
			if(board.Remove (x, y, _tier, currentFaction))
			{
				rolledATwo = false;
				Move move = new Move(currentFaction, x, y, 0, _tier);
				moves.Push (move);

				return true;
			}
		}
		return false;
	}
	public int getPlayerScore(PlayerManager.Faction factionToGet)
	{
		return playerScores[(int) factionToGet];
	}
	public void Win()
	{
		inWinningSequence = true; //see Update() method
		gameTimer = 0.0f;
	}
	//Returns the row,col,length for the biggest square of points scored by team after moving to topLeftRow,topLeftCol
	public static int[] bestSquareInfoForNewMove(int[,]grid,int topLeftRow, int topLeftCol, int team)
	{
		int thisLength = countSideLength (grid,topLeftRow, topLeftCol,team);
		int[] squareInfo = {topLeftRow, topLeftCol, thisLength};  Debug.Log ("Starting recursive search with: (" + topLeftRow + ", " + topLeftCol + ") with a length of " + thisLength + " for team " + team);
		return recursiveFindBiggestSquare(grid,topLeftRow, topLeftCol, team, squareInfo);
	}
	/**********************************************************************
	 * BE VERY CAREFUL HERE!!!  THERE ARE LOTS OF WEIRD EXTREME CASES!!!
	 * CANNOT MAKE MANY ASSUMPTIONS!!!
	 * ********************************************************************/
	private static int[] recursiveFindBiggestSquare(int[,]grid,int topLeftRow, int topLeftCol, int team, int[] currentBestSquareInfo)
	{
		int[] bestSquareUp = new int[3];
		int[] bestSquareLeft = new int[3];
		int[] bestSquareDiag = new int[3];
		
		int newLength = 0;
		if (topLeftRow > 0 && topLeftCol > 0)
		{
			newLength = countSideLength(grid,topLeftRow-1,topLeftCol-1,team);
			if (newLength > currentBestSquareInfo[2])
			{
				bestSquareDiag[0] = topLeftRow-1;
				bestSquareDiag[1] = topLeftCol-1;
				bestSquareDiag[2] = newLength;
				bestSquareDiag = recursiveFindBiggestSquare(grid,topLeftRow-1,topLeftCol-1,team,bestSquareDiag);
			}
		}		

		newLength = 0;
		if (topLeftRow > 0)
		{
			newLength = countSideLength(grid,topLeftRow-1,topLeftCol,team);
			if (newLength > currentBestSquareInfo[2])
			{
				bestSquareUp[0] = topLeftRow-1;
				bestSquareUp[1] = topLeftCol;
				bestSquareUp[2] = newLength;
				bestSquareUp = recursiveFindBiggestSquare(grid,topLeftRow-1,topLeftCol,team,bestSquareUp);
			}
		}
		
		newLength = 0;
		if (topLeftCol > 0)
		{
			newLength = countSideLength(grid,topLeftRow,topLeftCol-1,team);
			if (newLength > currentBestSquareInfo[2])
			{
				bestSquareLeft[0] = topLeftRow;
				bestSquareLeft[1] = topLeftCol-1;
				bestSquareLeft[2] = newLength;
				bestSquareLeft = recursiveFindBiggestSquare(grid,topLeftRow,topLeftCol-1,team,bestSquareLeft);
			}
		}
		
		int maxLength = Math.Max (bestSquareDiag[2],    Math.Max (bestSquareUp[2],      Math.Max(bestSquareLeft[2], currentBestSquareInfo[2])));
		
		
		if (bestSquareDiag[2] == maxLength)
			return bestSquareDiag;
		else if (bestSquareLeft[2] == maxLength)
			return bestSquareLeft;
		else if (bestSquareUp[2] == maxLength)
			return bestSquareUp;
		else
			return currentBestSquareInfo;
	}
	//returns the size of the largest square with an upper-left-hand corner at topLeftRow, topLeftCol
	public static int countSideLength(int[,]grid,int topLeftRow, int topLeftCol, int team)
	{
		int numRows = grid.GetLength(0);
		int numCols = grid.GetLength(1);
		
		int possibleSideLength = 1;
		int checkRow = topLeftRow;
		int checkCol = topLeftCol;
		
		if (topLeftRow >= numRows || topLeftCol >= numCols || grid[topLeftRow,topLeftCol] != team)
			return possibleSideLength;
		
		while (checkRow < numRows && checkCol < numCols)
		{
			for (int row = topLeftRow; row <= checkRow; row++)
				if (grid[row,checkCol] != team)
					return possibleSideLength-1;
			for (int col = topLeftCol; col <= checkCol; col++)
				if (grid[checkRow, col] != team)
					return possibleSideLength-1;
			
			checkRow++;
			checkCol++;
			possibleSideLength++;
		} 
		return possibleSideLength-1;
	}
	// find the size of the largest square in the entire grid for a particular team
	// starting looking at fromRow, fromCol
	public static int[] maxCount(int[,] grid, int team, int fromRow, int fromCol)
	{
		int numRows = grid.GetLength(0);
		int numCols = grid.GetLength(1);
		int maxCount =0;
		int[] maxCountData = new int[3];
		for (int r=fromRow;r<numRows;r++)
		{
			for(int c=fromCol;c<numCols;c++)
			{
				if (grid[r,c] == team)
				{
					int countSize = countSideLength(grid,r,c,team);
					if(countSize>maxCount)
					{
						maxCount = countSize;
						maxCountData[0]=maxCount;
						maxCountData[1]=r;
						maxCountData[2]=c;
					}
				}
			}
		}
		return maxCountData;
	}
	
	// sum up the all the largest squares and destroys until the largest square is 1x1
	//MR. HAZARD'S ORIGINAL VERSION THAT HE WROTE WITH CHONG EARLIER IN THE YEAR
	//THIS VERSION IS JUST FOR THE AI TO CALCULATE OPTIMAL MOVES
	public static int totalCount (int[,] grid, int team, int fromRow, int fromCol)
	{
		bool squareFound = false;
		while (!squareFound)
		{
			while (grid[fromRow,fromCol] != team)
			{
				fromCol++;
				if (fromCol >= grid.GetLength(1))
				{
					fromRow++;
					fromCol = 0;
				}
				if (fromRow >= grid.GetLength(0))
					return 0;
			}
			if (countSideLength (grid,fromRow,fromCol,team) >= 2)
				squareFound = true;
			else
			{
				fromCol++;
				if (fromCol >= grid.GetLength(1))
				{
					fromRow++;
					fromCol = 0;
				}
				if (fromRow >= grid.GetLength(0))
					return 0;
			}	
		}
		if (fromRow >= grid.GetLength (0))
			return 0;
		int[] maxCountResult = maxCount(grid, team, fromRow, fromCol);
		int maxSideLength = maxCountResult[0];
		if (maxSideLength <= 1)
			return 0;
		
		int thisSquareScore = (int)Math.Pow(maxSideLength, 3);
		destroySquare(grid,maxCountResult[0],maxCountResult[1],maxCountResult[2]);
		int futureMaxWith = totalCount(grid, team, fromRow, fromCol) + thisSquareScore;
		
		repairSquare(grid,maxCountResult[0],maxCountResult[1],maxCountResult[2],team);
		
		int futureMaxWithout = 0;
		//if (fromCol == grid[fromRow].length-1)
		if(fromCol == grid.GetLength(1) - 1)			
		{
			fromRow++;
			fromCol = -1;
		}
		if (fromRow < grid.GetLength(0))
		{	
			futureMaxWithout = totalCount(grid, team, fromRow, fromCol+1);
		}
		return Math.Max (futureMaxWith,futureMaxWithout);
	}
	
	//MR. HAZARD' OVERLOADED VERSION WRITTEN ON MAY 24 ... SO THAT THE NEW MODELS COULD POP UP APPROPRIATELY
	public int totalCount (int[,] grid, int team, int fromRow, int fromCol, List<int> pointScoringSquares) //row,col,length
	{
		bool squareFound = false;
		while (!squareFound)
		{
			while (grid[fromRow,fromCol] != team)
			{
				fromCol++;
				if (fromCol >= grid.GetLength(1))
				{
					fromRow++;
					fromCol = 0;
				}
				if (fromRow >= grid.GetLength(0))
					return 0;
			}
			if (countSideLength (grid,fromRow,fromCol,team) >= 2)
				squareFound = true;
			else
			{
				fromCol++;
				if (fromCol >= grid.GetLength(1))
				{
					fromRow++;
					fromCol = 0;
				}
				if (fromRow >= grid.GetLength(0))
					return 0;
			}	
		}
		if (fromRow >= grid.GetLength (0))
			return 0;
		int[] maxCountResult = maxCount(grid, team, fromRow, fromCol);
		int maxSideLength = maxCountResult[0];
		if (maxSideLength <= 1)
			return 0;
		
		List<int> nextTry = new List<int>(pointScoringSquares.Capacity);
		foreach(int value in pointScoringSquares)
			nextTry.Add (value);
		
		pointScoringSquares.Add (maxCountResult[1]);
		pointScoringSquares.Add (maxCountResult[2]);
		pointScoringSquares.Add (maxCountResult[0]);
		int thisSquareScore = (int)Math.Pow(maxSideLength, 3);
		destroySquare(grid,maxCountResult[0],maxCountResult[1],maxCountResult[2]);
		int futureMaxWith = totalCount(grid, team, fromRow, fromCol, pointScoringSquares) + thisSquareScore;
		
		repairSquare(grid,maxCountResult[0],maxCountResult[1],maxCountResult[2],team);
		
		int futureMaxWithout = 0;
		//if (fromCol == grid[fromRow].length-1)
		if(fromCol == grid.GetLength(1) - 1)			
		{
			fromRow++;
			fromCol = -1;
		}
		if (fromRow < grid.GetLength(0))
		{	
			futureMaxWithout = totalCount(grid, team, fromRow, fromCol+1, nextTry);
		}
		
		if (futureMaxWith > futureMaxWithout)
		{
			territoriesScoringPointsForCurrentFaction = pointScoringSquares;
			return futureMaxWith;
		}
		else
		{
			territoriesScoringPointsForCurrentFaction = nextTry;
			return futureMaxWithout;
		}
	}
	
	// return an ArrayList of ints repeating the length, the row, and the col in that order
	// so the size of the array 3*theNumberOfSquares
	public static ArrayList getSquares (int[,] grid, int team)
	{
		ArrayList squares = new ArrayList();
		int row = grid.GetLength(0);
		int col = grid.GetLength(1);
		int[] maxData = maxCount(grid,team,0,0) ;
		int iter =0;
		while (maxData[0] > 1)
		{
			squares.Add(maxData[0]);
			squares.Add(maxData[1]);
			squares.Add(maxData[2]);
			destroySquare(grid, (int)squares[iter*3],(int)squares[iter*3 +1],(int)squares[iter*3 +2]);
			maxData = maxCount(grid,team,0,0) ;			
			iter++;
		}
		return squares;
	}
	public static void destroySquare(int[,] grid,int length, int row, int col)
	{
		for (int r=row;r<row+length;r++)
		{
			for (int c=col;c<col+length;c++)
			{
				grid[r,c]=3;
			}
		}
	}
	public static void repairSquare (int[,]grid,int length, int row, int col,int team)
	{
		for (int r=row;r<row+length;r++)
		{
			for (int c=col;c<col+length;c++)
			{
				grid[r,c]=team;
			}
		}
	}
	public void ReLoadMove(Move move) {
		tier = move.getTier();
		currentFaction = move.getFaction();
		PurchasePiece(move.getRow(), move.getCol());
	}
	public void redo(PlayerManager.Faction faction, int x, int y, int tier)
	{
		Debug.Log("Redo at " + x + ", " + y);
		GameSquare square = board.GSArray[x, y];
		
		if(tier == 0)
		{
			//Debug.Log("attempting a remove...");
			if(board.Remove(x, y, square._tier, faction))
			{
				square.DestroyPiece();

				board.Reserve(square.xval, square.yval);
				square._tier = 0;
				//Debug.Log("Remove Successful");
			}
			
		}
		board.Set(x, y, tier);
		if(tier==2)
		{
			/*
			try
			{
				board.SetPlayable(x+1, y, player);
				board.Reserve(x+1, y);
				board.SetPlayable(x-1, y, player);
				board.Reserve(x-1, y);
				board.SetPlayable(x, y+1, player);
				board.Reserve(x, y+1);
				board.SetPlayable(x, y-1, player);
				board.Reserve(x, y-1);
			}
			catch(Exception e){}
			*/
			#pragma warning disable
			try
			{
				board.SetPlayable(x+1, y, faction);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x+1, y);
			}
			catch(Exception e){}
			try
			{
				board.SetPlayable(x-1, y, faction);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x-1, y);
			}
			catch(Exception e){}
			try
			{
				board.SetPlayable(x, y+1, faction);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x, y+1);
			}
			catch(Exception e){}
			try
			{
				board.SetPlayable(x, y-1, faction);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x, y-1);
			}
			catch(Exception e){}	
			#pragma warning enable
		}
		else if(tier == 3)
		{
			for(int i = x-1; i <= x+1; i++)
			{
				for(int j = y-1; j <= y+1; j++)
				{
					try
					{
						board.SetPlayable(i,j,faction);
						board.Reserve (i, j);
					}
					catch(Exception e)
					{
						continue;
					}
				}
			}
		}
		Debug.Log ("Setting color for player " + faction);
		square.SetColor(faction, tier);
		
	}
	public bool BrandonReloadMoves(Stack moves) 
	{
		loadingMoves = true;
		if (moves.Count < 1) {
			loadingMoves = false;
			return false;
		}
		object[] movesArray = moves.ToArray();
		Array.Reverse(movesArray);
		Move firstMove = (Move) movesArray[0];
		if (firstMove.getFaction () == PlayerManager.Faction.Industry)
			currentFaction = PlayerManager.Faction.Industry;
		for (int count = 0; count < movesArray.Length; count++)
		{
			Move currentMove = (Move) movesArray[count];
			PlayerManager.Faction faction = currentMove.getFaction();
			int tier = currentMove.getTier();
			int row = currentMove.getRow();
			int col = currentMove.getCol();
			if (tier == -1 && row == -1 && col == -1) {
				Debug.Log("TURN CHANGE. " + faction + " ended turn.");
				tier = 1;
				currentFaction = PlayerManager.SwitchFaction(currentFaction);
			}
			else
				redo (faction, row, col, tier);
			/*if (tier ==0)
			{
				int row = move.getRow();
				int col = move.getCol();
				GameSquare square = board.GSArray[row,col]; 
				
				
				if(board.Remove (row, col, tier,(player == 1)?2:1))
				{	
					
					square.DestroyPiece();
					board.Reserve(square.xval, square.yval);
					square._tier = 0;
					roll += 3*tier;
					rd.text = roll.ToString();
					Debug.Log("Remove Successful");
					Debug.Log("Undo TRUE");
					return true;
				}
				Debug.Log ("Undo FALSE");
				return false;
			}
			else if (tier !=0) 
			{
				int row = move.getRow();
				int col = move.getCol();
				int removedTier = move.getRemovedTier();
				
				//GameSquare square = board.GSArray[row,col];
				
				this.player = (this.player == 1)?2:1;
				roll += removedTier*3;
				this.tier = removedTier;
				PurchasePiece(row,col
				this.tier = 0;
				rolledATwo = true;
				this.player = (this.player == 1)?2:1;
				
				
				
				Debug.Log("Undo TRUE");
				return true;
			}
			else
			{
				Debug.Log ("Undo FALSE");
				return false;		
			}
		}
		return false;*/
		}
		currentFaction = PlayerManager.SwitchFaction (((Move) movesArray[movesArray.Length - 1]).getFaction ());
		loadingMoves = false;
		int[,] boardcopy = new int[12,12];
		Array.Copy(board.Board, boardcopy, 144);
		//Debug.Log ("Score" + totalCount(boardcopy, player, 0, 0));
		
		int newScore = totalCount(boardcopy, (int) PlayerManager.Faction.Life, 0, 0, new List<int>(30));
		Debug.Log ("Score: " + newScore);
		playerScores[1] = newScore;
		int newScore2 = totalCount(boardcopy, (int) PlayerManager.Faction.Industry, 0, 0, new List<int>(30));
		Debug.Log ("Score: " + newScore);
		playerScores[2] = newScore;

		ButtonManager.instance.lifeScore.text = playerScores[(int) PlayerManager.Faction.Life].ToString() + "/" + winningScore;
		ButtonManager.instance.industryScore.text =  playerScores[(int) PlayerManager.Faction.Industry].ToString() + "/" + winningScore;
		return true;
	}
	public void LoadMoves(Stack moves) {
		while (moves.Count > 0) {
			ReLoadMove((Move) moves.Pop());
		}
	}
	public void Promote(GameSquare square, PlayerManager.Faction faction) {
		Debug.Log ("Promoting (" + square.xval + ", " + square.yval + ") for " + faction);
		square.DestroyPiece();
		square.SetColor(faction, 4);
	}
	private void PromoteNewPieces(PlayerManager.Faction factionToPromoteFor)
	{
		int row = 0;
		int col = 0;
		int length = 0;
		bool[] pointSpots = new bool[144];
		bool promoted = false;
		for (int index = 0; index < territoriesScoringPointsForCurrentFaction.Count; index+=3)
		{
			row = territoriesScoringPointsForCurrentFaction[index];
			col = territoriesScoringPointsForCurrentFaction[index+1];
			length = territoriesScoringPointsForCurrentFaction[index+2];
			for (int r = row; r < row + length; r++)
			{
				for (int c = col; c < col + length; c++)
				{
					pointSpots[r*12+c] = true;
					if (board.GSArray[r,c]._tier != 4)
					{
						promotedTerritoryFormerTierValues[r,c] = 10*(int)factionToPromoteFor + board.GSArray[r,c]._tier;
						Promote (board.GSArray[r,c], factionToPromoteFor); 
						promoted = true;
				    }
				}
			}
		}
		territoriesScoringPointsForCurrentFaction = new List<int>(30);
		
		//Un-promote old pieces ...
		for (int r = 0; r < 12; r++)
		{
			for (int c = 0; c < 12; c++)
			{
				if (board.GSArray[r,c]._tier == 4 && !pointSpots[r*12+c] && board.GSArray[r,c].faction == factionToPromoteFor)
				{
					board.GSArray[r,c].DestroyPiece();
					board.GSArray[r,c].SetColor( factionToPromoteFor,promotedTerritoryFormerTierValues[r,c]%10 );
					promotedTerritoryFormerTierValues[r,c] = 0;
				}
			}
		}
		if (promoted) {
			promotionSmash.smash();
		}
	}
}