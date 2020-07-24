import java.util.ArrayList;

/**
 * Created by William on 21/04/15.
 */
public class SpanningUSA
{
	private static SeparateChainingHashST<String, Integer> cityVertices = new SeparateChainingHashST<>(); //Array with 5 letter words
	private static ArrayList<String> vName = new ArrayList<>(); //To print the edges of the MST
	private static int vCounter = 0;
	private static EdgeWeightedGraph graph;
	private static PrimMST primMST;
	private static boolean graphCreated = false;

	private static void saveVertices(String s){
		cityVertices.put(s.trim(), vCounter);
		vName.add(s); //To print the edges of the MST
		vCounter++;
	}

	private static void makeEdges(String s){
		String[] splitString = s.split("--|\\[|\\]");
		int v = cityVertices.get(splitString[0].trim());
		int w = cityVertices.get(splitString[1].trim());
		double weight = Double.valueOf(splitString[2].trim());

		graph.addEdge(new Edge(v, w, weight));
	}




	public static void main(String[] args){
		while(StdIn.hasNextLine())
		{
			String s = StdIn.readLine();
			if(!s.contains("--"))
			saveVertices(s);

			else if(s.contains("--") && !graphCreated){
				graph = new EdgeWeightedGraph(vCounter);
				graphCreated = true;
				makeEdges(s);
			}
			else {
				makeEdges(s);
			}
		}
		//Std out put data
		StdOut.println();
		StdOut.println("EdgeWeightedGraph Info:");
		StdOut.println("Vertices: " + graph.V());
		StdOut.println("Edges: " + graph.E());
		StdOut.println();

		primMST = new PrimMST(graph);

		StdOut.println("PrimMST Info:");
		StdOut.println("Weight of the edges in the minimum spanning tree: " + primMST.weight());
		StdOut.println();
		StdOut.println("The minimal spanning tree is: ");
		Iterable<Edge> iterable = primMST.edges();
		for(Edge edge : iterable)
			System.out.println(vName.get(edge.either()) + "-->" + vName.get(edge.other(edge.either())));

	}



}
