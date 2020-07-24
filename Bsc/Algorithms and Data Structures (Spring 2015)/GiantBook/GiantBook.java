
/**
* The class is a representation of the giant book
*/
public class GiantBook {
    //fields
    private MyUnionFind uf;

    private static boolean reachedNonIsolated = false;
    private static boolean reachedGiantStatus = false;
    private static boolean reachedConnected = false;

    private static double isoCount;
    private static double connectedCount;
    private static double giantCount;

    private  static double[] nonIsoArray;
    private static double[] giantArray;
    private static double[] connectArray;
    
    //Main method

    /*
        This method is responsble for printing out the results of the
    */
    private static void calcStdDev(int samples) {
        
        double stddevNonIso = StdStats.stddev(nonIsoArray);
        double stddevGiant = StdStats.stddev(giantArray);
        double stddevCon = StdStats.stddev(connectArray);

        StdOut.println( "Sampels = " + samples + "\n" + 
                        "stddev Non-Isolated: " + stddevNonIso + "\n" +
                        "stddev Giant: " + stddevGiant + "\n" + 
                        "stddev Connnected: " + stddevCon);

    }

    private static void calcMean(){

        double meanNonIso =  StdStats.mean(nonIsoArray);
        double meanGiant  =  StdStats.mean(giantArray);
        double meanCon    =  StdStats.mean(connectArray);

        StdOut.println( "mean Non-Isolated: " + meanNonIso + "\n" +
                        "mean Giant: " + meanGiant + "\n" +
                        "mean Connnected: " + meanCon);
    }


    public static void main(String[] args) {
        StdOut.println("Please enter N : number of components");
        int N = StdIn.readInt();    // Number of components.

        StdOut.println("Please enter T : number of samples");
        int T = StdIn.readInt();    // Number of components.
        double stopEnd = 0.0;


        int rep = 0;                    // Number of repetitions made.

        nonIsoArray = new double[T];
        giantArray = new double[T];
        connectArray = new double[T]; 


        StdOut.println("Calculating...");

    for(int i = 0 ;i < T; i++)
    {
        Stopwatch stopWatch = new Stopwatch();
        // connects and calculate until our graph reach non-isolated state, giant state and connected.
        MyUnionFind uf = new MyUnionFind(N);
        while (!reachedNonIsolated || !reachedGiantStatus || !reachedConnected) {

            rep++;  //Increase rep each time the loop repeats

            //selecting a random between 0 and N-1 and perform an union with the two numbers
            int p = StdRandom.uniform(N);
            int q = StdRandom.uniform(N);
            while(p == q) q = StdRandom.uniform(N);  //If p and q are the same change q
            
            uf.union(p,q);
            

            //Check if non-iso, giant and connection is reached.
            //When each state is reached, add number of reps to an array, used to proccess results for each state.

            //non-iso state
            if (!reachedNonIsolated && uf.isNonIsolated()) {
                reachedNonIsolated = true;
                isoCount = rep;
                nonIsoArray[i] = rep;

            }

            //Giant state
            if (!reachedGiantStatus && uf.isGiant()) {
                reachedGiantStatus = true;
                giantCount = rep;
                giantArray[i] = rep;
                stopEnd += stopWatch.elapsedTime();
            }

            //Connected state
            if (!reachedConnected && uf.isConnected()) {
                reachedConnected = true;
                connectedCount = rep;
                connectArray[i] = rep;
            }
        }
        rep = 0;
		reachedConnected = false;
		reachedGiantStatus = false;
		reachedNonIsolated = false;
    }

    StdOut.println("On average the giant component emerges after: " + stopEnd/T + " seconds");

        //When connection and T reps was reached, print results
        StdOut.println("| 100% |");
        StdOut.println();
        StdOut.println("----- Standard Deviation -----");
        calcStdDev(T);
        StdOut.println("");
        StdOut.println("----- Mean -----");
        calcMean();
    }
}

