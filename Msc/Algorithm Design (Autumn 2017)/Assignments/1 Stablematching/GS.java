import java.util.Scanner;
import java.util.regex.*;

public class GS {
    
    private final String totalPairsRegex = "n=(\\d+)";
    private final String nameRegex = "(\\d+) (.+)";
    private final String preferenceListRegex = "(\\d+): ([\\d+ ]+)";
    private String[] names;
    private int[] partners;
    
    public void run(int[][] people) {
        partners = new int[people.length]; //Womens current partners
        int[] pointers = new int[people.length]; //Mens current priorities
        boolean[] taken = new boolean[people.length]; //If a man is single
        int index = 1;
        
        while(index<people.length){
            if(taken[index])
            {
                index+=2;
                continue;
            }
            
            int currentWomanId = people[index][pointers[index]++];
            int previousPartner = partners[currentWomanId];
            if(previousPartner == 0)//woman is single
            {
                partners[currentWomanId] = index; //Set current man to woman
                taken[index] = true;
            }
            else if(people[currentWomanId][previousPartner] > people[currentWomanId][index]) // !currentPartnerIsBetter
            {
              taken[previousPartner] = false; 
              partners[currentWomanId] = index;
              taken[index] = true;
              index = previousPartner;
            }
        }
        
        
    }
    
    private void printResults()
    {
        for(int i = 2 ; i<partners.length ; i+=2)
        {
            System.out.println(names[partners[i]] +" -- "+ names[i]);
        }
    }
    
    
    private String getName(int id)
    {
        return names[id];
    }
    
    public int[][] parseInput(){
        Scanner scanner = new Scanner(System.in);
        Pattern totalPairsPattern = Pattern.compile(totalPairsRegex);
        Pattern namePattern = Pattern.compile(nameRegex);
        Pattern preferenceListPattern = Pattern.compile(preferenceListRegex);

        int[][] people = null;
        String line;
        
        while(scanner.hasNext()){
                line = scanner.nextLine();
                if(line.contains("#")) {
                  continue;
                }
                  
                Matcher preferenceListMatcher = preferenceListPattern.matcher(line);
                Matcher nameMatcher = namePattern.matcher(line);
                Matcher totalPairsMatcher = totalPairsPattern.matcher(line);
                  if(totalPairsMatcher.find()){
                    int totalPairs = Integer.parseInt(totalPairsMatcher.group(1)); 
                    people = new int[totalPairs*2+1][totalPairs*2+1];
                    names = new String[totalPairs*2+1];
                  } else if (preferenceListMatcher.find()){
                      int id = Integer.parseInt(preferenceListMatcher.group(1));
                      String[] preferencesString = preferenceListMatcher.group(2).split(" ");
                      int[] preferences = new int[preferencesString.length];
                      if(id%2==0) // woman
                      {
                            for(int i = 0; i < preferences.length ; i++)
                            {
                                //Womens' list of men is basically used as a dictionary (opslagsværk)
                                people[id][Integer.parseInt(preferencesString[i])] = i;
                            }
                      }
                      else // men
                      {
                            for(int i = 0; i < preferences.length ; i++)
                            {
                                //Mens preference are sorted by their actual preference value
                                people[id][i] = Integer.parseInt(preferencesString[i]);
                            }
                      }
                  } else if (nameMatcher.find()){
                      int id = Integer.parseInt(nameMatcher.group(1));
                      String name = nameMatcher.group(2);
                      names[id] = name;
                  }
              }
       return people;
    }
    
    public static void main (String[] args)
    {
        GS gs = new GS();
        int[][] people = gs.parseInput();
        gs.run(people);
        gs.printResults();
    }
    
} 