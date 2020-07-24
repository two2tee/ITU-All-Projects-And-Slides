import java.util.*;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.lang.Math;
//This was not the delivered version (it is recursive)

public class SqRec{
    
    private Map<Character,Integer> letterIndex;
    private int[][] scoreM;
    private List<Subject> subjects = new ArrayList<>();
    private int clusterPct = 62; // >= 62
    private double entropy = 0.6979;
    private double expected = -0.5209;
    
    public SqRec(){
        generateMatrix();
    }
    
    public static void main (String[] args) {
        Sq sq = new Sq();
        sq.parseInput();
        sq.alignSubjects();
    }
    
   
    public void parseInput() {
      Scanner scanner = new Scanner(System.in);
      Pattern namePattern = Pattern.compile(">(\\w+)+");
      Pattern sequencePattern = Pattern.compile("([A-Z]+)");
      String line;
      String name = "";
      String sequence = "";
      
       while(scanner.hasNext()){
        line = scanner.nextLine();
        Matcher nameMatcher = namePattern.matcher(line);
        Matcher sequenceMatcher = sequencePattern.matcher(line);
        if(nameMatcher.find()){
            if(!name.equals("") && !sequence.equals(""))
                subjects.add(new Subject(name, sequence.toCharArray()));
            name = nameMatcher.group(1);
            sequence = "";
        }
        else if(sequenceMatcher.find()){
            sequence = sequence + sequenceMatcher.group(1);
        }
       }
       subjects.add(new Subject(name, sequence.toCharArray()));
       for(Subject sub: subjects)
       System.out.println(sub.getName());
    }
    
    private void alignSubjects(){
        for(Subject subjectOne : subjects){
            for (Subject subjectTwo : subjects){
                if (subjectOne.equals(subjectTwo)) continue;
                System.out.println("Alignment for " + subjectOne.getName() + ", and " + subjectTwo.getName());
                alignment(subjectOne.getSequence(), subjectTwo.getSequence());
                System.out.println("\n");
                
            }
        }
    }
    
    
    private int opt(char i, char j){
        return 0;
    }
    
    
    public class TraceElement{
        private char i;
        private char j;
        
        public TraceElement(char i, char j){
            this.i = i;
            this.j = j;
        }
        
        public char getI(){
            return i;
        }
        
        public char getJ(){
            return j;
        }
    }

    List<TraceElement> trace = new ArrayList<>();

    //GET THE RIGHT PENALTY
    private int gapPenalty = -4;

    private void alignment(char[] X, char[] Y){
        int m = X.length;
        int n = Y.length;
        
        int[][] M = new int[m+1][n+1];
       
        System.out.println("\n" + Arrays.toString(X) + ", and " + Arrays.toString(Y));
        Trace trace = OPT(M, X, Y, m-1, n-1);
        int value = trace.getValue();
        List<TraceElement> way = trace.getWay();
        
        System.out.println("Value: " + value);
        
        String x = "";
        String y = "";
        
        for(TraceElement element : way){
            x += element.getI();
            y += element.getJ();
        }
        
        
        
        System.out.println("["+x+"]");
        System.out.println("["+y+"]");
    }
    
    public Trace OPT(Trace[][] M, char[] X, char[] Y, int i, int j){
        if(i == -1){
            Trace trace = new Trace();
            for(int k = 0; k < j+1; k++)
                trace = trace.addAndGet(gapPenalty*(j+1), new TraceElement('-', Y[k]));
                
            return trace;
        }
        else if (j == -1){
            Trace trace = new Trace();
            for(int k = 0; k < i+1; k++)
                 trace = trace.addAndGet(gapPenalty*(i+1), new TraceElement(X[k],'-'));
            return trace;
        }
        
        int alpha = scoreM[letterIndex.get(X[i])][letterIndex.get(Y[j])]; //Mismatch cost
        Trace trace1 = OPT(M, X, Y, i-1, j-1).addAndGet(alpha, new TraceElement(X[i], Y[j]));
        Trace trace2 = OPT(M, X, Y, i-1, j).addAndGet(gapPenalty, new TraceElement(X[i], '-'));
        Trace trace3 = OPT(M, X, Y, i, j-1).addAndGet(gapPenalty, new TraceElement('-', Y[j]));
        
        Trace correctTrace = max(trace1, trace2, trace3);
        Trace[i][j] = val;
         
        //TRACE SOLUTION
        
        if(correctTrace == trace1){ 
            return trace1;
        } else if (correctTrace == trace2){
            return trace2;
        } else if (correctTrace == trace3){
            return trace3;
        }
        return null;
    }
    
    private class Trace {
        private List<TraceElement> way = new ArrayList<>();
        private int value = 0;
        
        public Trace addAndGet(int diff, TraceElement e){
            way.add(e);
            this.value += diff;
            return this;
        }
        
        public int getValue(){
            return value;
        }
        
        public List<TraceElement> getWay() {
            return way;
        }
    }
    
    private Trace max(Trace t1, Trace t2, Trace t3){
        int v1 = t1.getValue();
        int v2 = t2.getValue();
        int v3 = t3.getValue();
        
        int max = Math.max(v1, Math.max(v2, v3));
        if(max == v1)
            return t1;
        else if(max == v2)
            return t2;
        else if(max == v3)
            return t3;
            
        return null;
    }

    private void generateMatrix(){
        letterIndex = new HashMap<>();
        char[] letters = new char[]{'A','R', 'N','D','C','Q','E','G','H','I','L','K','M','F','P','S','T','W','Y','V','B','Z','X','*'};
        for (int i = 0; i < letters.length; i++) 
        {
            letterIndex.put(letters[i],i);
        }
 
        
        scoreM = new int[][]{
            //  A    R    N    D    C    Q    E    G    H    I    L    K    M    F    P    S    T    W    Y    V    B    Z    X    *
            {   4,  -1,  -2,  -2,   0,  -1,  -1,   0,  -2,  -1,  -1,  -1,  -1,  -2,  -1,   1,   0,  -3,  -2,   0,  -2,  -1,   0,  -4,},//A
            {  -1,   5,   0,  -2,  -3,   1,   0,  -2,   0,  -3,  -2,   2,  -1,  -3,  -2,  -1,  -1,  -3,  -2,  -3,  -1,   0,  -1,  -4,},//R
            {  -2,   0,   6,   1,  -3,   0,   0,   0,   1,  -3,  -3,   0,  -2,  -3,  -2,   1,   0,  -4,  -2,  -3,   3,   0,  -1,  -4,},//N
            {  -2,  -2,   1,   6,  -3,   0,   2,  -1,  -1,  -3,  -4,  -1,  -3,  -3,  -1,   0,  -1,  -4,  -3,  -3,   4,   1,  -1,  -4,},//D
            {   0,  -3,  -3,  -3,   9,  -3,  -4,  -3,  -3,  -1,  -1,  -3,  -1,  -2,  -3,  -1,  -1,  -2,  -2,  -1,  -3,  -3,  -2,  -4,},//C
            {  -1,   1,   0,   0,  -3,   5,   2,  -2,   0,  -3,  -2,   1,   0,  -3,  -1,   0,  -1,  -2,  -1,  -2,   0,   3,  -1,  -4,},//Q
            {  -1,   0,   0,   2,  -4,   2,   5,  -2,   0,  -3,  -3,   1,  -2,  -3,  -1,   0,  -1,  -3,  -2,  -2,   1,   4,  -1,  -4,},//E
            {   0,  -2,   0,  -1,  -3,  -2,  -2,   6,  -2,  -4,  -4,  -2,  -3,  -3,  -2,   0,  -2,  -2,  -3,  -3,  -1,  -2,  -1,  -4,},//G
            {  -2,   0,   1,  -1,  -3,   0,   0,  -2,   8,  -3,  -3,  -1,  -2,  -1,  -2,  -1,  -2,  -2,   2,  -3,   0,   0,  -1,  -4,},//H
            {  -1,  -3,  -3,  -3,  -1,  -3,  -3,  -4,  -3,   4,   2,  -3,   1,   0,  -3,  -2,  -1,  -3,  -1,   3,  -3,  -3,  -1,  -4,},//I
            {  -1,  -2,  -3,  -4,  -1,  -2,  -3,  -4,  -3,   2,   4,  -2,   2,   0,  -3,  -2,  -1,  -2,  -1,   1,  -4,  -3,  -1,  -4,},//L
            {  -1,   2,   0,  -1,  -3,   1,   1,  -2,  -1,  -3,  -2,   5,  -1,  -3,  -1,   0,  -1,  -3,  -2,  -2,   0,   1,  -1,  -4,},//K
            {  -1,  -1,  -2,  -3,  -1,   0,  -2,  -3,  -2,   1,   2,  -1,   5,   0,  -2,  -1,  -1,  -1,  -1,   1,  -3,  -1,  -1,  -4,},//M
            {  -2,  -3,  -3,  -3,  -2,  -3,  -3,  -3,  -1,   0,   0,  -3,   0,   6,  -4,  -2,  -2,   1,   3,  -1,  -3,  -3,  -1,  -4,},//F
            {  -1,  -2,  -2,  -1,  -3,  -1,  -1,  -2,  -2,  -3,  -3,  -1,  -2,  -4,   7,  -1,  -1,  -4,  -3,  -2,  -2,  -1,  -2,  -4,},//P
            {   1,  -1,   1,   0,  -1,   0,   0,   0,  -1,  -2,  -2,   0,  -1,  -2,  -1,   4,   1,  -3,  -2,  -2,   0,   0,   0,  -4,},//S
            {   0,  -1,   0,  -1,  -1,  -1,  -1,  -2,  -2,  -1,  -1,  -1,  -1,  -2,  -1,   1,   5,  -2,  -2,   0,  -1,  -1,   0,  -4,},//T
            {  -3,  -3,  -4,  -4,  -2,  -2,  -3,  -2,  -2,  -3,  -2,  -3,  -1,   1,  -4,  -3,  -2,  11,   2,  -3,  -4,  -3,  -2,  -4,},//W
            {  -2,  -2,  -2,  -3,  -2,  -1,  -2,  -3,   2,  -1,  -1,  -2,  -1,   3,  -3,  -2,  -2,   2,   7,  -1,  -3,  -2,  -1,  -4,},//Y
            {   0,  -3,  -3,  -3,  -1,  -2,  -2,  -3,  -3,   3,   1,  -2,   1,  -1,  -2,  -2,   0,  -3,  -1,   4,  -3,  -2,  -1,  -4,},//V
            {  -2,  -1,   3,   4,  -3,   0,   1,  -1,   0,  -3,  -4,   0,  -3,  -3,  -2,   0,  -1,  -4,  -3,  -3,   4,   1,  -1,  -4,},//B
            {  -1,   0,   0,   1,  -3,   3,   4,  -2,   0,  -3,  -3,   1,  -1,  -3,  -1,   0,  -1,  -3,  -2,  -2,   1,   4,  -1,  -4,},//Z
            {   0,  -1,  -1,  -1,  -2,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -2,   0,   0,  -2,  -1,  -1,  -1,  -1,  -1,  -4,},//X
            {  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,   1} //*
            };
    }    
}
    
    
 /*  
   A  R  N  D  C  Q  E  G  H  I  L  K  M  F  P  S  T  W  Y  V  B  Z  X  *
A  4 -1 -2 -2  0 -1 -1  0 -2 -1 -1 -1 -1 -2 -1  1  0 -3 -2  0 -2 -1  0 -4 
R -1  5  0 -2 -3  1  0 -2  0 -3 -2  2 -1 -3 -2 -1 -1 -3 -2 -3 -1  0 -1 -4 
N -2  0  6  1 -3  0  0  0  1 -3 -3  0 -2 -3 -2  1  0 -4 -2 -3  3  0 -1 -4 
D -2 -2  1  6 -3  0  2 -1 -1 -3 -4 -1 -3 -3 -1  0 -1 -4 -3 -3  4  1 -1 -4 
C  0 -3 -3 -3  9 -3 -4 -3 -3 -1 -1 -3 -1 -2 -3 -1 -1 -2 -2 -1 -3 -3 -2 -4 
Q -1  1  0  0 -3  5  2 -2  0 -3 -2  1  0 -3 -1  0 -1 -2 -1 -2  0  3 -1 -4 
E -1  0  0  2 -4  2  5 -2  0 -3 -3  1 -2 -3 -1  0 -1 -3 -2 -2  1  4 -1 -4 
G  0 -2  0 -1 -3 -2 -2  6 -2 -4 -4 -2 -3 -3 -2  0 -2 -2 -3 -3 -1 -2 -1 -4 
H -2  0  1 -1 -3  0  0 -2  8 -3 -3 -1 -2 -1 -2 -1 -2 -2  2 -3  0  0 -1 -4 
I -1 -3 -3 -3 -1 -3 -3 -4 -3  4  2 -3  1  0 -3 -2 -1 -3 -1  3 -3 -3 -1 -4 
L -1 -2 -3 -4 -1 -2 -3 -4 -3  2  4 -2  2  0 -3 -2 -1 -2 -1  1 -4 -3 -1 -4 
K -1  2  0 -1 -3  1  1 -2 -1 -3 -2  5 -1 -3 -1  0 -1 -3 -2 -2  0  1 -1 -4 
M -1 -1 -2 -3 -1  0 -2 -3 -2  1  2 -1  5  0 -2 -1 -1 -1 -1  1 -3 -1 -1 -4 
F -2 -3 -3 -3 -2 -3 -3 -3 -1  0  0 -3  0  6 -4 -2 -2  1  3 -1 -3 -3 -1 -4 
P -1 -2 -2 -1 -3 -1 -1 -2 -2 -3 -3 -1 -2 -4  7 -1 -1 -4 -3 -2 -2 -1 -2 -4 
S  1 -1  1  0 -1  0  0  0 -1 -2 -2  0 -1 -2 -1  4  1 -3 -2 -2  0  0  0 -4 
T  0 -1  0 -1 -1 -1 -1 -2 -2 -1 -1 -1 -1 -2 -1  1  5 -2 -2  0 -1 -1  0 -4 
W -3 -3 -4 -4 -2 -2 -3 -2 -2 -3 -2 -3 -1  1 -4 -3 -2 11  2 -3 -4 -3 -2 -4 
Y -2 -2 -2 -3 -2 -1 -2 -3  2 -1 -1 -2 -1  3 -3 -2 -2  2  7 -1 -3 -2 -1 -4 
V  0 -3 -3 -3 -1 -2 -2 -3 -3  3  1 -2  1 -1 -2 -2  0 -3 -1  4 -3 -2 -1 -4 
B -2 -1  3  4 -3  0  1 -1  0 -3 -4  0 -3 -3 -2  0 -1 -4 -3 -3  4  1 -1 -4 
Z -1  0  0  1 -3  3  4 -2  0 -3 -3  1 -1 -3 -1  0 -1 -3 -2 -2  1  4 -1 -4 
X  0 -1 -1 -1 -2 -1 -1 -1 -1 -1 -1 -1 -1 -1 -2  0  0 -2 -1 -1 -1 -1 -1 -4 
* -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4  1 
*/