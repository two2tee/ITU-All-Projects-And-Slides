// For week 5
// sestoft@itu.dk * 2014-09-19

import java.net.URL;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.IOException;
import java.util.*;
import java.util.concurrent.Executors;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Callable;
import java.util.concurrent.*;

public class TestDownload {

  private static final String[] urls = 
  { "http://www.itu.dk", "http://www.di.ku.dk", "http://www.miele.de",
    "http://www.microsoft.com", "http://www.amazon.com", "http://www.dr.dk",
    "http://www.vg.no", "http://www.tv2.dk", "http://www.google.com",
    "http://www.ing.dk", "http://www.dtu.dk", "http://www.eb.dk", 
    "http://www.nytimes.com", "http://www.guardian.co.uk", "http://www.lemonde.fr",   
    "http://www.welt.de", "http://www.dn.se", "http://www.heise.de", "http://www.wsj.com", 
    "http://www.bbc.co.uk", "http://www.dsb.dk", "http://www.bmw.com", "https://www.cia.gov" 
  };

  private static final ExecutorService executor = Executors.newWorkStealingPool();

  public static void main(String[] args) throws IOException {
    //String url = "https://www.wikipedia.org/";
    //String page = getPage(url, 10);
    //System.out.printf("%-30s%n%s%n", url, page);

    // Exercise 5.3.2 
    // for(Map.Entry<String,String> entry :  getPages(urls, 200).entrySet())
    // {
    //   String url = entry.getKey();
    //   String content = entry.getValue();
    //   System.out.println("Url: " + url + " with content length: " + content.length());
    // }

    // Exercise 5.3.3 
    //Timer t = new Timer(); 
    //Map<String,String> result = getPages(urls, 200);
    // System.out.println("Time spent fetching URLS sequentially: " + t.check());
    
    // Exercise 5.3.4 
    Timer t = new Timer();
    Map<String,String> result = getPagesParallel(urls, 200); 
    System.out.println("Time spent fetching URLS in parallel: " + t.check());
  }

// Exercise 5.3.2     
public static Map<String, String> getPages(String[] urls, int maxLines) throws IOException {
    final HashMap<String, String> pages = new HashMap<String, String>();
    for(String url : urls) {
      String content = getPage(url, maxLines);
      pages.put(url, content);
    }
    return Collections.unmodifiableMap(pages);
}

public static Map<String, String> getPagesParallel(String[] urls, int maxLines) throws IOException {
  
  ConcurrentHashMap<String,String> pages = new ConcurrentHashMap<String,String>();
  List<Callable<Void>> tasks = new ArrayList<Callable<Void>>();
  for(String url : urls) {
    tasks.add(() -> {
      String content = getPage(url, maxLines);
      pages.put(url, content); 
      return null; // Void 
    });
  }
  try { executor.invokeAll(tasks); } 
  catch (InterruptedException exn) {
    System.out.println("Interrupted");
  } 
  finally {
    executor.shutdown(); 
  }
  return Collections.unmodifiableMap(pages);
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

