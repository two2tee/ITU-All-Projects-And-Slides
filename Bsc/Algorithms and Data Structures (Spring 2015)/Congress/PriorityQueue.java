/**
 * Main class
 */
public class PriorityQueue
{
	private static int numbOfStates = 0;
	private static int numbOfSeats = 0;

	private static State[] stateArr;
	private static MaxPQ<State> pq;

	/**
	 * Main method creating a congress and...
	 * @param args
	 */
	public static void main(String[] args) {
		createCongress();
		runPriorityQueue();
		printResults();
	}

	/**
	 * This method reads a file containing information of a state.
	 * A state object will be created for every state and will be stored in
	 * an array.
	 */
	private static void createCongress()
	{
		//Stores number of states and seats in congress
		int line = 0;
		while (!StdIn.isEmpty() && line < 2) {

			if (line == 0) numbOfStates = StdIn.readInt();
			if (line == 1) numbOfSeats = StdIn.readInt();
			StdIn.readLine(); //Used to move to next line
			line++;
		}

		//Create array with size = number if states
		stateArr = new State[numbOfStates];

		//Read state information and add them to an array
		if(stateArr.length > 0) {
			line = 1;
			int i = 0; //index of stateArr
			String state = "";
			int population = 0;
			boolean stateAdded = false;
			boolean populationAdded = false;


			//Retrieve data
			while(line <= numbOfStates*2){

				//If line number is odd = State name
				if (line % 2 == 1) {
					state = StdIn.readLine();
					stateAdded = true;
				}

				//If line number is even = state population
				else if (line % 2 == 0) {
					population = StdIn.readInt();
					StdIn.readLine(); // next line
					populationAdded = true;
				}

				//Add to array
				if(stateAdded && populationAdded)
				{
					stateArr[i] = new State(state,population,1);
					numbOfSeats--;

					//Reset
					stateAdded = false;
					populationAdded = false;
					i++;
				}

				line++;
			}
		pq = new MaxPQ<>(stateArr);
		}
	}

	/**
	 * Run priority queue and add seat to state accordingly
	 */
	private static void runPriorityQueue(){
		
		while (numbOfSeats > 0)
		{
			pq.max().addSeat();
			State temp = pq.delMax();
			pq.insert(temp);
			numbOfSeats--;
			
		}
	}

	/**
	 * Prints the results of how the seats have been dealt using the format:
	 * State 3
	 * Where State is the name of the state and 3 is the number of seats.
	 */
	private  static void printResults(){
		for(State state : stateArr){
			StdOut.println(state.stateName +" "+ state.seats);
		}
	}

	/**
	 * Each instance of this class represents a state.
	 * The instance contains name of state, population of state
	 * and number seats for that state
	 */
	private static class State implements Comparable<State> {
		private String stateName;
		private int population;
		private int seats;

		//Constructor
		public State(String stateName, int population, int seats)
		{
			this.stateName = stateName;
			this.population = population;
			this.seats = seats;
		}

		//Setter
		public void addSeat(){ seats++;}


		/**
		 * Compare method
		 * This method compares the population-seat ratio between two State instances using the Huntingdon-Hill method.
		 * @param state
		 * @return
		 */
		@Override
		public int compareTo(State state) {

			double divisorThisState = population/(Math.sqrt(seats*(seats+1)));
			double divisorCompareState = state.population/(Math.sqrt(state.seats*(state.seats+1)));

          //More than
			if(divisorThisState > divisorCompareState)
			{
				return 1; // one because state is greater than the state being compared with
			}

			//Equal
			else if(divisorThisState == divisorCompareState)
			{
				return 0; //Seats equal
			}

			//Less then
			else
			{
				return -1; //state is less than the state being compared with
			}
		}

	}

}
