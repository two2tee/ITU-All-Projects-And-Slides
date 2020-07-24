package itu.mmad.dttn.tingle;

import junit.framework.Assert;

import org.junit.Test;

import itu.mmad.dttn.tingle.Model.Dagger2_DependencyInjection.Components.DaggerRepositoryComponent;
import itu.mmad.dttn.tingle.Model.Dagger2_DependencyInjection.Components.RepositoryComponent;
import itu.mmad.dttn.tingle.Model.Dagger2_DependencyInjection.Modules.RepositoryModule;
import itu.mmad.dttn.tingle.Model.ThingsDatabase;

/**
 * Tests for dagger dependency injection
 */
public class DaggerDependencyInjectionTests
{

    @Test
    public void DatabaseProvided_NotNull(){
        ThingsDatabase SUT;
            RepositoryComponent component = DaggerRepositoryComponent.builder()
                    .repositoryModule(new RepositoryModule()).build();

            SUT = component.provideDatabase();

        Assert.assertNotNull(SUT);
    }

    @Test
    public void DatabaseProvided_instanceOfThingsDatabase(){
        ThingsDatabase SUT;
        RepositoryComponent component = DaggerRepositoryComponent.builder()
                .repositoryModule(new RepositoryModule()).build();

        SUT = component.provideDatabase();

        Assert.assertTrue(SUT instanceof  ThingsDatabase);
    }
}
