import java.util.Arrays;
import java.util.function.IntUnaryOperator;

public class Exercise32 {

    private final int N = 10_000_001;
    private final int[] a = new int[N];

    public Exercise32() {
        isPrimeInitialize();
        parallelPrimePrefix();
        ratio();
    }

    public static void main (String[] args) {
        Exercise32 exercise32 = new Exercise32();
    }

    // Exercise 3.2.1 : Initialize array
    private void isPrimeInitialize() {
        Arrays.parallelSetAll(array, i -> isPrime(i)? 1 : 0);
    }

    // Exercise 3.2.2: Compute prefix sum of prime numbers in array (y0 = x0, y1 = x0+x1, y2=x0+x1+x2 ...)
    private void parallelPrimePrefix() {
        Arrays.parallelPrefix(array, (acc, i) -> acc + i);
        System.out.println("The value of a[10_000_000] is : " + a[N-1]);
    }

    // Exercise 3.3.3: For loop ratio between a[i] and i/ln(i)
    private void ratio() {
        int interval = N/10;

        for(int i = interval; i <= N; i=i+interval) {
            int a = array[i];
            double b = i/Math.log(i);
            System.out.println("a[i] = " + a);
            System.out.println("i / ln(i) = " + b);
            System.out.println("ratio " + a/b + "\n");
        }
    }

    // Helper utility
    private static boolean isPrime(int n) {
    int k = 2;
    while (k * k <= n && n % k != 0)
      k++;
    return n >= 2 && k * k > n;
    }
    
    
}