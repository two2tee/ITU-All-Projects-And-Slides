import java.util.Iterator;

/**
*	This class will create a random queue object which supports insertion, 
*   deletion of a uniformly random element, and iteration in random order. 
**/
public class RandomQueue<Item> implements Iterable<Item>
{
	private Item[] array;         // array of items
	private int N = 0;            // number of elements in queue

	// create an empty random queue
	@SuppressWarnings("unchecked")
	public RandomQueue(){
		array = (Item[]) new Object[10];
	}

    /**
    * This method will check if the array does not 
    * contain any elements. If it is empty 
    * the method will return true
    * @return a boolean 
    **/
	public boolean isEmpty(){return N == 0;} 

	/**
    * This method will return the number of elements
    * queued up in the array
    * @return an integer representing number of elements in array
    **/
	public int size(){return N;} 

	/**
    * This method will "enqueue" or with other words add
    * a generic element into the array. The element will placed
    * at N + 1, where N is the number of elements.
    * The method will also double the size of the array, whenever
    * the number of elements is equal to the lenght of the array
    * @param item - a generic element
    **/
	@SuppressWarnings("unchecked")
	public void enqueue(Item item)
	{
		if(N == array.length)
		{
			Item[] temp = (Item[]) new Object[array.length * 2];  //Double array size
			for (int i = 0; i < N; i++)
				{
					temp[i] = array[i];
				}
			array = temp;
		}
		array[N] = item; //element to array
		N++;
		
	}

	/**
	 * This method creates a random integer in the interval [0-N) and returns the element referred to by the index
	 * corresponding to the random integer. 
	 * 
	 * @return Item
	 */
	// return (but do not remove) a random item
	public Item sample(){
		int i = StdRandom.uniform(N);
		if(array[i] != null)
		{
			return array[i];
		}
		else throw new RuntimeException("element at index "+ i + " is is empty");
	}

	/**
	 * This method will return a random item from the array and then remove it.
	 * Also it will half the size of the array if the length of the array is not null and
	 * the number of elements in the array is 1/4 the size of the array.
	 * @return Item
	 */
	// remove and return a random item
	@SuppressWarnings("unchecked")
	public Item dequeue(){
		if(array.length != 0)
		{
			int number = StdRandom.uniform(N);
			Item item = array[number];

			array[number] = array[N-1];
			array[N] = null;

			N--;

			if (N == array.length / 4)
			{
				Item[] temp = (Item[]) new Object[array.length / 2];  //Double array size
				for (int i = 0; i < N; i++)
				{
					temp[i] = array[i];
				}
				array = temp;
			}
			return item;
		}
		else throw new RuntimeException("Array is is empty");
	}

	/**
	 * Returns a new RandomArrayIterator object
	 * @return
	 */
	// return an iterator over the items in random order
	public Iterator<Item> iterator(){return new RandomArrayIterator();}

	/**
	 * This class will create a RandomArrayIterator object,
	 * which shuffles and then iterates over an array.
	 */
	// Inner class
	private class RandomArrayIterator implements Iterator<Item>
	{
		private int count = N;            // number of elements in queue
		private Item[] shuffledList = array.clone();         // array of items
		
		
		// create an empty random queue
		public RandomArrayIterator()
		{
			shuffle(shuffledList);
		}


		/**
		 * This method will randomly change the position of the elements in a given array. 
		 * @param a
		 */
		private void shuffle(Item[] a)
		{
			for (int i = 0; i < N; i++)
			{
				int r = StdRandom.uniform(N);     // between i and N-1
				Item temp = a[i]; //Storing i element
				a[i] = a[r]; //replace i element with r element
				a[r] = temp; //replace r element with stored i element
			}
		}

		/**
		 * This method will check if there is any elements left to iterate over. 
		 * @return
		 */
			@Override
			public boolean hasNext()
			{
				return count > 0;
			}

		/**
		 * This method will count down the number of elements by one and return a shuffled array.
		 * @return
		 */
			@Override
			public Item next()
			{
				return shuffledList[--count];
			}
		}

	
	
	//Test


	// The main method below tests your implementation. Do not change it.
	public static void main(String args[])
	{
		// Build a queue containing the Integers 1,2,...,6:
		RandomQueue<Integer> Q= new RandomQueue<Integer>();
		for (int i = 1; i < 7; ++i) Q.enqueue(i); // autoboxing! cool!

		// Print 30 die rolls to standard output
		StdOut.print("Some die rolls: ");
		for (int i = 1; i < 30; ++i) StdOut.print(Q.sample() +" ");
		StdOut.println();

		// Let’s be more serious: do they really behave like die rolls?
		int[] rolls= new int [10000];
		for (int i = 0; i < 10000; ++i)
			rolls[i] = Q.sample(); // autounboxing! Also cool!
		StdOut.printf("Mean (should be around 3.5): %5.4f\n", StdStats.mean(rolls));
		StdOut.printf("Standard deviation (should be around 1.7): %5.4f\n",
				StdStats.stddev(rolls));

		// Now remove 3 random values
		StdOut.printf("Removing %d %d %d\n", Q.dequeue(), Q.dequeue(), Q.dequeue());

		// Add 7,8,9
		for (int i = 7; i < 10; ++i) Q.enqueue(i);

		// Empty the queue in random order
		while (!Q.isEmpty()) StdOut.print(Q.dequeue() +" ");
		StdOut.println();

		// Let’s look at the iterator. First, we make a queue of colours:
		RandomQueue<String> C= new RandomQueue<String>();
		C.enqueue("red"); C.enqueue("blue"); C.enqueue("green"); C.enqueue("yellow");

		Iterator I= C.iterator();
		Iterator J= C.iterator();

		StdOut.print("Two colours from first shuffle: "+I.next()+" "+I.next()+" ");

		StdOut.print("\nEntire second shuffle: ");
		while (J.hasNext()) StdOut.print(J.next()+" ");

		StdOut.print("\nRemaining two colours from first shuffle: "+I.next()+" "+I.next());
	}
}