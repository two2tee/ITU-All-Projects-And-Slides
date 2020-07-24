import KMeansModel.{Cluster, Point}
import scala.annotation.tailrec
import scala.util.Random

/**
  * Trait (interface) responsible for defining the unsupervised KMeans classification algorithm used to find groups of similar data from the [[Model.PreprocessedAuction]] data.
  *
  *
  * K-means groups similar data together and is used to search through the data and group together data that have similar attributes.
  * In practice, k-means creates k groups from a set of objects so that the members of a group are more similar.
  * It is a a simple unsupervised clustering algorithm designed to form groups such that the group members are more similar versus non-group members.
  * However, it is sensitive to outliers and the initial choice of centroids.
  * Also, it is generally designed to operate on numeric continuous (ranged, e.g. height) data as opposed to numeric discrete data (specific, e.g. gender)
  * Thus, it may be used to group individual students based on features/attributes/variables such as height and shoe size that may help classify their gender.
  * The goal is to predict k centroids and a label for each data point.
  * NB: notice that the feature vectors of each data point do not have labels, explaining why it is unsupervised.
  */
sealed trait KMeans {
  def kMean (dataSet : List[Point], k: Int) : List[Cluster]
}

/**
  * Implementation of K-means algorithm.
  * In this particular implementation, each point represents a feature vector with student attributes such as height and shoe size.
  * The KMeans algorithm is used to predict the gender of students from the survey based on their shoe size (x) and height (y).
  */
object KMeans {

  val MAX_ITERATIONS = 1000

  /**
    * The k-means algorithm is given a training set used to group the data into similar clusters.
    * Each data point represents a feature vector with no label (unsupervised).
    * The algorithm predicts k cluster centroids and a label for each data point.
    * The centroid is calculated by finding the 'mean' (average or center) of a cluster.
    * @param dataSet Data set of points.
    * @param k Amount of centroids.
    */
  def kMean (dataSet : List[Point], k: Int = 3) : List[Cluster] = {

    // 1 Initialize; Build initial set of k mean clusters from random centroids in data
    val initialClusters = initializeClusterCentroids(dataSet, k)

    // Run k-means algorithm to get k centroids (which define clusters of similar data)
    @tailrec
    def repeatUntilConvergence(oldCentroids: List[Cluster], updatedCentroids: List[Cluster], iterations: Int) : List[Cluster] =
      isConverged(oldCentroids, updatedCentroids, iterations) match {
        case true => updatedCentroids // 4 Stop condition: algorithm has converged so assignments no longer change
        case _ =>

          val oldCentroids = updatedCentroids // Book keeping of old centroids for convergence test

          // 2 Assignment step: Assign points to similar cluster based on mean value of points in cluster
          val assignedClusters : List[Cluster] = dataSet.foldLeft (updatedCentroids) ((centroids, point) => assignPointToCluster(point, centroids))

          // 3 Update step: calculate new means to be the centroids of the observations in the new cluster
          val updatedClusters : List[Cluster] = updateClusterCentroids(assignedClusters)

          repeatUntilConvergence(oldCentroids, updatedClusters, iterations+1)
      }

    repeatUntilConvergence(oldCentroids = List(), initialClusters, iterations = 0)
  }

  /**
    * Returns initial clusters based on k random centroids (points which are at the center of a cluster).
    * @param dataSet Data set of points.
    * @param k Amount of centroids.
    * @return Clusters.
    */
  def initializeClusterCentroids(dataSet: List[Point], k: Int) : List[Cluster] = {
    val random = new Random()
    val randomCentroids = (0 until k).map(_ => dataSet(random.nextInt(dataSet.size))).toList
    val initialClusters = randomCentroids.map(centroid => Cluster(centroid, List(centroid)))
    initialClusters
  }

  /**
    * Assign a data point to the most similar cluster with the nearest mean/average value (i.e. least distance).
    * A point is part of a cluster if it is closer to that cluster's centroid than any other centroid.
    * @param point Point that should be assigned (i.e. labelled) to a cluster.
    * @param clusters Clusters representing distinct groups of similar data.
    * @return Clusters in which the point has been assigned to its most similar cluster.
    */
  def assignPointToCluster(point: Point, clusters: List[Cluster]) : List[Cluster] = {
    // Find index of cluster with lowest mean
    val clusterIndex = clusters.foldLeft (Double.MaxValue, 0) ((distIndexPair, cluster:Cluster) => {
      val currentDistance = point.distanceTo(cluster.centroid)
      if (currentDistance < distIndexPair._1) (currentDistance, clusters.indexOf(cluster))
      else distIndexPair
    })
    // Assign point to cluster based on index
    clusters.zipWithIndex.map { case (cluster:Cluster, index:Int) =>
      if (index == clusterIndex._2) cluster.copy(members = point :: cluster.members)
      else cluster
    }
  }

  /**
    * Updates the cluster centroids by calculating the new means to be the centroids of the new clusters.
    * Helper function used to calculate the centroid (center) of a cluster based on the mean.
    * @param clusters Clusters to be updated.
    * @return Updated clusters.
    */
  def updateClusterCentroids(clusters: List[Cluster]) : List[Cluster] = {
    clusters.map(cluster => {
      val newCentroid = calculateCentroidMean(cluster)
      cluster.copy(centroid = newCentroid, members = newCentroid :: cluster.members) // TODO: add old members of cluster
    })
  }

  /**
    * Returns true of false if the k-means algorithm is done.
    * K-means terminates if the algorithm has converged, meaning assignments to the cluster no longer change
    * Otherwise, it terminates because it has run a maximum number of iterations.
    * @param oldCentroids Old centroids for convergence test.
    * @param centroids Current centroids.
    * @param iterations Max number of iterations algorithm may run.
    * @return True if k-means has converged, meaning oldCentroids is equal to centroids.
    */
  def isConverged(oldCentroids: List[Cluster], centroids: List[Cluster], iterations: Int) : Boolean = {
    if (iterations > MAX_ITERATIONS) return true
    oldCentroids.toSet == centroids.toSet // Equality: oldCentroids.groupBy(identity) == centroids.groupBy(identity)
  }

  /**
    * Prints the closest centroid as label for each member (i.e. point) in the cluster
    * @param centroids Centroids representing final labels used to classify data.
    * @param dataSet Data to classify.
    */
  def printLabels(dataSet: List[Point], centroids: List[Cluster]) : Unit = {
    centroids.foreach(cluster =>
      cluster.members.foreach { member =>
        println(s"Centroid: ${cluster.centroid} Member: $member")
      })
  }

  // Helper function used to calculate the centroid (center) of a cluster based on the mean/average of its data points
  // Mean = Sum(Data) / Size(Data)
  def calculateCentroidMean (cluster: Cluster) : Point = {
    val size = cluster.members.size
    val itemBidMean = cluster.members.map(point => point.itemBid).sum / size
    val itemBuyoutMean = cluster.members.map(point => point.itemBuyout).sum / size
    val centroid = Point(itemBidMean, itemBuyoutMean)
    centroid
  }

}
