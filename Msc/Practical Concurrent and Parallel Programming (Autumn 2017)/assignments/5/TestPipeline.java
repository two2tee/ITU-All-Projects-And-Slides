// For week 5
// sestoft@itu.dk * 2014-09-23

// A pipeline of transformers connected by bounded queues.  Each
// transformer consumes items from its input queue and produces items
// on its output queue.

// This is illustrated by generating URLs, fetching the corresponding
// webpages, scanning the pages for links to other pages, and printing
// those links; using four threads connected by three queues:

// UrlProducer --(BlockingQueue<String>)--> 
// PageGetter  --(BlockingQueue<Webpage>)--> 
// LinkScanner --(BlockingQueue<Link>)--> 
// LinkPrinter


// For reading webpages
import java.net.URL;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.IOException;

// For regular expressions
import java.util.regex.Matcher;
import java.util.regex.MatchResult;
import java.util.regex.Pattern;
import java.util.*;
import java.util.concurrent.Executors;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.*;
import java.util.concurrent.Callable;


public class TestPipeline {
  
  // Exercise 5.4.3 
  private static final ExecutorService executor 
    = Executors.newWorkStealingPool();
  // Exercise 5.4.4
  //= Executors.newFixedThreadPool(6);
  // Exercise 5.4.5 
  //= Executors.newFixedThreadPool(3);
  
  public static void main(String[] args) {
    // runAsThreads();
    runAsTasksUniquifier();
    //runWithMultiplePageGetters();
  }

  private static void runAsThreads() {
    final BlockingQueue<String> urls = new OneItemQueue<String>();
    final BlockingQueue<Webpage> pages = new OneItemQueue<Webpage>();
    final BlockingQueue<Link> refPairs = new OneItemQueue<Link>();
    Thread t1 = new Thread(new UrlProducer(urls));
    Thread t2 = new Thread(new PageGetter(urls, pages));
    Thread t3 = new Thread(new LinkScanner(pages, refPairs));
    Thread t4 = new Thread(new LinkPrinter(refPairs));
    t1.start(); t2.start(); t3.start(); t4.start(); 
  }
  
/* Exercise 5.4.2:
 * Insert a Uniquifier<Link> stage, and suitable queues, between the LinkScanner and the LinkPrinter, so that
 * a link (from,to) from one webpage to another is printed only once 
 */
  private static void runAsThreadsUniquifier() {
    final BlockingQueue<String> urls = new OneItemQueue<String>();
    final BlockingQueue<Webpage> pages = new OneItemQueue<Webpage>();
    final BlockingQueue<Link> refPairs = new OneItemQueue<Link>();
    final BlockingQueue<Link> uniquePairs = new OneItemQueue<Link>();
   
    Thread t1 = new Thread(new UrlProducer(urls));
    Thread t2 = new Thread(new PageGetter(urls, pages));
    Thread t3 = new Thread(new LinkScanner(pages, refPairs));
    Thread t4 = new Thread(new Uniquifier(refPairs, uniquePairs));
    Thread t5 = new Thread(new LinkPrinter(uniquePairs));
    t1.start(); t2.start(); t3.start(); t4.start(); t5.start(); 
  }
  
  /*
  * Exercise 5.4.3: Change the implementation to submit the UrlProducer, PageGetter, LinkScanner, Uniquifier and LinkPrinter
  * as tasks on an executor service rather create threads from them. If you have Java 8, use a WorkStealingPool,
  * otherwise a CachedThreadPool, as executor service
  */
    private static void runAsTasksUniquifier() {
    final BlockingQueue<String> urls = new OneItemQueue<String>();
    final BlockingQueue<Webpage> pages = new OneItemQueue<Webpage>();
    final BlockingQueue<Link> refPairsIn = new OneItemQueue<Link>();
    final BlockingQueue<Link> refPairsOut = new OneItemQueue<Link>();
  
    List<Callable<Void>> tasks = new ArrayList<Callable<Void>>();
      tasks.add(() -> { new UrlProducer(urls).run(); return null; });
      tasks.add(() -> { new PageGetter(urls, pages).run(); return null; });
      tasks.add(() -> { new LinkScanner(pages, refPairsIn).run(); return null; });
      tasks.add(() -> { new Uniquifier(refPairsIn,refPairsOut).run(); return null; });
      tasks.add(() -> { new LinkPrinter(refPairsOut).run(); return null; });
      
    try { executor.invokeAll(tasks); }
    catch (InterruptedException exn) { System.out.println("Interrupted: " + exn);}
    finally{ executor.shutdown();}
  }
  
  // Tasks with futures 
  // private static void runAsTasksUniquifier() {
  // final BlockingQueue<String> urls = new OneItemQueue<String>();
  // final BlockingQueue<Webpage> pages = new OneItemQueue<Webpage>();
  // final BlockingQueue<Link> refPairsIn = new OneItemQueue<Link>();
  // final BlockingQueue<Link> refPairsOut = new OneItemQueue<Link>();

  // List<Future<?>> tasks = new ArrayList<>();
  //   tasks.add(executor.submit(() -> new UrlProducer(urls).run()));
  //   tasks.add(executor.submit(() -> new PageGetter(urls, pages).run()));
  //   tasks.add(executor.submit(() -> new LinkScanner(pages, refPairsIn).run()));
  //   tasks.add(executor.submit(() -> new Uniquifier(refPairsIn,refPairsOut).run()));
  //   tasks.add(executor.submit(() -> new LinkPrinter(refPairsOut).run()));
  //   try {
  //     for (Future<?> fut : tasks)
  //       fut.get();
  //   } catch (InterruptedException exn) { 
  //     System.out.println("Interrupted: " + exn);
  //   } catch (ExecutionException exn) { 
  //     throw new RuntimeException(exn.getCause()); 
  //   }

  // //try { executor.invokeAll(tasks); }
  // /*catch (InterruptedException exn) { System.out.println("Interrupted: " + exn);}
  // finally{ executor.shutdown();}*/
  // }
  
  /* Exercise 5.4.6
   * Probably the pipeline’s most time-consuming stage is the PageGetter. To improve overall throughput,
   * one may simply create two PageGetter objects (as threads or tasks), both taking input from the BlockingQueue<String>
   * queue and sending output to the BlockingQueue<Webpage> queue.
   */
  private static void runWithMultiplePageGetters() {
    final BlockingQueue<String> urls = new OneItemQueue<String>();
    final BlockingQueue<Webpage> pages = new OneItemQueue<Webpage>();
    final BlockingQueue<Link> refPairsIn = new OneItemQueue<Link>();
    final BlockingQueue<Link> refPairsOut = new OneItemQueue<Link>();
  
    List<Callable<Void>> tasks = new ArrayList<Callable<Void>>();
      tasks.add(() -> { new UrlProducer(urls).run(); return null; });
      tasks.add(() -> { new PageGetter(urls, pages).run(); return null; });
      tasks.add(() -> { new PageGetter(urls, pages).run(); return null; });
      tasks.add(() -> { new LinkScanner(pages, refPairsIn).run(); return null; });
      tasks.add(() -> { new Uniquifier(refPairsIn,refPairsOut).run(); return null; });
      tasks.add(() -> { new LinkPrinter(refPairsOut).run(); return null; });
      
    try { executor.invokeAll(tasks); }
    catch (InterruptedException exn) { System.out.println("Interrupted: " + exn);}
    finally{ executor.shutdown();}
  }
  
}


/* Exercise 5.4.2: Instead of changing LinkScanner or LinkPrinter, write a new class Uniquifier<T> that consumes items of
 * type T and produces items of type T, but only once each. The class should implement Runnable and its
 * constructor should take as argument an input queue and an output queue, each of type BlockingQueue<T>.
 */
class Uniquifier<T> implements Runnable {
  //that consumes items of type T and produces items of type T, but only once each.
  private final BlockingQueue<T> input;
  private final BlockingQueue<T> output;

  public Uniquifier(BlockingQueue<T> input, BlockingQueue<T> output) {
    this.input = input;
    this.output = output;
  }
  
  /*
   * The uniquifier’s run method may maintain a HashSet<T> containing the items already seen. When an item
   * received from the input queue is already in the hashset, it is ignored; otherwise it is added to the hashset and
   * also sent to the output queue
   */
  public void run() {
    HashSet<T> uniqueItems = new HashSet<>();
    while(true) {
      T item = input.take();
      if(!uniqueItems.contains(item)) {
        output.put(item);
        uniqueItems.add(item);
      }
    }
  }
  
}


class UrlProducer implements Runnable {
  private final BlockingQueue<String> output;

  public UrlProducer(BlockingQueue<String> output) {
    this.output = output;
  }

  public void run() { 
    for (int i=0; i<urls.length; i++)
      output.put(urls[i]);
  }

  private static final String[] urls = 
  { "http://www.itu.dk", "http://www.di.ku.dk", "http://www.miele.de",
    "http://www.microsoft.com", "http://www.amazon.com", "http://www.dr.dk",
    "http://www.vg.no", "http://www.tv2.dk", "http://www.google.com",
    "http://www.ing.dk", "http://www.dtu.dk", "http://www.bbc.co.uk"
  };
}

class PageGetter implements Runnable {
  private final BlockingQueue<String> input;
  private final BlockingQueue<Webpage> output;

  public PageGetter(BlockingQueue<String> input, BlockingQueue<Webpage> output) {
    this.input = input;
    this.output = output;
  }

  public void run() { 
    while (true) {
      String url = input.take();
      //      System.out.println("PageGetter: " + url);
      try { 
        String contents = getPage(url, 200);
        output.put(new Webpage(url, contents));
      } catch (IOException exn) { System.out.println(exn); }
    }
  }

  public static String getPage(String url, int maxLines) throws IOException {
    // This will close the streams after use (JLS 8 para 14.20.3):
    try (BufferedReader in 
         = new BufferedReader(new InputStreamReader(new URL(url).openStream()))) {
      StringBuilder sb = new StringBuilder();
      for (int i=0; i<maxLines; i++) {
        String inputLine = in.readLine();
        if (inputLine == null)
          break;
        else
        sb.append(inputLine).append("\n");
      }
      return sb.toString();
    }
  }
}

class LinkScanner implements Runnable {
  private final BlockingQueue<Webpage> input;
  private final BlockingQueue<Link> output;

  public LinkScanner(BlockingQueue<Webpage> input, 
                     BlockingQueue<Link> output) {
    this.input = input;
    this.output = output;
  }

  private final static Pattern urlPattern 
    = Pattern.compile("a href=\"(\\p{Graph}*)\"");

  public void run() { 
    while (true) {
      Webpage page = input.take();
      //      System.out.println("LinkScanner: " + page.url);
      // Extract links from the page's <a href="..."> anchors
      Matcher urlMatcher = urlPattern.matcher(page.contents);
      while (urlMatcher.find()) {
        String link = urlMatcher.group(1);
        output.put(new Link(page.url, link));
      }
    }
  }
}

class LinkPrinter implements Runnable {
  private final BlockingQueue<Link> input;

  public LinkPrinter(BlockingQueue<Link> input) {
    this.input = input;
  }

  public void run() { 
    while (true) {
      Link link = input.take();
      //      System.out.println("LinkPrinter: " + link.from);
      System.out.printf("%s links to %s%n", link.from, link.to);
    }
  }
}


class Webpage {
  public final String url, contents;
  public Webpage(String url, String contents) {
    this.url = url;
    this.contents = contents;
  }
}

class Link {
  public final String from, to;
  public Link(String from, String to) {
    this.from = from;
    this.to = to;
  }

  // Override hashCode and equals so can be used in HashSet<Link>

  public int hashCode() {
    return (from == null ? 0 : from.hashCode()) * 37
         + (to == null ? 0 : to.hashCode());
  }

  public boolean equals(Object obj) {
    Link that = obj instanceof Link ? (Link)obj : null;
    return that != null 
      && (from == null ? that.from == null : from.equals(that.from))
      && (to == null ? that.to == null : to.equals(that.to));
  }
}

// Different from java.util.concurrent.BlockingQueue: Allows null
// items, and methods do not throw InterruptedException.

interface BlockingQueue<T> {
  void put(T item);
  T take();
}

class OneItemQueue<T> implements BlockingQueue<T> {
  private T item;
  private boolean full = false;

  public void put(T item) {
    synchronized (this) {
      while (full) {
        try { this.wait(); } 
        catch (InterruptedException exn) { }
      }
      full = true;
      this.item = item;
      this.notifyAll();
    }
  }

  public T take() {
    synchronized (this) {
      while (!full) {
        try { this.wait(); } 
        catch (InterruptedException exn) { }
      }
      full = false;
      this.notifyAll();
      return item;
    }
  }
}