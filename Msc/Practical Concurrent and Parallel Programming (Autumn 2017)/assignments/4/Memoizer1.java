
import java.util.concurrent.*;
import java.util.*;
/**
 * Initial cache attempt using HashMap and synchronization;
 * suffers from lack of parallelism due to coarse locking.
 * From Goetz p. 103
 * @author Brian Goetz and Tim Peierls
 */
class Memoizer1 <A, V> implements Computable<A, V> {
  private final Map<A, V> cache = new HashMap<A, V>();
  private final Computable<A, V> c;

  public Memoizer1(Computable<A, V> c) { this.c = c; }

  public synchronized V compute(A arg) throws InterruptedException {
    V result = cache.get(arg);
    if (result == null) {
      result = c.compute(arg);
      cache.put(arg, result);
    }
    return result;
  }
}
