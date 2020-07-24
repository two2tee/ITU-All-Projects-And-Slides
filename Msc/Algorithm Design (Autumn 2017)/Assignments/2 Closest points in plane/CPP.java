import java.util.List;
import java.util.ArrayList;
import java.util.Scanner;
import java.util.Comparator;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.Collections;
import java.util.*;
import java.lang.Math.*;

public class CPP {
 private final String number = "[-+]?\\d*.?\\d+(?:[eE][-+]?\\d+)?";
 private final String inRegex = "(\\S+)\\s+("+number+")\\s+("+number+")";
    
 private List<Point> parse(){
     String simplePairData = "romeo 0 0 \njuliet 1 0"; 
     
     Scanner scanner = new Scanner(System.in);
     Pattern inPattern = Pattern.compile(inRegex);
     ArrayList<Point> points = new ArrayList<>();
     String line;
     while(scanner.hasNext()){
        line = scanner.nextLine();
        Matcher numMatcher = inPattern.matcher(line);
        
        // TODO: if it is in -in file
        if (numMatcher.find()) {
            String name = "";
            name = numMatcher.group(1);
            double x = Double.parseDouble(numMatcher.group(2));
            double y = Double.parseDouble(numMatcher.group(3));
            Point point = new Point(name, x, y);
            points.add(point);
          // Parse number into list TODO 
        }
     }
    //TODO: parse if it is an -tsp file
    
     return points;
 }
 
 
 private double distance(Point p1, Point p2){
     double x1 = p1.getX();
     double x2 = p2.getX();
     double y1 = p1.getY();
     double y2 = p2.getY();
     return Math.sqrt((x2-x1)*(x2-x1)+(y2-y1)*(y2-y1));
 }
 
 private Point[] basecase(List<Point> points){
     double dMin = Double.MAX_VALUE;
     Point[] pMin = null;
     for(int i = 0; i<points.size();i++){
        Point pi =  points.get(i);
        for(int j = i+1; j<points.size();j++){
            Point pj =  points.get(j);
            double dist = distance(pi,pj);
            if(dist < dMin)
            {
                dMin = dist;
                pMin = new Point[]{pi,pj};
            }
         }
     }
     return pMin;
 }
 
 public Point[] closestPairRec(List<Point> pointsX, List<Point> pointsY){ 
    //find closest pair by measuring all pairwise distances
    //Brute force
    if(pointsX.size() <= 3)
       return basecase(pointsX);

     //Construct Qx, Qy, Rx, Ry (O(n) time)
     int splitIndex = pointsX.size()/2;
     
     //Split on x
     List<Point> Qx = pointsX.subList(0, splitIndex);
     List<Point> Rx = pointsX.subList(splitIndex, pointsX.size());
     
     //Split on y
     List<Point> Qy = new ArrayList<Point>();
     List<Point> Ry = new ArrayList<Point>();
     
     for(int i = 0; i < pointsY.size(); i++){
         Point point = pointsY.get(i);
         double x = pointsY.get(i).getX();
         if(x < pointsX.get(splitIndex).getX()){
             Qy.add(point);
         } else {
             Ry.add(point);
         }
     }
     
     //Finding closest pair recursively on left and right 
     Point[] pointsQ = closestPairRec(Qx, Qy);
     Point[] pointsR = closestPairRec(Rx, Ry);
     
     Point[] closestPair;
     double dMin;
     
     //Calc minimum distance on returned pairs of left and right
     double distQ = distance(pointsQ[0], pointsQ[1]);
     double distR = distance(pointsR[0], pointsR[1]); 
     if(distQ < distR){
        dMin = distQ;
        closestPair = new Point[] {pointsQ[0], pointsQ[1]};
     } else {
        dMin = distR;
        closestPair = new Point[] {pointsR[0], pointsR[1]};
     }
     
     //maximum x-coordinate of a point in set Q (The middle line)
     double xStar = Qx.get(splitIndex-1).getX(); 
     
     ///Find points that are within distance between dmin and xstar --- Construct Sy (O(n) time) (Traverse on y axis)
     ArrayList<Point> Sy = new ArrayList<Point>();

     for(int i = 0; i < pointsY.size(); i++){
         if(Math.abs(pointsY.get(i).getX()-xStar) < dMin)
            Sy.add(pointsY.get(i));
     }
     
    /*
        For each point s ∈ Sy, compute distance from s
        to each of next 15 points in Sy 
        Let s, s' be pair achieving minimum of these distances
        (O(n) time)
    */
    
    for(int i = 0; i < Sy.size(); i++)
    {
        int interval = Math.min(i+15, Sy.size()); // Maximum number of points part of search (at most 15 points)
        
        for(int j = i+1; j < interval && Sy.get(j).getY()-Sy.get(i).getY() < dMin; j++)
        {
            double dist = distance(Sy.get(i),Sy.get(j));
            if(dist < dMin)
            {
                dMin = dist;
                closestPair = new Point[] {Sy.get(i), Sy.get(j)};
            }
        }
    }
    
     //Omax(15*n) for hver n laver vi 15 beregninger hvor n er antallet af punkter i Sy
     return closestPair;
 }

 public void closestPair(List<Point> points){
     List<Point> pointsByX = points;
     Collections.sort(pointsByX, Point.getByXComparator());
     List<Point> pointsByY = new ArrayList<>(points);
     Collections.sort(pointsByY, Point.getByYComparator());
     Point[] p = closestPairRec(pointsByX, pointsByY);
    System.out.println(" "+String.valueOf(distance(p[0],p[1])));   
    
 }

 public static void main(String[] args) {
     CPP cpp = new CPP();
     List<Point> points = cpp.parse();
     if(points.isEmpty())
     {
        System.out.println("Parsed point lists were empty.");
        return;
     }
     System.out.print(points.size());
     cpp.closestPair(points);
  }
 }
 