// For week 12
// sestoft@itu.dk * 2015-11-05

import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.Timer;
import java.util.concurrent.*;
import java.util.concurrent.atomic.AtomicLong;
import java.util.concurrent.atomic.LongAdder;

public class TestChaseLevQueue extends Tests {
  final static int size = 100_000_000; // Number of integers to sort

  public static void main(String[] args) throws Exception {
    //multiQueueMultiThreadCL(8);
    sequentialTest(new ChaseLevDeque<>(10));
    concurrentTest(new ChaseLevDeque<>(1_000_000));
  }

  // ----------------------------------------------------------------------
  // Version E: Multi-queue multi-thread setup SOLUTION, thread-local queues


  /*1. Write test cases for the ChaseLevDeque<T> implementation, first a precise sequential functional tests (for
  instance, that if you push an item and then immediately pop, you get the same item back).*/
  private static void sequentialTest(Deque<Integer> queues) throws Exception{
    queues.push(10);
    assertEquals(10, queues.pop());
    assertTrue(queues.pop() == null);
    System.out.println("Sequential test passed.");

  }

  /*
  * 2. Write approximate concurrent tests, as discussed in course week 8. In these tests you should create a single
  ChaseLevDeque<T> instance and then have a single thread that performs push and pop operations on
  the instance being tested, and multiple other threads that perform concurrent steal operations on it. For
  example, you may push one million random Integers, and concurrently pop or steal one million random
  Integers, and afterwards check that the sum of the pushed numbers equals the sum of the popped or stolen
  numbers.
  Make the queue instance large enough so that you avoid overflowing it; that would throw an exception and
  ruin the test. Use a CyclicBarrier to make sure all the testing threads are ready to start at the same time, and
  use a CyclicBarrier to make sure all the testing threads terminate before checking the results.
  Show your test code, explain what it does, explain what parameters (how many threads, how many pushes,
  and so on) you run it with, and show results of running it.*/

  private static void concurrentTest(Deque<Integer> queues) throws Exception {
    ExecutorService executor = Executors.newCachedThreadPool();
    int parties = 7;
    int numOFStealers = parties-1;
    int trials = 2_000_000;
    int push = 1_000_000;

    CyclicBarrier barrier = new CyclicBarrier(parties);
    List<Future<Integer>> stealResults = new ArrayList<>();
    List<Future<int[]>> poppushResults = new ArrayList<>();

    //Add PopPusher
    poppushResults.add(executor.submit(new PushPopOperation(queues,push,barrier, trials)));

    //Add Stealers
    for (int i = 0 ; i < numOFStealers ; i++)    {
      stealResults.add(executor.submit(new StealOperation(queues,barrier, trials)));
    }


    int sumPush = 0;
    int sumSteal = 0;
    int sumPop = 0;

    for (Future<int[]> sum : poppushResults) {
      int[] result = sum.get();
      sumPush = sumPush + result[0];
      sumPop = sumPop + result[1];
    }

    for (Future<Integer> sum : stealResults) {
       sumSteal = sumSteal + sum.get();
    }

    assertEquals(sumPush,sumSteal+sumPop);
    System.out.println("Concurrency test passed.");
  }

  public static class PushPopOperation implements Callable<int[]> {

    private Deque<Integer> queues;
    private int numberOfPushes;
    private final CyclicBarrier barrier;
    private final int trials;

    public PushPopOperation(Deque<Integer> queues, int numberOfPushes, CyclicBarrier barrier, int trials){
      this.queues = queues;
      this.numberOfPushes = numberOfPushes;
      this.barrier = barrier;
      this.trials = trials;
    }

    @Override
    public int[] call() throws BrokenBarrierException, InterruptedException {
      int numberPopped = 0;
      int numberPushed = 0;
      Random random = new Random();

      barrier.await();

      for (int i = 0; i < numberOfPushes; i++) {
        int randomInt = random.nextInt();
        queues.push(randomInt);
        numberPushed += randomInt;
      }

      Integer popped;
      for (int i = 0; i < trials; i++) {
        popped = queues.pop();
        if (popped != null)
          numberPopped += popped;
      }
      return new int[]{numberPushed,numberPopped};
    }
  }

  public static class StealOperation implements Callable<Integer> {
    private Deque<Integer> queues;
    private final CyclicBarrier barrier;
    private final int trials;

    public StealOperation(Deque<Integer> queues, CyclicBarrier barrier, int trials){
      this.queues = queues;
      this.barrier = barrier;
      this.trials = trials;
    }

    @Override
    public Integer call() throws Exception {
      Integer item;
      int sumStolen = 0;
      barrier.await();
      for(int i = 0 ; i < trials ; i++){
        item = queues.steal();
        if(item != null) {
          sumStolen += item;
        }
      }
      return sumStolen;
    }
  }

  @SuppressWarnings("unchecked")
  private static void multiQueueMultiThreadCL(final int threadCount) {
    // Java's @$#@?!! type system requires this unsafe cast 
    ChaseLevDeque<SortTask>[] queues
            = (ChaseLevDeque<SortTask>[])(new ChaseLevDeque[threadCount]);
    for (int t=0; t<threadCount; t++)
      queues[t] = new ChaseLevDeque<SortTask>(100000);
    int[] arr = IntArrayUtil.randomIntArray(size);
    queues[0].push(new SortTask(arr, 0, arr.length-1));
    Timer t = new Timer();
    mqmtWorkers(queues, threadCount);
    //System.out.printf("multiQueueMultiThreadCL %3d %15.3f s ", threadCount, t.check());
    System.out.println(IntArrayUtil.isSorted(arr));
    // IntArrayUtil.printout(arr, 100);
  }

  private static void mqmtWorkers(Deque<SortTask>[] queues, int threadCount) {
    final Thread[] threads = new Thread[threadCount];
    final LongAdder ongoing = new LongAdder();
    ongoing.add(1);
    for (int t=0; t<threadCount; t++) {
      final int myNumber = t;
      threads[t] = new Thread(() -> {
        SortTask task;
        while (null != (task = getTask(myNumber, queues, ongoing))) {
          final int[] arr = task.arr;
          final int a = task.a, b = task.b;
          if (a < b) {
            int i = a, j = b;
            int x = arr[(i+j) / 2];
            do {
              while (arr[i] < x) i++;
              while (arr[j] > x) j--;
              if (i <= j) {
                swap(arr, i, j);
                i++; j--;
              }
            } while (i <= j);
            ongoing.add(2);
            queues[myNumber].push(new SortTask(arr, a, j));
            queues[myNumber].push(new SortTask(arr, i, b));
          }
          ongoing.decrement();
        }
      });
    }
    for (int t=0; t<threadCount; t++)
      threads[t].start();
    try {
      for (int t=0; t<threadCount; t++)
        threads[t].join();
    } catch (InterruptedException exn) { }
  }

  // Swap arr[s] and arr[t]
  private static void swap(int[] arr, int s, int t) {
    int tmp = arr[s];  arr[s] = arr[t];  arr[t] = tmp;
  }

  // Tries to get a sorting task.  If task queue is empty, repeatedly
  // try to steal, cyclically, from other threads and if that fails
  // wait a moment, while some tasks are computing.

  private static SortTask getTask(final int myNumber, final Deque<SortTask>[] queues,
                                  LongAdder ongoing) {
    final int threadCount = queues.length;
    SortTask task = queues[myNumber].pop();
    if (null != task)
      return task;
    else {
      do {
        for (int t=0; t<threadCount-1; t++)
          if (null != (task = queues[(myNumber+t) % threadCount].steal()))
            return task;
        Thread.yield();
      } while (ongoing.longValue() > 0);
      return null;
    }
  }
}

// ----------------------------------------------------------------------
// SortTask class, Deque<T> interface, SimpleDeque<T> 

// Represents the task of sorting arr[a..b]
class SortTask {
  public final int[] arr;
  public final int a, b;

  public SortTask(int[] arr, int a, int b) {
    this.arr = arr;
    this.a = a;
    this.b = b;
  }
}

interface Deque<T> {
  void push(T item);    // at bottom
  T pop();              // from bottom
  T steal();            // from top
}

// ----------------------------------------------------------------------

// A lock-free queue simplified from Chase and Lev: Dynamic circular
// work-stealing queue, SPAA 2005.  We simplify it by not reallocating
// the array; hence this queue may overflow.  This is close in spirit
// to the original ABP work-stealing queue (Arora, Blumofe, Plaxton:
// Thread scheduling for multiprogrammed multiprocessors, 2000,
// section 3) but in that paper an "age" tag needs to be added to the
// top pointer to avoid the ABA problem (see ABP section 3.3).  This
// is not necessary in the Chase-Lev dequeue design, where the top
// index never assumes the same value twice.

class ChaseLevDeque<T> implements Deque<T> {
  // Invariants and meaning of fields is the same as in the SimpleDeque.
  private volatile long bottom = 0;
  private final AtomicLong top = new AtomicLong();
  private final T[] items;

  public ChaseLevDeque(int size) {
    this.items = makeArray(size);
  }

  @SuppressWarnings("unchecked")
  private static <T> T[] makeArray(int size) {
    // Java's @$#@?!! type system requires this unsafe cast    
    return (T[])new Object[size];
  }

  private static int index(long i, int n) {
    return (int)(i % (long)n);
  }

  public void push(T item) { // at bottom
    final long b = bottom, t = top.get(), size = b - t;
    if (size == items.length)
      throw new RuntimeException("queue overflow");
    items[index(b, items.length)] = item;
    bottom = b+1;
  }

  public T pop() { // from bottom
    final long b = bottom - 1;
    bottom = b;
    final long t = top.get(), afterSize = b - t;
    if (afterSize < 0) { // empty before call
      bottom = t;
      return null;
    } else {
      T result = items[index(b, items.length)];
      if (afterSize > 0) // non-empty after call
        return result;
      else {             // became empty, update both top and bottom
        if (!top.compareAndSet(t, t+1)) // somebody stole result
          result = null;
        bottom = t+1;
        return result;
      }
    }
  }

  public T steal() { // from top
    final long t = top.get(), b = bottom, size = b - t;
    if (size <= 0)
      return null;
    else {
      T result = items[index(t, items.length)];
      if (top.compareAndSet(t, t+1))
        return result;
      else
        return null;
    }
  }
}

// ----------------------------------------------------------------------

class IntArrayUtil {
  public static int[] randomIntArray(final int n) {
    int[] arr = new int[n];
    for (int i = 0; i < n; i++)
      arr[i] = (int) (Math.random() * n * 2);
    return arr;
  }

  public static void printout(final int[] arr, final int n) {
    for (int i=0; i < n; i++)
      System.out.print(arr[i] + " ");
    System.out.println("");
  }

  public static boolean isSorted(final int[] arr) {
    for (int i=1; i<arr.length; i++)
      if (arr[i-1] > arr[i])
        return false;
    return true;
  }
}
