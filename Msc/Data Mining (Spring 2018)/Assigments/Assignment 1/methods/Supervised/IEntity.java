package methods.Supervised;

import java.util.List;

/**
 * This interface ensures that any class that uses KNN must satisfy these constraints
 * This ensures that knn is generic
 * @param <K> The type of the entity
 */
public interface IEntity<K> {
    Object getAttributeValue(Object Attribute);
    List<Object> getAttributeList(Object toCompare);

    int compareTo(K other, Object toCompare);
}
