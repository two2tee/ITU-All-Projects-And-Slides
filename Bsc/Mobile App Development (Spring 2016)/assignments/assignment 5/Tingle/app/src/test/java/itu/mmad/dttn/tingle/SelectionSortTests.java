package itu.mmad.dttn.tingle;

import junit.framework.Assert;

import org.junit.After;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;

import java.util.Arrays;
import java.util.Random;
import java.util.UUID;

import itu.mmad.dttn.tingle.model.Interfaces.ISort;
import itu.mmad.dttn.tingle.model.Searching.SelectionSort;
import itu.mmad.dttn.tingle.model.Thing;

/**
 * Tests for selection sort
 */
public class SelectionSortTests {
    private static SelectionSort SUT;
    private static int TOTAL_ITEMS;
    private Thing[] toSortList;
    private Random r = new Random();


    @BeforeClass
    public static void init() {
        SUT = new SelectionSort();
        TOTAL_ITEMS = 1000;
    }

    @Before
    public void setup() {
        toSortList = new Thing[TOTAL_ITEMS];
        insertRandomItems();
    }

    @After
    public void clear() {
        toSortList = null;
    }


    private void insertRandomItems() {
        Random r = new Random();
        String alphabet = "abcdefghijklmnopgrstuvxyz";
        for (int i = 0; i < TOTAL_ITEMS; i++) {
            toSortList[i] = new Thing(
                    String.valueOf(alphabet.charAt(r.nextInt(alphabet.length()))),
                    String.valueOf(alphabet.charAt(r.nextInt(alphabet.length()))),
                    UUID.randomUUID());

        }
    }

    @Test
    public void sort_where_isSorted_true() {
        SUT.sort(toSortList, ISort.type.SORT_WHERE);
        for (Thing s : toSortList) {
            System.out.println(s.getWhere());
        }
        Assert.assertTrue(Arrays.toString(toSortList), SUT.isSorted(toSortList));

    }

    @Test
    public void sort_what_isSorted_true() {
        SUT.sort(toSortList, ISort.type.SORT_WHAT);
        for (Thing s : toSortList) {
            System.out.println(s.getWhere());
        }
        Assert.assertTrue(Arrays.toString(toSortList), SUT.isSorted(toSortList));

    }

    @Test
    public void sort_date_isSorted_true() {
        SUT.sort(toSortList, ISort.type.SORT_DATE);
        for (Thing s : toSortList) {
            System.out.println(s.getDate());
        }
        Assert.assertTrue(Arrays.toString(toSortList), SUT.isSorted(toSortList));
    }

    @Test
    public void isSorted_unsortedList_false() {
        SUT.sort(toSortList, ISort.type.SORT_WHAT);

        Thing[] unSorted = new Thing[]{
                new Thing("b", "z", UUID.randomUUID()),
                new Thing("a", "z", UUID.randomUUID()),
        };

        Assert.assertFalse(SUT.isSorted(unSorted));
    }

    @Test
    public void isSorted_sortedList_true() {
        SUT.sort(toSortList, ISort.type.SORT_WHAT);

        Thing[] sorted = new Thing[]{
                new Thing("a", "z", UUID.randomUUID()),
                new Thing("b", "z", UUID.randomUUID()),
        };

        Assert.assertTrue(SUT.isSorted(sorted));
    }

    @Test
    public void isSorted_emptyList_true() {
        SUT.sort(toSortList, ISort.type.SORT_WHAT);

        Thing[] unSorted = new Thing[0];
        Assert.assertTrue(SUT.isSorted(unSorted));
    }
}
