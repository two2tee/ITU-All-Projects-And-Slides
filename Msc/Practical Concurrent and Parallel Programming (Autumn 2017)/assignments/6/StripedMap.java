
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
      return ItemNode.search(buckets[hash], k).v;
    }
  }

  public int size() {
    // TO DO: IMPLEMENT
    return 0;
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

  // Put v at key k only if absent
  public V putIfAbsent(K k, V v) {
    // TO DO: IMPLEMENT
    return null;
  }

  // Remove and return the value at key k if any, else return null
  public V remove(K k) {
    // TO DO: IMPLEMENT
    return null;
  }

  // Iterate over the hashmap's entries one stripe at a time;
  // stripewise less locking and more concurrency.  An intervening
  // reallocateBuckets (cannot happen while holding the lock on a
  // stripe so no need to take a local copy bs of the buckets field)
  // may redistribute items between buckets but each item stays in the
  // same stripe.
  public void forEach(Consumer<K,V> consumer) {
    // TO DO: IMPLEMENT
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



