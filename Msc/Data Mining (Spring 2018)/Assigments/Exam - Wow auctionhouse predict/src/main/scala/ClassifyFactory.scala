import org.apache.spark.ml.PipelineModel
import org.apache.spark.mllib.tree.model.{DecisionTreeModel, RandomForestModel}

/**
  * Abstract factory trait provides an interface for creating families of related classifiers without specifying the concrete classes.
  */
trait ClassifyFactory[Model] {

  def getClassifier: Classifier[Model]

}

/**
  * Implementation of classifier factory used to retrieve different groups of classifiers at runtime.
  */
object ClassifyFactory {

  def getClassifier[Model](implicit factory: ClassifyFactory[Model]): Classifier[Model] = factory.getClassifier

  implicit def getANNClassifier: Classifier[PipelineModel] = ArtificialNeuralNetwork

  implicit def getRandomForestClassifier: Classifier[RandomForestModel] = RandomForestClassifier

  implicit def getDecisionTreeClassifier: Classifier[DecisionTreeModel] = DecisionTreeClassifier

}

