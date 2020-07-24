import java.util.*;

// Ford Fulkerson computes the maximum flow in a flow network
public class FordFulkerson {
	private static boolean[] visited;
	
	// Traverse graph by breadth 
	public static boolean BFS(int[][] residualGraph, int[] path,int source,int sink) {
		Queue<Integer> queue = new LinkedList<Integer>();
		Arrays.fill(visited, false);

		queue.add(source); //Add source and set visited on source to true
		path[source] = -1; //Set path to -1 for source node
		visited[source] = true; //Set visited to true for source node

		while(!queue.isEmpty()){
			int from = queue.poll();
			for(int to = 0; to < path.length; to++){
			    if(residualGraph[from][to] <= 0 || visited[to]){
			        continue;
			    }
				queue.add(to);
				visited[to] = true;
				path[to] = from;
			}
		}
		return visited[sink];
	}

	// Construct residual graph and find maximum flow 
	// Repeatedly augments the flow using BFS while there is path from source to sink
	public static int run(int[][] graph, int source, int sink) {
		int v = graph.length;
		visited = new boolean[v];
		int maxFlow = 0;
		int[][] residualGraph = new int[v][v];

		for(int i = 0; i < v; i++)
			for(int j = 0; j < v; j++)
				residualGraph[i][j] = graph[i][j];
		int[] path = new int[v];

		while(BFS(residualGraph, path, source, sink)){
			int flow = augment(path, residualGraph,source,sink);
			maxFlow += flow;
		}
		return maxFlow;
	}
	
	// Helper function to augment flow to the flow already established in the graph  
	private static int augment(int[] path, int[][] residualGraph,int source,int sink){
    	int b = bottleneck(path, residualGraph, source, sink);
		for(int to = sink; to != source; to = path[to]){
			 int from = path[to];
			 //When we send flow along forward edge, decrease what can be sent on it 
			 //and when we send flow along backward edge, decrease what can be sent on it as well
			 residualGraph[from][to] -= b;
			 //Increase what can be sent on the reverse edge - either backward or forward edge
			 residualGraph[to][from] += b;
		}
		return b;
	}

	// Identify bottleneck capacity in graph 
	private static int bottleneck(int[] path, int[][] residualGraph, int source,int sink){
		int minimumCapacity = Integer.MAX_VALUE;

		for(int to = sink; to != source; to = path[to]){
			int from = path[to];
			if(residualGraph[from][to] < minimumCapacity)
				minimumCapacity = residualGraph[from][to];
		}
		return minimumCapacity;
	}
	
}
