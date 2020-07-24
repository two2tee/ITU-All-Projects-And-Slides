package itu.mmad.dttn.tingle;

import junit.framework.Assert;

import org.junit.Before;
import org.junit.Test;
import org.mockito.Mockito;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

import itu.mmad.dttn.tingle.model.Interfaces.ISort;
import itu.mmad.dttn.tingle.model.Searching.SearchHandler;
import itu.mmad.dttn.tingle.model.Thing;

/**
 * SearchHandler Tests
 */
public class SearchHandlerTests {
    private SearchHandler SUT;
    private ISort mockSort;

    @Before
    public void setup() {
        mockSort = Mockito.mock(ISort.class);
        SUT = new SearchHandler(mockSort);
    }

    @Test
    public void search_empty_input_return_same_list() {
        List<Thing> list = new ArrayList();
        list.add(new Thing("a", "x", UUID.randomUUID()));
        list.add(new Thing("b", "y", UUID.randomUUID()));
        list.add(new Thing("c", "z", UUID.randomUUID()));

        String expected = "";

        List<Thing> actual = SUT.search(expected, list);

        Assert.assertTrue(actual.size() == 3);  //returning same list again
    }

    @Test
    public void search_what_single_existing_true() {
        List<Thing> list = new ArrayList();
        list.add(new Thing("a", "x", UUID.randomUUID()));
        list.add(new Thing("b", "y", UUID.randomUUID()));
        list.add(new Thing("c", "z", UUID.randomUUID()));

        String expected = "a";

        List<Thing> actual = SUT.search(expected, list);

        Assert.assertTrue(actual.size() == 1);
        Assert.assertTrue(actual.get(0).getWhat().equals(expected));
    }

    @Test
    public void search_what_single_none_existing_null() {
        List<Thing> list = new ArrayList();
        list.add(new Thing("a", "x", UUID.randomUUID()));
        list.add(new Thing("b", "y", UUID.randomUUID()));
        list.add(new Thing("c", "z", UUID.randomUUID()));

        String expected = "k";

        List<Thing> actual = SUT.search(expected, list);

        Assert.assertTrue(actual == null);
    }


    @Test
    public void search_what_many_existing_same_true() {
        List<Thing> list = new ArrayList();
        list.add(new Thing("a", "x", UUID.randomUUID()));
        list.add(new Thing("a", "y", UUID.randomUUID()));
        list.add(new Thing("b", "y", UUID.randomUUID()));
        list.add(new Thing("c", "z", UUID.randomUUID()));

        String expected = "a";

        List<Thing> actual = SUT.search(expected, list);

        Assert.assertTrue(actual.size() == 2);
        Assert.assertTrue(actual.get(0).getWhat().equals(expected));
        Assert.assertTrue(actual.get(1).getWhat().equals(expected));
    }

    @Test
    public void search_what_many_existing_almost_same_true() {
        List<Thing> list = new ArrayList();
        list.add(new Thing("a", "x", UUID.randomUUID()));
        list.add(new Thing("ab", "y", UUID.randomUUID()));
        list.add(new Thing("b", "y", UUID.randomUUID()));
        list.add(new Thing("c", "z", UUID.randomUUID()));

        String expected1 = "a";
        String expected2 = "ab";


        List<Thing> actual = SUT.search(expected1, list);

        Assert.assertTrue(actual.size() == 2);
        Assert.assertTrue(actual.get(0).getWhat().equals(expected1));
        Assert.assertTrue(actual.get(1).getWhat().equals(expected2));
    }


    @Test
    public void search_where_single_existing_true() {
        SUT.setCurrentSearchType(SearchHandler.searchType.SEARCH_WHERE);
        List<Thing> list = new ArrayList();
        list.add(new Thing("a", "x", UUID.randomUUID()));
        list.add(new Thing("b", "y", UUID.randomUUID()));
        list.add(new Thing("c", "z", UUID.randomUUID()));

        String expected = "x";

        List<Thing> actual = SUT.search(expected, list);

        Assert.assertTrue(actual.size() == 1);
        Assert.assertTrue(actual.get(0).getWhere().equals(expected));
    }


    @Test
    public void search_where_many_existing_same_true() {
        SUT.setCurrentSearchType(SearchHandler.searchType.SEARCH_WHERE);

        List<Thing> list = new ArrayList();
        list.add(new Thing("a", "x", UUID.randomUUID()));
        list.add(new Thing("a", "y", UUID.randomUUID()));
        list.add(new Thing("b", "y", UUID.randomUUID()));
        list.add(new Thing("c", "z", UUID.randomUUID()));

        String expected = "y";

        List<Thing> actual = SUT.search(expected, list);

        Assert.assertTrue(actual.size() == 2);
        Assert.assertTrue(actual.get(0).getWhere().equals(expected));
        Assert.assertTrue(actual.get(1).getWhere().equals(expected));
    }

    @Test
    public void search_where_many_existing_almost_same_true() {
        SUT.setCurrentSearchType(SearchHandler.searchType.SEARCH_WHERE);
        List<Thing> list = new ArrayList();
        list.add(new Thing("a", "x", UUID.randomUUID()));
        list.add(new Thing("ab", "y", UUID.randomUUID()));
        list.add(new Thing("b", "yx", UUID.randomUUID()));
        list.add(new Thing("c", "z", UUID.randomUUID()));

        String expected1 = "y";
        String expected2 = "yx";


        List<Thing> actual = SUT.search(expected1, list);

        Assert.assertTrue(actual.size() == 2);
        Assert.assertTrue(actual.get(0).getWhere().equals(expected1));
        Assert.assertTrue(actual.get(1).getWhere().equals(expected2));
    }

}
