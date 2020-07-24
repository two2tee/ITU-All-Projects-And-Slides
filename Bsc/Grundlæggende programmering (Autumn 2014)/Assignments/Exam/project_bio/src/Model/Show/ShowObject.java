package Model.Show;

import java.util.Date;

/**
 * ShowObject will create an object that represent a show and its specific details.
 * The object of this class will be created within class and is stored in an ArrayList.
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class ShowObject {

    private String  movieName;
    private String  theaterName;
    private Date    startTime;

    public ShowObject()
    {
        startTime = new Date();
    }

    public ShowObject(String movieName, Date startTime, String theaterName)
    {
        this.movieName = movieName;
        this.theaterName = theaterName;
        this.startTime = startTime;
    }

    //--------Add methods, The element has predetermined locations --------

    public void addMovie (String movieName)
    {
        this.movieName = movieName;
    }

    public void addDate(Date startTime)
    {
        this.startTime = startTime;
    }

    public void addTheaterName(String theaterName)
    {
        this.theaterName = theaterName;
    }

    //-------- get methods --------


    public String getMovie()
    {
        return movieName;
    }

    public Date getStartTime()
    {
        return startTime;
    }

    public String getTheaterName()
    {
        return theaterName;
    }






}
