// For week 2

// Code from Goetz et al 5.6, written by Brian Goetz and Tim Peierls.
// Modifications by sestoft@itu.dk * 2014-09-08

import java.util.concurrent.*;
import java.util.concurrent.atomic.AtomicLong;
import java.util.concurrent.atomic.AtomicReference;
import java.util.*;
import java.util.function.Function;
import java.util.function.IntToDoubleFunction;


public class TestCache {
  public static void main(String[] args) throws InterruptedException {
    SystemInfo();

    System.out.println("Memoizer 0");
    Mark7(String.format("countFactorizerParallel %6d", 16), 
            i -> countFactorizerParallel(new Memoizer0<Long, long[]>(new Factorizer())));
            
    System.out.println("Memoizer 1");
    Mark7(String.format("countFactorizerParallel %6d", 16), 
        i -> countFactorizerParallel(new Memoizer1<Long, long[]>(new Factorizer())));
    
    System.out.println("Memoizer 2");
    Mark7(String.format("countFactorizerParallel %6d", 16), 
        i -> countFactorizerParallel(new Memoizer2<Long, long[]>(new Factorizer())));
    
    System.out.println("Memoizer 3");
    Mark7(String.format("countFactorizerParallel %6d", 16), 
        i -> countFactorizerParallel(new Memoizer3<Long, long[]>(new Factorizer())));
            
    System.out.println("Memoizer 4");
    Mark7(String.format("countFactorizerParallel %6d", 16), 
        i -> countFactorizerParallel(new Memoizer4<Long, long[]>(new Factorizer())));
            
    System.out.println("Memoizer 5");
    Mark7(String.format("countFactorizerParallel %6d", 16), 
        i -> countFactorizerParallel(new Memoizer5<Long, long[]>(new Factorizer())));
            
}

// Factorize in parallel in intervals of 2000 factorization numbers 
private static long countFactorizerParallel(Computable<Long, long[]> f) {
    final int threadCount = 16;
    final long start = 10_000_000_000L, range = 2000L;
    final Thread[] threads = new Thread[threadCount];
    for (int t=0; t < threadCount; t++) {
      final int threadId = t;
      threads[t] =
        new Thread(() -> {
          long from1 = start;
          long to1 = from1+range;
          long from2 = start+range+threadId*range/4;
          long to2 = from2+range;

          try {

          for (long i = from1; i < to1; i++) {
            long[] value = f.compute(i);
          }

          for(long i = from2; i < to2; i++){
            long[] value = f.compute(i);
            //System.out.println("Thread: " + threadId + ", from: " + from2 +", to: " + to2);
          }
        }
        catch (InterruptedException ex) {
          System.out.println("Computation was interrupted");
        }
        });
        threads[t].start();
      }

      for(int i = 0; i < threadCount; i++){
        try {
          threads[i].join();
        } catch (InterruptedException e ){
          e.printStackTrace();
        }
      }
      return 0L;
  	}

public static double Mark7(String msg, IntToDoubleFunction f) {
    int n = 10, count = 1, totalCount = 0;
    double dummy = 0.0, runningTime = 0.0, st = 0.0, sst = 0.0;
    do { 
      count *= 2;
      st = sst = 0.0;
      for (int j=0; j<n; j++) {
        Timer t = new Timer();
        for (int i=0; i<count; i++) 
          dummy += f.applyAsDouble(i);
        runningTime = t.check();
        double time = runningTime * 1e6 / count; // microseconds
        st += time; 
        sst += time * time;
        totalCount += count;
      }
    } while (runningTime < 0.25 && count < Integer.MAX_VALUE/2);
    double mean = st/n, sdev = Math.sqrt((sst - mean*mean*n)/(n-1));
    System.out.printf("%-25s %15.1f us %10.2f %10d%n", msg, mean, sdev, count);
    return dummy / totalCount;
  }

public static void SystemInfo() {
    System.out.printf("# OS:   %s; %s; %s%n", 
                      System.getProperty("os.name"), 
                      System.getProperty("os.version"), 
                      System.getProperty("os.arch"));
    System.out.printf("# JVM:  %s; %s%n", 
                      System.getProperty("java.vendor"), 
                      System.getProperty("java.version"));
    // The processor identifier works only on MS Windows:
    System.out.printf("# CPU:  %s; %d \"cores\"%n", 
                      System.getenv("PROCESSOR_IDENTIFIER"),
                      Runtime.getRuntime().availableProcessors());
    java.util.Date now = new java.util.Date();
    System.out.printf("# Date: %s%n", 
      new java.text.SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ssZ").format(now));
  }

}


