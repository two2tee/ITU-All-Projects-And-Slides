package methods.Unsupervised.kMean;

/**
 * This class stores all the attributes used when clustering and is also
 * considered as a member inside a cluster.
 * The class ensures that Kmeans is generic and can be used with any attributes that are numeric
 * @param <K> the type of the stored entity
 */
public class Member<K> {
    public final double[] attributes;
    public final K entity;

    public Member(double[] attributes, K entity) {
        this.attributes = attributes;
        this.entity = entity;
    }
}

