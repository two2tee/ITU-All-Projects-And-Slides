
public interface Histogram {
  public void increment(int bin);
  public int getCount(int bin);
  public int getSpan();
  public int[] getBins();
}