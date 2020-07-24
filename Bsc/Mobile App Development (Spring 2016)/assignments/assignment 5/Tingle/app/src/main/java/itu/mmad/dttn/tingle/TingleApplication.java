package itu.mmad.dttn.tingle;

import android.app.Application;
import android.content.Context;

import javax.inject.Inject;

import itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Components.ApplicationComponent;
import itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Components.DaggerApplicationComponent;
import itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Modules.ApplicationModule;
import itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Modules.NetworkModule;
import itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Modules.RepositoryModule;
import itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Modules.SearchModule;
import itu.mmad.dttn.tingle.model.Networking.NetworkHandler;
import itu.mmad.dttn.tingle.model.Searching.SearchHandler;
import itu.mmad.dttn.tingle.model.database.ThingsDatabase;


/**
 * Is mainly used as container and initializer for dagger2 components
 */
public class TingleApplication extends Application {

    @Inject
    static ThingsDatabase DATABASE;

    @Inject
    static NetworkHandler NETWORKHANDLER;

    @Inject
    static SearchHandler SEARCHHANDLER;

    private ApplicationComponent mApplicationComponent;

    public static ThingsDatabase getDatabase() {
        return DATABASE;
    }

    public static NetworkHandler getNetworkManager() {
        return NETWORKHANDLER;
    }

    public static SearchHandler getSearchHandler() {
        return SEARCHHANDLER;
    }

    public static TingleApplication getApplication(Context context) {
        return (TingleApplication) context.getApplicationContext();
    }

    /**
     * Method retrieves a component of application so it can be injected
     *
     * @param context Context
     * @return Application component
     */
    public static ApplicationComponent getComponent(Context context) {
        return ((TingleApplication) context.getApplicationContext()).mApplicationComponent;
    }

    @Override
    public void onCreate() {
        super.onCreate();
        mApplicationComponent = DaggerApplicationComponent.builder()
                .applicationModule(new ApplicationModule(this))
                .repositoryModule(new RepositoryModule())
                .networkModule(new NetworkModule())
                .searchModule(new SearchModule())
                .build();

        getComponent(this).inject(this);
    }
}
