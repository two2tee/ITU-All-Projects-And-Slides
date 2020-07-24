import java.util.concurrent.atomic.*;

public class ParallelProgramTwo {

  public static void main(String[] args){
    final int work = 499_999;
    Thread[] threads = new Thread[10];
    final AtomicInteger count = new AtomicInteger(0);

    for(int i = 0; i < 10; i++){
      final int lower = i*work+i*1;
      final int upper = lower+work;
      System.out.println("Lower: "+ lower + ", upper:" + upper);
      Thread t = new Thread(() -> {
        for(int j = lower; j <= upper; j++){
         count.addAndGet(Primes.countFactors(j));
        }
      });
      threads[i] = t;
      t.start();
    }

    for(int i = 0; i < 10; i++){
      try {
        threads[i].join();
      } catch (InterruptedException e) {
        e.printStackTrace();
      }
    }
    System.out.println("Result:" + count.get());
  }

}
