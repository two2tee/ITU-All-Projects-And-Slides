package itu.mmad.dttn.tingle.model.database;

import android.content.Context;
import android.os.Environment;
import android.support.v4.os.OperationCanceledException;

import java.io.File;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.UUID;

import javax.inject.Inject;
import javax.inject.Singleton;

import itu.mmad.dttn.tingle.model.Interfaces.IRepository;
import itu.mmad.dttn.tingle.model.Thing;

/**
 * Represents a database that contains a repository.
 * This class is used to separate the repository of application logic
 */

@Singleton
public class ThingsDatabase {

    //Setup database with injection
    private final IRepository<Thing> repository;
    private Context mContext;

    //annotation to request dependencies in constructor,
    @Inject
    public ThingsDatabase(IRepository repository, Context context) {
        this.repository = repository;
        this.mContext = context;
    }

    public Thing get(UUID id) {
        return repository.get(id);
    }

    public List<Thing> getAll() {

        ArrayList things = new ArrayList();
        Iterator<Thing> items = repository.getAll();
        while (items.hasNext()) {
            things.add(items.next());
        }

        return things;
    }

    public void put(Thing entity) {
        if (entity == null) return;
        repository.put(entity);
    }

    /**
     * Updates existing thing
     *
     * @param entity thing
     *               <p/>
     *               not implemented yet
     */
    public boolean update(Thing entity) {
        return repository.update(entity);
    }

    /**
     * Deletes a thing from database
     *
     * @param id of thing
     * @throws OperationCanceledException if item can not be found
     */
    public void delete(UUID id) throws OperationCanceledException {
        boolean isSuccess = repository.delete(id);
        if (!isSuccess) throw new OperationCanceledException("Deletion failed.");
    }

    /**
     * Returns the total numbers of things
     *
     * @return int
     */
    public int getTotalSize() {

        int size = 0;

        Iterator<Thing> items = repository.getAll();
        while (items.hasNext()) {
            size++;
            items.next();
        }

        return size;
    }

    /**
     * This code does not create any files on the filesystem. It only returns File objects that point to the right
     * locations. It does perform one check: it verifies that there is external storage to save them to. If there is
     * no external storage, it will return null.
     *
     * @param entity
     * @return a file or null
     */
    public File getPhotoFile(Thing entity) {
        File externalDir = mContext.getExternalFilesDir(Environment.DIRECTORY_PICTURES);
        if (externalDir == null) {
            return null;
        }

        return new File(externalDir, entity.getPhotoName());
    }

}
