
import java.util.concurrent.*;
import java.util.*; 
import java.util.concurrent.atomic.AtomicReference;

/**
 * Memoizer5, modern variant of Memoizer4 using the new Java 8
 * computeIfAbsent.  Atomically test whether arg is in cache and if
 * not create Future and put it there, then run on calling thread.
 */

class Memoizer5<A, V> implements Computable<A, V> {
  private final Map<A, Future<V>> cache
    = new ConcurrentHashMap<A, Future<V>>();
  private final Computable<A, V> c;

  public Memoizer5(Computable<A, V> c) { this.c = c; }

  public V compute(final A arg) throws InterruptedException {
    // AtomicReference is used as a simple assignable holder; no atomicity needed
    final AtomicReference<FutureTask<V>> ftr = new AtomicReference<FutureTask<V>>();
    Future<V> f = cache.computeIfAbsent(arg, (A argv) -> {
	  Callable<V> eval = new Callable<V>() {
	      public V call() throws InterruptedException {
		return c.compute(argv);
	      }};
	  ftr.set(new FutureTask<V>(eval));
	  return ftr.get();
      });
    // Important to run() the future outside the computeIfAbsent():
    if (ftr.get() != null)
      ftr.get().run();
    try { return f.get(); }
    catch (ExecutionException e) { throw launderThrowable(e.getCause()); }
  }

  public static RuntimeException launderThrowable(Throwable t) {
    if (t instanceof RuntimeException)
      return (RuntimeException) t;
    else if (t instanceof Error)
      throw (Error) t;
    else
      throw new IllegalStateException("Not unchecked", t);
  }
}