package methods.FrequentPatternMining.Apriori;

class StrongFrequency implements Comparable<StrongFrequency> {
    public final String rule;
    public final int confidence;
    public final double minConf;

    public StrongFrequency(String rule, int confidence, double minConf) {
        this.rule = rule;
        this.confidence = confidence;
        this.minConf = minConf;
    }


    @Override
    public int compareTo(StrongFrequency o) {
        return this.confidence-o.confidence;
    }
}
