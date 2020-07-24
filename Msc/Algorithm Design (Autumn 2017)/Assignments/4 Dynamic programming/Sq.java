import java.util.*;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.lang.Math;


public class Sq {
    
    private Map<Character,Integer> letterIndex;
    private int[][] alignmentCostsM;
    private List<Subject> subjects = new ArrayList<>();
    private int gapPenalty = -4; 
    private final int diagonalDirection = 0; 
    private final int horizontalDirection = -1;
    private final int verticalDirection = 1;
    
    // Setup matrix with alignment cost 
    public Sq(){
        generateLetterPositions();
    }
        
    // Setup alignment cost, parse input and align species 
    public static void main (String[] args)
    {
        Sq sq = new Sq();
        sq.parseInput();
        sq.alignSubjects();
    }
    
    // Parse species and their sequences from data 
    public void parseInput() {
      Scanner scanner = new Scanner(System.in);
      Pattern namePattern = Pattern.compile(">([\\w-]+)+");
      Pattern sequencePattern = Pattern.compile("([A-Z]+)");
      String line;
      String name = "";
      String sequence = "";
      
       while(scanner.hasNext()) {
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
    }
    
    // Repeatedly finds and prints alignment between between all species 
    private void alignSubjects(){
        for(int i = 0; i<subjects.size();i++){
            for (int j = i+1; j<subjects.size();j++){
                Subject subjectOne = subjects.get(i);
                Subject subjectTwo = subjects.get(j);
                System.out.print(subjectOne.getName() + "--" + subjectTwo.getName());
                alignment(subjectOne.getSequence(), subjectTwo.getSequence());
            }
        }
    }

    // Finds alignment between two species sequences (X and Y) using the OPT method 
    // Finds traceback of optimal alignment by keeping track of traces in a stack
    private void alignment(char[] X, char[] Y){
        int m = X.length;
        int n = Y.length;
        
        int[][] M = new int[m+1][n+1]; // keeps track of alignment costs
        int[][] T = new int[m+1][n+1]; // Keeps track of traces used to find optimal alignment
        
        //Find optimal alignment
        int finalAlignmentCost = OPT(M, T, X, Y);
        System.out.println(": " + finalAlignmentCost);
        
        //Trace alignment with stack to fix reverse order of bottom-up alignment
        Stack<Trace> trace = trace(new Stack<Trace>(),T,X,Y,m,n);
        
        String x = "";
        String y = "";
        
        while (!trace.empty()){
            Trace t = trace.pop();
            x += t.getI();
            y += t.getJ();
        } 
        System.out.println(x);
        System.out.println(y);
    }
    

    // OPT finds the optimal alignment between to species sequences using a bottom-up approach
    // Dynamic programming is used via array M to reuse results in alignment cost calculations 
    private int OPT(int[][] M, int[][] T, char[] X, char[] Y){
        for(int i = 0 ; i <= X.length; i++)
        {
            M[i][0] = i*gapPenalty;
            T[i][0] = 1;
        }
        for(int j = 0 ; j <= Y.length; j++)
        {
            M[0][j] = j*gapPenalty;
            T[0][j] = -1;
        }


        for (int i = 1 ; i <= X.length ; i++){
            for (int j = 1; j <= Y.length ; j++){
                int alpha = alignmentCostsM[letterIndex.get(X[i-1])][letterIndex.get(Y[j-1])]; //Mismatch cost

                // Dynamic programming reuses values in array M 
                int diagonalCost = alpha + M[i-1][j-1];      // M[i-1][j-1] means move diagonally in alignment cost array 
                int horizontalCost = gapPenalty + M[i][j-1]; // M[i][j-1] means move on x axis in alignment cost array 
                int verticalCost = gapPenalty + M[i-1][j];   // M[i-1][j] means move on y axis in alignment cost array
                
                M[i][j] = max(diagonalCost,horizontalCost,verticalCost);
                
                if(M[i][j] == diagonalCost)
                {
                    T[i][j] = diagonalDirection; // 0 
                }
                else if(M[i][j] == horizontalCost)
                {
                    T[i][j] = horizontalDirection; // -1  
                }
                else
                {            
                    T[i][j] = verticalDirection; // 1 
                }
            }
        }
        return M[X.length][Y.length]; //optimal alignment cost for given two chars
        
    }
    
    // Finds trace of alignment and set gap if horizontal/vertical or char if diagonal based on direction   
    private Stack<Trace> trace(Stack<Trace> traceList, int[][] T, char[] X, char[] Y, int i, int j){
        char cX = ' ';
        char cY = ' ';
        int direction = T[i][j];
        
        switch(direction) 
        {
            case diagonalDirection : // 0 
                cX = X[--i];
                cY = Y[--j];
                break;
            case verticalDirection : // 1 
                cX = X[--i];
                cY = '-'; 
                break;
            case horizontalDirection : // -1 
                cX = '-';
                cY = Y[--j];
                break;
        }
        
        traceList.push(new Trace(cX, cY)); // Add to trace stack
        if(i==0 && j==0)
        {
            return traceList;
        }
        else {
            return trace(traceList, T, X, Y, i,j);
        }
    }

    // Used to find max alignment cost of the 3 subproblem cases  (diagonal, horizontal and vertical)
    private int max(int v1, int v2, int v3) {
        return Math.max(v1, Math.max(v2,v3));
    }
    
    // Trace represents a trace between characters in two species sequences 
    private class Trace {
        private char i;
        private char j;
        
        public Trace(char i, char j){
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

    // Generates a map that maps each char to their corresponding index in the alignmentCostsM array.
    // Used as index lookup for a given char.
    private void generateLetterPositions() {
        letterIndex = new HashMap<>();
        char[] letters = new char[]{'A','R', 'N','D','C','Q','E','G','H','I','L','K','M','F','P','S','T','W','Y','V','B','Z','X','*'};
        for (int i = 0; i < letters.length; i++) 
        {
            letterIndex.put(letters[i],i);  //('A',0) 
        }
    
    // Array containing alignment costs   
    alignmentCostsM = new int[][]{
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
    