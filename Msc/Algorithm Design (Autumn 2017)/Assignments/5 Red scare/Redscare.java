import java.util.*;

public class Redscare {
    
    private boolean[][] graph;
    private boolean directed;
    private int[][] redWeightedGraph;
    private int[][] manyWeightedGraph;
    private boolean[] isRed;
    private boolean[] visited;
    private Map<String, Integer> vertexPositionMap;
    private String source, sink;
    private int edgeCount;
    private int vertexCount;
        
    public static void main(String[] args)
    {
        Redscare rs = new Redscare();
        rs.parse();
        System.out.println("Find Some: " + rs.findSome());
        System.out.println("Find Many:"+rs.findMany());
        System.out.println("Find Few: " + rs.findFew());
        System.out.println("Find Alternate: " + rs.findAlternate());
        System.out.println("Find None: " +rs.findNone());
    }
    
    public void parse() {
        Scanner scanner = new Scanner(System.in);
        vertexCount = scanner.nextInt();
        edgeCount = scanner.nextInt();
        int redCount = scanner.nextInt();
        source = scanner.next();
        sink = scanner.next();
        vertexPositionMap = new HashMap<>();
             
        visited = new boolean[vertexCount];
        isRed = new boolean[vertexCount];
        
        graph = new boolean[vertexCount][vertexCount];
        redWeightedGraph = new int[vertexCount][vertexCount];
        manyWeightedGraph = new int[vertexCount][vertexCount];
   
        // Parse vertices
        for(int i = 0; i < vertexCount; i++){
            Arrays.fill(manyWeightedGraph[i],1);
        }
        String vertexName; 
        for(int i = 0 ; i < vertexCount ; i++)
        {
            vertexName = scanner.nextLine();
            while(vertexName.isEmpty()) {
            vertexName = scanner.nextLine();
            }
            
            if(vertexName.contains("*"))
            {
                isRed[i] = true;
                vertexName = vertexName.replace("*","").trim();
                vertexPositionMap.put(vertexName, i);
            }
    
            else {
                vertexPositionMap.put(vertexName.trim(), i);
                isRed[i] = false;
            }
        }
        
        //Parse edges
        if(edgeCount == 0) return;
        String[] edgeValues; 
   
        for(int i = 0; i < edgeCount; i ++) {
            String edgeLine = scanner.nextLine();
           
            if(edgeLine.contains("--")) { // Undirected edge
                directed = false;
                edgeValues = edgeLine.split("--");    
                int from = vertexPositionMap.get(edgeValues[0].trim());
                int to =  vertexPositionMap.get(edgeValues[1].trim());
                graph[from][to] = true;
                graph[to][from] = true;

                if (isRed[to]) {
                    redWeightedGraph[from][to] = 1;
                    redWeightedGraph[to][from] = 1;
                }
            }
            else if(edgeLine.contains("->")) { // Directed edge 
                directed = true;
                edgeValues = edgeLine.split("->");
                int from = vertexPositionMap.get(edgeValues[0].trim());
                int to =  vertexPositionMap.get(edgeValues[1].trim());

                graph[from][to] = true;
                if (isRed[to]) redWeightedGraph[from][to] = 1;
                
                if (isRed[to]) manyWeightedGraph[from][to] = -1;
                else manyWeightedGraph[from][to] = 0; 
            }
        }
   }
    
 
    // NP

    public int findNone(){
        return getNoRedPath(new int[graph.length], vertexPositionMap.get(source), vertexPositionMap.get(sink));
    }
    
    public int findFew(){
        return few(vertexPositionMap.get(source), vertexPositionMap.get(sink));
    }

    public boolean findAlternate(){
        return alternate(new int[graph.length], vertexPositionMap.get(source), vertexPositionMap.get(sink));
    }
    
    // NP-HARD 
     public int findMany(){
        if(!directed){
            System.out.print("ERR: undirected graph! ");
            return -1;
        }
        int sourceInt = vertexPositionMap.get(source);
        int sinkInt = vertexPositionMap.get(sink);
        int[] path = bellmanFord(manyWeightedGraph, sourceInt, sinkInt);
        if(path == null){
            return -1;
        }
        int redCount = 0;
        for(int i = 0; i < path.length; i++)
            if(isRed[i]) redCount++;
            
        return redCount;
    }
    
    public boolean findSome() {
        if(directed){
            System.out.print("ERR: directed graph! ");
            return false;
        }
        int s = vertexPositionMap.get(source);
        int t = vertexPositionMap.get(sink);
        if(isRed[s] || isRed[t] || findFew()<0) {return true;}
        
        
        int len = graph.length;
        int[][] flowGraph = new int[len+2][len+2];
        for(int i=0; i<len;i++)
        {
            for(int j=0;j<len; j++) {
                if(graph[i][j]){ //Has edge
                    flowGraph[i][j] = 1;
                }
            }
        }
        // Create connection between S-s and S-t
        flowGraph[len][s] = 1;
        flowGraph[len][t] = 1;
        
        // Check between S and T if there is a flow for each red
        for(int i=0; i<len;i++)
        {
            if(isRed[i]){
                flowGraph[i][len+1] = 2; //sets connection between red and T
                if(FordFulkerson.run(flowGraph,len,len+1)==2){
                    return true; //If max flow is 2 if there is a connection from s to t and there is a connection to the red node
                }
                flowGraph[i][len+1] = 0;
            }
        }
        return false;
    }

    // Dijkstra algorithm adapted from: http://www.gitta.info/Accessibiliti/en/html/Dijkstra_learningObject1.html
    private int few(int source, int sink) {
        final Set<Integer> vertexSet = new HashSet<>();
        final int gLength = redWeightedGraph.length;
        final int[] dist = new int[gLength];
        final int[] prev = new int[gLength];

        Arrays.fill(visited,false);

        for (int i = 0 ; i < graph.length ; i++)
        {
            dist[i] = Integer.MAX_VALUE; //initial infinite distance from source to all nodes
            prev[i] = -1; //undefined
            vertexSet.add(i);
        }

        dist[source] = 0; //set inital distance of source
        while(!vertexSet.isEmpty()) {
            int currentNode = getMinDistNode(dist);
            
            if(currentNode==-1 || currentNode == sink){break;}
            
            vertexSet.remove(currentNode);
            visited[currentNode] = true;
            
            for(int to = 0; to < gLength ; to++){
                if(graph[currentNode][to] && !visited[to]){
                    int newDist = dist[currentNode] + redWeightedGraph[currentNode][to];

                    if(newDist < dist[to]){ // A shorter path to v has been found
                        dist[to] = newDist;
                        prev[to] = currentNode;
                    }
                }
            }
        }

        // Count reds
        int totalReds = 0;
        if(prev[sink] == -1){
            //System.out.println("No path between source and sink");
            return totalReds;
        }

        int current = sink;
        String path = Integer.toString(sink);
        while (true)
        {
            if(current == source || current == -1){
                //System.out.println("Shortest path with fewest reds: " + path);
                return totalReds;
            }

            if(isRed[current]) {
                totalReds = totalReds+1;
            }
            current = prev[current];
            
            path = appendPath(path,current);
        }
    }

    private String appendPath(String path, int prevNode){
        if(prevNode != -1){
            return  prevNode+"--"+path;
        }
        else return path;
    }

    private int getMinDistNode(int[] dist){
        int minNode = -1;
        int minDist = Integer.MAX_VALUE;
        for (int i = 0; i < dist.length; i++) {

            if(visited[i]){
                continue;
            }

            int distance = dist[i];
            if(distance < minDist)
            {
                minNode = i;
                minDist = distance;
            }
        }
        return minNode;
    }
    
	                           
    private boolean alternate(int[] path, int source, int sink) {
		Queue<Integer> queue = new LinkedList<Integer>();
		Arrays.fill(visited, false);
		queue.add(source); //Add source and set visited on source to true
		path[source] = -1; //Set path to -1 for source node
		visited[source] = true; //Set visited to true for source node
        int v = graph.length;
        
        boolean currentColor;
		while(!queue.isEmpty()){
			int from = queue.poll();
			currentColor = isRed[from];
			for(int to = 0; to < v; to++){
			    if(graph[from][to] == false || visited[to] || isRed[to] == currentColor)
			      continue;
				queue.add(to);
				visited[to] = true;
				path[to] = from;
			}
		}
		return visited[sink];
	}
	
    private int getNoRedPath(int[] path, int source, int sink) {
    	Queue<Integer> queue = new LinkedList<Integer>();
    	Arrays.fill(visited, false);
    	queue.add(source); //Add source and set visited on source to true
    	path[source] = -1; //Set path to -1 for source node
    	visited[source] = true; //Set visited to true for source node
        int v = graph.length;
        
    	while(!queue.isEmpty()){
    		int from = queue.poll();
    		for(int to = 0; to < v; to++){
    		    if(graph[from][to] == false || visited[to] || (isRed[to] && to!=sink) )
    		      continue;
    			queue.add(to);
    			visited[to] = true;
    			path[to] = from;
    		}
    	}
    	if(!visited[sink])
    	    return -1;
	    else {
	        int count = 0;
	        for(int i = sink; i!=source; i = path[i]){
	            count++;
	        }
	        return count;
	    }
	}
	
	
    /*
    Find Shortest path from source to sink in weighted directed graph (may be negative)
	BellmanFord implementation based on Bhojasia, Manish: Samfoundry - Java Program to Use the Bellman-Ford Algorithm (2011-17), accessed 10-11-2017 at:
	http://www.sanfoundry.com/java-program-use-bellman-ford-algorithm-find-shortest-path-between-two-vertices-assuming-that-negative-size-edges-exist-graph/
	*/
	private int[] bellmanFord(int[][] weightedGraph, int source, int sink) {
	    if(weightedGraph == null) {
	        //Only works for directed acyclic graphs - if graph is null, then it was undirected (meaning there are cycles)
	        return null;
	    } 
	    //Directed graph
    	int[] distances = new int[isRed.length];
        int[] path = new int[isRed.length];
        Arrays.fill(distances, Integer.MAX_VALUE);
        Arrays.fill(path, -1);
        distances[source] = 0;
        
        //Relax edges
        for (int node = 0; node < isRed.length-1; node++)
        {
            for (int sourcenode = 0; sourcenode < isRed.length; sourcenode++)
            {
                for (int destinationnode = 0; destinationnode < isRed.length; destinationnode++)
                {
                    if (weightedGraph[sourcenode][destinationnode] != Integer.MAX_VALUE && weightedGraph[sourcenode][destinationnode] > 0)
                    {
                        if (distances[destinationnode] > distances[sourcenode]
                                + weightedGraph[sourcenode][destinationnode])
                            distances[destinationnode] = distances[sourcenode]
                                    + weightedGraph[sourcenode][destinationnode];
                            path[destinationnode] = sourcenode;
                    }
                }
            }
        }
        //Check for negative edge cycles
        for (int sourcenode = 0; sourcenode < isRed.length; sourcenode++)
        {
            for (int destinationnode = 0; destinationnode < isRed.length; destinationnode++)
            {
                if (weightedGraph[sourcenode][destinationnode] != Integer.MAX_VALUE && weightedGraph[sourcenode][destinationnode] > 0)
                {
                    if (distances[destinationnode] > distances[sourcenode] + weightedGraph[sourcenode][destinationnode]) {
                              System.out.println("The Graph contains a negative edge cycle");  
                              return null; // Cyclic graph!
                            }
                }
            }
        }
        for (int vertex = 1; vertex <= isRed.length; vertex++)
        {
            if (vertex == sink)
                System.out.println("distance of source  " + source + " to "
                        + vertex + " is " + distances[vertex]);
        }
        return path;
    }
    
}