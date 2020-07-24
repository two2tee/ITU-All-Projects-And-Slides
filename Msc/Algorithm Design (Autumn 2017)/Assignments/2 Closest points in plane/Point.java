import java.util.Comparator;


public class Point{
    private final String name;
    private final double x;
    private final double y;
    
    
    public Point(String name, double x, double y)
    {
        this.name = name;
        this.x = x;
        this.y = y;
    }
    
    public double getX()
    {
        return x;
    }
    
    public double getY()
    {
        return y;
    }
    
    public String getName()
    {
        return name;
    }
    
    
    public static Comparator<Point> getByXComparator(){
        return new CompareByX();
    }
    
    public static Comparator<Point> getByYComparator(){
        return new CompareByY();
    }
 
    private static class CompareByX implements Comparator<Point> {
     @Override
    	public int compare(Point p1, Point p2) {
    		return Double.compare(p1.getX(),p2.getX());
    	}
    }
 
    private static class CompareByY implements Comparator<Point>{
     @Override
    	public int compare(Point p1, Point p2) {
    		return Double.compare(p1.getY(),p2.getY());
    	}
    }
    	
}