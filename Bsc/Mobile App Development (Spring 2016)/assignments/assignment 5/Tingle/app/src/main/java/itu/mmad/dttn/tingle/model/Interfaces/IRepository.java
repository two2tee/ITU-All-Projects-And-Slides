package itu.mmad.dttn.tingle.model.Interfaces;

import java.util.Iterator;
import java.util.UUID;

import itu.mmad.dttn.tingle.model.database.Entity;

/**
 * Interface used to create repositories for the app
 */
public interface IRepository<E extends Entity> {
    E get(UUID id);

    Iterator<E> getAll();

    boolean put(E entity);

    boolean update(E toStore);

    boolean delete(UUID id);

}
