public class MyAtomicInteger {
  public int atomicInt;

  public MyAtomicInteger(int startValue){
    atomicInt = startValue;
  }

  public synchronized int addAndGet(int amount){
    atomicInt += amount;
    return atomicInt;
  }

  public synchronized int get(){
    return atomicInt;
  }

}
