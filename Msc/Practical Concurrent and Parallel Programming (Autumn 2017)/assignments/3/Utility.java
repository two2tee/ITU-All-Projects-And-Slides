
// Utility class with methods used to reuse streams and measure time of operations
public class Utility {

    // Measure elapsed time in Java (or use time command in terminal)
    protected static void measure(Runnable func) { // or use Thunk.apply()
        long startTime = System.nanoTime();
        func.run(); // Task
        long endTime = System.nanoTime();
        long output = endTime - startTime; // Elapsed time
        System.out.println("Elapsed time in milliseconds : " + output / 100000);

    }

    // Functional interface used to describe a function that takes no arguments and returns no result
    //private interface Thunk { void apply(); }

}
