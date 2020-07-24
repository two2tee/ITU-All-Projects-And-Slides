package methods.Unsupervised.kMean;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

//ToDo: Compute cluster mean based on cluster members.
public class KMeanCluster {

	public final ArrayList<Member> ClusterMembers;
	public final Member centroid;
	
	public KMeanCluster(Member centroid)
	{
		this.centroid = centroid;
		this.ClusterMembers = new ArrayList<>();
	}
	
	@Override
	public String toString() {
		String toPrintString = "-----------------------------------CLUSTER START------------------------------------------" + System.getProperty("line.separator");
		
		for(Member i : this.ClusterMembers)
		{
			toPrintString += i.entity.toString() + System.getProperty("line.separator");
		}
		toPrintString += "-----------------------------------CLUSTER END-------------------------------------------" + System.getProperty("line.separator");
		
		return toPrintString;
	}

	public static boolean equals(List<KMeanCluster> clusters1, List<KMeanCluster> clusters2 ){
		if(clusters1.size() != clusters2.size()) return false;

		for(int i = 0; i < clusters1.size(); i++){
			boolean sameCentroid = Arrays.equals(clusters1.get(i).centroid.attributes,clusters2.get(i).centroid.attributes);
			boolean sameMembersize = clusters1.get(i).ClusterMembers.size() == clusters2.get(i).ClusterMembers.size();
			if(!sameCentroid || !sameMembersize){
				return false;
			}
		}
		return true;

	}

}
