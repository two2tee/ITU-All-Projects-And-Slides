// Week 3
// sestoft@itu.dk * 2015-09-09

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.util.*;
import java.util.function.Supplier;
import java.util.stream.Collectors;
import java.util.stream.Stream;

// Exercise 3.3 : Streaming on English words
// A stream pipeline consists of a source (i.e. words), a generator function
// and 0 or more intermediate operations (filter, map, forEach, reduce)
public class TestWordStream {

  public static void main(String[] args) {
    Supplier<Stream<String>> stream = getStreamSupplier(); // Reuse or use getStream()

    // Exercise 3.3.1 : Count words
    System.out.println("Word count : " + stream.get().count());

    // Exercise 3.3.2 : Print 100 words
    get100Words(stream.get());

    // Exercise 3.3.3 : Print words with 22 letters or more
    get22LetterWords(stream.get());

    // Exercise 3.3.4 : Print 22 letter word
    get22LetterWord(stream.get());

    // Exercise 3.3.5 : Palindromes
    palindromes(stream.get());

    // Exercise 3.3.6 : Palindromes in parallel ~25% faster (measured it with time command too)
    Utility.measure(() -> palindromes(stream.get()));
    Utility.measure(() -> printPalindromesParallel(stream.get()));

    // Exercise 3.3.7 : Word length statistics
    wordLengths(stream.get()); 

    // Exercise 3.3.8 : Collect and group words by length
    collectGroupWords(stream.get());

    // Exercise 3.3.9 : Count letter occurrences in words
    letters("Persistent");
    transformWordsIntoMap(stream.get().limit(100));

    // Exercise 3.3.10 : count letter e occurrences in all words
    countLetterE(stream.get());

    // Exercise 3.3.11 : Anagrams of tree map letter counts (takes long time to print)
    Utility.measure(() -> anagrams(stream.get())); // 183177 ms print

    // Exercise 3.3.12 : Parallel version of anagrams
    // 4x slower since groupingBy uses HashMaps -> 2 hash maps combined to 1 repeatedly
    Utility.measure(() -> parallelAnagrams(stream.get())); // 840468 ms or 44000 ms with groupingByConcurrent
  }


  // Exercise 3.3.1 : Read file as stream
  public static Stream<String> readWords(String filename) {
    try {
      BufferedReader reader = new BufferedReader(new FileReader(filename));
      return reader.lines(); //Stream.<String>empty();
    } catch (IOException exn) {
      return Stream.<String>empty();
    }
  }

  // Exercise 3.3.2 : Print first 100 words
  public static void get100Words(Stream<String> stream) {
    System.out.println("The first 100 words are: ");
    stream.limit(100).forEach(System.out::println);
    //stream.limit(100).forEach(s -> System.out.println(l));
  }

  // Exercise 3.3.4 : Find words with length 22 letters or more
  public static void get22LetterWords(Stream<String> stream) {
    stream.filter(w -> w.length() >= 22).forEach(System.out::println);
  }

  // Exercise 3.3.4 : Some word of 22 letters
  public static void get22LetterWord(Stream<String> stream) {
    Optional word = stream.filter(s -> s.length() >= 22).findAny();
    if(word.isPresent())
      System.out.println(word.get());
    else
      System.out.println("No word with at least 22 letters exists");
  }
  

  // Exercise 3.3.5 : Print palindromes
  public static void palindromes(Stream<String> stream) {
    System.out.println("Palindromes : ");
    stream.filter(w -> isPalindrome(w)).forEach(System.out::println);
  }

  public static boolean isPalindrome(String s) {
     return s.equals(new StringBuilder(s).reverse().toString());
  }

  // Exercise 3.3.6 : Print palindromes parallel
  public static void printPalindromesParallel(Stream<String> stream) {
    System.out.println("Palindromes found in parallel: ");
    stream.parallel().filter(w -> isPalindrome(w)).forEach(System.out::println);
  }

  // Exercise 3.3.7 : Convert words to lengths -> find min, max and avg word lengths
  public static void wordLengths(Stream<String> stream){
    IntSummaryStatistics summaryStatistics = stream.mapToInt(s -> s.length()).summaryStatistics();
    int max = summaryStatistics.getMax();
    int min = summaryStatistics.getMin();
    double average = summaryStatistics.getAverage();
    System.out.println("Max: " + max);
    System.out.println("Min: " + min);
    System.out.println("Average: " + average);
  }
  
    // Exercise 3.8.8 : Collect and group words by length
  public static void collectGroupWords(Stream<String> stream) {
    stream.collect(Collectors.groupingBy(String::length))
            .forEach((length, words) -> {
              System.out.println(length + "-letter words: ");
              for(String word : words) {
                System.out.println(word);
              }
              System.out.println("\n");
    });
  }
  
  /*public static void collectByLength(Stream<String> stream){
    Map<Integer, List<String>> map = stream.collect(Collectors.groupingBy(String::length))
           .forEach(System.out::println);
  }*/

  // Exercise 3.3.9 : Tree map showing how many times letters are used in word
  public static TreeMap<Character, Integer> letters(String s) {
    TreeMap<Character,Integer> frequentChars = new TreeMap<>();
    s.toLowerCase().chars().mapToObj(i -> (char) i).collect(
            Collectors.groupingBy(c -> c, Collectors.counting()))
            .forEach((letter, count) -> frequentChars.put(letter, Math.toIntExact(count)));
    return frequentChars;
  }
  
  /*
  public static Map<Character,Integer> letters(String s) {
    Stream<Character> charStream = s.toLowerCase().chars().mapToObj(i -> (char) i);
    Map<Character,Integer> res = charStream.collect(Collectors.groupingBy(Function.identity(), TreeMap::new, Collectors.counting()));
    return res;
  }*/

  public static void transformWordsIntoMap(Stream<String> stream) {
    System.out.println("Transforming words into tree maps and printing first 100: ");
    stream.limit(100).forEach(s -> System.out.println(letters(s)));
  }
  
  // Exercise 3.3.10 : Count occurrences of e in words
  public static void countLetterE(Stream<String> words){
    Stream<Map<Character,Integer>> mapStream = words.map(s -> letters(s));
    int eCount = mapStream.mapToInt(map -> map.containsKey('e')? map.get('e') : 0)
                          .reduce(0, (acc, c) -> acc + c);
    System.out.println("Number of e's: " + eCount);
  }
  

  // Exercise 3.3.11 : Group all tree map anagrams by similarity of letter in words
  public static void anagrams(Stream<String> words){
    Map<Map<Character, Integer>, Integer> map = words.collect(Collectors.groupingBy(s -> letters(s), Collectors.summingInt(s -> 1)));
    System.out.println(map.entrySet().stream().filter(entry -> entry.getValue() > 1).count());
    /*System.out.println(map);
    System.out.println("Count of anagrams: " + map.size());*/
  }
  
  /*public static void anagrams(Stream<String> stream) {
    Map<TreeMap<Character, Integer>, List<String>> anagrams =
            stream.collect(Collectors.groupingBy(s -> letters(s))).stream();
    System.out.println(anagrams);
  }*/

  // Exercise 3.3.12 : Parallel version of anagrams - use groupingByConcurrent for performance
  
  public static void anagramsParallel(Stream<String> words){
    Map<Map<Character, Integer>, Integer> map = words.parallel().collect(Collectors.groupingBy(s -> letters(s), Collectors.summingInt(s->1)));
    System.out.println(map.entrySet().stream().filter(o->o.getValue()>1).count());
  }
  
  /*
    public static void parallelAnagrams(Stream<String> stream) {
    Map<TreeMap<Character, Integer>, List<String>> anagrams =
            stream.parallel().collect(Collectors.groupingBy(s -> letters(s)));
    System.out.println(anagrams);
  }
  */
  
  
  /*Non parallel version:
  real 0m13.494s
  user 0m0.015s
  sys  0m0.015s

  Parallel version:
  real 1m19.059s
  user 0m0.000s
  sys 0m0.015s

  The parallel version is very much slower!!!
 */
  

  // Create new stream chain for every terminal operation to be executed (streams cannot be reused in Java)
  private static Supplier<Stream<String>> getStreamSupplier() {
    return () -> readWords("./words"); // Supplier Functional interface with get(), Stream.of()
  }

  // Alternative for opening a new stream for each execution
  private static Stream<String> getStream() {
    String filename = "./words";
    return readWords(filename);
  }

}
