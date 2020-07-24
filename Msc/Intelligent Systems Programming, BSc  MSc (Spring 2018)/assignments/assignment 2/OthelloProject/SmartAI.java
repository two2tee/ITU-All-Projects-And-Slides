import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;

import static java.lang.Math.*;

/**
* A minimax AI
* @author Dennis, Thor og Daniel
*/
public class SmartAI implements IOthelloAI {
	private final int MAX_DEPTH = 5;
	private int playerToMaximise;

	public Position decideMove(GameState state) {
		Position optimalMove = minimax(state);
		return optimalMove;
	}

	private Position minimax(GameState state) {
		float alpha = Float.MIN_VALUE;
		float beta = Float.MAX_VALUE;
		int depth = 0;
		playerToMaximise = state.getPlayerInTurn();

		return maximum(state,alpha,beta,depth).finalPosition;
	}

	private Value maximum(GameState state, float alpha, float beta, int depth) {
		float bestScore = Float.MIN_VALUE;
		Value bestMove = null;

		if(terminalTest(state,depth)) return eval(state);
		if(state.legalMoves().isEmpty()) return minimum(state, alpha, beta, depth+1);

		for(Position action : state.legalMoves()) {
			float childScore = minimum(performMove(state, action), alpha, beta, depth+1).Value;

			if(childScore > bestScore) {
				bestScore = childScore;
				bestMove = new Value(bestScore,action);
			}
			if (bestScore >= beta) return bestMove; //beta cutoff
			alpha = max(alpha, bestScore);
		}
		return bestMove;
	}

	private Value minimum(GameState state, float alpha, float beta, int depth) {
		float bestScore = Float.MAX_VALUE;
		Value bestMove = null;

		if(terminalTest(state, depth)) return eval(state);
		if(state.legalMoves().isEmpty()) return maximum(state, alpha, beta, depth + 1);
		
		for(Position action :  state.legalMoves()) {
			float childScore = maximum(performMove(state, action), alpha, beta, depth + 1).Value;

			if(childScore < bestScore) {
				bestScore = childScore;
				bestMove = new Value(bestScore, action);
			}

			if(bestScore <= alpha) return bestMove; // alpha cutoff
			beta = min(beta, bestScore);
		}
		return bestMove;
	}


	private boolean terminalTest(GameState state, int depth) {
		return state.isFinished() || depth >= MAX_DEPTH;
	}

	private Value utility(GameState state) {
		int[] tokens = state.countTokens();
		if(playerToMaximise == 1) return new Value(tokens[0], null); // Max tokens 
		else return new Value(tokens[1], null); // Min tokens 
	}

	private Value eval(GameState state) {
		float score = 0.0f;
		int[][] board = state.getBoard();
		int width = board.length - 1;
		int height = board[0].length - 1;
		
		//check corners
		score +=checkCorner(board, 0, 0);
		score +=checkCorner(board, width, 0);
		score +=checkCorner(board, 0, height);
		score +=checkCorner(board, width, height);

		//check tokens on board
		int[] tokens = state.countTokens();
		score += playerToMaximise == 1 ? 0.1f * tokens[0] : 0.1f * tokens[1]; // Tokens of max or min

		return new Value(score,null);
	}

	private float checkCorner(int[][] board, int cornerX, int cornerY){
		float score = 0.0f;
		int xDiff = cornerX > 0 ? -1 : 1;
		int yDiff = cornerY > 0 ? -1 : 1;
		if(playerToMaximise == board[cornerX][cornerY]) score += 100.0f;

		score += countCornerScore(cornerX, cornerY, board, xDiff, 0); // traverse horizontally
		score += countCornerScore(cornerX, cornerY, board, 0, yDiff); // traverse vertically 
		score += countCornerScore(cornerX, cornerY, board, xDiff, yDiff); //traverse X,Y diagonally

		return score;
	}

	private float countCornerScore(int cornerX, int cornerY, int[][] board, int xDiff, int yDiff) {
		float score = 0.0f;
		int y = cornerY;
		int x = cornerX;

		while((x < board.length && x >= 0 && y < board[0].length && y >= 0)) // Inside board
		{
			if(board[x][y] == playerToMaximise) score += 10.0f;
			else break;
			x += xDiff;
			y += yDiff;
		}
		return score;
	}

	private GameState performMove(GameState currentState, Position position) {
		GameState newState = new GameState(currentState.getBoard(), currentState.getPlayerInTurn());
		newState.insertToken(position);
		return newState;
	}

	private class Value {
		final float Value;
		final Position finalPosition;

		private Value(float value, Position finalPosition) {
			Value = value;
			this.finalPosition = finalPosition;
		}
	}

}
