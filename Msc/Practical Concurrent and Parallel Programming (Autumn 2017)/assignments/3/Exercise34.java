import java.util.concurrent.atomic.AtomicInteger;
import java.util.function.DoubleSupplier;
import java.util.function.Function;
import java.util.stream.DoubleStream;
import java.util.stream.IntStream;

public class Exercise34 {

    private static final int N = 999_999_999;

    public static void main(String[] args) {
        // Exercise 3.4.1 : Compute sum in finite DoubleStream
        Utility.measure(() -> computeSum()); // 90000 ms (measured with time command too)

        // Exercise 3.4.2 : Compute sum in parallel
        Utility.measure(() -> computeSumParallel()); // 15000 ms (measured with time command too)

        // Exercise 3.4.3 : For loop sum yields less precise result because:
        // Stream sum performs summation by taking into account order of summation
        // (floating point addition is not associative)
        Utility.measure(() -> computeSumLoop()); // 50000ms

        // Exercise 3.4.4 : Imperative double stream
        Utility.measure(() -> imperativeSum()); // 18000 ms

        // Exercise 3.4.5 : Parallel imperative sum
        Utility.measure(() -> imperativeParallelSum()); // 27000 ms
    }

    // Exercise 3.4.1 : Compute sum of finite stream
    public static void computeSum() {
        DoubleStream finiteStream = IntStream.range(1, N).mapToDouble(n -> 1 / (double) n);
        System.out.printf("Normal Sum = %20.16f%n", finiteStream.sum());
    }

    // Exercise 3.4.2 : Compute sum in parallel
    public static void computeSumParallel() {
        DoubleStream finiteStream = IntStream.range(1, N).mapToDouble(n -> 1 / (double) n);
        System.out.printf("Parallel Sum = %20.16f%n", finiteStream.parallel().sum());
    }

    // Exercise 3.4.3 : Compute sum loop sequentially
    public static void computeSumLoop() {
        double sum = 0.0;
        for(double n = 1; n <= N; n++)
        {
            sum += 1/n;
        }
        System.out.printf("Sequential Loop Sum = %20.16f%n", sum);
    }

    // Exercise 3.4.4 : Imperative summation (the imperative paradigm describes both what the program should do and how, functional only what)
    public static void imperativeSum() {
        AtomicInteger sum = new AtomicInteger(1);
        DoubleStream imperativeStream = DoubleStream.generate(() -> 1.0 / (double) sum.getAndIncrement()).limit(N);
        System.out.printf("Imperative Sum = %20.16f%n", imperativeStream.sum());
    }

    // Exercise 3.4.5 : Imperative summation in parallel
    public static void imperativeParallelSum() {
        AtomicInteger sum = new AtomicInteger(1);
        DoubleStream parallelImperativeStream = DoubleStream.generate(() -> 1.0 / (double) sum.getAndIncrement()).parallel().limit(N);
        System.out.printf("Parallel Imperative Sum = %20.16f%n", parallelImperativeStream.sum());
    }

}
