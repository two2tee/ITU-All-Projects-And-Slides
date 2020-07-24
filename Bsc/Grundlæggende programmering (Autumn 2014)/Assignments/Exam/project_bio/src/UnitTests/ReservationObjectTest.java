package UnitTests;

import Model.Reservation.ReservationObject;
import junit.framework.TestCase;
import org.junit.Before;
import org.junit.Test;

public class ReservationObjectTest extends TestCase {

    // To save a local Reservation Object
    private ReservationObject reservationObject;

    // Fields
    private String reservation_id = "1";
    private String customer_name = "testname";
    private String customer_phone = "123456789";
    private String reserved_seats = "1";

    @Before // Used to setup object before running any test in the class
    public void setUp() throws Exception {
        reservationObject = new ReservationObject(reservation_id, customer_name, customer_phone, reserved_seats);
    }

    @Test
    public void testGetReservation_id() throws Exception
    {
        reservation_id = reservationObject.getReservation_id();
        assertEquals("1", reservation_id);
    }
    @Test
    public void testGetCustomer_name() throws Exception
    {
        customer_name = reservationObject.getCustomer_name();
        assertEquals("testname", customer_name);
    }
    @Test
    public void testGetCustomer_phone() throws Exception
    {
        customer_phone = reservationObject.getCustomer_phone();
        assertEquals("123456789", customer_phone);
    }
    @Test
    public void testGetReserved_seats() throws Exception
    {
        reserved_seats = reservationObject.getReserved_seats();
        assertEquals("1",reserved_seats);
    }
}