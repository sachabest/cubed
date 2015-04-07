using UnityEngine;
using System.Collections;

public class GameState {
	
	public Stack moves;
	public int player1score, player2score, winningScore;
	
	// Use this for initialization
	public GameState(int winningScore) {
		moves = new Stack();
		player1score = 0;
		player2score = 0;
		this.winningScore = winningScore;
	}
	void addMove(Move move) {
		moves.Push(move);
	}
	Move lastMove() {
		return (Move) moves.Peek();
	}
}
