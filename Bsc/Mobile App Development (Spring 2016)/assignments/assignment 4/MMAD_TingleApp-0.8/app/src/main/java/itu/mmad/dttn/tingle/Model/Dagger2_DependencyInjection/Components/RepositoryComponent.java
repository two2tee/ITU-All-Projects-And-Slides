package itu.mmad.dttn.tingle.Model.Dagger2_DependencyInjection.Components;

import javax.inject.Singleton;

import dagger.Component;
import itu.mmad.dttn.tingle.Model.Dagger2_DependencyInjection.Modules.RepositoryModule;
import itu.mmad.dttn.tingle.Model.ThingsDatabase;

/**
 * This interface is a component in Dagger2 dependency injection framework
 * A component (hence the tag @component) act is a bridge interface between modules and injection
 * http://google.github.io/dagger/users-guide.html
 */

@Singleton
@Component(modules = RepositoryModule.class) //specify which modules are going to be used
public interface RepositoryComponent {
    ThingsDatabase provideDatabase();
}
