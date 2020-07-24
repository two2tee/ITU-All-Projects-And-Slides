package UnitTests;

import Model.Show.ShowObject;
import junit.framework.TestCase;
import org.junit.Before;

import java.util.Date;

public class ShowObjectTest extends TestCase {

    // Local object
    private ShowObject showObject;

    // Fields
    private String movieName = "testmovie";
    private Date date;
    private String theaterName = "theater 1";

    @Before // Initializes object once for use in all test methods
    public void setUp()
    {
        showObject = new ShowObject();
    }

    public void testAddMovie() throws Exception
    {
        showObject.addMovie(movieName);
        date = new Date();
    }

    public void testAddTheaterName() throws Exception
    {
        showObject.addTheaterName(theaterName);
        assertEquals("theater 1", theaterName);
    }

    public void testGetMovie() throws Exception
    {
        showObject.addMovie("testmovie");
        assertEquals("testmovie", showObject.getMovie());
    }

    public void testGetTheaterName() throws Exception
    {
        showObject.addTheaterName("theater 1");
        assertEquals("theater 1", showObject.getTheaterName());
    }
}