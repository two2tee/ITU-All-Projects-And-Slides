package itu.mmad.dttn.tingle.Model.Dagger2_DependencyInjection.Modules;

import javax.inject.Singleton;

import dagger.Module;
import dagger.Provides;
import itu.mmad.dttn.tingle.Model.InMemoryRepository;
import itu.mmad.dttn.tingle.Model.Interfaces.IRepository;
import itu.mmad.dttn.tingle.Model.ThingsDatabase;

/**
 * A module is a class whose methods declares and provides the dependencies of classes
 * <p/>
 * Using dagger2 framework
 * http://google.github.io/dagger/use
 * rs-guide.html
 */

@Module //annotation to provide dependencies
public class RepositoryModule {


    @Provides
    @Singleton
        //Will automatically make sure that the returned object is a singleton
    IRepository provideInMemRepository() {
        return new InMemoryRepository();
    }


    @Provides
    @Singleton
    ThingsDatabase provideDatabase(IRepository repository) {
        return new ThingsDatabase(repository);
    }


}
