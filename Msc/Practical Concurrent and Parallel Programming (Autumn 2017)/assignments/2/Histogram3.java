import  java.util.concurrent.atomic.AtomicInteger;
public class Histogram3 implements Histogram {

   private final AtomicInteger[] counts;

  public Histogram3(int span) {
    this.counts = new AtomicInteger[span];
    for(int i = 0; i < counts.length; i++){
      counts[i] = new AtomicInteger(0);
    }
  }

  public void increment(int bin) {
    counts[bin].addAndGet(1);
  }

  public int getCount(int bin) {
    return counts[bin].get();
  }

  public int getSpan() {
    return counts.length;
  }

    public int[] getBins()
    {
      int[] bin = new int[counts.length];
      for(int i = 0 ; i< counts.length; i++)
      {
        bin[i] = counts[i].get(); 
      }
      return bin; 
    }

}
