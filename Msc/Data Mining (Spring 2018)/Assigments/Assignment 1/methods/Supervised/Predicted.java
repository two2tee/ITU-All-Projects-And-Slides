package methods.Supervised;


class Predicted<T> {
    public final T actualLabel;     //The actual label of the other (any label which can be the predicted or something else)
    public final T expectedLabel;   //The expected label (eg null if it should be a negative prediction)
    public final T predictedLabel;  //The predicted label

    public Predicted(T actualLabel, T expectedLabel, T predictedLabel) {
        this.actualLabel = actualLabel;
        this.expectedLabel = expectedLabel;
        this.predictedLabel = predictedLabel;
    }

    @Override
    public String toString() {
        return String.format("Predicted: %s | Actual: %s", predictedLabel, expectedLabel);
    }

    public boolean isEqualToExpected(){
        return expectedLabel == predictedLabel;
    }


}
