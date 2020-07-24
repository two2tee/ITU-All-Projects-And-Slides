import java.util.*;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.lang.Math;

public class Sqr{

    private Map<Character,Integer> letterIndex;
    private int[][] scoreM;
    private List<Subject> subjects = new ArrayList<>();
    private int clusterPct = 62; // >= 62
    private double entropy = 0.6979;
    private double expected = -0.5209;

    class TraceElement{
            private char i;
            private char j;
            public TraceElement(char i, char j ){
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

    class Trace{
        private int val;
        
        
        private List<TraceElement> way;
        public Trace(int val, List<TraceElement> way)
        {
            this.way = way;
            this.val = val;
        }
        public Trace add(int newVal, char i, char j)
        {
            way.add(new TraceElement(i, j));
            return new Trace(newVal+val,way);
        }

        public int getVal(){
            return val;
        }
        public List<TraceElement> getWay(){
            return way;
        }
    }

    public Sqr(){
        generateMatrix();
    }


    public static void main(String[] args)
    {
        Sqr sq = new Sqr();
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

    //GET THE RIGHT PENALTY
    private int gapPenalty = -4;

    private void alignment(char[] X, char[] Y){
        int m = X.length;
        int n = Y.length;

        int[][] M = new int[m][n];
        System.out.println("\n" + Arrays.toString(X) + ", and " + Arrays.toString(Y));
        Trace result = OPT(M, X, Y, m-1, n-1);
        System.out.println(result.getVal());
        
        //Print trace 
        String s1 = "";
        String s2 = "";
        for (int i = 0;i<result.getWay().size() ;i++ )
        {
            s1 += result.getWay().get(i).getI();
            s2 += result.getWay().get(i).getJ();
        }
            
        System.out.println(s1+"\n"+s2);

    }

     

    public Trace OPT(int[][] M, char[] X, char[] Y, int i, int j){
        
        //Calc gap penalty 
        if(i == -1){
            List<TraceElement> way = new ArrayList<>();
            if(j >=0) //Set gaps on rest of current sequence if other sequence is longer
            {
              for (int a = 0; a < j ; a++) 
              {
                TraceElement te = new TraceElement('-',Y[a]);
                way.add(0,te);
              }
            }
            return new Trace(gapPenalty*(j +1 ),way); 
        }
        if (j == -1)
        {
            List<TraceElement> way = new ArrayList<>();
            if(i >=0) //Set gaps on rest of current sequence if other sequence is longer
            {
              for (int a = 0; a < i ; a++) 
              {
                  
                TraceElement te = new TraceElement(X[a],'-');
                way.add(te);
              }
            }
            return new Trace(gapPenalty*(i +1 ),way); 
        }

         //Trace recursively 
        int alpha = scoreM[letterIndex.get(X[i])][letterIndex.get(Y[j])]; //Mismatch cost
        Trace val1 = OPT(M, X, Y, i-1, j-1).add(alpha, X[i], Y[j]); //If top and bot mismatch
        Trace val2 = OPT(M, X, Y, i-1, j).add(gapPenalty, X[i],'-'); //If top is gap
        Trace val3 = OPT(M, X, Y, i, j-1).add(gapPenalty, '-', Y[j]); //If bot is gap
        
        return max(val1,val2,val3); //Return best value

    }

    private Trace max(Trace i1, Trace i2, Trace i3){
      if(i1.getVal()>=i2.getVal())
      {
        if(i3.getVal()>i1.getVal())
          return i3;
        else
          return i1;
      }
      else{
        if(i2.getVal()>i3.getVal())
          return i2;
        else
          return i3;
      }
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
