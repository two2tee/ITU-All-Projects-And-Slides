package itu.mmad.dttn.tingle.model.Searching;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import javax.inject.Inject;

import itu.mmad.dttn.tingle.model.Interfaces.ISort;
import itu.mmad.dttn.tingle.model.Thing;

/**
 * This class is responsible for searching for things
 * What must be known is searching method is based on a combination of
 * binary search and linear search.
 * <p/>
 * One can specify which sorting method are to be used as SearchHandler is taking an interface
 * called ISort. Thus, one can inject another sort method if required. Use Dagger to
 * dependency inject it.
 */
public class SearchHandler {

    private final ISort sortHandler;

    private searchType currentSearchType;


    @Inject
    public SearchHandler(ISort sortHandler) {
        this.sortHandler = sortHandler;
        setCurrentSearchType(searchType.SEARCH_WHAT);//set what as default

    }

    /**
     * Returns current search type
     *
     * @return current search type
     */
    public searchType getCurrentSearchType() {
        return currentSearchType;
    }

    /**
     * Sets the current search type. That is what search is based on.
     * eg What or where etc.
     *
     * @param currentSearchType search type
     */
    public void setCurrentSearchType(searchType currentSearchType) {
        this.currentSearchType = currentSearchType;
    }

    /**
     * Sorts a given list based on sort type
     *
     * @param things list of items
     * @param t      sort type
     * @return sorted list
     */
    public List sort(List<Thing> things, ISort.type t) {
        if (things.size() == 0) return things;

        Thing[] thingArr = things.toArray(new Thing[things.size()]); //convert list to array
        sortHandler.sort(thingArr, t);
        return Arrays.asList(thingArr);
    }

    /**
     * Default sorting. Sorts based on what
     *
     * @param things thing
     * @return sorted list
     */
    public List sortDefault(List<Thing> things) {
        return sort(things, ISort.type.SORT_WHAT);
    }

    /**
     * Code is based on my first year project
     * https://github.itu.dk/abhn/GroupC/blob/master/src/main/java/dk/itu/cvitamin/model/AddressSearchModel.java
     * <p/>
     * Used to find the first thing that starts with the same letter as
     * the inputted text (based on searchtype eg what or where)
     * <p/>
     * It uses the first char of inputted value and the Thing from list as comparison elements
     * <p/>
     * It starts by doing a binary search(Fuzzy search) to find an element with a char
     * that is less by one than the char we want to find. When an element is found, the method will
     * start to fine tune by iterating from the located element until the first element that
     * starts with the char we want to find is located.
     * <p/>
     * Modified version of binary navigate found on page 5 in
     * "Searching and sorting with Java" by Peter Sestoft
     * English version 1.04, 2003-05-21
     *
     * @param charInput - first char from input
     * @return index of located element
     */
    private int searchFirstElement(char charInput, List<Thing> things) {
        int start = 0;
        int end = things.size() - 1;
        boolean found = false;
        Thing thing;
        int locatedChar = 0;
        int charToCompare = charInput;

        //Fuzzy search until char is lower by one than charToCompare (Binary search)
        int i = 0;
        while (!found && start <= end) {
            i = (start + end) / 2;
            thing = things.get(i);

            switch (currentSearchType) {
                case SEARCH_WHAT:
                    locatedChar = thing.getWhat().charAt(0);
                    break;
                case SEARCH_WHERE:
                    locatedChar = thing.getWhere().charAt(0);
                    break;
            }

            if (charToCompare < locatedChar)
                end = i - 1;
            else if (charToCompare > locatedChar)
                start = i + 1;
            else found = true;
        }


        //Fine tune until the first element is found that starts with charToCompare (linear search)
        if (found) {
            while (locatedChar == charToCompare) {
                thing = things.get(i);

                switch (currentSearchType) {
                    case SEARCH_WHAT:
                        locatedChar = thing.getWhat().charAt(0);
                        break;
                    case SEARCH_WHERE:
                        locatedChar = thing.getWhere().charAt(0);
                        break;
                }

                // Found an a!
                if (i == 0 && locatedChar == charToCompare) return 0;

                // Found something smaller than charInput, so return the next element.
                if (locatedChar < charToCompare) return i + 1;

                i--;
            }
        }

        return -1; //Nothing was found
    }

    /**
     * Compare input value with stored addresses
     *
     * @param input - String eg. address
     */
    public List<Thing> search(String input, List<Thing> things) {
        if (input.length() == 0 || things.size() == 0) return things; //No input
        input = input.toLowerCase().trim();

        List<Thing> auxThings = new ArrayList<>();

        //Add items in aux list to avoid modifying original
        for (Thing t : things) {
            auxThings.add(t);
        }

        //Sort list based on search type
        switch (currentSearchType) {
            case SEARCH_WHAT:
                auxThings = sort(auxThings, ISort.type.SORT_WHAT);
                break;
            case SEARCH_WHERE:
                auxThings = sort(auxThings, ISort.type.SORT_WHERE);
                break;
        }


        int i = searchFirstElement(input.charAt(0), auxThings); //search for first element that starts with the same char as input
        if (i == -1) return null; // No start element found


        //Defining Strings
        Thing thing = auxThings.get(i);
        String toCompare = getCurrentSearchString(thing);

        //Defining result list
        List<Thing> result = new ArrayList<>();


        //Compare and insert
        while (toCompare != null ? toCompare.charAt(0) <= input.charAt(0) && i < auxThings.size() : false) //Don't compare Strings greater than input
        {
            thing = auxThings.get(i);
            toCompare = getCurrentSearchString(thing);

            if (toCompare != null ? toCompare.startsWith(input) : false) {
                //Add to suggestion list
                result.add(thing);
            }

            i++;
        }
        return result;
    }

    /**
     * Returns string based on what is searched on
     *
     * @param thing thing
     * @return string
     */
    private String getCurrentSearchString(Thing thing) {
        switch (currentSearchType) {
            case SEARCH_WHAT:
                return thing.getWhat().toLowerCase().trim();
            case SEARCH_WHERE:
                return thing.getWhere().toLowerCase().trim();
        }
        return null;
    }

    public enum searchType {
        SEARCH_WHAT,
        SEARCH_WHERE
    }


}
