// For week 12
// sestoft@itu.dk * 2014-11-16

// Unbounded list-based lock-free queue by Michael and Scott 1996 (who
// call it non-blocking).

import java.util.Random;
import java.util.concurrent.CyclicBarrier;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.concurrent.atomic.AtomicReference;
import java.util.concurrent.atomic.AtomicReferenceFieldUpdater;
import java.util.function.IntToDoubleFunction;

public class TestMSQueue extends Tests {
  public static void main(String[] args) throws Exception {
    sequentialTest(new MSLockQueue<>());
    parallelTest(new MSLockQueue<>());
    benchmarkTest(new MSLockQueue<>());
  }

  //Dequeue from head, enqueue from tail
  //1. Write a simple sequential test for the Michael-Scott queue implementation. You may adapt the sequential
  // test from week 8’s TestBoundedQueueTest.java file.

  private static void sequentialTest(UnboundedQueue<Integer> bq) throws Exception {
    System.out.printf("%nSequential test: %s", bq.getClass());
    assertTrue(bq.dequeue() == null);
    bq.enqueue(7); bq.enqueue(9); bq.enqueue(13);
    assertEquals(bq.dequeue(), 7);
    assertEquals(bq.dequeue(), 9);
    assertEquals(bq.dequeue(), 13);
    assertTrue(bq.dequeue() == null);
    System.out.println("... passed");
  }

  /*2. Write a concurrent test for the Michael-Scott queue implementation. You may adapt the concurrent test from
    the same file mentioned above. In the original test it is enough for each consumer to perform nTrials
    calls to take because each call is guaranteed to return an item, but when testing the non-blocking queue a
    consumer must loop and call dequeue until it has obtained nTrials actual non-null items.*/
  private static void parallelTest(UnboundedQueue<Integer> bq) throws Exception {
    System.out.printf("%nParallel test: %s", bq.getClass());
    final ExecutorService pool = Executors.newCachedThreadPool();
    new PutTakeTest(bq, 17, 100000).test(pool);
    pool.shutdown();
    System.out.println("... passed");
  }

  private static void benchmarkTest(UnboundedQueue<Integer> bq) throws Exception {
    int threadCount = 4;
    for(int i = 1; i <= threadCount; i++) {
      final int thread = i;
      Mark7(String.format("%-21s %d", "MSQueue", thread), j -> benchmark(bq, thread));
    }
  }

  private static double benchmark(UnboundedQueue<Integer> bq, int threadCount) {
    final ExecutorService pool = Executors.newCachedThreadPool();
    double putSum = new BenchmarkTest(bq, threadCount, 100).test(pool);
    pool.shutdown();
    return putSum;
  }


  // --- Benchmarking infrastructure ---

  private static class Timer {
    private long start, spent = 0;
    public Timer() { play(); }
    public double check() { return (System.nanoTime()-start+spent)/1e9; }
    public void pause() { spent += System.nanoTime()-start; }
    public void play() { start = System.nanoTime(); }
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
}




interface UnboundedQueue<T> {
  void enqueue(T item);
  T dequeue();
}

// ------------------------------------------------------------
// Unbounded lock-based queue with sentinel (dummy) node

class LockingQueue<T> implements UnboundedQueue<T> {
  // Invariants:
  // The node referred by tail is reachable from head.
  // If non-empty then head != tail,
  //    and tail points to last item, and head.next to first item.
  // If empty then head == tail.

  private static class Node<T> {
    final T item;
    Node<T> next;

    public Node(T item, Node<T> next) {
      this.item = item;
      this.next = next;
    }
  }

  private Node<T> head, tail;

  public LockingQueue() {
    head = tail = new Node<T>(null, null);
  }

  public synchronized void enqueue(T item) { // at tail
    Node<T> node = new Node<T>(item, null);
    tail.next = node;
    tail = node;
  }

  public synchronized T dequeue() {     // from head
    if (head.next == null)
      return null;
    Node<T> first = head;
    head = first.next;
    return head.item;
  }
}


// ------------------------------------------------------------
// Unbounded lock-free queue (non-blocking in M&S terminology),
// using CAS and AtomicReference

// This creates one AtomicReference object for each Node object.  The
// next MSQueueRefl class further below uses one-time reflection to
// create an AtomicReferenceFieldUpdater, thereby avoiding this extra
// object.  In practice the overhead of the extra object apparently
// does not matter much.

class MSLockQueue<T> implements UnboundedQueue<T> {
  private  Node<T> head, tail;

  public MSLockQueue() {
    Node<T> dummy = new Node<>(null, null);
    head = dummy;
    tail = dummy;
  }

  public synchronized void enqueue(T item) { // at tail
    Node<T> newNext = new Node<>(item, null);
    Node<T> last = tail;
    last.setNext(newNext);
    tail = newNext;
  }

  public synchronized T dequeue() { // from head
      Node<T> first = head,  next = first.getNext(); // D3
      if(next == null){
        return null;
      }
      else {
        T item = first.getNext().item;
        head = next;
        return item;
      }
  }

  private static class Node<T> {
    final T item;
    private Node<T> next;

    public Node(T item, Node<T> next) {
      this.item = item;
      this.next = next;
    }

    public Node<T> getNext(){
      return next;
    }

    public void setNext(Node<T> node){
      next = node;
    }
  }



}


// --------------------------------------------------
// Lock-free queue, using CAS and reflection on field Node.next

class MSQueueRefl<T> implements UnboundedQueue<T> {
  private final AtomicReference<Node<T>> head, tail;

  public MSQueueRefl() {
    // Essential to NOT make dummy a field as in Goetz p. 334, that
    // would cause a memory management disaster, huge space leak:
    Node<T> dummy = new Node<T>(null, null);
    head = new AtomicReference<Node<T>>(dummy);
    tail = new AtomicReference<Node<T>>(dummy);
  }

  @SuppressWarnings("unchecked")
  // Java's @$#@?!! generics type system: abominable unsafe double type cast
  private final AtomicReferenceFieldUpdater<Node<T>, Node<T>> nextUpdater
          = AtomicReferenceFieldUpdater.newUpdater((Class<Node<T>>)(Class<?>)(Node.class),
          (Class<Node<T>>)(Class<?>)(Node.class),
          "next");

  public void enqueue(T item) { // at tail
    Node<T> node = new Node<T>(item, null);
    while (true) {
      Node<T> last = tail.get(), next = last.next;
      if (last == tail.get()) {         // E7
        if (next == null)  {
          // In quiescent state, try inserting new node
          if (nextUpdater.compareAndSet(last, next, node)) {
            // Insertion succeeded, try advancing tail
            tail.compareAndSet(last, node);
            return;
          }
        } else {
          // Queue in intermediate state, advance tail
          tail.compareAndSet(last, next);
        }
      }
    }
  }

  public T dequeue() { // from head
    while (true) {
      Node<T> first = head.get(), last = tail.get(), next = first.next;
      if (first == head.get()) {        // D5
        if (first == last) {
          if (next == null)
            return null;
          else
            tail.compareAndSet(last, next);
        } else {
          T result = next.item;
          if (head.compareAndSet(first, next)) {
            return result;
          }
        }
      }
    }
  }

  private static class Node<T> {
    final T item;
    volatile Node<T> next;

    public Node(T item, Node<T> next) {
      this.item = item;
      this.next = next;
    }
  }


}

//----------------------------------------------
//lock based queue
class MSQueue<T> implements UnboundedQueue<T> {
  private final AtomicReference<Node<T>> head, tail;

  public MSQueue() {
    Node<T> dummy = new Node<T>(null, null);
    head = new AtomicReference<Node<T>>(dummy);
    tail = new AtomicReference<Node<T>>(dummy);
  }

  public void enqueue(T item) { // at tail
    Node<T> node = new Node<>(item, null);
    while (true) {
      Node<T> last = tail.get(), next = last.next.get();
      if (last == tail.get()) {         // E7
        if (next == null)  {
          // In quiescent state, try inserting new node
          if (last.next.compareAndSet(next, node)) { // E9
            // Insertion succeeded, try advancing tail
            tail.compareAndSet(last, node);
            return;
          }
        } else
          // Queue in intermediate state, advance tail
          tail.compareAndSet(last, last);
      }
    }
  }

  public T dequeue() { // from head
    while (true) {
      Node<T> first = head.get(), last = tail.get(), next = first.next.get(); // D3
      if (first == head.get()) {        // D5
        if (first == last) {
          if (next == null)
            return null;
          else
            tail.compareAndSet(last, next);
        } else {
          T result = next.item;
          if (head.compareAndSet(first, next)) // D13
            return result;
        }
      }
    }
  }

  private static class Node<T> {
    final T item;
    final AtomicReference<Node<T>> next;

    public Node(T item, Node<T> next) {
      this.item = item;
      this.next = new AtomicReference<Node<T>>(next);
    }
  }

}

//-----------------------------------
class PutTakeTest extends Tests {
  // We could use one CyclicBarrier for both starting and stopping,
  // precisely because it is cyclic, but the code becomes clearer by
  // separating them:
  protected CyclicBarrier startBarrier, stopBarrier;
  protected final UnboundedQueue<Integer> bq;
  protected final int nTrials, nPairs;
  protected final AtomicInteger putSum = new AtomicInteger(0);
  protected final AtomicInteger takeSum = new AtomicInteger(0);

  public PutTakeTest(UnboundedQueue<Integer> bq, int npairs, int ntrials) {
    this.bq = bq;
    this.nTrials = ntrials;
    this.nPairs = npairs;
    this.startBarrier = new CyclicBarrier(npairs * 2 + 1);
    this.stopBarrier = new CyclicBarrier(npairs * 2 + 1);
  }

  void test(ExecutorService pool) {
    try {
      for (int i = 0; i < nPairs; i++) {
        pool.execute(new Producer());
        pool.execute(new Consumer());
      }
      startBarrier.await(); // wait for all threads to be ready
      stopBarrier.await();  // wait for all threads to finish
      assertTrue(bq.dequeue() == null);
      assertEquals(putSum.get(), takeSum.get());
    } catch (Exception e) {
      throw new RuntimeException(e);
    }
  }

  class Producer implements Runnable {
    public void run() {
      try {
        Random random = new Random();
        int sum = 0;
        startBarrier.await();
        for (int i = nTrials; i > 0; --i) {
          int item = random.nextInt();
          bq.enqueue(item);
          sum += item;
        }
        putSum.getAndAdd(sum);
        stopBarrier.await();
      } catch (Exception e) {
        throw new RuntimeException(e);
      }
    }
  }

  class Consumer implements Runnable {
    public void run() {
      try {
        startBarrier.await();
        int sum = 0;
        int actualNumberOfNonNullDequeues = 0;
        while(actualNumberOfNonNullDequeues != nTrials) {
          //Exercise 11.1.2
          Integer value = bq.dequeue();
          if(value == null)
            continue;
          else {
            sum += value;
            actualNumberOfNonNullDequeues++;
          }
        }
        takeSum.getAndAdd(sum);
        stopBarrier.await();
      } catch (Exception e) {
        throw new RuntimeException(e);
      }
    }
  }
}




//--------------------------------------------------------
class BenchmarkTest extends Tests {
  // We could use one CyclicBarrier for both starting and stopping,
  // precisely because it is cyclic, but the code becomes clearer by
  // separating them:
  protected CyclicBarrier startBarrier, stopBarrier;
  protected final UnboundedQueue<Integer> bq;
  protected final int nTrials, nPairs;
  protected final AtomicInteger putSum = new AtomicInteger(0);
  protected final AtomicInteger takeSum = new AtomicInteger(0);

  public BenchmarkTest(UnboundedQueue<Integer> bq, int npairs, int ntrials) {
    this.bq = bq;
    this.nTrials = ntrials;
    this.nPairs = npairs;
    this.startBarrier = new CyclicBarrier(npairs * 2 + 1);
    this.stopBarrier = new CyclicBarrier(npairs * 2 + 1);
  }

  double test(ExecutorService pool) {
    try {
      for (int i = 0; i < nPairs; i++) {
        pool.execute(new Producer());
        pool.execute(new Consumer());
      }
      startBarrier.await(); // wait for all threads to be ready
      stopBarrier.await();  // wait for all threads to finish
      assertTrue(bq.dequeue() == null);
      assertEquals(putSum.get(), takeSum.get());
      return putSum.get();
    } catch (Exception e) {
      throw new RuntimeException(e);
    }
  }

  class Producer implements Runnable {
    public void run() {
      try {
        Random random = new Random();
        startBarrier.await();
        for (int i = nTrials; i > 0; --i) {
          int item = random.nextInt();
          bq.enqueue(item);
        }
        stopBarrier.await();
      } catch (Exception e) {
        throw new RuntimeException(e);
      }
    }
  }

  class Consumer implements Runnable {
    public void run() {
      try {
        startBarrier.await();
        int actualNumberOfNonNullDequeues = 0;
        while(actualNumberOfNonNullDequeues != nTrials) {
          //Exercise 11.1.2
          Integer value = bq.dequeue();
          if(value == null)
            continue;
          else {
            isPrime(value);
            actualNumberOfNonNullDequeues++;
          }
        }
        stopBarrier.await();
      } catch (Exception e) {
        throw new RuntimeException(e);
      }
    }
  }
  private static boolean isPrime(int n) {
    int k = 2;
    while (k * k <= n && n % k != 0)
      k++;
    return n >= 2 && k * k > n;
  }
}




