//import java.lang;

import java.util.function.Predicate;

public class TestFunList{
    public static void main(String[] args)
    {
        //TestRemove();
        //TestCount();
        //TestFilter();
        TestFlatten();
    }

    private static void TestFlatten() {
        FunList<Integer> l = genSample();
    }
    

    private static void TestCount() {
        FunList<Integer> l = genSample();
        Predicate<Integer> p  = n -> n.equals(13);
        int c = l.count(p);
        System.out.println(c);

    }

    private static void TestRemove()
    {
        FunList<Integer> l = genSample();
        FunList l2 = l.remove(13);
        System.out.println();
        l.toString();
        l2.toString();
    }

    private static void TestFilter()
    {
        FunList<Integer> l = genSample();
        Predicate<Integer> p  = n -> n.equals(13);
        FunList<Integer> l2 = l.filter(p);
        System.out.println(l2.toString());
    }

    private static FunList<Integer> genSample()
    {
        FunList<Integer> empty = new FunList<>(null),
                list1 = cons(9, cons(13, cons(0, empty)));                  // 9 13 0
        return list1;
    }

    private static <T> FunList<T> cons(T item, FunList<T> list) {
        return list.insert(0, item);
    }
}

