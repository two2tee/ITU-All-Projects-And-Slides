package itu.mmad.dttn.tingle.Model;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import itu.mmad.dttn.tingle.Model.Interfaces.IRepository;

/**
 * Created by new on 12-Feb-16.
 */
public class InMemoryRepository implements IRepository<Thing>, Serializable {

    private static InMemoryRepository inMemoryRepository;
    private final List<Thing> thingsDB;
    private static boolean isCreated = false;

    private InMemoryRepository() {
        thingsDB = new ArrayList<>();
    }

    public static InMemoryRepository getInMemoryRepository()
    {
        if(inMemoryRepository != null) return inMemoryRepository;
        else {
            inMemoryRepository = new InMemoryRepository();
            return  inMemoryRepository;
        }
    }


    @Override
    public Thing get(int id) {
        return thingsDB.get(id);
    }

    @Override
    public Iterator<Thing> getAll() {
        return thingsDB.iterator();
    }

    @Override
    public void put(Thing entity) {
        thingsDB.add(entity);
    }

    @Override
    public void update(Thing entity) {
        //no implemented
    }

    @Override
    public void delete(int id) {
        thingsDB.remove(id);
    }

    @Override
    public int returnSize() {
        return thingsDB.size()-1;
    }


}
