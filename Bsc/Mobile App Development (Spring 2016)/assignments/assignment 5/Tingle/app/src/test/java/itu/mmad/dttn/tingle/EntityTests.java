package itu.mmad.dttn.tingle;

import junit.framework.Assert;

import org.junit.Test;

import java.util.UUID;

import itu.mmad.dttn.tingle.Stubs.EntityStub;

/**
 * To work on unit tests, switch the Test Artifact in the Build Variants view.
 */
public class EntityTests {
    @Test
    public void Id_NotNull_true() {
        EntityStub SUT = new EntityStub(UUID.randomUUID());
        Assert.assertNotNull("No id created", SUT.getId());
    }

    @Test
    public void Id_get_correctId_true() {
        UUID expected = UUID.randomUUID();
        EntityStub SUT = new EntityStub(expected);

        UUID actual = SUT.getId();
        Assert.assertEquals("Returned id is not equal to inserted id", expected, actual);
    }


}