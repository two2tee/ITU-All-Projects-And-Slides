package itu.mmad.dttn.tingle.model.Interfaces;

import itu.mmad.dttn.tingle.model.Thing;

/**
 * interface used to specify sort method eg mergesort, quicksort etc
 */
public interface ISort {

    void sort(Thing[] items, type t);

    enum type {
        SORT_WHAT,
        SORT_WHERE,
        SORT_DATE
    }

}
