// For week 6 -- four incomplete implementations of concurrent hash maps
// sestoft@itu.dk * 2014-10-07, 2015-09-25

// Parts of the code are missing.  Your task in the exercises is to
// write the missing parts.

import java.util.ArrayList;
import java.util.Random;

import java.util.concurrent.*;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.concurrent.atomic.AtomicIntegerArray;
import java.util.function.IntToDoubleFunction;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class TestStripedMap {
  public static void main(String[] args) {
    SystemInfo();
    testAllMaps();    // Must be run with: java -ea TestStripedMap
    //exerciseAllMaps();
    // timeAllMaps();
  }

  private static void timeAllMaps() {
    final int bucketCount = 100_000, lockCount = 32;
    for (int t = 1; t <= 32; t++) {
      final int threadCount = t;
      Mark7(String.format("%-21s %d", "SynchronizedMap", threadCount),
              i -> timeMap(threadCount,
                      new SynchronizedMap<Integer, String>(bucketCount)));
      Mark7(String.format("%-21s %d", "StripedMap", threadCount),
              i -> timeMap(threadCount,
                      new StripedMap<Integer, String>(bucketCount, lockCount)));
      Mark7(String.format("%-21s %d", "StripedWriteMap", threadCount),
              i -> timeMap(threadCount,
                      new StripedWriteMap<Integer, String>(lockCount, lockCount)));
      Mark7(String.format("%-21s %d", "WrapConcHashMap", threadCount),
              i -> timeMap(threadCount,
                      new WrapConcurrentHashMap<Integer, String>()));
    }
  }

  // TO BE HANDED OUT
  private static double timeMap(int threadCount, final OurMap<Integer, String> map) {
    final int iterations = 5_000_000, perThread = iterations / threadCount;
    final int range = 200_000;
    return exerciseMap(threadCount, perThread, range, map);
  }

  // TO BE HANDED OUT
  private static double exerciseMap(int threadCount, int perThread, int range,
                                    final OurMap<Integer, String> map) {
    Thread[] threads = new Thread[threadCount];
    for (int t = 0; t < threadCount; t++) {
      final int myThread = t;
      threads[t] = new Thread(() -> {
        Random random = new Random(37 * myThread + 78);
        for (int i = 0; i < perThread; i++) {
          Integer key = random.nextInt(range);
          if (!map.containsKey(key)) {
            // Add key with probability 60%
            if (random.nextDouble() < 0.60)
              map.put(key, Integer.toString(key));
          } else // Remove key with probability 2% and reinsert
            if (random.nextDouble() < 0.02) {
              map.remove(key);
              map.putIfAbsent(key, Integer.toString(key));
            }
        }
        final AtomicInteger ai = new AtomicInteger();
        map.forEach(new Consumer<Integer, String>() {
          public void accept(Integer k, String v) {
            ai.getAndIncrement();
          }
        });
        // System.out.println(ai.intValue() + " " + map.size());
      });
    }
    for (int t = 0; t < threadCount; t++)
      threads[t].start();
    map.reallocateBuckets();
    try {
      for (int t = 0; t < threadCount; t++)
        threads[t].join();
    } catch (InterruptedException exn) {
    }
    return map.size();
  }

  private static void exerciseAllMaps() {
    final int bucketCount = 100_000, lockCount = 32, threadCount = 16;
    final int iterations = 1_600_000, perThread = iterations / threadCount;
    final int range = 100_000;
    System.out.println(Mark7(String.format("%-21s %d", "SynchronizedMap", threadCount),
            i -> exerciseMap(threadCount, perThread, range,
                    new SynchronizedMap<Integer, String>(bucketCount))));
    System.out.println(Mark7(String.format("%-21s %d", "StripedMap", threadCount),
            i -> exerciseMap(threadCount, perThread, range,
                    new StripedMap<Integer, String>(bucketCount, lockCount))));
    System.out.println(Mark7(String.format("%-21s %d", "StripedWriteMap", threadCount),
            i -> exerciseMap(threadCount, perThread, range,
                    new StripedWriteMap<Integer, String>(lockCount, lockCount))));
    System.out.println(Mark7(String.format("%-21s %d", "WrapConcHashMap", threadCount),
            i -> exerciseMap(threadCount, perThread, range,
                    new WrapConcurrentHashMap<Integer, String>())));
  }

  // Very basic sequential functional test of a hash map.  You must
  // run with assertions enabled for this to work, as in
  //   java -ea TestStripedMap
  private static void testMap(final OurMap<Integer, String> map) {
    System.out.printf("%n%s%n", map.getClass());
    assert map.size() == 0;
    assert map.remove(117) == null;
    assert !map.containsKey(117);
    assert !map.containsKey(-2);
    assert map.get(117) == null;
    assert map.put(117, "A") == null;
    assert map.containsKey(117);
    assert map.get(117).equals("A");
    assert map.put(17, "B") == null;
    assert map.size() == 2;
    assert map.containsKey(17);
    assert map.get(117).equals("A");
    assert map.get(17).equals("B");
    assert map.put(117, "C").equals("A");
    assert map.containsKey(117);
    assert map.get(117).equals("C");
    assert map.size() == 2;
    map.forEach((k, v) -> System.out.printf("%10d maps to %s%n", k, v));
    assert map.remove(117).equals("C");
    assert !map.containsKey(117);
    assert map.get(117) == null;
    assert map.size() == 1;
    assert map.putIfAbsent(17, "D").equals("B");
    assert map.get(17).equals("B");
    assert map.size() == 1;
    assert map.containsKey(17);
    assert map.putIfAbsent(217, "E") == null;
    assert map.get(217).equals("E");
    assert map.size() == 2;
    assert map.containsKey(217);
    assert map.putIfAbsent(34, "F") == null;
    map.forEach((k, v) -> System.out.printf("%10d maps to %s%n", k, v));
    map.reallocateBuckets();
    assert map.size() == 3;
    assert map.get(17).equals("B") && map.containsKey(17);
    assert map.get(217).equals("E") && map.containsKey(217);
    assert map.get(34).equals("F") && map.containsKey(34);
    map.forEach((k, v) -> System.out.printf("%10d maps to %s%n", k, v));
    map.reallocateBuckets();
    assert map.size() == 3;
    assert map.get(17).equals("B") && map.containsKey(17);
    assert map.get(217).equals("E") && map.containsKey(217);
    assert map.get(34).equals("F") && map.containsKey(34);
    map.forEach((k, v) -> System.out.printf("%10d maps to %s%n", k, v));

    assert map.putIfAbsent(10, "Z") == null;
    assert map.putIfAbsent(11, "X") == null;
    assert map.putIfAbsent(12, "C") == null;
  }

  private static void testMapConcurrently(final OurMap<Integer, String> map) {
    int numberOfThreads = 16;
    final CyclicBarrier barrier = new CyclicBarrier(numberOfThreads + 1); //Argument is the number of threads we wait for
    AtomicInteger totalSum = new AtomicInteger(0); //Introduces synchronization SO YOU SHOULD NOT USE ATOMIC INTEGER
    int[][] counts = new int[numberOfThreads][numberOfThreads];
    for (int i = 0; i < numberOfThreads; i++) {
      new Thread(new ConcurrentTestThread(map, 1, barrier, totalSum, i, counts[i])).start();
    }

    try {
      barrier.await(); //Wait for them to start
      barrier.await(); //Wait for them to finish
    } catch (Exception e) {
      e.printStackTrace();
    }

    //Exercise 8.1.2
    assert totalSum.get() == map.size();

    Pattern p = Pattern.compile("([0-9]+):[0-9]+");
    int[] actualCounts = new int[numberOfThreads];
    //Exercise 8.1.4 and 8.1.5
    map.forEach((k, v) -> {
      Matcher matcher = p.matcher(v);
      if (matcher.matches()) {
        int threadNo = Integer.parseInt(matcher.group(1));
        actualCounts[threadNo] = actualCounts[threadNo] + 1;
      }
    });

    int[] thisBetterBeRight = new int[numberOfThreads];
    for (int j = 0; j < numberOfThreads; j++) {
      int totalCountPerArray = 0;
      for (int i = 0; i < numberOfThreads; i++) {
        totalCountPerArray += counts[i][j];
      }
      thisBetterBeRight[j] +=totalCountPerArray;
    }

    for(int i = 0; i < numberOfThreads; i++)
      System.out.println(actualCounts[i] == thisBetterBeRight[i]);
  }

  //Exercise 8.1.2
  static class ConcurrentTestThread implements Runnable {
    private final OurMap<Integer, String> map;
    private final int rounds;
    private final CyclicBarrier barrier;
    private final AtomicInteger sum;
    private final int threadNumber;
    private final int[] counts;


    public ConcurrentTestThread(OurMap<Integer, String> map, int rounds, CyclicBarrier barrier,
                                AtomicInteger sum, int number, int[] counts) {
      this.map = map;
      this.rounds = rounds;
      this.barrier = barrier;
      this.sum = sum;
      threadNumber = number;
      this.counts = counts;
    }

    @Override
    public void run() {

      Random random = new Random();
      try {
        barrier.await();
      } catch (InterruptedException e) {
        e.printStackTrace();
      } catch (BrokenBarrierException e) {
        e.printStackTrace();
      }
      for (int i = 0; i < rounds; i++) {
        boolean containsKey = map.containsKey(random.nextInt(100));
        int randomNumber1 = random.nextInt(100);
        //PUT
        String oldVal1 = map.put(randomNumber1, threadNumber + ":" + randomNumber1);
        if (oldVal1 == null) {
          counts[threadNumber] = counts[threadNumber] + 1;
          sum.incrementAndGet();
        } else {
          int oldThreadNo = getThreadNumber(oldVal1);
          counts[oldThreadNo] = counts[oldThreadNo] - 1;
          counts[threadNumber] = counts[threadNumber] +1;
        }
        //PUTIFABSENT
        int randomNumber2 = random.nextInt(100);
        String oldVal2 = map.putIfAbsent(randomNumber2, threadNumber + ":" + randomNumber2);
        if (oldVal2 == null) {
          counts[threadNumber] = counts[threadNumber] + 1;
          sum.incrementAndGet();
        }

        //Exercise 8.1.4 and 8.1.5
        //REMOVE
        String removed = map.remove(random.nextInt(100));
        if (removed != null) {
          int threadNo = getThreadNumber(removed);
          counts[threadNo] = counts[threadNo] - 1;
          sum.decrementAndGet();
        }
      }

      try {
        barrier.await();
      } catch (InterruptedException e) {
        e.printStackTrace();
      } catch (BrokenBarrierException e) {
        e.printStackTrace();
      }
    }

    public int getThreadNumber(String value) {
      Pattern p = Pattern.compile("([0-9]+):[0-9]+");
      if (value != null) {
        Matcher matcher = p.matcher(value);
        if (matcher.matches()) {
          int threadNo = Integer.parseInt(matcher.group(1));
          return threadNo;
        }
      }
      return -1;
    }
  }



  private static void testAllMaps() {
    //testMap(new SynchronizedMap<Integer,String>(25));
    //testMap(new StripedMap<Integer,String>(25, 5));
    //testMap(new StripedWriteMap<Integer,String>(5, 5));
    testMapConcurrently(new StripedWriteMap<Integer,String>(77,7));
    //testMapConcurrently(new WrapConcurrentHashMap<Integer, String>());
    //testMap(new WrapConcurrentHashMap<Integer,String>());
  }


  // --- Benchmarking infrastructure ---

  private static class Timer {
    private long start, spent = 0;
    public Timer() { play(); }
    public double check() { return (System.nanoTime()-start+spent)/1e9; }
    public void pause() { spent += System.nanoTime()-start; }
    public void play() { start = System.nanoTime(); }
  }

  // NB: Modified to show microseconds instead of nanoseconds

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

interface Consumer<K,V> {
  void accept(K k, V v);
}

interface OurMap<K,V> {
  boolean containsKey(K k);
  V get(K k);
  V put(K k, V v);
  V putIfAbsent(K k, V v);
  V remove(K k);
  int size();
  void forEach(Consumer<K,V> consumer);
  void reallocateBuckets();
}

// ----------------------------------------------------------------------
// A hashmap that permits thread-safe concurrent operations, similar
// to a synchronized version of HashMap<K,V>.

class SynchronizedMap<K,V> implements OurMap<K,V>  {
  // Synchronization policy:
  //   buckets[hash] and cachedSize are guarded by this
  private ItemNode<K,V>[] buckets;
  private int cachedSize;

  public SynchronizedMap(int bucketCount) {
    this.buckets = makeBuckets(bucketCount);
  }

  @SuppressWarnings("unchecked")
  private static <K,V> ItemNode<K,V>[] makeBuckets(int size) {
    // Java's @$#@?!! type system requires this unsafe cast
    return (ItemNode<K,V>[])new ItemNode[size];
  }

  // Protect against poor hash functions and make non-negative
  private static <K> int getHash(K k) {
    final int kh = k.hashCode();
    return (kh ^ (kh >>> 16)) & 0x7FFFFFFF;
  }

  // Return true if key k is in map, else false
  public synchronized boolean containsKey(K k) {
    final int h = getHash(k), hash = h % buckets.length;
    return ItemNode.search(buckets[hash], k) != null;
  }

  // Return value v associated with key k, or null
  public synchronized V get(K k) {
    final int h = getHash(k), hash = h % buckets.length;
    ItemNode<K,V> node = ItemNode.search(buckets[hash], k);
    if (node != null)
      return node.v;
    else
      return null;
  }

  public synchronized int size() {
    return cachedSize;
  }

  // Put v at key k, or update if already present
  public synchronized V put(K k, V v) {
    final int h = getHash(k), hash = h % buckets.length;
    ItemNode<K,V> node = ItemNode.search(buckets[hash], k);
    if (node != null) {
      V old = node.v;
      node.v = v;
      return old;
    } else {
      buckets[hash] = new ItemNode<K,V>(k, v, buckets[hash]);
      cachedSize++;
      return null;
    }
  }

  // Put v at key k only if absent
  public synchronized V putIfAbsent(K k, V v) {
    final int h = getHash(k), hash = h % buckets.length;
    ItemNode<K,V> node = ItemNode.search(buckets[hash], k);
    if (node != null)
      return node.v;
    else {
      buckets[hash] = new ItemNode<K,V>(k, v, buckets[hash]);
      cachedSize++;
      return null;
    }
  }

  // Remove and return the value at key k if any, else return null
  public synchronized V remove(K k) {
    final int h = getHash(k), hash = h % buckets.length;
    ItemNode<K,V> prev = buckets[hash];
    if (prev == null)
      return null;
    else if (k.equals(prev.k)) {        // Delete first ItemNode
      V old = prev.v;
      cachedSize--;
      buckets[hash] = prev.next;
      return old;
    } else {                            // Search later ItemNodes
      while (prev.next != null && !k.equals(prev.next.k))
        prev = prev.next;
      // Now prev.next == null || k.equals(prev.next.k)
      if (prev.next != null) {  // Delete ItemNode prev.next
        V old = prev.next.v;
        cachedSize--;
        prev.next = prev.next.next;
        return old;
      } else
        return null;
    }
  }

  // Iterate over the hashmap's entries one bucket at a time
  public synchronized void forEach(Consumer<K,V> consumer) {
    for (int hash=0; hash<buckets.length; hash++) {
      ItemNode<K,V> node = buckets[hash];
      while (node != null) {
        consumer.accept(node.k, node.v);
        node = node.next;
      }
    }
  }

  // Double bucket table size, rehash, and redistribute entries.

  public synchronized void reallocateBuckets() {
    final ItemNode<K,V>[] newBuckets = makeBuckets(2 * buckets.length);
    for (int hash=0; hash<buckets.length; hash++) {
      ItemNode<K,V> node = buckets[hash];
      while (node != null) {
        final int newHash = getHash(node.k) % newBuckets.length;
        ItemNode<K,V> next = node.next;
        node.next = newBuckets[newHash];
        newBuckets[newHash] = node;
        node = next;
      }
    }
    buckets = newBuckets;
  }

  static class ItemNode<K,V> {
    private final K k;
    private V v;
    private ItemNode<K,V> next;

    public ItemNode(K k, V v, ItemNode<K,V> next) {
      this.k = k;
      this.v = v;
      this.next = next;
    }

    public static <K,V> ItemNode<K,V> search(ItemNode<K,V> node, K k) {
      while (node != null && !k.equals(node.k))
        node = node.next;
      return node;
    }
  }
}

// ----------------------------------------------------------------------
// A hash map that permits thread-safe concurrent operations, using
// lock striping (intrinsic locks on Objects created for the purpose).

// NOT IMPLEMENTED: get, putIfAbsent, size, remove and forEach.

// The bucketCount must be a multiple of the number lockCount of
// stripes, so that h % lockCount == (h % bucketCount) % lockCount and
// so that h % lockCount is invariant under doubling the number of
// buckets in method reallocateBuckets.  Otherwise there is a risk of
// locking a stripe, only to have the relevant entry moved to a
// different stripe by an intervening call to reallocateBuckets.

class StripedMap<K,V> implements OurMap<K,V> {
  // Synchronization policy:
  //   buckets[hash] is guarded by locks[hash%lockCount]
  //   sizes[stripe] is guarded by locks[stripe]
  private volatile ItemNode<K,V>[] buckets;
  private final int lockCount;
  private final Object[] locks;
  private final int[] sizes;

  public StripedMap(int bucketCount, int lockCount) {
    if (bucketCount % lockCount != 0)
      throw new RuntimeException("bucket count must be a multiple of stripe count");
    this.lockCount = lockCount;
    this.buckets = makeBuckets(bucketCount);
    this.locks = new Object[lockCount];
    this.sizes = new int[lockCount];
    for (int stripe=0; stripe<lockCount; stripe++)
      this.locks[stripe] = new Object();
  }

  @SuppressWarnings("unchecked")
  private static <K,V> ItemNode<K,V>[] makeBuckets(int size) {
    // Java's @$#@?!! type system requires this unsafe cast
    return (ItemNode<K,V>[])new ItemNode[size];
  }

  // Protect against poor hash functions and make non-negative
  private static <K> int getHash(K k) {
    final int kh = k.hashCode();
    return (kh ^ (kh >>> 16)) & 0x7FFFFFFF;
  }

  // Return true if key k is in map, else false
  public boolean containsKey(K k) {
    final int h = getHash(k), stripe = h % lockCount;
    synchronized (locks[stripe]) {
      final int hash = h % buckets.length;
      return ItemNode.search(buckets[hash], k) != null;
    }
  }

  // Return value v associated with key k, or null
  /*1. Implement method V get(K k) using lock striping. It is similar to containsKey, but returns the value
    associated with key k if it is in the map, otherwise null. It should use the ItemNode.search auxiliary
    method.*/
  public V get(K k) {
    final int h = getHash(k), stripe = h % lockCount;
    synchronized (locks[stripe]) {
      final int hash = h % buckets.length;
      ItemNode<K, V> item = ItemNode.search(buckets[hash], k);
      return item == null? null : item.v;
    }
  }

  /*
  2. Implement method int size() using lock striping; it should return the total number of entries in the
  hash map. The size of stripe s is maintained in sizes[s], so the size() method could simply compute
  the sum of these values, locking each stripe in turn before accessing its value.
  Explain why it is important to lock stripe s when reading its size from sizes[s]
  */
  public int size() {
    int size = 0;
    for(int stripe = 0; stripe < sizes.length; stripe++){
      synchronized(locks[stripe]){
        size += sizes[stripe];
      }
    }
    return size;
  }

  // Put v at key k, or update if already present
  public V put(K k, V v) {
    final int h = getHash(k), stripe = h % lockCount;
    synchronized (locks[stripe]) {
      final int hash = h % buckets.length;
      final ItemNode<K,V> node = ItemNode.search(buckets[hash], k);
      if (node != null) {
        V old = node.v;
        node.v = v;
        return old;
      } else {
        buckets[hash] = new ItemNode<K,V>(k, v, buckets[hash]);
        sizes[stripe]++;
        return null;
      }
    }
  }

  /*
  3. Implement method V putIfAbsent(K k, V v) using lock striping. It must work as in Java’s ConcurrentHashMap,
  that is, atomically do the following: check whether key k is already in the map; if it is,
  return the associated value; and if it is not, add (k,v) to the map and return null. The implementation can
  be similar to putIfAbsent in class SynchronizedMap<K,V> but should of course only lock on the stripe
  that will hold key k. It should use the ItemNode.search auxiliary method. Remember to increment the
  relevant sizes[stripe] count if any entry was added. Ignore reallocateBuckets for now.
  */

  /*
  4. Extend method putIfAbsent to call reallocateBuckets when the bucket lists grow too long. For
  simplicity, you can test whether the size of a stripe is greater than the number of buckets divided by the
  number of stripes, as in method put.
  */

  // Put v at key k only if absent
  public V putIfAbsent(K k, V v) {
    final int h = getHash(k), stripe = h % lockCount;
    synchronized (locks[stripe]) {
      final int hash = h % buckets.length;
      ItemNode<K, V> itemNode = ItemNode.search(buckets[hash], k);
      if(itemNode == null){
        buckets[hash] = new ItemNode<K,V>(k, v, buckets[hash]);
        sizes[stripe]++;
        if (sizes[stripe] * lockCount > buckets.length)
          reallocateBuckets();
        return null;
      } else {
        return itemNode.v;
      }
    }
  }

  /*
  5. Implement method V remove(K k) using lock striping. Again very similar to SynchronizedMap<K,V>.
  Remember to decrement the relevant sizes[stripe] count if any entry was removed.
  */
  // Remove and return the value at key k if any, else return null
  public V remove(K k) {
    final int h = getHash(k), stripe = h % lockCount;
    synchronized(locks[stripe]){
      final int hash = h % buckets.length;
      ItemNode<K,V> prev = buckets[hash];
      if (prev == null)
        return null;
      else if (k.equals(prev.k)) {        // Delete first ItemNode
        V old = prev.v;
        buckets[hash] = prev.next;
        sizes[stripe]--;
        return old;
      } else {                            // Search later ItemNodes
        while (prev.next != null && !k.equals(prev.next.k))
          prev = prev.next;
        // Now prev.next == null || k.equals(prev.next.k)
        if (prev.next != null) {  // Delete ItemNode prev.next
          V old = prev.next.v;
          prev.next = prev.next.next;
          sizes[stripe]--;
          return old;
        } else
          return null;
      }
    }
  }


  /*
  6. Implement method void forEach(Consumer<K,V> consumer). Apparently, this may be implemented
  in two ways: either (1) iterate through the buckets as in the SynchronizedMap<K,V> implementation;
  or (2) iterate over the stripes, and for each stripe iterate over the buckets that belong to that stripe.
  It seems that (1) requires locking on all stripes before iterating over the buckets. Otherwise an intervening
  reallocateBuckets call on another thread may replace the buckets array with one of a different
  size between observing the length of the array and accessing its elements. It does not work to just obtain
  a copy bs of the buckets field, because reallocateBuckets destructively redistributes item node
  lists from the old buckets array to the new one.
  It seems that (2) can be implemented by locking only stripe-wise and then iterating over the stripe’s buckets.
  While holding the lock on a stripe, no reallocateBuckets can happen.
  Explain what version you have implemented and why.

  */
  // Iterate over the hashmap's entries one stripe at a time;
  // stripewise less locking and more concurrency.  An intervening
  // reallocateBuckets (cannot happen while holding the lock on a
  // stripe so no need to take a local copy bs of the buckets field)
  // may redistribute items between buckets but each item stays in the
  // same stripe.
  public void forEach(Consumer<K,V> consumer) {
    for(int stripe = 0; stripe < lockCount; stripe++){
      synchronized(locks[stripe]){
        for (int hash=stripe; hash<buckets.length; hash+=lockCount) {
          ItemNode<K,V> node = buckets[hash];
          while (node != null) {
            consumer.accept(node.k, node.v);
            node = node.next;
          }
        }
      }
    }
  }

  // First lock all stripes.  Then double bucket table size, rehash,
  // and redistribute entries.  Since the number of stripes does not
  // change, and since buckets.length is a multiple of lockCount, a
  // key that belongs to stripe s because (getHash(k) % N) %
  // lockCount == s will continue to belong to stripe s.  Hence the
  // sizes array need not be recomputed.

  public void reallocateBuckets() {
    lockAllAndThen(() -> {
      final ItemNode<K,V>[] newBuckets = makeBuckets(2 * buckets.length);
      for (int hash=0; hash<buckets.length; hash++) {
        ItemNode<K,V> node = buckets[hash];
        while (node != null) {
          final int newHash = getHash(node.k) % newBuckets.length;
          ItemNode<K,V> next = node.next;
          node.next = newBuckets[newHash];
          newBuckets[newHash] = node;
          node = next;
        }
      }
      buckets = newBuckets;
    });
  }

  // Lock all stripes, perform the action, then unlock all stripes
  private void lockAllAndThen(Runnable action) {
    lockAllAndThen(0, action);
  }

  private void lockAllAndThen(int nextStripe, Runnable action) {
    if (nextStripe >= lockCount)
      action.run();
    else
      synchronized (locks[nextStripe]) {
        lockAllAndThen(nextStripe + 1, action);
      }
  }

  static class ItemNode<K,V> {
    private final K k;
    private V v;
    private ItemNode<K,V> next;

    public ItemNode(K k, V v, ItemNode<K,V> next) {
      this.k = k;
      this.v = v;
      this.next = next;
    }

    // Assumes locks[getHash(k) % lockCount] is held by the thread
    public static <K,V> ItemNode<K,V> search(ItemNode<K,V> node, K k) {
      while (node != null && !k.equals(node.k))
        node = node.next;
      return node;
    }
  }
}

// ----------------------------------------------------------------------
// A hashmap that permits thread-safe concurrent operations, using
// lock striping (intrinsic locks on Objects created for the purpose),
// and with immutable ItemNodes, so that reads do not need to lock at
// all, only need visibility of writes, which is ensured through the
// AtomicIntegerArray called sizes.

// NOT IMPLEMENTED: get, putIfAbsent, size, remove and forEach.

// The bucketCount must be a multiple of the number lockCount of
// stripes, so that h % lockCount == (h % bucketCount) % lockCount and
// so that h % lockCount is invariant under doubling the number of
// buckets in method reallocateBuckets.  Otherwise there is a risk of
// locking a stripe, only to have the relevant entry moved to a
// different stripe by an intervening call to reallocateBuckets.

class StripedWriteMap<K,V> implements OurMap<K,V> {
  // Synchronization policy: writing to
  //   buckets[hash] is guarded by locks[hash % lockCount]
  //   sizes[stripe] is guarded by locks[stripe]
  // Visibility of writes to reads is ensured by writes writing to
  // the stripe's size component (even if size does not change) and
  // reads reading from the stripe's size component.
  private volatile ItemNode<K,V>[] buckets;
  private final int lockCount;
  private final Object[] locks;
  private final AtomicIntegerArray sizes;

  public StripedWriteMap(int bucketCount, int lockCount) {
    if (bucketCount % lockCount != 0)
      throw new RuntimeException("bucket count must be a multiple of stripe count");
    this.lockCount = lockCount;
    this.buckets = makeBuckets(bucketCount);
    this.locks = new Object[lockCount];
    this.sizes = new AtomicIntegerArray(lockCount);
    for (int stripe=0; stripe<lockCount; stripe++)
      this.locks[stripe] = new Object();
  }

  @SuppressWarnings("unchecked")
  private static <K,V> ItemNode<K,V>[] makeBuckets(int size) {
    // Java's @$#@?!! type system requires "unsafe" cast here:
    return (ItemNode<K,V>[])new ItemNode[size];
  }

  // Protect against poor hash functions and make non-negative
  private static <K> int getHash(K k) {
    final int kh = k.hashCode();
    return (kh ^ (kh >>> 16)) & 0x7FFFFFFF;
  }

  // Return true if key k is in map, else false
  public boolean containsKey(K k) {
    final ItemNode<K,V>[] bs = buckets;
    final int h = getHash(k), stripe = h % lockCount, hash = h % bs.length;
    // The sizes access is necessary for visibility of bs elements
    return sizes.get(stripe) != 0 && ItemNode.search(bs[hash], k, null);
  }

  /*1. Implement method V get(K k). It is similar to containsKey, but returns the value associated with
    key k if it is in the map, otherwise null. It should use the ItemNode.search auxiliary method, and
    for visibility reasons, read the stripe’s size before reading the buckets array entry
  */
  // Return value v associated with key k, or null


  public V get(K k) {
    final ItemNode<K,V>[] bs = buckets;
    final int h = getHash(k), stripe = h % lockCount, hash = h % bs.length;
    // The sizes access is necessary for visibility of bs elements
    if(sizes.get(stripe) == 0)
      return null;
    Holder<V> holder = new Holder();
    boolean containsKey = ItemNode.search(bs[hash], k, holder);
    return containsKey? holder.get() : null;
  }

  /*2. Implement method int size(). This is very straightforward: simply compute the sum of the stripe
  sizes. Since these are represented in an AtomicIntegerArray, all writes are visible to this method’s reads; no
  locking is needed.*/
  public int size() {
    int size = 0;
    for(int i = 0; i < sizes.length(); i++)
      size += sizes.get(i);
    return size;
  }

  // Put v at key k, or update if already present.  The logic here has
  // become more contorted because we must not hold the stripe lock
  // when calling reallocateBuckets, otherwise there will be deadlock
  // when two threads working on different stripes try to reallocate
  // at the same time.
  public V put(K k, V v) {
    final int h = getHash(k), stripe = h % lockCount;
    final Holder<V> old = new Holder<V>();
    ItemNode<K,V>[] bs;
    int afterSize;
    synchronized (locks[stripe]) {
      bs = buckets;
      final int hash = h % bs.length;
      final ItemNode<K,V> node = bs[hash],
              newNode = ItemNode.delete(node, k, old);
      bs[hash] = new ItemNode<K,V>(k, v, newNode);
      // Write for visibility; increment if k was not already in map
      afterSize = sizes.addAndGet(stripe, newNode == node ? 1 : 0);
    }
    if (afterSize * lockCount > bs.length)
      reallocateBuckets(bs);
    return old.get();
  }

  /*3. Implement method V putIfAbsent(K k, V v). You must lock on the relevant stripe. Use auxiliary
  method ItemNode.search(bl, k, old) to determine whether k is already in the hash map, where
  bl is the bucket list reference obtained from buckets[hash]. If yes, then do nothing; else create a new
  item node from k, v and bl, and update the buckets table with that. Remember to update the stripe size
  if an entry was added*/
  // Put v at key k only if absent.
  public V putIfAbsent(K k, V v) {
    ItemNode<K,V>[] bs = buckets;
    final int h = getHash(k), stripe = h % lockCount, hash = h % bs.length;
    final Holder<V> old = new Holder<>();
    int afterSize;

    synchronized (locks[stripe]){
      if(ItemNode.search(bs[hash], k, old)) {
        return old.get();
      }

      final ItemNode<K,V> node = bs[hash];
      final ItemNode<K,V> newNode = ItemNode.delete(node, k, old);
      bs[hash] = new ItemNode<>(k, v, newNode);
      // Write for visibility; increment if k was not already in map
      afterSize = sizes.incrementAndGet(stripe);
    }

    if (afterSize * lockCount > bs.length) {
      reallocateBuckets(bs);
    }
    return null;
  }

  /*4. Implement method V remove(K k). Lock on the relevant stripe. Use ItemNode.delete(bl, k,
  old) to delete the entry with key k, if any, from bucket list bl, and update the buckets table with the result.
Remember to update the stripe size if an entry was removed.*/
  // Remove and return the value at key k if any, else return null
  public V remove(K k) {
    ItemNode<K,V>[] bs = buckets;
    final int h = getHash(k), stripe = h % lockCount, hash = h % bs.length;
    final Holder<V> deletedHolder = new Holder<>();

    synchronized (locks[stripe]){
      if(ItemNode.search(bs[hash], k, deletedHolder)) {
        ItemNode<K,V> newNode = ItemNode.delete(bs[hash],k,deletedHolder);
        bs[hash] = newNode;

        sizes.decrementAndGet(stripe);
        return deletedHolder.get();
      }
    }
    return null;
  }


  /*
  5. Implement method void forEach(Consumer<K,V> consumer). Unlike in Exercise 6.1.6 there
  seems to be no need to lock anything while you iterate over the buckets table and the item node lists, since
  the latter are immutable and assignment to the buckets array entries are atomic. But to protect against
  the redistribution effect of an intervening reallocateBuckets, you must either read the buckets
  reference into a local variable bs as the first thing and then refer to bs during the iteration over buckets or
  stripes; or iterate stripewise and read the buckets reference into a local variable bs before iterating over
  the stripe’s buckets. Additionally, to ensure visibility of writes to the buckets array entries, you must read
  the stripe’s size before iterating over its buckets.
  */

  // Iterate over the hashmap's entries one stripe at a time.
  public void forEach(Consumer<K,V> consumer) {
    for(int stripe = 0; stripe < lockCount; stripe++){
      final ItemNode<K, V>[] bs = buckets;
      if(sizes.get(stripe) == 0)
        continue;
      for (int hash=stripe; hash<buckets.length; hash+=lockCount) {
        ItemNode<K,V> node = bs[hash];
        while (node != null) {
          consumer.accept(node.k, node.v);
          node = node.next;
        }
      }
    }
  }

  // Now that reallocation happens internally, do not do it externally
  public void reallocateBuckets() { }

  // First lock all stripes.  Then double bucket table size, rehash,
  // and redistribute entries.  Since the number of stripes does not
  // change, and since buckets.length is a multiple of lockCount, a
  // key that belongs to stripe s because (getHash(k) % N) %
  // lockCount == s will continue to belong to stripe s.  Hence the
  // sizes array need not be recomputed.

  // In any case, do not reallocate if the buckets field was updated
  // since the need for reallocation was discovered; this means that
  // another thread has already reallocated.  This happens very often
  // with 16 threads and a largish buckets table, size > 10,000.

  public void reallocateBuckets(final ItemNode<K,V>[] oldBuckets) {
    lockAllAndThen(() -> {
      final ItemNode<K,V>[] bs = buckets;
      if (oldBuckets == bs) {
        // System.out.printf("Reallocating from %d buckets%n", bs.length);
        final ItemNode<K,V>[] newBuckets = makeBuckets(2 * bs.length);
        for (int hash=0; hash<bs.length; hash++) {
          ItemNode<K,V> node = bs[hash];
          while (node != null) {
            final int newHash = getHash(node.k) % newBuckets.length;
            newBuckets[newHash]
                    = new ItemNode<K,V>(node.k, node.v, newBuckets[newHash]);
            node = node.next;
          }
        }
        buckets = newBuckets; // Visibility: buckets field is volatile
      }
    });
  }

  // Lock all stripes, perform action, then unlock all stripes
  private void lockAllAndThen(Runnable action) {
    lockAllAndThen(0, action);
  }

  private void lockAllAndThen(int nextStripe, Runnable action) {
    if (nextStripe >= lockCount)
      action.run();
    else
      synchronized (locks[nextStripe]) {
        lockAllAndThen(nextStripe + 1, action);
      }
  }

  static class ItemNode<K,V> {
    private final K k;
    private final V v;
    private final ItemNode<K,V> next;

    public ItemNode(K k, V v, ItemNode<K,V> next) {
      this.k = k;
      this.v = v;
      this.next = next;
    }

    // These work on immutable data only, no synchronization needed.

    public static <K,V> boolean search(ItemNode<K,V> node, K k, Holder<V> old) {
      while (node != null)
        if (k.equals(node.k)) {
          if (old != null)
            old.set(node.v);
          return true;
        } else
          node = node.next;
      return false;
    }

    public static <K,V> ItemNode<K,V> delete(ItemNode<K,V> node, K k, Holder<V> old) {
      if (node == null)
        return null;
      else if (k.equals(node.k)) {
        old.set(node.v);
        return node.next;
      } else {
        final ItemNode<K,V> newNode = delete(node.next, k, old);
        if (newNode == node.next)
          return node;
        else
          return new ItemNode<K,V>(node.k, node.v, newNode);
      }
    }
  }

  // Object to hold a "by reference" parameter.  For use only on a
  // single thread, so no need for "volatile" or synchronization.

  static class Holder<V> {
    private V value;
    public V get() {
      return value;
    }
    public void set(V value) {
      this.value = value;
    }
  }
}

// ----------------------------------------------------------------------
// A wrapper around the Java class library's sophisticated
// ConcurrentHashMap<K,V>, making it implement OurMap<K,V>

class WrapConcurrentHashMap<K,V> implements OurMap<K,V> {
  final ConcurrentHashMap<K,V> underlying = new ConcurrentHashMap<K,V>();

  public boolean containsKey(K k) {
    return underlying.containsKey(k);
  }

  public V get(K k) {
    return underlying.get(k);
  }

  public V put(K k, V v) {
    return underlying.put(k, v);
  }

  public V putIfAbsent(K k, V v) {
    return underlying.putIfAbsent(k, v);
  }

  public V remove(K k) {
    return underlying.remove(k);
  }

  public int size() {
    return underlying.size();
  }

  public void forEach(Consumer<K,V> consumer) {
    underlying.forEach((k,v) -> consumer.accept(k,v));
  }

  public void reallocateBuckets() { }
}