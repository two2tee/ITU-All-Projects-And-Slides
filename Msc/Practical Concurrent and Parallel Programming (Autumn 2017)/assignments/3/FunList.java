    import java.util.function.BiFunction;
    import java.util.function.Consumer;
    import java.util.function.Function;
    import java.util.function.*;

    public class FunList<T> {
    final Node<T> first;

    protected static class Node<U> {
        public final U item;
        public final Node<U> next;

        public Node(U item, Node<U> next) {
            this.item = item;
            this.next = next;
        }
    }

    public FunList(Node<T> xs) {
        this.first = xs;
    }

    public FunList() {
        this(null);
    }

    public int getCount() {
        Node<T> xs = first;
        int count = 0;
        while (xs != null) {
            xs = xs.next;
            count++;
        }
        return count;
    }

    public T get(int i) {
        return getNodeLoop(i, first).item;
    }

    // Loop-based version of getNode
    protected static <T> Node<T> getNodeLoop(int i, Node<T> xs) {
        while (i != 0) {
            xs = xs.next;
            i--;
        }
        return xs;
    }

    // Recursive version of getNode
    protected static <T> Node<T> getNodeRecursive(int i, Node<T> xs) {    // Could use loop instead
        return i == 0 ? xs : getNodeRecursive(i-1, xs.next);
    }

    public static <T> FunList<T> cons(T item, FunList<T> list) {
        return list.insert(0, item);
    }

    public FunList<T> insert(int i, T item) {
        return new FunList<T>(insert(i, item, this.first));
    }

    protected static <T> Node<T> insert(int i, T item, Node<T> xs) {
        return i == 0 ? new Node<T>(item, xs) : new Node<T>(xs.item, insert(i-1, item, xs.next));
    }

    // Exercise 3.1.1 - remove ==========================
    public FunList<T> remove(T x){
        return new FunList<T>(remove(x, this.first));
    }

    protected Node<T> remove(T x, Node<T> xs) {
        if(xs == null) { return xs; } // Base case

        if(x.equals(xs.item)) // Logical equality
        {
            return remove(x, xs.next); // Continue recursively on remaining list (next)
        }
        else
        {
            return new Node<>(xs.item, remove(x, xs.next));
        }
    }

    //Exercise 3.1.2 - Predicate ======================================

    public int count(Predicate<T> p) {
        return count(p, this.first);
    }

    protected int count (Predicate<T> p, Node<T> xs) {
        if(xs == null) { return 0; } // Base case

        boolean matchesPredicate = p.test(xs.item);
        if(matchesPredicate) { return 1 + count(p, xs.next); }
        else return count(p, xs.next);
    }

    //Exercise 3.1.3 - Filter ===========================================

    public FunList<T> filter(Predicate<T> p) {
        return new FunList<>(filter(p,this.first));
    }

    protected Node<T> filter(Predicate<T> p, Node<T> xs) {
        if(xs == null) { return null; } // Base case

        boolean matchesPredicate = p.test(xs.item);

        if(!matchesPredicate) {
            return filter(p, xs.next);
        }
        else return new Node<>(xs.item, filter(p, xs.next));

    }

    //Exercise 3.1.4 - removeFun using filter and lambda ============================================
    public FunList<T> removeFun(T x) {
       Predicate<T> p  = n -> !n.equals(x); // Do not include element x in list
       return filter(p);
    }

    //Exercise 3.1.5 - Flatten lists of lists into list ============================================
    public static <T> FunList<T> flatten(FunList<FunList<T>> xss) {
        return new FunList<T>(flatten(xss.first, xss.first.item.first));
    }

    protected static <T> Node<T> flatten(Node<FunList<T>> xss, Node<T> xs) {
        if (xs == null) {
            xss = xss.next;
            if (xss != null) {
                xs = xss.item.first;
            } else {
                return null;
            }
        }
        return new Node<T>(xs.item, flatten(xss, xs.next));
    }

    // QUESTION: WHY DOES THIS NOT WORK FUNCTIONALLY???
    //private static <T> Node<T> flatten2(FunList<T> xss, Node<T> xs) {
    //  if(xss == null) return null; // Base case
    //  return append(xs, flatten2(xss.next, xss.next.first));
    //}

    //Exercise 3.1.6 - flatten with reduce and lambda ============================================

    public FunList<T> flattenFun(FunList<FunList<T>> xss) {
    return xss.reduce(new FunList<T>(null), (l1, l2) -> l1.append(l2));
    }

    /*public FunList<T> flattenFun(FunList<FunList<T>> xss){ // same with explicit bi function
     BiFunction<FunList<T>,FunList<T>,FunList<T>> appendLists = (list1, list2) -> list1.append(list2);
     return xss.reduce(new FunList<>(),appendLists);
    }*/

    //Exercise 3.1.7 - maps elements in all lists and flatten ============================================
    public <U> FunList<U> flatMap(Function<T,FunList<U>> f) {
        return flatMap(f, this.first);
    }

    protected  <U> FunList<U> flatMap (Function<T,FunList<U>> f, Node<T> xs) {
    if (xs == null) { return null; } // Base case

    return f.apply(xs.item).append(flatMap(f, xs.next));
    }

    //An implementation called flatMapFun using map and flatten, and no explicit recursion or iteration.
    public <U> FunList<U> flatMapFun(Function<T,FunList<U>> f) {
    return flatten(this.map(f));
    }

    //Exercise 3.1.8 - scan results in new list with f.apply(yi-1, xi) ============================================
    public FunList<T> scan(BinaryOperator<T> f){
        return new FunList<T>(scan(f, this.first, new FunList<T>(), null));
    }

    protected Node<T> scan(BinaryOperator<T> f, Node<T> x, FunList<T> yList, T previousY){
        if(previousY == null) {
            return scan(f, x.next, cons(x.item, yList), x.item);
        }

        if(x.next == null) {
            return cons(f.apply(previousY, x.item), yList).reverse().first;
        }

        T prevY = f.apply(previousY, x.item); // yi-1

        return scan(f, x.next, cons(prevY, yList), prevY);
    }

    //==========================================================


    public FunList<T> removeAt(int i) {
        return new FunList<T>(removeAt(i, this.first));
    }

    protected static <T> Node<T> removeAt(int i, Node<T> xs) {
        return i == 0 ? xs.next : new Node<T>(xs.item, removeAt(i-1, xs.next));
    }

    public FunList<T> reverse() {
        Node<T> xs = first, reversed = null;
        while (xs != null) {
            reversed = new Node<T>(xs.item, reversed);
            xs = xs.next;
        }
        return new FunList<T>(reversed);
    }

    public FunList<T> append(FunList<T> ys) {
        return new FunList<T>(append(this.first, ys.first));
    }

    protected static <T> Node<T> append(Node<T> xs, Node<T> ys) {
        return xs == null ? ys : new Node<T>(xs.item, append(xs.next, ys));
    }

    @Override
    @SuppressWarnings("unchecked")
    public boolean equals(Object that) {
        return equals((FunList<T>)that);             // Unchecked cast
    }

    public boolean equals(FunList<T> that) {
        return that != null && equals(this.first, that.first);
    }

    // Could be replaced by a loop
    protected static <T> boolean equals(Node<T> xs1, Node<T> xs2) {
        return xs1 == xs2
                || xs1 != null && xs2 != null && xs1.item == xs2.item && equals(xs1.next, xs2.next);
    }

    public <U> FunList<U> map(Function<T,U> f) {
        return new FunList<U>(map(f, first));
    }

    protected static <T,U> Node<U> map(Function<T,U> f, Node<T> xs) {
        return xs == null ? null : new Node<U>(f.apply(xs.item), map(f, xs.next));
    }

    public <U> U reduce(U x0, BiFunction<U,T,U> op) {
        return reduce(x0, op, first);
    }

    // Could be replaced by a loop
    protected static <T,U> U reduce(U x0, BiFunction<U,T,U> op, Node<T> xs) {
        return xs == null ? x0 : reduce(op.apply(x0, xs.item), op, xs.next);
    }

    // This loop is an optimized version of a tail-recursive function
    public void forEach(Consumer<T> cons) {
        Node<T> xs = first;
        while (xs != null) {
            cons.accept(xs.item);
            xs = xs.next;
        }
    }

    @Override
    public String toString() {
        StringBuilder sb = new StringBuilder();
        forEach(item -> sb.append(item).append(" "));
        return sb.toString();
    }


    }
