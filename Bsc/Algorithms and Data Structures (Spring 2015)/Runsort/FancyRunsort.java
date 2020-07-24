/**
 * Created by William on 09/03/15.
 */
/**
 *  The <tt>MergeBU</tt> class provides static methods for sorting an
 *  array using bottom-up mergesort.
 *  <p>
 *  For additional documentation, see <a href="http://algs4.cs.princeton.edu/21elementary">Section 2.1</a> of
 *  <i>Algorithms, 4th Edition</i> by Robert Sedgewick and Kevin Wayne.
 *
 *  @author Robert Sedgewick
 *  @author Kevin Wayne
 */
public class FancyRunsort
{

		// This class should not be instantiated.
		private FancyRunsort() { }

		// stably merge a[lo..mid] with a[mid+1..hi] using aux[lo..hi]
		private static void merge(Comparable[] a, Comparable[] aux, int lo, int mid, int hi) {

			// copy to aux[]
			for (int k = lo; k <= hi; k++) {
				aux[k] = a[k];
			}

			// merge back to a[]
			int i = lo, j = mid+1;
			for (int k = lo; k <= hi; k++) {
				if      (i > mid)              a[k] = aux[j++];  // this copying is unneccessary
				else if (j > hi)               a[k] = aux[i++];
				else if (less(aux[j], aux[i])) a[k] = aux[j++];
				else                           a[k] = aux[i++];
			}

		}

	//For insertion sort
	public static void sort(Comparable[] a, int lo, int hi) {
		for (int i = lo; i <= hi; i++) {
			for (int j = i; j > 0 && less(a[j], a[j-1]); j--) {
				exch(a, j, j-1);
			}
		}
	}

		/**
		 * Rearranges the array in ascending order, using the natural order.
		 * @param a the array to be sorted
		 */
		public static void sort(Comparable[] a) {
			// Sort a[] into increasing order.
			int N = a.length; // array length
			Comparable[] aux = new Comparable[N];
			int lo;
			int mid = 0;
			int hi = 0;
			int run = 0;

			if (N != 0)
			{
				do
				{
					lo = 0;
					for (int i = 1; i < N; i++)
					{
						//When not last element
						if (less(a[i], a[i - 1]) && i != N-1)
						{
							if (run == 0)
							{
								mid = i - 1;
								run++;
							}
							else
							{
								hi = i - 1;

								//insertion sort
								if(hi-lo < 8){
									sort(a, lo, hi);
								}

								merge(a, aux, lo, mid, hi);

								lo = i;

								run = 0;
							}

						}
						//When last element
						else if(i == N-1 && run == 1)
						{
							hi = i;
							merge(a, aux, lo, mid, hi);
						}
					}
				}
				while (!isSorted(a));
			}
		}

		/***********************************************************************
		 *  Helper sorting functions
		 ***********************************************************************/

		// is v < w ?
		private static boolean less(Comparable v, Comparable w) {
			return (v.compareTo(w) < 0);
		}


		// exchange a[i] and a[j] for insertion sort
		private static void exch(Object[] a, int i, int j) {
			Object swap = a[i];
			a[i] = a[j];
			a[j] = swap;
		}

		/***********************************************************************
		 *  Check if array is sorted - useful for debugging
		 ***********************************************************************/
		private static boolean isSorted(Comparable[] a) {
			for (int i = 1; i < a.length; i++)
				if (less(a[i], a[i-1])) return false;
			return true;
		}

		// print array to standard output
		private static void show(Comparable[] a) {
			for (int i = 0; i < a.length; i++) {
				StdOut.println(a[i]);
			}
		}

		/**
		 * Reads in a sequence of strings from standard input; bottom-up
		 * mergesorts them; and prints them to standard output in ascending order.
		 */
		public static void main(String[] args) {
			String[] a = StdIn.readAllStrings();
			FancyRunsort.sort(a);
			show(a);
		}
}
