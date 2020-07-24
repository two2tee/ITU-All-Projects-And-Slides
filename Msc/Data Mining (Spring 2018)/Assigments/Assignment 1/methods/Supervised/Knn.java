package methods.Supervised;

import java.util.*;


/**
 * Knn class to run program from.
 */
public class Knn<T,K> {
    private final T labelToPredict;

    public Knn(T labelToPredict) {
        this.labelToPredict = labelToPredict;
    }


    public void run(List<IEntity<K>> entities,int ratio){
		System.out.println("Loaded "+entities.size() + " students");

		List<IEntity<K>> testData = entities.subList(0,entities.size()/ratio); // 1/ratio is test data
		List<IEntity<K>> trainingData = entities.subList((entities.size()/ratio),entities.size()); // rest is training data

		runKnn(testData,trainingData);

	}




	@SuppressWarnings("unchecked")
    private void runKnn(List<IEntity<K>> testData, List<IEntity<K>> trainingData){
        System.out.println(String.format("Training data size %d | Test data size: %d",trainingData.size(),testData.size()));
        System.out.println("Label to predict: "+labelToPredict);
        int k = 7;
		List<Predicted<K>> results = new ArrayList<>();
		for (IEntity<K> testEntity :testData) {

            Object actualLabel = testEntity.getAttributeValue(labelToPredict.getClass());
            T expected = actualLabel != labelToPredict ? null : labelToPredict;
            T prediction = kNN(k,trainingData,testEntity);

			results.add(new Predicted(actualLabel,expected,prediction));
		}

        System.out.println("\n-- Predicted Results --");

        Evaluator e = new Evaluator(results, labelToPredict);

        System.out.println("\n-- Evaluating KNN --");
        System.out.println(e.toString()+"\n");

        double accuracy = e.estimateAccuracy();
	    double errorRate = e.estimateErrorRate();
		double precision = e.estimatePrecision();
		double specifity = e.estimateSpecificity();
		double sensitivity = e.estimateSensitivity();


        System.out.println(String.format("Accuracy:  %.2f percent",accuracy*100));
        System.out.println(String.format("Error Rate:  %.2f percent",errorRate*100));
        System.out.println(String.format("Precision :  %.2f percent", precision*100));
        System.out.println(String.format("Specificity:  %.2f percent", specifity*100));
        System.out.println(String.format("Sensitivity:  %.2f percent", sensitivity*100));
	}



	@SuppressWarnings("unchecked")
    private T kNN(int k, List<IEntity<K>> trainingData, IEntity<K> toPredict){

		//Find Distance to all other entities and sort them on those that are the most similar
		PriorityQueue<Dist<IEntity<K>>> queue = new PriorityQueue<>();
		for (IEntity<K> trainingEntity: trainingData) {
			int dist = toPredict.compareTo((K) trainingEntity,labelToPredict);
			queue.add(new Dist<>(dist,trainingEntity));
		}

		//Take the first k similar entities and see if they are same or not
		int same = 0;
		IEntity<K> nearest = queue.peek().entity;
		Object actualLabel = queue.poll().entity.getAttributeValue(labelToPredict.getClass());
		for(int i = 0; i < k; i++){
			if(actualLabel!= null && actualLabel.equals(labelToPredict)){
				same++;
			}
		}

		//Take majority class label and return that -- if 50/50 take most similar entity label
		if(same == k/2){
			return (T) nearest.getAttributeValue(labelToPredict.getClass());
		}
		return same > k/2 ? labelToPredict : null;

	}

}
