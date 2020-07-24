package methods.FrequentPatternMining.Apriori;

import java.util.Objects;

/***
 * The ItemSet class is used to store information concerning a single transaction.
 * Should not need any changes.
 *
 */
class ItemSet {
	
	/***
	 * The PRIMES array is internally in the ItemSet-class' hashCode method
	 */
	private static final int[] PRIMES = { 2, 3, 5, 7, 11, 13, 17, 23, 27, 31, 37 };
    final String[] set;

    /***
     * Creates a new instance of the ItemSet class.
     * @param set Transaction content
     */
    public ItemSet( String[] set ) {
        this.set = set;
    }



    @Override
    public int hashCode() {
        int code = 0;
        for (int i = 0; i < set.length; i++) {
            code += set[i].hashCode() * PRIMES[i];
        }
        return code;
    }

    
    @Override
    public boolean equals( Object o ) {
        if (!(o instanceof ItemSet)) {
            return false;
        }
        ItemSet other = (ItemSet) o;
        if (other.set.length != this.set.length) {
            return false;
        }
        for (int i = 0; i < set.length; i++) {
            if (!Objects.equals(set[i], other.set[i])) {
                return false;
            }
        }
        return true;
    }

    @Override
    public String toString() {
        StringBuilder c = new StringBuilder("{ ");
        for (String i:set) {
            c.append(i).append(" ");
        }
        return c+"}";
    }
}
