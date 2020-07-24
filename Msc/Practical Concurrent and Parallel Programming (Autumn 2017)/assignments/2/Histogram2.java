
class Histogram2 implements Histogram {
  private final int[] counts;

  public Histogram2(int span) {
    this.counts = new int[span];
  }

  public synchronized void increment(int bin) {
    counts[bin] = counts[bin] + 1; 
  }

  public synchronized int getCount(int bin) {
    return counts[bin];
  }

  public int getSpan() {
    return counts.length; 
  }

  public synchronized int[] getBins(){
    return counts.clone();  //Returns a snapshot
  }

}