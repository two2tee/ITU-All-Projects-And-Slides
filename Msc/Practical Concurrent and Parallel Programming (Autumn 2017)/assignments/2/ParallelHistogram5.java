
public class ParallelHistogram5 {

    private final Histogram5 histogram;

    public ParallelHistogram5(int span) {
        histogram = new Histogram5(span);
    }

    public Histogram5 getHistogram() {
        return histogram;
    }

    public static void main (String[] args) {
        final int rangeMax = 4_999_999;
        final int span = 25;
        ParallelHistogram5 histogramProgram = new ParallelHistogram5(span);

        histogramProgram.countParallelN(rangeMax, 10);
        for(int i = 0; i < span; i++) {
            System.out.println(histogramProgram.getHistogram().getCount(i));
        }
    }

    private static boolean isPrime(int n) {
        int k = 2;
        while (k * k <= n && n % k != 0)
            k++;
        return n >= 2 && k * k > n;
    }

    // General parallel solution, using multiple threads
    private void countParallelN(int range, int threadCount) {

        final int perThread = range / threadCount;
        Thread[] threads = new Thread[threadCount];
        for (int t=0; t<threadCount; t++) {
        final int from = perThread * t,
            to = (t+1==threadCount) ? range : perThread * (t+1);
        threads[t] = new Thread(() -> {
            int primeFactors = 0;
            for (int i=from; i<to; i++) {
                primeFactors = TestCountFactors.countFactors(i);
                histogram.increment(primeFactors);
            }
        });
        }
        for (int t=0; t<threadCount; t++)
        threads[t].start();
        try {
        for (int t=0; t<threadCount; t++)
            threads[t].join();
        } catch (InterruptedException exn) { }
    }

}
