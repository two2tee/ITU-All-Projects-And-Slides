package methods.Unsupervised.kMean;

import java.util.*;



public class KMeans {

	public static List<KMeanCluster> KMeansPartition(int k, List<Member> data)
	{
		//Choose random k objects as inital cluster centers
		List<KMeanCluster> clusters = GenerateInitalClusters(k, data);
		List<KMeanCluster> oldClusters = new ArrayList<>();


		do{
		//Assign each objects to cluster which it is closes to

			for (Member member:data) {
				assignToCluster(member,clusters);
			}

			if(KMeanCluster.equals(oldClusters,clusters)) return clusters;

			//Update cluster mean and use that as new center point
			oldClusters = new ArrayList<>(clusters);
			clusters = compressClusters(clusters);


		}while(true);

	}

	private static ArrayList<KMeanCluster> compressClusters(List<KMeanCluster> clusters) {
		ArrayList<KMeanCluster> newClusters = new ArrayList<>();

		for (KMeanCluster cluster:clusters){
			Member centroid = calculateCentroid(cluster); //New centroid
			KMeanCluster newCluster = new KMeanCluster(centroid);
			newClusters.add(newCluster);
		}
		return newClusters;
	}


	private static void assignToCluster(Member member,List<KMeanCluster> clusters){
		int clusterIndex = 0;
		double minDistance = Double.MAX_VALUE;

		for (int i = 0 ; i < clusters.size(); i++) {
			double currentDistance = distance(member,clusters.get(i).centroid);
			if(currentDistance < minDistance){
				clusterIndex = i;
				minDistance = currentDistance;
			}
		}
		clusters.get(clusterIndex).ClusterMembers.add(member);
	}

	private static List<KMeanCluster> GenerateInitalClusters(int k, List<Member> members) {
		Random r = new Random();
		Set<Integer> usedIndex = new HashSet<>();
		int index= r.nextInt(members.size());
		List<KMeanCluster> clusters = new ArrayList<>();

		for(int i = 0 ; i<k ; i++) {

			while (usedIndex.contains(index)){
				index = r.nextInt(members.size());
			}
			usedIndex.add(index);
			KMeanCluster cluster = new KMeanCluster(members.get(index)); //Set centroid
			clusters.add(cluster);
		}
		return clusters;
	}



	
	@SuppressWarnings("unchecked")
	private static Member calculateCentroid(KMeanCluster cluster) {
		double[] attributeMeans = new double[cluster.centroid.attributes.length];

		//Calc average
		int total = 0;
		for (Member member : cluster.ClusterMembers) {
			for(int i=0; i< member.attributes.length;i++){
				attributeMeans[i] += member.attributes[i];
			}
			total++;
		}

		for(int i=0; i< attributeMeans.length;i++){
			attributeMeans[i] = attributeMeans[i]/total;
		}

		return new Member(attributeMeans,null);
	}


	private static double distance(Member x, Member y){
		if(x.attributes.length != y.attributes.length){
			throw new IllegalArgumentException("length of attribute mismatch");
		}

		double distance = 0;
		for(int i=0; i<x.attributes.length;i++){
			distance += Math.pow(Math.abs(x.attributes[i]-y.attributes[i]),2);
		}

		return Math.sqrt(distance);
	}

}
