using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour 
{
	public GameBoard board;
	public GameObject myBoardSphere;
	public UILabel rollDisplay;
	public bool rolledATwo, mouseUI;
	public int player, tier, roll, winningScore;
	public bool singleplayer = true;
	private int humanFactionChoice;
	public AI AI;
	public String[] PlayerFactions;
	private int[] playerScores;
	private Stack moves;
	public UILabel player1score, player2score;
	public PieceManager pieceManager;
	public GCCubedListener gameCenter;
	public SaveLoadManager saveLoad;
	private bool loadingMoves, previousTurnEnded, hasMovedThisTurn, hasRolled;
	private Move topOfStack;
	private int lastPlayer;
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
	
	private AudioSource[] audioClips;
	public AudioSource industry, life;
	private int audioIndex;
	
	// Use this for initialization
	void Awake () 
	{
		territoriesScoringPointsForCurrentFaction = new List<int>();
		promotedTerritoryFormerTierValues = new int[12,12]; //faction *10 + tier value
		winningScore = 100; //temp
		WinCamera.camera.enabled = false;
		player = 9;
		roll = 0;
		SetTier(1);
		playerScores = new int[3];
		PlayerFactions = new String[2];
		setPlayerFaction(1, "Life");
		setPlayerFaction(2, "Industry");
		mouseUI = false;
		moves = new Stack();
		//RollDistribution = new int[36] {2,3,3,4,4,4,5,5,5,5,6,6,6,6,6,7,7,7,7,7,7,8,8,8,8,8,9,9,9,9,10,10,10,11,11,12};
		var temp = GameObject.Find("GameCenter");
		gameInfo = GameObject.Find ("GameInfo").GetComponent<GameInfo>();
		singleplayer = gameInfo.getSinglePlayer();
		winningScore = gameInfo.getWinCondition();
		humanFactionChoice = gameInfo.getHumanFactionChoice();
		AI = new AI(winningScore);
		Debug.Log("HumanF faction choice from GameInfo: " + humanFactionChoice);
		Debug.Log("Final player variable: " + player);
		audioIndex = player == 1 ? 0:1;
		Debug.Log("Audio index: " + audioIndex);
		audioClips = new AudioSource[2];
		audioClips[0] = industry;
		audioClips[1] = life;
		audioClips[audioIndex].Play();

		GameIsOver = false;
		escapeAttempt = false;
		// in case GameCenter is not working for some reason
		if (temp != null) {
			gameCenter = temp.GetComponentInChildren<GCCubedListener>();
			saveLoad = temp.GetComponentInChildren<SaveLoadManager>();
		//	singleplayer = gameCenter.singleplayer;
			if (!singleplayer) {
				moves = saveLoad.ParseJSONGameStateString(gameCenter.currentMatchData);
				if (moves != null && moves.Count > 0) {
					topOfStack = (Move) moves.Peek();
					previousTurnEnded = (topOfStack.getCol() == -1) && (topOfStack.getRow() == -1) && (topOfStack.getTier() == -1);
					lastPlayer = topOfStack.getPlayer();
					hasMovedThisTurn = false;
					hasRolled = false;
					Debug.Log("Faction that last ended turn (Life/Industry) (1/2): " + lastPlayer);
				}
			}
		/*	string opponent = gameCenter.GetOpponent();
			if (opponent != null && opponent != "")
				player2score.text = opponent; */
		}
		player1score.text = "0" + "/" + winningScore;
		player2score.text = "0" + "/" + winningScore;
		//create pieces offscreen
	}
	// This prevents the game from misintrepreting drag motions as clicks on the board
	void OnDrag(DragGesture gesture) { 
		if (gesture.Phase == ContinuousGesturePhase.Started)
			mouseUI = true;
		else if (gesture.Phase == ContinuousGesturePhase.Ended)
			mouseUI = false;
		Debug.Log(mouseUI);
	}
	public Stack GetMoves() {
		return moves;
	}
	
	void Update () 
	{
		if (inWinningSequence)
		{	
			gameTimer += Time.deltaTime;
			if (gameTimer > 3.0f)
			{
				if(playerScores[1] > winningScore)
				{
					winCameraLight.intensity = lifeWinsIntensity;
					WinScreenCube.renderer.material = (Material)Resources.Load ("Materials/LifeWIN");

				}
				else if(playerScores[2] > winningScore)
				{
					winCameraLight.intensity = industryWinsIntensity;
					WinScreenCube.renderer.material = (Material)Resources.Load ("Materials/IndustryWIN");	
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
		int player = move.getPlayer();
		if(player != this.player)
		{
			moves.Push(move);
			Debug.Log ("Not your move to undo BITCH");
			return false;
		}
		int tier = move.getTier();
		if (player == this.player && tier ==0)// tier == 0, thus I need to undo a remover -> replace
		{
			int row = move.getRow();
			int col = move.getCol();
			int removedTier = move.getRemovedTier();
			
			//GameSquare square = board.GSArray[row,col];
			
			this.player = (this.player == 1)?2:1;
			roll += removedTier*3;
			this.tier = removedTier;
			HandleClick(row, col);
			this.tier = 0;
			rolledATwo = true;
			this.player = (this.player == 1)?2:1;
			
			
			
			Debug.Log("Undo TRUE");
			return true;
		}
		else if (player == this.player && tier !=0) 
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
				rollDisplay.text = roll.ToString();
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
		if(GameIsOver)
		{
			return;
		}
		GameSquare square = board.GSArray[xval,yval];
		//gamecenter
		hasMovedThisTurn = true;
		
		if(tier == 0 || (rolledATwo && square._player != player))
		{
			if(Remove(xval, yval, square._tier))
			{
				square.DestroyPiece();
				board.Reserve(square.xval, square.yval);  //sets the image for what level of reservation should be displayed
				square._tier = 0;
				updateRemoveModels(xval,yval,square._tier);
			}
			
		}
		else if(PurchasePiece(xval, yval))    
		{
			square.SetColor(player, tier);                                                   
			Move move = new Move(player, xval, yval, tier);
			moves.Push(move);                                 
			PromoteNewPieces(player);                              
			myBoardSphere.GetComponent<CameraSphere>().zoomToSquare(square);
		}
		rollDisplay.text = roll.ToString();
	}
	private void updateRemoveModels(int row, int col, int removedTier)
	{
		if (removedTier == 4)
			promotedTerritoryFormerTierValues[row,col] = 0;
		
		//Make sure opponent's tier 4 pieces are removed appropriately
		int[,] gridCopy = new int[board.Board.GetLength(0),board.Board.GetLength(1)];
		for(int i = 0; i < board.Board.GetLength(0); i++)
			for(int j = 0; j < board.Board.GetLength(1); j++)
				gridCopy[i,j] = board.Board[i,j];
		int opponent = player == 1 ? 2:1;
		int newOpponentScore = totalCount (gridCopy,opponent,0,0,new List<int>(30));
		PromoteNewPieces (opponent);
		if (opponent == 1)
			player1score.text = "" + newOpponentScore;
		else
			player2score.text = "" + newOpponentScore;
	}
	public void NextTurn()
	{
		Debug.Log ("Next Turn Clicked");
		Debug.Log ("CurrentPlayer: " + player + ". Has moved? " + hasMovedThisTurn + ". Has Rolled? " + hasRolled);
		if (player == 9 && singleplayer) { //this is the case on the first roll
			Debug.Log("First turn detected");
			if (humanFactionChoice == 1)
				player = 2;
			else
				player = 1;
		}
		if(GameIsOver) //disables the button when the game is over
		{
			return;
		}
		//pushes empty move to the stack to signify the end of a turn - player references the player that just played
		if (gameCenter != null  && !singleplayer && !loadingMoves) {
			if (!GameCenterTurnBasedBinding.isCurrentPlayersTurn()) {
				gameCenter.LoadLevel("MainMenuV2");
				return;
			}
			else if (hasRolled) {
				if (hasMovedThisTurn) {
					moves.Push(new Move(player, -1, -1, -1));
					Debug.Log("GameCenter " + gameCenter);
					Debug.Log("SaveLoad " + saveLoad);
					hasMovedThisTurn = false;
					hasRolled = false;
					gameCenter.EndTurn(saveLoad.CreateJSONGameStateString(GetMoves()));
					return;
				}
				else {
					string[] options = { "OK" };
					EtceteraBinding.showAlertWithTitleMessageAndButtons("Ending Turn...", "You diddn't move!", options);
					return;
				}
			}
		}
		hasRolled = true;
		roll = (UnityEngine.Random.Range(1, 7)+UnityEngine.Random.Range(1,7));
		if(singleplayer && player != humanFactionChoice)
		{
			//Debug.Log("Changing AI roll to not 2");
			roll = (UnityEngine.Random.Range(2, 7)+UnityEngine.Random.Range(1,7));
		}
		rollDisplay.text = roll.ToString();
		if(roll == 2)
		{
			rolledATwo = true;
		}
		else
		{
			rolledATwo = false;
		}
		tier = 1;
		player = (player==1)?2:1;
		if(singleplayer && player != humanFactionChoice)
		{
			int[][] grid = new int[board.Board.GetLength(0)][];
			for(int i = 0; i < board.Board.GetLength(0); i++)
			{
				grid[i] = new int[board.Board.GetLength(1)];
				for(int j = 0; j < board.Board.GetLength(1); j++)
				{
					grid[i][j] = board.Board[i,j];
				}
			}
			
			bool[][] check = board.PlayableToBoolArray(player);
			List<GameStat> ListOfMove = AI.makeMove(grid, check, player, roll);
			foreach(GameStat gs in ListOfMove)
			{
				SetTier (gs.getTier());
				Debug.Log ("AI moves at " + gs.getMaxX() + ", " + gs.getMaxY());
				HandleClick(gs.getMaxX(), gs.getMaxY());
			}
			if (playerScores[1] < winningScore && playerScores[2] < winningScore) {
				NextTurn();
			}
		}
	}
	public bool PurchasePiece(int x, int y)
	{
		bool output = false;
		roll -= (3*tier);
		if(roll < 0 || !board.GetPlayable(x, y, player))
		{
			roll += (3*tier);
			Debug.Log ("Nope");
			return false;
		}
		if(board.GetPlayable(x, y, player))
		{
			board.Set(x, y, player);
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
				board.SetPlayable(x+1, y, player);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x+1, y);
			}
			catch(Exception e){}
			try
			{
				board.SetPlayable(x-1, y, player);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x-1, y);
			}
			catch(Exception e){}
			try
			{
				board.SetPlayable(x, y+1, player);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x, y+1);
			}
			catch(Exception e){}
			try
			{
				board.SetPlayable(x, y-1, player);
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
						board.SetPlayable(i,j,player);
						board.Reserve (i, j);
					}
					catch(Exception e)
					{
						continue;
					}
				}
			}
		}
		Debug.Log("Can purchase piece: tier " +tier+", player "+player+": "+output);
		
		int[,] boardcopy = new int[12,12];
		Array.Copy(board.Board, boardcopy, 144);
		//Debug.Log ("Score" + totalCount(boardcopy, player, 0, 0));
		
		int newScore = totalCount(boardcopy, player, 0, 0, new List<int>(30));
		Debug.Log ("Score: " + newScore);
		playerScores[player] = newScore;
		if(newScore >= winningScore)
		{
			Win();
		}
		player1score.text = getPlayerScore(1).ToString() + "/" + winningScore;
		player2score.text = getPlayerScore(2).ToString() + "/" + winningScore;
		return output;
	}
	public void SetTier(int newTier)
	{
			this.tier = newTier;
	}
	public String getPlayerFaction(int playernumber)
	{
		return PlayerFactions[playernumber - 1];
	}
	public void setPlayerFaction(int p, String faction)
	{
		PlayerFactions[p-1] = faction;
	}
	public bool Remove(int x, int y, int _tier)
	{
		
		if(rolledATwo)
		{
			if (_tier == 4)
			{
				int opponent = player == 1 ? 2:1;
				board.Remove (x,y,_tier,player); //pass in the player who is removing the piece
				board.GSArray[x,y].DestroyPiece();
				_tier = promotedTerritoryFormerTierValues[x,y]%10;
				promotedTerritoryFormerTierValues[x,y] = 0;
				board.Set (x,y,_tier);
				board.GSArray[x,y].SetColor(opponent, _tier);
			}
	
			if(board.Remove (x, y, _tier, player))
			{
				rolledATwo = false;
				Move move = new Move(player, x, y, 0, _tier);
				moves.Push (move);

				return true;
			}
		}
		return false;
	}
	public int getPlayerScore(int playerToGet)
	{
		return playerScores[playerToGet];
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
		player = move.getPlayer();
		PurchasePiece(move.getRow(), move.getCol());
	}
	public void redo(int player, int x, int y, int tier)
	{
		Debug.Log("Redo at " + x + ", " + y);
		GameSquare square = board.GSArray[x, y];
		
		if(tier == 0)
		{
			//Debug.Log("attempting a remove...");
			if(board.Remove(x, y, square._tier, player))
			{
				square.DestroyPiece();

				board.Reserve(square.xval, square.yval);
				square._tier = 0;
				//Debug.Log("Remove Successful");
			}
			
		}
		board.Set(x, y, player);
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
				board.SetPlayable(x+1, y, player);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x+1, y);
			}
			catch(Exception e){}
			try
			{
				board.SetPlayable(x-1, y, player);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x-1, y);
			}
			catch(Exception e){}
			try
			{
				board.SetPlayable(x, y+1, player);
			}
			catch(Exception e){}
			try
			{
				board.Reserve(x, y+1);
			}
			catch(Exception e){}
			try
			{
				board.SetPlayable(x, y-1, player);
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
						board.SetPlayable(i,j,player);
						board.Reserve (i, j);
					}
					catch(Exception e)
					{
						continue;
					}
				}
			}
		}
		Debug.Log ("Setting color for player " + player);
		square.SetColor(player, tier);
		
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
		if (firstMove.getPlayer() == 2)
			this.player = 2;
		for (int count = 0; count < movesArray.Length; count++)
		{
			Move currentMove = (Move) movesArray[count];
			int player = currentMove.getPlayer();
			int tier = currentMove.getTier();
			int row = currentMove.getRow();
			int col = currentMove.getCol();
			if (tier == -1 && row == -1 && col == -1) {
				Debug.Log("TURN CHANGE. " + player + " ended turn.");
				tier = 1;
				this.player = (this.player==1)?2:1;
			}
			else
				redo (player, row, col, tier);
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
		this.player = lastPlayer;
		loadingMoves = false;
		int[,] boardcopy = new int[12,12];
		Array.Copy(board.Board, boardcopy, 144);
		//Debug.Log ("Score" + totalCount(boardcopy, player, 0, 0));
		
		int newScore = totalCount(boardcopy, 1, 0, 0, new List<int>(30));
		Debug.Log ("Score: " + newScore);
		playerScores[1] = newScore;
		int newScore2 = totalCount(boardcopy, 2, 0, 0, new List<int>(30));
		Debug.Log ("Score: " + newScore);
		playerScores[2] = newScore;
		
		player1score.text = getPlayerScore(1).ToString() + "/" + winningScore;
		player2score.text = getPlayerScore(2).ToString() + "/" + winningScore;
		this.player = lastPlayer;
		return true;
	}
	public void LoadMoves(Stack moves) {
		while (moves.Count > 0) {
			ReLoadMove((Move) moves.Pop());
		}
	}
	public void Promote(GameSquare square, int player) {
		square.DestroyPiece();
		square.SetColor(player, 4);
	}
	private void PromoteNewPieces(int playerToPromoteFor)
	{
		int row = 0;
		int col = 0;
		int length = 0;
		bool[] pointSpots = new bool[144];
		
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
						promotedTerritoryFormerTierValues[r,c] = 10*playerToPromoteFor + board.GSArray[r,c]._tier;
						Promote (board.GSArray[r,c], playerToPromoteFor); 
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
				if (board.GSArray[r,c]._tier == 4 && !pointSpots[r*12+c] && board.GSArray[r,c]._player == playerToPromoteFor)
				{
					board.GSArray[r,c].DestroyPiece();
					board.GSArray[r,c].SetColor (playerToPromoteFor,promotedTerritoryFormerTierValues[r,c]%10 );
					promotedTerritoryFormerTierValues[r,c] = 0;
				}
			}
		}
	}
}