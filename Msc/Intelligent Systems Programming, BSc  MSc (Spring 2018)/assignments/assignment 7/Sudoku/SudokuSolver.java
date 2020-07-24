import java.util.ArrayList;


public class SudokuSolver implements ISudokuSolver {

	private int[][] puzzle;
	private int size;
	private ArrayList<ArrayList<Integer>> domains;

	/**
	 * Get contents of puzzle.
	 * @return 2D array representing game board state.
	 */
	public int[][] getPuzzle() {
		return puzzle;
	}

	/**
	 * Insert value in specified position given by a column and row number in the puzzle board.
	 * @param col the column that the value should be inserted in (starts with
	 * 0 in the leftmost column).
	 * @param row the row that the value should be inserted in (starting with
	 * 0 in the topmost row).
	 * @param value the value to insert. Note that this can be 0 to erase a value.
	 */
	public void setValue(int col, int row, int value) {
		// Check if within board
		if(col >= 0 && row >= 0 && col <= puzzle.length && row <= puzzle.length) {
			// Check if valid number
			if(value >= 0 && value <= 9) {
				puzzle[col][row] = value;
			}
		}
	}

	/**
	 * Initializes an empty puzzle.
	 * @param size the size measured in blocks in a row, ie. a normal soduko
	 */
	public void setup(int size) {
		this.size = size;

		int dimensionSize = this.size * this.size; // E.g. Sudoku size is 3 by default (3x3=9)
		int totalElements = dimensionSize * dimensionSize;

		puzzle = new int[dimensionSize][dimensionSize];
		domains = new ArrayList<>(totalElements);

		//Initialize each domain in domains[X]...
		ArrayList<Integer> dimension = new ArrayList<>(dimensionSize); // 1 to 9 for regular sudoku
		for (int boardValue = 0; boardValue < dimensionSize; boardValue++) {
			dimension.add(boardValue);
		}

		// Set dimensions in board
		for (int i = 0; i < totalElements; i++) {
			domains.add(new ArrayList<>(dimension));
		}

		// Examples used for testing
		int[][] fullValidSudokuConfiguration = {
				{5, 3, 4, 6, 7, 8, 9, 1, 2},
				{6, 7, 2, 1, 9, 5, 3, 4, 8},
				{1, 9, 8, 3, 4, 2, 5, 6, 7},

				{8, 5, 9, 7, 6, 1, 4, 2, 3},
				{4, 2, 6, 8, 5, 3, 7, 9, 1},
				{7, 1, 3, 9, 2, 4, 8, 5, 6},

				{9, 6, 1, 5, 3, 7, 2, 8, 4},
				{2, 8, 7, 4, 1, 9, 6, 3, 5},
				{3, 4, 5, 2, 8, 6, 1, 7, 9}
		};

		int[][] partialValidSudokuConfiguration = {
				{5, 6, 0, 8, 4, 7, 0, 0, 0},
				{3, 0, 9, 0, 0, 0, 6, 0, 0},
				{0, 0, 8, 0, 0, 0, 0, 0, 0},
				{0, 1, 0, 0, 8, 0, 0, 4, 0},
				{7, 9, 0, 6, 0, 2, 0, 1, 8},
				{0, 5, 0, 0, 3, 0, 0, 9, 0},
				{0, 0, 0, 0, 0, 0, 2, 0, 0},
				{0, 0, 6, 0, 0, 0, 8, 0, 7},
				{0, 0, 0, 3, 1, 6, 0, 5, 9}
		};

		int[][] invalidSudokuConfiguration = {
				{5, 6, 0, 8, 4, 7, 0, 0, 8}, // 8 on same row twice
				{3, 0, 9, 0, 0, 0, 6, 0, 0},
				{0, 0, 8, 0, 0, 0, 0, 0, 0},
				{0, 1, 0, 0, 8, 0, 0, 4, 0},
				{7, 9, 0, 6, 0, 2, 0, 1, 8},
				{0, 5, 0, 0, 3, 0, 0, 9, 0},
				{0, 0, 0, 0, 0, 0, 2, 0, 0},
				{0, 0, 6, 0, 0, 0, 8, 0, 7},
				{0, 0, 0, 3, 1, 6, 0, 5, 9}
		};

		readInPuzzle(partialValidSudokuConfiguration);

	}


	/**
	 * Checks if the puzzle with the current content can be solved.
	 * If so, it solves the puzzle and remembers the "result" used in getPuzzle().
	 * @return true if puzzle board has valid solution.
	 */
	public boolean solve() {
		ArrayList<Integer> initialAssignment = getAssignment(puzzle);

		// Check consistency on initial assignment to see if puzzle is valid
		if (initialFC(initialAssignment)) {
			// Try to solve the puzzle
			//ArrayList<Integer> remainingAssignments = forwardChecking(initialAssignment);
			initialAssignment = forwardChecking(initialAssignment);

			// Store solution if exists
			if (initialAssignment != null) {
				puzzle = getPuzzle(initialAssignment);
				return true;
			}
		}

		// Puzzle cannot be solved
		// Either user entered invalid value or current board setting is not solvable
		return false;
	}

	/**
	 * Reads in the puzzle from an array.
	 * Checks if the specified input array has the same dimensions as the board.
	 * @param puzzle the puzzle that should be read in.
	 */
	public void readInPuzzle(int[][] puzzle) {
		int rowSize = puzzle.length;
		int colSize = puzzle[0].length; // assuming rows >= 1 ans rows have same length
		int originalRowSize = this.puzzle.length;
		int originalColumnSize = this.puzzle[0].length;

		// check if specified input array has the same dimensions as the Sudoku board
		if(rowSize == originalRowSize && colSize == originalColumnSize) {
			this.puzzle = puzzle;
		}
	}
	
	
		//---------------------------------------------------------------------------------
		//YOUR TASK:  Implement forwardChecking(assignment)
		//---------------------------------------------------------------------------------
		public ArrayList<Integer> forwardChecking(ArrayList<Integer> assignment) {

			// Check if board contains solution (all values between 1 and 9)
			if (!assignment.contains(0)) return assignment;

			// Find first unassigned variable (X)
			int tentativeAssignmentX = assignment.indexOf(0);

			// Maintain separate copy of domains in case of roll-back when backtracking
			ArrayList<ArrayList<Integer>> oldDomains = deepCopy(domains); // D_old
			ArrayList<Integer> temporaryAssignmentDomainDX = new ArrayList<>(domains.get(tentativeAssignmentX)); // DX

			for(int valueV : temporaryAssignmentDomainDX) {
				// Check if value (V) at assignment position (X) satisfies constraints
				if(arcConsistencyFC(tentativeAssignmentX, valueV)) { // If no violated constraint

					// Assign value V to variable X
					assignment.set(tentativeAssignmentX, valueV);

					// Look ahead (backtracking): do forward checking recursively to reduce domain
					ArrayList<Integer> forwardCheckingResult = forwardChecking(assignment);

					// Check for valid solution in look ahead (backtracking) result
					if (forwardCheckingResult != null) return forwardCheckingResult;

					// Value assignment did not result in valid configuration so undo assignment
					assignment.set(tentativeAssignmentX, 0); // 0 is equivalent to undo (reset)
					domains = deepCopy(oldDomains); // Rollback
				}
				// No consistent variable assignments so rollback
				else domains = deepCopy(oldDomains);
			}

			return null; // Failure
		}

		// Helper function used to deep copy domains used for backtracking in forward checking algorithm
		private ArrayList<ArrayList<Integer>> deepCopy(ArrayList<ArrayList<Integer>> domains) {
			ArrayList<ArrayList<Integer>> clonedDomains = new ArrayList<>(domains.size());
			for(ArrayList<Integer> domain : domains) { // Fill in values
				clonedDomains.add(new ArrayList<>(domain));
			}
			return clonedDomains;
		}

		//---------------------------------------------------------------------------------
		// CODE SUPPORT FOR IMPLEMENTING forwardChecking(assignment)
		//
		// It is possible to implement forwardChecking(asn) by using only arcConsistencyFC function from below.
		// 
		// If you have time, I strongly reccomend that you implement arcConsistencyFC and revise from scratch
		// using only implementation of isConsistent algorithm and general utility functions. In my opinion
		// by doing this, you will gain much more from this exercise.
		//
		//---------------------------------------------------------------------------------
		
		
	
		//------------------------------------------------------------------
		//				arcConsistencyFC (AC-3)
		//
		// Implementation of acr-consistency for forward-checking AC-forwardChecking(cv).
		// This is a key component of forwardChecking algorithm, and the only function you need to
		// use in your forwardChecking(asn) implementation
		//------------------------------------------------------------------
		public boolean arcConsistencyFC(Integer X, Integer V){
			//Reduce domain Dx
			domains.get(X).clear();
			domains.get(X).add(V);
			
			//Put in Q all relevant Y where Y>X
			ArrayList<Integer> Q = new ArrayList<>(); //list of all relevant Y
			int col = getColumn(X);
			int row = getRow(X);
			int cell_x = row / size;
			int cell_y = col / size;
			
			//all variables in the same column
			for (int i=0; i<size*size; i++){
				if (getVariable(i,col) > X) {
					Q.add(getVariable(i,col));
				}
			}
			//all variables in the same row
			for (int j=0; j<size*size; j++){
				if (getVariable(row,j) > X) {
					Q.add(getVariable(row,j));
				}
			}
			//all variables in the same size*size box
			for (int i=cell_x*size; i<=cell_x*size + 2; i++) {
				for (int j=cell_y*size; j<=cell_y*size + 2; j++){
					if (getVariable(i,j) > X) {
						Q.add(getVariable(i,j));
					}
				}
			}
		
			//revise(Y,X)
			boolean consistent = true;
			while (!Q.isEmpty() && consistent){
				Integer Y = Q.remove(0);
				if (revise(Y,X)) {
					consistent = !domains.get(Y).isEmpty();
				}
			}
			return consistent;
		}	
		
		
		//------------------------------------------------------------------
		//				revise (part of AC-3 algorithm)
		//------------------------------------------------------------------
		public boolean revise(int Xi, int Xj) {
			boolean isDeleted = false;
			Integer zero = 0;
			
			assert(Xi >= 0 && Xj >=0);
			assert(Xi < size*size*size*size && Xj <size*size*size*size);
			assert(Xi != Xj);

			ArrayList<Integer> Di = domains.get(Xi);
			ArrayList<Integer> Dj = domains.get(Xj);
			
			for (int i=0; i<Di.size(); i++){
				Integer vi = Di.get(i);
				ArrayList<Integer> xiEqVal = new ArrayList<>(size*size*size*size);
				for (int var=0; var<size*size*size*size; var++){
					xiEqVal.add(var,zero);				
				}

				xiEqVal.set(Xi,vi);
				
				boolean hasSupport = false;
				for (int j=0; j<Dj.size(); j++){
					Integer vj = Dj.get(j);
					if (isConsistent(xiEqVal, Xj, vj)) {
						hasSupport = true;
						break;
					}
				}
				
				if (!hasSupport) {
					Di.remove( vi );
					isDeleted = true;
				}
				
			}
			
			return isDeleted;
		}
				
		

		
		//------------------------------------------------------------------
		//isConsistent:
		//
		//Given a partial assignment checks whether its extension with
		//variable = val is consistent with Sudoku rules, i.e. whether it violates
		//any of constraints whose all variables in the scope have been assigned. 
		//This implicitly encodes all constraints describing Sudoku.
		//
		//Before it returns, it undoes the temporary assignment variable=val
		//It can be used as a building block for revise and AC-forwardChecking
		//
		//NOTE: the procedure assumes that all assigned values are in the range 
		// 		{0,..,9}. 
		//-------------------------------------------------------------------
		public boolean isConsistent(ArrayList<Integer> assignment, Integer variable, Integer val) {
			Integer v1,v2;
			
			//variable to be assigned must be clear
			assert(assignment.get(variable) == 0);
			assignment.set(variable,val);

			//alldiff(col[i])
		 	for (int i=0; i<size*size; i++) {
		 		for (int j=0; j<size*size; j++) {
		 			for (int k=0; k<size*size; k++) {
			 			if (k != j) {
			 				v1 = assignment.get(getVariable(i,j));
			 				v2 = assignment.get(getVariable(i,k));
				 			if (v1 != 0 && v2 != 0 && v1.compareTo(v2) == 0) {
				 				assignment.set(variable,0);
				 				return false;
				 			}
				 		}
		 			}
		 		}
		 	}

		 	//alldiff(row[j])
		 	for (int j=0; j<size*size; j++) {
		 		for (int i=0; i<size*size; i++) {
		 			for (int k=0; k<size*size; k++) {
			 			if (k != i) {
			 				v1 = assignment.get(getVariable(i,j));
			 				v2 = assignment.get(getVariable(k,j));
				 			if (v1 != 0 && v2 != 0 && v1.compareTo(v2) == 0) {
				 				assignment.set(variable,0);
				 				return false;
				 			}
			 			}
		 			}
		 		}
		 	}

		 	//alldiff(block[size*i,size*j])
		 	for (int i=0; i<size; i++) {
		 		for (int j=0; j<size; j++) {
		 			for (int i1 = 0; i1<size; i1++) {
		 				for (int j1=0; j1<size; j1++) {
		 					int var1 = getVariable(size*i + i1, size*j + j1);
		 		 			for (int i2 = 0; i2<size; i2++) {
		 		 				for (int j2=0; j2<size; j2++) {
		 		 					int var2 = getVariable(size*i+i2, size*j + j2);
		 		 					if (var1 != var2) {
		 				 				v1 = assignment.get(var1);
		 				 				v2 = assignment.get(var2);
		 		 			 			if (v1 != 0 && v2 != 0 && v1.compareTo(v2) == 0) {
		 					 				assignment.set(variable,0);
		 					 				return false;
		 					 			}
		 		 					}
		 		 				}
		 		 			}
	 
		 				}
		 			}
		 		}
		 	}

			assignment.set(variable,0);
			return true;
		}	
		
		

	
		//------------------------------------------------------------------
		//						initialFC
		//------------------------------------------------------------------
		public boolean initialFC(ArrayList<Integer> anAssignment) {
			//Enforces consistency between unassigned variables and all 
			//initially assigned values; 
			for (int i=0; i<anAssignment.size(); i++){
				Integer V = anAssignment.get(i);
				if (V != 0){
					ArrayList<Integer> Q = getRelevantVariables(i);
					boolean consistent = true;
					while (!Q.isEmpty() && consistent){
						Integer Y = Q.remove(0);
						if (revise(Y,i)) {
							consistent = !domains.get(Y).isEmpty();
						}
					}	
					if (!consistent) return false;
				}
			}
			
			return true;
		}
		
		
	
		
		//------------------------------------------------------------------
		//						getRelevantVariables
		//------------------------------------------------------------------
		public ArrayList<Integer> getRelevantVariables(Integer X){
			//Returns all variables that are interdependent of X, i.e. 
			//all variables involved in a binary constraint with X
			ArrayList<Integer> Q = new ArrayList<>(); //list of all relevant Y
			int col = getColumn(X);
			int row = getRow(X);
			int cell_x = row / size;
			int cell_y = col / size;
			
			//all variables in the same column
			for (int i=0; i<size*size; i++){
				if (getVariable(i,col) != X) {
					Q.add(getVariable(i,col));
				}
			}
			//all variables in the same row
			for (int j=0; j<size*size; j++){
				if (getVariable(row,j) != X) {
					Q.add(getVariable(row,j));
				}
			}
			//all variables in the same size*size cell
			for (int i=cell_x*size; i<=cell_x*size + 2; i++) {
				for (int j=cell_y*size; j<=cell_y*size + 2; j++){
					if (getVariable(i,j) != X) {
						Q.add(getVariable(i,j));
					}
				}
			}	
			
			return Q;
		}
		
		



		//------------------------------------------------------------------
		// Functions translating between the puzzle and an assignment
		//-------------------------------------------------------------------
		public ArrayList<Integer> getAssignment(int[][] p) {
			ArrayList<Integer> assignments = new ArrayList<>();
			for (int i = 0; i < size * size; i++) {
				for (int j=0; j< size * size; j++) {
					assignments.add(getVariable(i,j), p[i][j]);
					if (p[i][j] != 0) {
							//restrict domain
							domains.get(getVariable(i,j)).clear();
							domains.get(getVariable(i,j)).add(p[i][j]);
					}
				}
			}
			return assignments;
		}	
		
	
		public int[][] getPuzzle(ArrayList assignments) {
			int[][] p = new int[size * size][size * size];
			for (int i = 0; i < size * size; i++) {
				for (int j=0; j < size * size; j++) {
					Integer val = (Integer) assignments.get(getVariable(i,j));
					p[i][j] = val;
				}
			}
			return p;
		}

	
		//------------------------------------------------------------------
		//Utility functions
		//-------------------------------------------------------------------
		public int getVariable(int row, int col) {
			assert( row < size * size && col < size * size);
			assert( row >= 0 && col >= 0);
			return (row * size * size + col);
		}	
		
		
		public int getRow(int X) {
			return (X / (size * size));
		}	
		
		public int getColumn(int X){
			return X - ((X / (size * size)) * size * size);
		}	
		
		
		
		
		
}
