package itu.mmad.dttn.tingle.model.Dagger2_DependencyInjection.Modules;

import javax.inject.Singleton;

import dagger.Module;
import dagger.Provides;
import itu.mmad.dttn.tingle.model.Interfaces.ISort;
import itu.mmad.dttn.tingle.model.Searching.SearchHandler;
import itu.mmad.dttn.tingle.model.Searching.SelectionSort;

/**
 * Search module
 */
@Module //annotation to provide dependencies
public class SearchModule {

    @Provides
    @Singleton
    static ISort provideSelectionSort() {
        return new SelectionSort();
    }


    @Provides
    @Singleton
    static SearchHandler provideSearchHandler(ISort sortMethod) {
        return new SearchHandler(sortMethod);
    }
}
