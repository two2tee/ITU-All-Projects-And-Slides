package itu.mmad.dttn.tingle;

import android.content.Context;

import junit.framework.Assert;

import org.junit.Before;
import org.junit.Test;
import org.mockito.Mockito;

import java.util.UUID;

import itu.mmad.dttn.tingle.model.Thing;
import itu.mmad.dttn.tingle.model.database.repositories.inMemory.InMemoryRepository;

/**
 * UNIT test of inMemoryRepo
 */
public class InMemoryRepositoryTest {

    private InMemoryRepository SUT;

    @Before
    public void setup() {
        Context mockContext = Mockito.mock(Context.class);
        SUT = new InMemoryRepository(mockContext);
    }

    @Test
    public void newRepo_isEmpty_True() {
        Assert.assertFalse("Sut contained items on first creation", SUT.getAll().hasNext());
    }

    @Test
    public void put_ValidItem_notNull() {
        Thing dummy = new Thing("x", "y", UUID.randomUUID());
        SUT.put(dummy);
        Assert.assertNotNull("Valid item was not added", SUT.get(dummy.getId()));
    }

    @Test
    public void put_ValidItem_True() {
        Thing dummy = new Thing("x", "y", UUID.randomUUID());
        boolean result = SUT.put(dummy);
        Assert.assertTrue("put did not return true on valid argument", result);
    }

    @Test
    public void put_invalidItem_Null_False() {
        boolean result = SUT.put(null);
        Assert.assertFalse("put should not accept null arguments", result);
    }


    @Test
    public void delete_existingItem_True() {
        Thing dummy = new Thing("x", "y", UUID.randomUUID());
        SUT.put(dummy);
        boolean result = SUT.delete(dummy.getId());
        Assert.assertTrue("Did not delete item", result);
    }

    @Test
    public void delete_NonExistingItem_False() {
        UUID invalidId = UUID.randomUUID();
        boolean result = SUT.delete(invalidId);
        Assert.assertFalse("Item should not exists, but delete was still called", result);
    }

    @Test
    public void update_ExistingItem_ValidArgument_True() {
        Thing dummy = new Thing("x", "y", UUID.randomUUID());
        SUT.put(dummy);

        boolean expected = SUT.update(dummy);

        Assert.assertTrue(expected);
    }


    @Test
    public void update_NonExistingItem_False() {
        Thing dummy = new Thing("x", "y", UUID.randomUUID());
        boolean expected = SUT.update(dummy);
        Assert.assertFalse(expected);
    }


}
