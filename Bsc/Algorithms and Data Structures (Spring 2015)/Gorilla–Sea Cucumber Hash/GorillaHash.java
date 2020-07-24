/**
 * Created by William and Dennis on 09/04/15.
 */
public class GorillaHash
{
	private int speciesCount;
	private int k;
	private int d;
	private String[] names;
	private int[][] species;

	public GorillaHash( String[] names, int d){
		speciesCount = 0;
		k = 10;
		this.names = names;
		this.d = d;
		species = new int[names.length][d];
	}

	/**
	 * Generate hashCode of key
	 * @return hash code value for the key
	 */
	private int hash(String string)
	{
		int value = string.hashCode() % d;

		if(value > 0)
		{
			return value;
		}
		else
			return value + d;
	}

	private void toSubString(String s){

		if(s.contains(">")){
			String mainString = "";
			for (int i = 0; i < 3; i++){
				mainString += StdIn.readLine();
			}

			String subString;

				for (int j = 0; j < mainString.length() - k; j++)
				{
					subString = mainString.substring(j, j + k);
					int subIndex = hash(subString);
						species[speciesCount][subIndex]++;
				}
			}
			speciesCount++;
	}


	private void calcSimilar(){
		int dotP;
		double vecLenP;
		double vecLenQ;
		double sumP;
		double sumQ;
		double similarity;

		StdOut.println();

		for (int i = 0; i < species.length; i++){
			dotP = 0;
			sumP = 0;
			sumQ = 0;


			for (int j = i; j < species.length; j++)
			{

				for (int l = 0; l < d; l++)
				{
					dotP += (species[i][l] * species[j][l]);

					sumP += Math.pow(species[i][l], 2);
					sumQ += Math.pow(species[j][l], 2);
				}
				vecLenP = Math.sqrt(sumP);
				vecLenQ = Math.sqrt(sumQ);

				similarity = dotP/(vecLenP*vecLenQ);

				StdOut.println(names[i] + " and " + names[j] + " similarity = " + similarity);
			}

			StdOut.println();
		}

	}

	public static void main(String[] args){
		String[] names = new String[]{"Human", "Gorilla", "Spider Monkey", "Horse", "Deer", "Pig",
				"Cow", "Gull", "Trout", "Rockcod", "Lamprey", "Sea-Cucumber"};

		GorillaHash matchMaker = new GorillaHash(names, 10000);
		while (StdIn.hasNextLine()){
			matchMaker.toSubString(StdIn.readLine());
		}
		matchMaker.calcSimilar();

		//For testing purposes
		StdOut.println();
		StdOut.println("######## Testing ########");
		int d = 1000;
		int[] p = new int[d];
		int[] q = new int[d];
		for (int i = 0; i < d; i++)
		{
			if(i < d-1)
			{
				p[i] = 0;
			}
			else
				p[i] = 1;

			if (i == 0)
				q[i] = 1;
			else
				q[i] = 0;
		}
		StdOut.println("Cosine of the angle: " + cos_angle(p, q));
		StdOut.println("Length of vector: " + length(q));

	}

	//##################################### Method testing ####################################################

	/**
	 * Method for testing the way of calculating the similarity of species
	 * @param p
	 * @param q
	 * @return double similarity
	 */
	private static double cos_angle(int[]p, int[]q){
		double similarity;

		StdOut.println();

			similarity = dotPro(p,q) / (length(p) * length(q));

		return similarity;
	}


	/**
	 * Method of testing the way of calculating the length of a vector
	 * @param p
	 * @return double length
	 */
	private static double length(int[] p){
		double length = 0;
		double sum = 0;

		for (int i = 0; i < p.length; i++){
			sum += Math.pow(p[i], 2);
		}
		length = Math.sqrt(sum);

		return length;
	}

	/**
	 * Method for testing the way of calculating the dot product of two vectors of equal length
	 * @param p
	 * @param q
	 * @return
	 */
	private static double dotPro(int[]p, int[]q){
		int dotPro = 0;
		for (int i = 0; i < p.length; i++)
		{
			dotPro += (p[i] * q[i]);
		}

		return dotPro;
	}
}
