package itu.mmad.dttn.tingle;

import junit.framework.Assert;

import org.junit.Test;

import itu.mmad.dttn.tingle.Model.Entity;
import itu.mmad.dttn.tingle.Stubs.EntityStub;

/**
 * To work on unit tests, switch the Test Artifact in the Build Variants view.
 */
public class EntityTests {
    @Test
    public void Id_NotNull_true()
    {
        EntityStub SUT = new EntityStub();
        Assert.assertNotNull("No id created",SUT.getId());
    }

    @Test
    public void Id_Unique_true()
    {
        EntityStub SUT = new EntityStub();
        EntityStub toCompare = new EntityStub();
        Assert.assertFalse("Id is not unique", SUT.getId().equals(toCompare.getId()));
    }

    @Test
    public void equals_IsEqual_true()
    {
        EntityStub SUT = new EntityStub();
        Assert.assertTrue("overrided equals should return true here", SUT.equals(SUT));
    }

    @Test
    public void equals_IsEqual_false()
    {
        EntityStub SUT = new EntityStub();
        EntityStub toCompare = new EntityStub();

        Assert.assertFalse("overrided equals should return true here",SUT.equals(toCompare));
    }

    @Test
    public void hashcode_fromIdOnly_true()
    {
        EntityStub SUT = new EntityStub();
        int expected  = SUT.getId().hashCode();
        int actual = SUT.hashCode();
        Assert.assertTrue("overrided equals should return true here",expected==actual);
    }



}