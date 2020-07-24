package itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Components;

import android.content.Context;

import javax.inject.Singleton;

import dagger.Component;
import dagger.Subcomponent;
import itu.mmad.dttn.tingle.TingleApplication;
import itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Modules.ApplicationModule;
import itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Modules.NetworkModule;
import itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Modules.RepositoryModule;
import itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Modules.SearchModule;
import itu.mmad.dttn.tingle.model.Networking.NetworkHandler;
import itu.mmad.dttn.tingle.model.Searching.SearchHandler;
import itu.mmad.dttn.tingle.model.database.ThingsDatabase;

/**
 * This interface is a component in Dagger2 dependency injection framework
 * A component (hence the tag @component) act is a bridge interface between modules and injection
 * This component provides dependencies that has a lifetime of the application.
 * http://google.github.io/dagger/users-guide.html
 */
@Singleton
@Component(modules = {ApplicationModule.class, RepositoryModule.class, NetworkModule.class, SearchModule.class})
public interface ApplicationComponent {
    DataComponent dataComponent(RepositoryModule repositoryModule);

    SearchComponent searchComponent(SearchModule searchModule);

    Context getContext();

    NetworkHandler getNetworkManager();

    void inject(TingleApplication application);


}

/**
 * Sub-component that handles repository module
 * Since it is a sub-component of ApplicationComponent it will automatically
 * inherent methods from ApplicationComponent. Thus context can be provided to database
 */
@Singleton
@Subcomponent(modules = {RepositoryModule.class})
interface DataComponent {
    ThingsDatabase getDatabase();
}

/**
 * Sub-component that handles search modules
 */
@Singleton
@Subcomponent(modules = {SearchModule.class})
interface SearchComponent {
    SearchHandler getSearchHandler();
}