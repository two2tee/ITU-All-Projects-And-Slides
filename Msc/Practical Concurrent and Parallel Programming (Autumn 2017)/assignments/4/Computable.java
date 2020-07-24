// Interface that represents a function from A to V
interface Computable <A, V> {
  V compute(A arg) throws InterruptedException;
}