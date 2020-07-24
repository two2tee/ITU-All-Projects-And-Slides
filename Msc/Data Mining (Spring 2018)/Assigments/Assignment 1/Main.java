import FileHandler.CSVFileReader;
import methods.FrequentPatternMining.Apriori.Apriori;
import methods.Supervised.Knn;
import methods.Supervised.IEntity;
import methods.Unsupervised.kMean.KMeanCluster;
import methods.Unsupervised.kMean.KMeans;
import methods.Unsupervised.kMean.Member;
import model.DataObject;
import model.Student;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class Main {

    public static void main(String[] args) {
        //Part 1 – Read specific raw data from file
        List<DataObject> dataobjects = CSVFileReader.ParseMasterData(CSVFileReader.readInputFile("FileHandler/input.csv"));

        //Part 2 – pre-processing and conversion to domain entities : Missing value + normalization
        List<Student> students = Cleaner.clean(dataobjects);

        //Part 3 - Mining : Apriori + Knn + Kmeans
        RunPatternMining(students);
        RunUnsupervisedMining(students);
        RunSupervisedMining(students);
    }

    /**
     * Frequent pattern mining with Apriori
     */
    private static void RunPatternMining(final List<Student> students){
        System.out.println("======= Started Pattern Mining =======");

        //Create transactions
        List<List<String>> transactions = new ArrayList<>();
        List<String> transaction;
        for (Student s : students) {
            transaction = new ArrayList<>();
            transaction.addAll(Arrays.asList(s.programmingLanguage)); //Select relevant attributes
            transactions.add(transaction);
        }

        //Run
        Apriori apriori = new Apriori();
        apriori.run(transactions);
    }

    /**
     * Unsupervised learning with k-means clustering
     */
    private static void RunUnsupervisedMining(final List<Student> students){
        System.out.println("======= Started Cluster Mining =======");

        //Create cluster members with specified attributes
        List<Member> members = new ArrayList<>();
        for (Student s : students) {
            double[] attributes = new double[]{s.shoesize,s.height,s.age};
            members.add(new Member<>(attributes, s));
        }

        //Run Kmeans
        int k = 6;
        List<KMeanCluster> clusters = KMeans.KMeansPartition(k,members);

        //Print results
        for (KMeanCluster cluster:clusters) {
            System.out.println(cluster.toString());
        }
    }

    /**
     * Supervised learning with Apriori
     */
    private static void RunSupervisedMining(final List<Student> students){
        System.out.println("======= Started Supervised Mining =======");

        Knn<Student.PhoneOS,Student> knn = new Knn<>(Student.PhoneOS.iOS); //Knn<ClassToPredict,Entity>
        List<IEntity<Student>> input = new ArrayList<>(students);
        knn.run(input,8); //ratio denotes the ratio of training data and test data eg 8 = 1/8 test data and 7/8 training data
    }

}
