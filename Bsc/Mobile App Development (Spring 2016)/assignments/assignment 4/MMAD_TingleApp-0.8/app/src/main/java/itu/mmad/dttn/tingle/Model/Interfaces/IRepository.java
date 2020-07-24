package itu.mmad.dttn.tingle.Model.Interfaces;

import java.util.Iterator;

import itu.mmad.dttn.tingle.Model.Entity;

/**
 * Interface used to create repositories for the app
 */
public interface IRepository<E extends Entity>{
    E get(int id);

    Iterator<E> getAll ();

    boolean put(E entity);

    void update (int id, String what, String where);

    boolean delete(int id);

}
