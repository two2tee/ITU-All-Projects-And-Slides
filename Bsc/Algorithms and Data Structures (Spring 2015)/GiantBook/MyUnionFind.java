/****************************************************************************
 *  Compilation:  javac WeightedQuickUnionUF.java
 *  Execution:  java WeightedQuickUnionUF < input.txt
 *  Dependencies: StdIn.java StdOut.java
 *
 *  Weighted quick-union (without path compression).
 *
 ****************************************************************************/

/**
 *  The <tt>WeightedQuickUnionUF</tt> class represents a union-find data structure.
 *  It supports the <em>union</em> and <em>find</em> operations, along with
 *  methods for determinig whether two objects are in the same component
 *  and the total number of components.
 *  <p>
 *  This implementation uses weighted quick union by size (without path compression).
 *  Initializing a data structure with <em>N</em> objects takes linear time.
 *  Afterwards, <em>union</em>, <em>find</em>, and <em>connected</em> take
 *  logarithmic time (in the worst case) and <em>count</em> takes constant
 *  time.
 *  <p>
 *  For additional documentation, see <a href="http://algs4.cs.princeton.edu/15uf">Section 1.5</a> of
 *  <i>Algorithms, 4th Edition</i> by Robert Sedgewick and Kevin Wayne.
 *     
 *  @author Robert Sedgewick
 *  @author Kevin Wayne
 */
public class MyUnionFind {
    //Fields:
    private int[] parent;   // parent[i] = parent of i
    private int[] size;     // size[i] = number of objects in subtree rooted at i
    private int count;      // number of components at each union
    private int total;       // total components declared
    private int isolatedTotal; // total components after each union, will be 0 or -1 after last union
    private int greatestComponent;

    // Boolean variables indicated if the graph has reached
    // non-isolated, giant and connection
    private boolean isNonIsolated = false;
    private boolean isGiant = false;
    private boolean isConnected = false;


    //Constructor
    /**
     * Initializes an empty union-find data structure with N isolated components 0 through N-1.
     * @throws java.lang.IllegalArgumentException if N < 0
     * @param N the number of objects
     */
    public MyUnionFind(int N) 
    {
        count = N;
        total = N;
        parent = new int[N];
        size = new int[N];
        isolatedTotal = N;
        greatestComponent = 0;

        for (int i = 0 ; i < N ; i++) {
            parent[i] = i;
            size[i] = 1;
        }
    }

    /**
     * Returns the number of components.
     * @return the number of components (between 1 and N)
     */
    public int count() {
        return count;
    }


    //getter methods
    public boolean isNonIsolated() { return isConnected; }
    public boolean isGiant()       { return isGiant;     }
    public boolean isConnected()   { return isConnected; }


    //setter method

    /**
    * The network is nonisolated when node has at least
    * one connection.
    * That is when the connected array (array with connected components) is
    * greater or equal to the total declared components
    */
    private void setNonIsolated(){

        if(isolatedTotal == 0)
        {
            isNonIsolated = true;
        }
    }

  /**
   * The network has a giant component when it contains
   * a component that is connected with at least half the components.
   *  - newly unified component >= total/2.
   **/
    private void setGiant()
    {
        if (greatestComponent >= total / 2) 
        {
            isGiant = true;
        }
    }

    /**
    * The network is connected when every components are connected as 
    * a single component.
    */
    private void setConnected()
    {
        if(count == 1) isConnected = true;
    }


    /**
     * Returns the component identifier for the component containing site <tt>p</tt>.
     * @param p the integer representing one site
     * @return the component identifier for the component containing site <tt>p</tt>
     * @throws java.lang.IndexOutOfBoundsException unless 0 <= p < N
     */
    public int find(int p) 
    {
        validate(p);
        while (p != parent[p])
            p = parent[p]; // Assignment: added tree-depth reduction fix
        return p;
    }

    // validate that p is a valid index
    private void validate(int p) {
        int N = parent.length;
        if (p < 0 || p >= N) {
            throw new IndexOutOfBoundsException("index " + p + " is not between 0 and " + N);
        }
    }

    /**
     * Are the two sites <tt>p</tt> and <tt>q</tt> in the same component?
     * @param p the integer representing one site
     * @param q the integer representing the other site
     * @return <tt>true</tt> if the two sites <tt>p</tt> and <tt>q</tt>
     *    are in the same component, and <tt>false</tt> otherwise
     * @throws java.lang.IndexOutOfBoundsException unless both 0 <= p < N and 0 <= q < N
     */
    public boolean connected(int p, int q) {
        return find(p) == find(q);
    }

  
    /**
     * Merges the component containing site<tt>p</tt> with the component
     * containing site <tt>q</tt>.
     * @param p the integer representing one site
     * @param q the integer representing the other site
     * @throws java.lang.IndexOutOfBoundsException unless both 0 <= p < N and 0 <= q < N
     */
    public void union(int p, int q) {
        int rootP = find(p);
        int rootQ = find(q);


		if (parent[p] == p && size[p] == 1) isolatedTotal--;
		if (parent[q] == q && size[q] == 1) isolatedTotal--;
        if (rootP == rootQ) return;

        // make smaller root point to larger one
        if (size[rootP] < size[rootQ]) 
        {
            parent[rootP] = rootQ;
            size[rootQ] += size[rootP];
            if (size[rootQ] > greatestComponent){ 
                greatestComponent = size[rootQ];
            }
        }
        else {
            parent[rootQ] = rootP;
            size[rootP] += size[rootQ];
            if (size[rootQ] > greatestComponent){
             greatestComponent = size[rootP];
         }
        }
        count--;



        // when p and q is connected add the element at index p and q = 1 (NOTE: 1 = true)


        //Check if the graph is non-isolated, giant or connected
        if (!isNonIsolated) setNonIsolated();
        if (!isGiant)       setGiant();
        if (!isConnected)   setConnected();
    }

}
