package methods.FrequentPatternMining.Apriori;

import java.util.*;
import java.util.stream.Collectors;

/**
 * This is an implementation of Aprirori that will find the most frequent patterns
 * in a set of transactions
 * Note that we are using strings as elements
 */
public class Apriori {

    public void run(final List<List<String>> transactions){
        if(transactions.isEmpty()){
            throw new IllegalArgumentException("Transactions was empty");
        }
        final int supportThreshold = transactions.size()/6; //at least size/k percent of total data must be supported
        System.out.println(String.format("Size: %d | Treshhold %d",transactions.size(),supportThreshold));

        List<StrongFrequency> strongFreq = apriori(transactions, supportThreshold );
        strongFreq.sort(Comparator.naturalOrder()); //sort result based on confidence

        for (StrongFrequency strong: strongFreq) {
            System.out.println(strong.rule + "| confidence: "+strong.confidence+"%" );
        }

    }

    private List<StrongFrequency> apriori( List<List<String>> transactions, int supportThreshold ) {
        Hashtable<ItemSet, Integer> frequentItemSets = generateFrequentItemSetsLevel1( transactions, supportThreshold );
        List<ItemSet> freqList = new ArrayList<>();

        System.out.println("Support Threshold: "+supportThreshold);

        //Create all frequent itemsets
        for (; frequentItemSets.size() > 0; ) {
            frequentItemSets = generateFrequentItemSets( supportThreshold, transactions, frequentItemSets );

            //Adding freq items to list
            for (Map.Entry<ItemSet,Integer> s :frequentItemSets.entrySet()) {
                freqList.add(s.getKey());
            }
        }

        //Find strong frequencies
        List<StrongFrequency> strongFreq = new ArrayList<>();
        double minConf = 0.70;
        double countSupportL;
        for (ItemSet l: freqList) {
            countSupportL = countSupport(l.set,transactions);

            List<ItemSet> subsets = generateSubsets(l);
            for (ItemSet s: subsets){
                String rule = generateAssociationRule(l,s);
                double confidence = countSupportL/(double)countSupport(s.set,transactions);
                if(confidence>= minConf){
                    strongFreq.add(new StrongFrequency(rule,(int)(confidence*100),minConf));
                }
            }
        }
        return strongFreq;
    }

    private Hashtable<ItemSet, Integer> generateFrequentItemSets( int supportThreshold, List<List<String>> transactions,
                    Hashtable<ItemSet, Integer> lowerLevelItemSets ) {

        ItemSet[] itemSets = lowerLevelItemSets.keySet().toArray(new ItemSet[lowerLevelItemSets.size()]);

        //Join itemsets
        Hashtable<ItemSet,Integer> candidates = new Hashtable<>();
        ItemSet joined;
        for (int i = 0 ;  i < itemSets.length ; i++) {
            for(int j = i+1 ; j <itemSets.length ; j++){
                joined = joinSets(itemSets[i],itemSets[j]);
                if(!candidates.containsKey(joined)){
                    candidates.put(joined,countSupport(joined.set,transactions));
                }
            }
        }
         return GetFilteredTable(supportThreshold, candidates);
    }

    private String generateAssociationRule(ItemSet superset, ItemSet subset){
        Set<String> diff = new HashSet<>();
        Set<String> sset = new HashSet<>();

        Collections.addAll(diff, superset.set);
        Collections.addAll(sset, subset.set);

        diff.removeAll(sset); // l - s

        StringBuilder sb = new StringBuilder();
        for (String aDiff : diff) {
            sb.append(aDiff).append(" ");
        }

        return subset + " => {"+sb.toString().trim()+"}";
    }

    private List<ItemSet> generateSubsets(ItemSet itemSet){
        List<ItemSet> subsets = new ArrayList<>();
        String[] items =  itemSet.set;
        StringBuilder sb = new StringBuilder();
        sb.append("superset: ").append(Arrays.toString(items)).append("| subsets:");

        // add subsets {1,2} {1,3} {2,3}
        ItemSet subset;

        for (int i = 0 ;  i < items.length ; i++) {
            for (int j = i + 1; j < items.length; j++) {
                subset = new ItemSet(new String[]{items[i],items[j]});
                if(!itemSet.equals(subset)) {
                    subsets.add(subset);
                    sb.append(Arrays.toString(subset.set)).append(" | ");
                }
            }
        }

        // add remaining single subsets {1} {2}...
        for (String item : items) {
            subset = new ItemSet(new String[]{item});
            subsets.add(subset);
            sb.append(Arrays.toString(subset.set)).append(" | ");
        }
        //System.out.println(sb.toString());

        return subsets.stream().distinct().collect(Collectors.toList());

    }

    private ItemSet joinSets( ItemSet first, ItemSet second ) {
        if(first.set.length != second.set.length) return new ItemSet(new String[]{});
        if(first.equals(second)) return first;
        int l = first.set.length,  kMinus = l-1;

        String[] fMan = Arrays.copyOf(first.set,kMinus); //First k-1 mandatory
        String[] sMan = Arrays.copyOf(second.set,kMinus);//Second k-1 mandatory
        if(Arrays.equals(fMan,sMan)){
            List<String> firstMandatory = fMan.length == 0 ? new ArrayList<>() : new ArrayList<>(Arrays.asList(fMan));

            String min = first.set[l-1].compareTo(second.set[l-1]) < 0 ? first.set[l-1] : second.set[l-1];
            String max = first.set[l-1].compareTo(second.set[l-1] ) > 0 ? first.set[l-1] : second.set[l-1] ;
            //joining first mandatory with remains
            firstMandatory.add(min);
            firstMandatory.add(max);

            return new ItemSet(firstMandatory.toArray(new String[firstMandatory.size()]));

        }
        return new ItemSet(new String[]{});
    }

    private Hashtable<ItemSet, Integer> generateFrequentItemSetsLevel1( List<List<String>> transactions, int supportThreshold ) {
        Hashtable<ItemSet,Integer> freq = new Hashtable<>();
        for (List<String> transaction:transactions) {
            {
                for (String item:transaction) {
                    ItemSet s = new ItemSet(new String[]{item});
                    if(!freq.containsKey(s)){
                        freq.put(s,countSupport(s.set,transactions));
                    }
                }
            }
        }
        //calc support and take only all above threshold
        return GetFilteredTable(supportThreshold, freq);
    }

    private Hashtable<ItemSet, Integer> GetFilteredTable(int supportThreshold, Hashtable<ItemSet, Integer> freq) {
        Enumeration<ItemSet> keys = freq.keys();
        Hashtable<ItemSet,Integer> toReturn = new Hashtable<>();
        while(keys.hasMoreElements()){
            ItemSet s = keys.nextElement();
            int support = freq.get(s);
            if(support >= supportThreshold){
                toReturn.put(s,support);
            }

        }
        return toReturn;
    }

    private int countSupport( String[] itemSet, List<List<String>> transactions ) {
        // Assumes that items in ItemSets and transactions are both unique
        int count = 0;
        boolean isFound = false;

        for (List<String> transaction : transactions) {

            for (String item : itemSet) {
                isFound = false;

                for (String tItem : transaction) {
                    if (item.equals(tItem)) {
                        isFound = true;
                        break;
                    }
                }
                if(!isFound){break;}
            }
            if(isFound){count++;}
        }
        return count;
    }

    private static void printValues(Map<ItemSet,Integer> values){
        for (Map.Entry<ItemSet,Integer> e: values.entrySet()) {
            System.out.println("itemset: "+Arrays.toString(e.getKey().set)+ "| support: " + e.getValue());
        }
    }


}
