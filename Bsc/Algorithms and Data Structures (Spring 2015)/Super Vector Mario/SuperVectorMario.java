import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

/**
 * Created by William and Dennis on 05/05/15.
 */
public class SuperVectorMario
{
	char[][] raceTrack;
	int rows;
	int columns;

	Set<State> stateSet;
	Queue<State> stateQueue;

	/**
	 * Constructor
	 * @param rows
	 * @param columns
	 */
	public SuperVectorMario(int rows, int columns)
	{
		this.rows = rows;
		this.columns = columns;
		raceTrack = new char[rows][columns];
		stateSet = new HashSet<State>();
		stateQueue = new Queue<State>();

		initiate();
	}

	/**
	 * Creates the race track for Super Mario Vector Race
	 */
	private void initiate()
	{
		for (int i = 0; i < rows; i++)
		{
			String using = StdIn.readLine();
			for (int j = 0; j < columns; j++)
			{
				raceTrack[i][j] = using.charAt(j);
				if (using.charAt(j) == 'S')
				{
					State usedState = new State(i, j, 0, 0, 1);
					stateQueue.enqueue(usedState);
					stateSet.add(usedState);
				}
			}
		}
	}

	/**
	 * Calculates the minimum moves need to complete the race track
	 * @return minimum number of moves
	 */
	private int calcMinMoves()
	{

		while (!stateQueue.isEmpty())
		{
			State currentState = stateQueue.dequeue();
			if (raceTrack[currentState.xCoord][currentState.yCoord] == 'F')
			{
				return currentState.count;
			}
			List<State> validMoves = currentState.getValidMoves();
			if (!validMoves.isEmpty())
			{
				for (State t : validMoves)
				{
					if (!stateSet.contains(t))
					{
						stateSet.add(t);
						stateQueue.enqueue(t);
					}
				}
			}
		}
		return 0;
	}

	//Inner class

	/**
	 * Inner class to create the different states which Super Mario can have during his run on the race track
	 */
	private class State
	{
		private final int xCoord, yCoord, vX, vY, count;

		/**
		 * Constructor
		 * @param xCoord
		 * @param yCoord
		 * @param vX
		 * @param vY
		 * @param count
		 */
		public State(int xCoord, int yCoord, int vX, int vY, int count)
		{
			this.xCoord = xCoord;
			this.yCoord = yCoord;
			this.vX = vX;
			this.vY = vY;
			this.count = count;
		}

		/**
		 * Returns the valid moves that Super Mario can choose from his current state
		 * @return a List of State objects
		 */
		private List<State> getValidMoves()
		{
			List<State> moves = new ArrayList<State>();
			int[] vRegulartor = new int[]{
											 0,  0,
											 0,  1,
											 1,  1,
											 1,  0,
											 1, -1,
											 0, -1,
											-1, -1,
											-1,  0,
											-1,  1,
										  };


			for (int i = 0; i < vRegulartor.length; i += 2)
			{
				int newVX = this.vX + vRegulartor[i];
				int newVY = this.vY + vRegulartor[i + 1];

				if (isValid(xCoord, yCoord, newVX, newVY))
				{
					moves.add(new State((xCoord + newVX), (yCoord + newVY), newVX, newVY, count + 1));
				}
			}

			return moves;

		}

		/**
		 * Checks if a Sate is valid for Super Mario to choose
		 * @param x
		 * @param y
		 * @param newVX
		 * @param newVY
		 * @return true if the State is valid
		 */
		private boolean isValid(int x, int y, int newVX, int newVY)
		{
			return
					(x + newVX < rows && x + newVX >= 0 && y + newVY < columns && y + newVY >= 0 && raceTrack[x + newVX][y + newVY] != 'O');
		}

		@Override
		public boolean equals(Object o)
		{
			if (this == o) return true;
			if (o == null || getClass() != o.getClass()) return false;

			State state = (State) o;

			if (vX != state.vX) return false;
			if (vY != state.vY) return false;
			if (xCoord != state.xCoord) return false;
			if (yCoord != state.yCoord) return false;

			return true;
		}

		@Override
		public int hashCode()
		{
			int result = xCoord;
			result = 31 * result + yCoord;
			result = 31 * result + vX;
			result = 31 * result + vY;
			return result;
		}
	}

	//Main method
	public static void main(String[] args)
	{
		int rows = Integer.valueOf(StdIn.readLine());
		int columns = Integer.valueOf(StdIn.readLine());
		SuperVectorMario SMC = new SuperVectorMario(rows, columns);
		StdOut.println("Minimum moves: " + SMC.calcMinMoves());
	}
}