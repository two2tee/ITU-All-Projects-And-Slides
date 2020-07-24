package UnitTests;

import Controller.DatabaseController;
import Controller.ReservationController;
import Model.Reservation.ReservationObject;
import junit.framework.TestCase;
import org.junit.Before;
import org.junit.FixMethodOrder;
import org.junit.Test;
import org.junit.runners.MethodSorters;

import java.util.ArrayList;

/** ReservationModelTest will  perform test on reservationController regarding
 *  - New creation of reservation
 *  - Delete reservation.
 *  - Retrieval of reservations
 *  - Retrieval of ArrayList of reservations
 *
 *  Its important to note by the fact that reservationController will not perform the actual action, but only parse
 *  inputs and methods to reservationModelTest. We are only testing whether the controller will parse these inputs
 *  correctly
 *
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
@FixMethodOrder(MethodSorters.NAME_ASCENDING)
public class ReservationControllerTest extends TestCase {

    //Fields -- Details of our test reservation
   ReservationController reservationController;
   DatabaseController databaseController;

   int testReservationId;
   int testCustomerId = 124;
   int testShowID = 4;
   int testSeatID = 967;


    @Before
    public void setUp()
    {
        databaseController = new DatabaseController();
        reservationController = new ReservationController();
    }

    /**
     * //Creating new reservation in database
     */
    @Test
    public void testA_NewReservation()
    {
            ArrayList<String> queryList = new ArrayList<>();
            reservationController = new ReservationController();
            DatabaseController databaseController = new DatabaseController();

            testReservationId = (reservationController.getNewstReservationKey()) + 1;
            reservationController.newReservationQuery(queryList, testCustomerId, testShowID, testSeatID, testReservationId);
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

        int reservation_id = reservationController.getReservationId(testCustomerId, testShowID);
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
        testReservationId = reservationController.getNewstReservationKey();
        reservationController.deleteReservation(testReservationId);


        boolean result = 0 == reservationController.getReservationId(testCustomerId, testShowID);
        assertTrue(result);
    }


    /**
     * testGetAllReservations, will test whether the method will retrieve an arrayList of ReservationObjectsModel.
     * @throws Exception
     */
    @Test
    public void testE_GetAllReservations() throws Exception
    {
        ArrayList<ReservationObject> retrievedReservations = reservationController.getAllReservations();

        boolean result = 0 < retrievedReservations.size();
        assertTrue(result);

    }



}