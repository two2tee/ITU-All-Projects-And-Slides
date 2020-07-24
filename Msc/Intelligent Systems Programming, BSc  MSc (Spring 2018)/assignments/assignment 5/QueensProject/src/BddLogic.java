import net.sf.javabdd.*;

/**
 * Solution to the NP-hard n-queens problem using a Binary Decision Diagram library.
 * Binary decision diagrams provide a compact representation and efficient operations on boolean functions.
 * Thus, a BDD is used to represent the n-queens problem and its rules as a logical function over some variables.
 * In chess, the N-Queens Puzzle is described as follows:
 * Given an N x N chessboard and N queens, arrange the queens onto the board such that no two queens threaten each other.
 * Namely, no two queens should be on the same row, column or diagonal.
 */
public class BddLogic implements IQueensLogic {
    private int N;
    private int[][] board;
    private BDDFactory bddFactory;
    private BDD currentBdd;
    private BDD True;
    private BDD False;

    /**
     * Initializes the quadratic board with the given size and initializes the board according to the rules
     * of the n-queen problem.
     * @param size The size of the board (i.e. size = #rows = #columns)
     */
    @Override
    public void initializeBoard(int size) {
        N = size;
        board = new int[N][N];
        initializeBDD();
        initializeRules();
    }

    /**
     * Return a representation of the board where each entry [c][r] is either
     *  1 : a queen is or must be present in column c and row r,
     * -1 : a queen cannot be present in column c and row r
     *  0 : a queen has not yet been placed in column c and row r. It does not have to
     *      be there but it is allowed to be there.
     * Columns are counted from left to right (starting with 0),
     * and rows are counted from top to bottom (counting from 0).
     */
    @Override
    public int[][] getBoard() {
        return board;
    }

    /**
     * Inserts a queen at the specified position and updates the rest of the board accordingly,
     * that is afterwards the board specifies where there _must_ be queens and where there _cannot_ be queens.
     */
    @Override
    public void insertQueen(int column, int row) {
        //Check if queen is placeable on cell
        if(board[column][row] == -1 || board[column][row] == 1) { return; }

        //Restrict board to remaining options
        restrictBoard(column,row);

        // Place queen
        board[column][row] = 1;
    }

    // Initialize empty BDD with correct number of variables, cache and nodes
    private void initializeBDD() {
        // Initialize Bdd factory
        final int nodes = 2_000_000,
                  cache = 200_000,
                  variables = N * N;
        bddFactory = JFactory.init(nodes,cache);
        bddFactory.setVarNum(variables);

        // Clarify true and false
        True = bddFactory.one();
        False = bddFactory.zero();

        // Initial board has a solution
        currentBdd = True;
    }


    // Apply logical rules of the n-queens problem to the board
    private void initializeRules() {
        createHorizontalRules();
        createVerticalRules();
        createDiagonalRules();
    }

    // Convert assignment into logical restriction and restrict BDD to remaining available options
    private void restrictBoard(int placedX, int placedY){
        currentBdd = currentBdd.restrict(getVar(atCell(placedX,placedY)));

        for (int otherX = 0; otherX < N; otherX++) {
            for (int otherY = 0; otherY < N; otherY++) {
                if(!isCellValid(otherX,otherY)){
                    board[otherX][otherY] = -1; // -1 = invalid
                }
            }
        }
    }

    // Test BDD by trying to place a queen
    private boolean isCellValid(int x, int y) {
        BDD testBdd = currentBdd.restrict(getVar(atCell(x,y)));
        return !testBdd.isZero(); // BDD is satisfiable
    }

    // Retrieves variable id
    private int atCell(int x, int y) {
        return x + (y * N);
    }

    // Restrict board vertically
    private void createVerticalRules() {
        for (int x = 0; x < N; x++) {
            BDD combined  = False;

            // If queen is placed on column (Placed) (or clause)
            for (int y = 0; y < N; y++) {
                BDD placed = getVar(atCell(x,y));
                for (int otherY = 0; otherY < N; otherY++) {
                    if(otherY == y) continue; //skip current y
                    placed = placed.and(negateVar(atCell(x, otherY))); // Unique
                }
                combined = combined.or(placed);
            }

            // Conjunction of 'or' clauses / disjunctions
            currentBdd = currentBdd.and(combined);
        }
    }

    // Restrict board horizontally
    private void createHorizontalRules() {
        for (int y = 0; y < N; y++) {
            BDD combined  = False;

            // If queen is placed on row (Placed) (or clause)
            for (int x = 0; x < N; x++) {
                BDD placed = getVar(atCell(x,y));
                for (int otherX = 0; otherX < N; otherX++) {
                    if(otherX == x) continue; //skip current x
                    placed = placed.and(negateVar(atCell(otherX, y))); // Unique
                }
                combined = combined.or(placed);
            }

            // Conjunction of 'or' clauses / disjunctions
            currentBdd = currentBdd.and(combined);
        }
    }

    // Restrict BDD diagonally to remaining available assignments after placing assignment on (x,y)
    private void createDiagonalRules() {
        for(int y = 0; y < N; y++) {
            BDD combined = False;
            for (int x = 0; x < N; x++) {
                combined = combined.or(traverseDiagonal(x, y));
            }
            currentBdd = currentBdd.and(combined);
        }
    }

    // Restrict BDD diagonally to remaining available assignments after placing assignment on (x,y)
    private BDD traverseDiagonal(int x, int y) {
        BDD placed = getVar(atCell(x,y));
        for (int otherX = 0; otherX < N; otherX++) {
            if (otherX != x) {
                // Traverse upper right diagonal within board
                int otherY = y - otherX + x; // Right diagonal
                if ( otherY < N && otherY > 0) {
                    placed = placed.and(negateVar(atCell(otherX, otherY)));
                }

                // Traverse upper left diagonal within board
                otherY = y + otherX - x;
                if (otherY < N && otherY > 0) {
                    placed = placed.and(negateVar(atCell(otherX, otherY)));
                }
            }
        }
        return placed;
    }

    // Negate variable in BDD node cell
    private BDD negateVar(int cell) {
        return bddFactory.nithVar(cell);
    }

    // Retrieve variable from node cell in BDD
    private BDD getVar(int cell) {
        return bddFactory.ithVar(cell);
    }

}
