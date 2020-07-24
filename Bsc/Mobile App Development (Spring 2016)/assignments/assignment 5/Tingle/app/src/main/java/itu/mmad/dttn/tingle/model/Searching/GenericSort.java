package itu.mmad.dttn.tingle.model.Searching;

import itu.mmad.dttn.tingle.model.Interfaces.ISort;
import itu.mmad.dttn.tingle.model.Thing;

/**
 * Abstract class containing methods common in sorting classes
 */
public abstract class GenericSort {

    protected ISort.type sortType;


    protected void exch(Thing[] items, int i, int j) {
        Thing thing = items[i];
        items[i] = items[j];
        items[j] = thing;
    }

    protected boolean less(Thing x, Thing y) {
        switch (sortType) {
            case SORT_WHAT:
                return (x.getWhat().toLowerCase().trim().compareTo(y.getWhat().toLowerCase().trim()) < 0);

            case SORT_WHERE:
                return (x.getWhere().toLowerCase().trim().compareTo(y.getWhere().toLowerCase().trim()) < 0);

            case SORT_DATE:
                return (x.getDate().compareTo(y.getDate()) < 0);
            default:
                return false; //impossible

        }
    }


    /**
     * Mainly used for testing
     *
     * @param list
     * @return boolean if items has been sorted
     */
    public boolean isSorted(Thing[] list) {
        if (list.length == 0) return true;
        for (int i = 1; i < list.length; i++) {
            if (less(list[i], list[i - 1])) return false;
        }
        return true;
    }
}
