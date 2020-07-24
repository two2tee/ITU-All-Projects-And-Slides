import java.util.Map;
import java.util.concurrent.*;
import java.util.*;
public class Memoizer0 <A, V> implements Computable<A, V>{
  private final Map<A, V> cache = new ConcurrentHashMap();
  private final Computable<A, V> c;

  public Memoizer0(Computable<A, V> c) { this.c = c; }


    public V compute(A arg) throws InterruptedException 
    {
        V result = cache.computeIfAbsent(arg, (A argv)  -> {
            try
            {
            return c.compute(argv);
            } catch(InterruptedException ex) {System.out.println("interrupted"+ex.toString());}
            return null;
        });
        return result;
    }
}
