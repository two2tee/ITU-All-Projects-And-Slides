package methods.Supervised;

class Dist <T> implements Comparable<Dist> {
    public final int dist;
    public final T entity;

    public Dist(int dist, T entity) {
        this.dist = dist;
        this.entity = entity;
    }

    @Override
    public int compareTo(Dist o) {
        return dist - o.dist;
    }
}
