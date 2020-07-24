package UnitTests;

import Model.Seat.SeatObject;
import junit.framework.TestCase;
import org.junit.Before;
import org.junit.Test;

public class SeatObjectTest extends TestCase {

    //Fields
    private SeatObject seatObject; //to save a local seat object

    private int seat_id = 1;
    private boolean istAvailable = false;
    private int seatNumber = 1;
    private int rowNumber = 1;
    private int theaterId = 1;
    private int showId = 1;
    private int rowsInTheater = 1;
    private int seatsInEachRow = 1;


    @Before
    public void setUp() //Initialise
    {
        seatObject = new SeatObject(seat_id,istAvailable,seatNumber,rowNumber,theaterId,showId,rowsInTheater,seatsInEachRow);
    }

    @Test
    public void testGetSeatsInEachRow() throws Exception
    {
       assertEquals(seatsInEachRow, seatObject.getSeatsInEachRow());
    }

    @Test
    public void testGetRowsInTheater() throws Exception
    {
       assertEquals(rowsInTheater,seatObject.getRowsInTheater());
    }

    @Test
    public void testGetShow_id() throws Exception
    {
        assertEquals(showId,seatObject.getShow_id());
    }

    @Test
    public void testGetTheater_id() throws Exception
    {
        assertEquals(theaterId,seatObject.getTheater_id());
    }

    @Test
    public void testGetRowNumber() throws Exception
    {
        assertEquals(rowNumber,seatObject.getRowNumber());
    }

    @Test
    public void testGetSeatNumber() throws Exception
    {
        assertEquals(seatNumber,seatObject.getSeatNumber());
    }

    @Test
    public void testGetSeat_id() throws Exception
    {
        assertEquals(seat_id,seatObject.getSeat_id());
    }

    @Test
    public void testGetIsAvailable() throws Exception
    {
        assertFalse(seatObject.getIsAvailable());
    }
}