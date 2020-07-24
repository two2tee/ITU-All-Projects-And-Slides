import java.util.*;
public class NetworkFlow {
    private int[][] graph;
	private int[][] residual;
	private String[] names;
	private int source;
	private int sink;
	private int v;
	private boolean[] visited;

    public void parse(){
        Scanner scanner = new Scanner(System.in);
        String line;
        String name = "";
        String sequence = "";

        line = scanner.nextLine();
        v = Integer.parseInt(line);

        graph = new int[v][v];
		names = new String[v];
        visited = new boolean[v];
        
        for(int i = 0; i < v; i++){
          line = scanner.nextLine();
		  names[i] = line;
        }

        line = scanner.nextLine();
        int e = Integer.parseInt(line);

        for(int i = 0; i < e; i ++){
          line = scanner.nextLine();
          String[] edgeValues = line.split(" ");
          int from = Integer.parseInt(edgeValues[0]);
          int to = Integer.parseInt(edgeValues[1]);
          int capacity = Integer.parseInt(edgeValues[2]);

          if(capacity == -1)
	       capacity = Integer.MAX_VALUE;

          graph[from][to] = capacity;
        }
    }

	public boolean BFS(int[][] residualGraph, int[] path){
		Queue<Integer> queue = new LinkedList<Integer>();
		Arrays.fill(visited, false);

		queue.add(0); //Add source and set visited on source to true
		path[0] = -1; //Set path to -1 for source node
		visited[0] = true; //Set visited to true for source node

		while(!queue.isEmpty()){
			int from = queue.poll();
			
			for(int to = 0; to < v; to++){
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

	public int runFordFulkerson() {
		int maxFlow = 0;
		source = 0;
		sink = v-1;
		int[][] residualGraph = new int[v][v];

		for(int i = 0; i < v; i++)
			for(int j = 0; j < v; j++)
				residualGraph[i][j] = graph[i][j];

		int[] path = new int[v];

		while(BFS(residualGraph, path)){
			int flow = augment(path, residualGraph);
			maxFlow += flow;
		}
		
		residual = residualGraph;
		return maxFlow;
	}
	
	private int augment(int[] path, int[][] residualGraph){
    	int b = bottleneck(path, residualGraph);
		for(int to = sink; to != 0; to = path[to]){
			 int from = path[to];
			 //When we send flow along forward edge, decrease what can be sent on it 
			 //and when we send flow along backward edge, decrease what can be sent on it as well
			 residualGraph[from][to] -= b;
			 residualGraph[to][from] += b;//Increase what can be sent on the reverse edge - either backward or forward edge
		}

		return b;
	}

	private int bottleneck(int[] path, int[][] residualGraph){
		int minimumCapacity = Integer.MAX_VALUE;

		for(int to = sink; to != 0; to = path[to]){
			int from = path[to];
			if(residualGraph[from][to] < minimumCapacity)
				minimumCapacity = residualGraph[from][to];
		}
		return minimumCapacity;
	}

	private void getMinimumCut(){
		int[] path = new int[v];
		
		System.out.println("Minimum cut: ");
		for(int i = 0; i < v; i++){
		  for(int j = 0; j < v; j++) {
			 if(visited[i] && !visited[j] && graph[i][j] > 0){
                if(graph[i][j] > 0)
				    System.out.println(i + " " + j + " " + graph[i][j]);
				}
			}
		}
	}

	public static void main(String args[]){
		NetworkFlow networkFlow = new NetworkFlow();
		networkFlow.parse();
		int maxFlow = networkFlow.runFordFulkerson();
        System.out.println("MaxFlow: " + maxFlow);
		networkFlow.getMinimumCut();
	}
}
