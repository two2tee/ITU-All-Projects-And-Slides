package methods.Supervised;

import java.util.List;

class Evaluator<T> {
    private double TP, TN, FP, FN;

    private final double P; //TP + TN
    private final double N; //FP + FN
    private final double PN; //P + N




    Evaluator(List<Predicted<T>> predictions, T expected) {
        //Load all results to map
        System.out.println(" Actual | Expected | Predicted");

        for (Predicted<T> p: predictions) {
            System.out.println(p.actualLabel +" | "+ p.expectedLabel +" | " + p.predictedLabel);
            if(p.isEqualToExpected()){
                Measurement m = p.predictedLabel != null ? Measurement.TP : Measurement.TN;
                AddMeasurement(m);
            }
            else{
                Measurement m = p.predictedLabel != null ? Measurement.FP : Measurement.FN;
                AddMeasurement(m);
            }
        }
        P = TP + FP;
        N = TN + FN;
        PN = P + N;


    }

    @Override
    public String toString() {
        return String.format("TP: %s | TN: %s | FP: %s | FN: %s\n P: %s | N %s | PN %s", TP, TN, FP, FN, P, N, PN);
    }

    private  void AddMeasurement(Measurement measure){
        switch (measure){
            case FN: {
                this.FN++;
                break;
            }
            case TN:{
                this.TN++;
                break;
            }
            case FP: {
                this.FP++;
                break;
            }
            case TP: {
                this.TP++;
                break;
            }
        }
    }

    /**
     * Percentage of test tuple that are correct by classifier
     */
    public double estimateAccuracy(){
        return (P/PN);
    }

    /**
     * Percentage of test tuple that are incorrect by classifier
     */
    public double estimateErrorRate(){
        return (N/PN);
    }

    /**
     * True positive rate (recognition rate) -- How many edible mushrooms are in fact edible
     */
    public double estimateSensitivity() {
        return (TP / P);
    }

    /**
     * True Negative rate -- How many non-edible mushrooms are in fact non-edible
     */
    public double estimateSpecificity() {
        return (TN / N);
    }

    /**
     * Measure of exactness - What percentage of tuples labeled as positive are actually such
     */
    public double estimatePrecision() {
        double TPFP =  TP + FP;
        final double precision = (TP/TPFP);
        return precision;
    }


    /**
     * equal weight to precision and recall
     */
    public double estimateFscore() {
        double top = 2*estimatePrecision() * estimateSensitivity();
        double bot = estimatePrecision() + estimateSensitivity();
        return (top/bot);
    }

    /**
     * equal weight to precision and recall
     */
    public double estimateFBscore(int B) {
        double top = (1+B^2)*estimatePrecision() * estimateSensitivity();
        double bot = (B^2)*estimatePrecision() + estimateSensitivity();
        return (top/bot);
    }






}
