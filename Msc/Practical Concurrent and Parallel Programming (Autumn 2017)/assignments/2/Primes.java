

public class Primes {
  public static int countFactors(int p) {
    if (p < 2)
      return 0;
    int factorCount = 1, k = 2;
    while (p >= k * k) {
      if (p % k == 0) {
        factorCount++;
        p /= k;
    } else
      k++;
    }
    return factorCount;
  }

  public static void main(String[] args){
    int totalCount = 0;
    for(int i = 0; i < 4_999_999; i++){
     totalCount+= Primes.countFactors(i);
    }
    System.out.println("Result: " + totalCount);
  }
}
