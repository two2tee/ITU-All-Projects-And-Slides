package Controller;


import Model.Reservation.ReservationObject;
import Model.Reservation.ReservationModel;
import java.util.ArrayList;

/**
 * The reservationController class will handle and parse every reservation oriented tasks to a proper model class.
 * Created by new on 26-11-2014.
 */
public class ReservationController {

    //fields
    private ReservationModel   reservationModel;
    private DatabaseController databaseController;

    //constructor
    public ReservationController()
    {
        databaseController = new DatabaseController();
        reservationModel = new ReservationModel();
    }

    //methods

    /**
     * Fetch all current reservation details: Customer details and show details as object in an ArrayList.
     * The method will then return the arrayList for further processing eg. show the reservations in the gui.
     * @return - An arrayList of reservation objects
     */
    public ArrayList<ReservationObject> getAllReservations()
    {
        return reservationModel.getAllReservations();
    }


    /**
     * deleteReservation will delete a reservation from the DB by using the phone number and name of a specific customer.
     */
    public void deleteReservation(int reservation_id)
    {
        reservationModel.deleteReservation(reservation_id); //deleting reservation
        System.out.println("reservation deleted");
    }


    public int getReservationId(int customer_id, int show_id)
    {
        return reservationModel.getReservationID(customer_id, show_id);
    }

    /**
     * Better insert name method. Returns key on insert.
     * @param name
     * @param phone
     * @return
     */
    public void createUser(String name, String phone)
    {
            String query =  "INSERT INTO customers (name, phone) " +
                            "SELECT * FROM (SELECT '"+name+"', '"+phone+"') AS tmp " +
                            "WHERE NOT EXISTS (" +
                            "SELECT name FROM customers WHERE name = '"+name+"' AND phone = '"+phone+"' " +
                            ")";

            databaseController.injectQuery(query);
    }

    // Runs when a reservation is created in the GUI

    /**
     * This method "merges" several specific queries into a complete query list that fetches all information needed in the program through one connection.
     * This is used to avoid having to connect to the database repeatedly, which may slow down the program.
     * The method runs when a reservation is created in the GUI through an event handler.
     * @param queryList
     * @param customer_ID
     * @param show_id
     * @param seat_id
     * @param reservation_id
     */
    public void newReservationQuery(ArrayList<String> queryList, int customer_ID, int show_id, int seat_id, int reservation_id)
    {
        reservationModel.createReservationQuery(queryList,customer_ID,show_id,seat_id,reservation_id);
    }

    /**
     * Get the current highest key value in reservations.
     * @return reservation ID
     */
    public int getNewstReservationKey()
    {
        return reservationModel.getNewstReservationKey();
    }

}
