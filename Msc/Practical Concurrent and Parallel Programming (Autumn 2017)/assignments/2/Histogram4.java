import  java.util.concurrent.atomic.AtomicIntegerArray;

public class Histogram4 implements Histogram{

  private final AtomicIntegerArray counts;

  public Histogram4(int span) {
    this.counts = new AtomicIntegerArray(span);
  }

  public void increment(int bin) {
    counts.addAndGet(bin,1);
  }

  public int getCount(int bin) {
    return counts.get(bin);
  }

  public int getSpan() {
    return counts.length();
  }

  public int[] getBins(){
    int[] bin = new int[counts.length()];
    for(int i = 0 ; i< counts.length(); i++)
    {
      bin[i] = counts.get(i); 
    }
    return bin; 
  }
  


}
