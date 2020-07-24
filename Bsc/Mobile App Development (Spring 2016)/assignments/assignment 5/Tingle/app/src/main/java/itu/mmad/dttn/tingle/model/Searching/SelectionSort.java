package itu.mmad.dttn.tingle.model.Searching;

import itu.mmad.dttn.tingle.model.Interfaces.ISort;
import itu.mmad.dttn.tingle.model.Thing;

/**
 * Simple Selection sort method
 * <p/>
 * The algorithm divides the input list into two parts: the sublist of items already sorted, which
 * is built up from left to right at the front (left) of the list, and the sublist of items
 * remaining to be sorted that occupy the rest of the list. Initially, the sorted sublist is
 * empty and the unsorted sublist is the entire input list. The algorithm proceeds by finding the
 * smallest (or largest, depending on sorting order) element in the unsorted sublist, exchanging
 * (swapping) it with the leftmost unsorted element (putting it in sorted order), and moving the
 * sublist boundaries one element to the right.
 * <p/>
 * Taken from: Algorithms Fourth edition - Robert Sedgewick | Kevin Wayne
 */
public class SelectionSort extends GenericSort implements ISort {

    @Override
    public void sort(Thing[] items, type t) {
        if (items.length == 0) return;

        sortType = t;

        int N = items.length;

        for (int i = 0; i < N; i++) {
            int min = i;
            for (int j = i + 1; j < N; j++) {
                if (less(items[j], items[min])) {
                    min = j;
                }
            }
            exch(items, i, min);
        }

    }

}
