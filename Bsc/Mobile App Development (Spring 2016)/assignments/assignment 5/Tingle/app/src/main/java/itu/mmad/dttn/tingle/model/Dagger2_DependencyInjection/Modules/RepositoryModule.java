package itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Modules;

import android.content.Context;

import javax.inject.Singleton;

import dagger.Module;
import dagger.Provides;
import itu.mmad.dttn.tingle.model.Interfaces.IRepository;
import itu.mmad.dttn.tingle.model.database.ThingsDatabase;
import itu.mmad.dttn.tingle.model.database.repositories.sqlSchema.SQLRepository;

/**
 * A module is a class whose methods declares and provides the dependencies of classes
 * <p/>
 * Using dagger2 framework
 * http://google.github.io/dagger/use
 * rs-guide.html
 */

@Module //annotation to provide dependencies
public class RepositoryModule {

//    @Provides
//    @Singleton
//        //Will automatically make sure that the returned object is a singleton
//    IRepository provideInMemRepository(Context context) {
//        return new InMemoryRepository(context);
//    }

    @Provides
    @Singleton
    static IRepository provideSQLRepository(Context context) {
        return new SQLRepository(context);
    }

    @Provides
    @Singleton
    static ThingsDatabase provideDatabase(IRepository repository, Context context) {
        return new ThingsDatabase(repository, context);
    }

}
