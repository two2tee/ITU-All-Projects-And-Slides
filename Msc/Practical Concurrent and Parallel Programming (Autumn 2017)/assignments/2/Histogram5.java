import  java.util.concurrent.atomic.LongAdder;

public class Histogram5 implements Histogram{

  private final LongAdder[] counts;

  public Histogram5(int span) {
    this.counts = new LongAdder[span];
    for(int i = 0; i < counts.length; i++){
      counts[i] = new LongAdder();
    }
  }

  public void increment(int bin) {
    counts[bin].increment();
  }

  public int getCount(int bin) {
    return counts[bin].intValue();
  }

  public int getSpan() {
    return counts.length;
  }

  public int[] getBins(){
    int[] bins = new int[counts.length];
    for(int i = 0; i < counts.length; i++){
      bins[i] = counts[i].intValue();
    }
    return bins;
  }

}
