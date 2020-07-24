import java.util.concurrent.atomic.*;
import java.util.*;

// Prime factorization is a function from Long to long[]
public class Factorizer implements Computable<Long, long[]> {
  // For statistics only, count number of calls to factorizer:
  private final AtomicLong count = new AtomicLong(0);
  public long getCount() { return count.longValue(); }

  public long[] compute(Long wrappedP) {
    count.getAndIncrement();
    long p = wrappedP;
    ArrayList<Long> factors = new ArrayList<Long>();
    long k = 2;
    while (p >= k * k) {
      if (p % k == 0) {
	factors.add(k);
	p /= k;
      } else
	k++;
    }
    // Now k * k > p and no number in 2..k divides p
    factors.add(p);
    long[] result = new long[factors.size()];
    for (int i=0; i<result.length; i++)
      result[i] = factors.get(i);
    return result;
  }
}