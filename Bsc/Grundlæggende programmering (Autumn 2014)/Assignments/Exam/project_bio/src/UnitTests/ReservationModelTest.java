package UnitTests;

import Controller.DatabaseController;
import Model.Reservation.ReservationModel;
import junit.framework.TestCase;
import org.junit.Before;
import org.junit.FixMethodOrder;
import org.junit.Test;
import org.junit.runners.MethodSorters;

import java.util.ArrayList;

/** ReservationModelTest will  perform test reservationModel regarding
*  - New creation of reservation
*  - Delete reservation.
*  - Retrieval of reservations
* Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
*/
@FixMethodOrder(MethodSorters.NAME_ASCENDING)
public class ReservationModelTest extends TestCase {

    ReservationModel reservationModel;
    DatabaseController databaseController;

    int testReservationId;
    int testCustomerId = 124;
    int testShowID = 4;
    int testSeatID = 967;

    @Before
    public void setUp()
    {
        reservationModel = new ReservationModel();
        databaseController = new DatabaseController();
    }
    /**
     * //Creating new reservation in database
     */
    @Test
    public void testA_newReservation() throws Exception
    {
        ArrayList<String> queryList = new ArrayList<>();
        testReservationId = (reservationModel.getNewstReservationKey()) + 1;
        reservationModel.createReservationQuery(queryList,testCustomerId,testShowID,testSeatID,testReservationId);

        databaseController.injectMany(queryList);
    }


    /**
     * testRetrieveReservation will try to retrieve the reservation.
     * This test will also confirm if the test reservation was  successfully created by the
     * testNewReservation.
     */
    @Test
    public void testB_RetrieveReservation()
    {

        int reservation_id = reservationModel.getReservationID(testCustomerId, testShowID);
        boolean result = reservation_id>0;
        assertTrue(result);
    }



    /**
     * testDeleteReservation will try to delete the test reservation
     * created by the testNewReservation method above.
     */
    @Test
    public void testC_DeleteReservation()
    {

        testReservationId = reservationModel.getNewstReservationKey();
        reservationModel.deleteReservation(testReservationId);

        boolean result = 0 == reservationModel.getReservationID(testCustomerId, testShowID);
        assertTrue(result);
    }

}