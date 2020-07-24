public class FactorizeExercise {

  public FactorizeExercise(){

  }

  public static void main (String[] args) {
    memoizer0();
  }

 private static void memoizer0() {
    Factorizer f = new Factorizer();
    exerciseFactorizer(new Memoizer0<Long, long[]>(f));
    System.out.println(f.getCount());
  }


  private static void memoizer1() {
    Factorizer f = new Factorizer();
    exerciseFactorizer(new Memoizer1<Long,long[]>(f));
    System.out.println(f.getCount());
  }

  private static void memoizer2() {
    Factorizer f = new Factorizer();
    exerciseFactorizer(new Memoizer2<Long, long[]>(f));
    System.out.println(f.getCount());
  }

  private static void memoizer3() {
    Factorizer f = new Factorizer();
    exerciseFactorizer(new Memoizer3<Long, long[]>(f));
    System.out.println(f.getCount());
  }

  private static void memoizer4() {
    Factorizer f = new Factorizer();
    exerciseFactorizer(new Memoizer4<Long, long[]>(f));
    System.out.println(f.getCount());
  }

  private static void memoizer5() {
    Factorizer f = new Factorizer();
    exerciseFactorizer(new Memoizer5<Long, long[]>(f));
    System.out.println(f.getCount());
  }

  private static void exerciseFactorizer(Computable<Long, long[]> f) {
    final int threadCount = 16;
    final long start = 10_000_000_000L, range = 20_000L;
    System.out.println(f.getClass());

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
  	}

}

// Interface that represents a function from A to V
interface Computable <A, V> {
  V compute(A arg) throws InterruptedException;
}
