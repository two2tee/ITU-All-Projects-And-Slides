package Model.Reservation;

import Controller.DatabaseController;
import Model.DatabaseModel.DatabaseInjector;
import javafx.collections.ObservableList;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;

/**
 * Reservation class will handle every reservation related tasks.
 * The class can create, delete and retrieve reservation details within the database
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class ReservationModel
{
    DatabaseController databaseController;
    DatabaseInjector databaseInjector;


    public ReservationModel()
    {

        try
        {
            databaseController = new DatabaseController();
            databaseInjector = new DatabaseInjector();
        } catch (SQLException e)
        {
            e.printStackTrace();
        }
    }


    /**
     * Fetch all current reservation details: Customer details and show details as object in an ArrayList.
     * The method will then return the arrayList for further processing eg. show the reservations in the gui
     *
     * @return - An arrayList of reservation objects
     */
    public ArrayList<ReservationObject> getAllReservations()
    {

        ArrayList<ReservationObject> list = new ArrayList<>();

        try
        {//--------- inject query ---------
            String query =  "SELECT  reservation_id, name, phone, seat_id " +
                            "FROM reservations " +
                            "INNER JOIN customers " +
                            "ON  reservations.customer_id = customers.customer_id ";

            ResultSet rs = databaseController.getResultSet(query);

            //-------- Retrieving data from result set ------------
            //Local fields
            String reservationID;
            String customer_name;
            String customer_phone;
            String seat_id;

            while (rs.next())
            {
                reservationID = rs.getString("reservation_id");
                customer_name = rs.getString("name");
                customer_phone = rs.getString("phone");
                seat_id = rs.getString("seat_id");

                //Creates reservationArrayModel objects and store them in an array
                list.add(new ReservationObject(reservationID, customer_name, customer_phone, seat_id));
            }
            databaseInjector.closeConnection();

        } catch (SQLException e)
        {
            e.printStackTrace();
        }
        //--------- SQL data acquired and stored as ShowObject in ArrayList list

        //Returns the list
        return list;
    }


    /**
     * getReservationID will return a reservation ID from a customer ID and a show ID
     * @param customer_id - Specific customer
     * @param show_id - Specific show
     * @return - reservation ID
     */
    public int getReservationID(int customer_id, int show_id)
    {
        //Get reservation number
        String query = 
                "SELECT reservation_id " +
                "FROM reservations " +
                "WHERE customer_id ='" + customer_id + "'" +
                "AND show_id ='" + show_id + "'";


        int reservation_id = databaseController.retrieveEntryInt(query, "reservation_id");
        return reservation_id;
    }



    /**
     * deleteReservation will delete a specific reservation by using
     * customer ID and reservation ID is input.
     * @param reservation_id - ID of reservation
     */
    public void deleteReservation(int reservation_id)
    {
        ArrayList<String> queryList = new ArrayList<>();
        int seat_id;

            seat_id = databaseController.retrieveEntryInt(
                    "SELECT seat_id " +
                    "FROM reservations " +
                    "WHERE reservation_id = '" + reservation_id + "'", "seat_id");


            //Creating and adding query commands.
            //Delete reservation
            queryList.add("DELETE FROM reservations " +  // Deletes from reservations
                    "WHERE reservation_id = '" + reservation_id + "'");

            //Update seat table
            queryList.add("UPDATE seats " +
                    "SET isAvailable = 1, reservation_id = '0'" +
                    "WHERE seat_id = '"+ seat_id +"'");

            databaseController.injectMany(queryList);

    }

    /**
     * This method "merges" several specific queries into a complete query list that fetches all information needed in the program through one connection.
     * This is used to avoid connecting to the database repeatedly, which may down the program.
     * The method runs when a reservation is created in the GUI through an event handler.
     * @param queryList
     * @param customer_ID
     * @param show_id
     * @param seat_id
     * @param reservation_id
     */
    public void createReservationQuery(ArrayList<String> queryList, int customer_ID, int show_id, int seat_id, int reservation_id)
    {
        queryList.add("INSERT INTO reservations (customer_id, show_id, seat_id, reservation_id) " +
                "VALUES (" + customer_ID + ", " + show_id + ", " + seat_id + ", " + reservation_id + "); \n");
        queryList.add("UPDATE seats " +
                "SET isAvailable = " + 0 + ", reservation_id = " + reservation_id + " " +
                "WHERE seat_id = " + seat_id + " \n");

    }


    /**
     * Retrieves an ObservableList containing objects of type ReservationTableRow from a show with the given show_id
     * @param list
     * @param show_id
     */
    public void getReservationData(ObservableList<ReservationTableRow> list, int show_id)
    {

        //Load show details : MovieName, StartTime, Theater
        ArrayList<ReservationObject> sqlData = getShowReservations(show_id);

        //Insert all shows MovieName, StartTime and Theater into given ObservableList (Used for reflections)
        for (int i = 0; i < sqlData.size(); i++)
        {
            String reservationsId = sqlData.get(i).getReservation_id();
            String customerName = sqlData.get(i).getCustomer_name();
            String customerPhone = sqlData.get(i).getCustomer_phone();
            String seatId = sqlData.get(i).getReserved_seats();

            list.add(new ReservationTableRow(reservationsId, customerName, customerPhone, seatId));

        }

    }


    /**
     * Returns an ArrayList containing RservationObjectModel types based on a show_id which is retrieved from the GUI's
     * showTable when a specific show is clicked.
     * @param show_id
     * @return
     */
    public ArrayList<ReservationObject> getShowReservations(int show_id)
    {
        ArrayList<ReservationObject> list = new ArrayList<>();

        try
        {//--------- inject query ---------
            String query = "SELECT  reservation_id, name, phone, seat_id " +
                    "FROM reservations " +
                    "INNER JOIN customers as c " +
                    "ON  reservations.customer_id = c.customer_id " +
                    "WHERE show_id = "+show_id;

            ResultSet rs = databaseController.getResultSet(query);

            //-------- Retrieving data from result set ------------
            //Local fields
            String reservationID;
            String customer_name;
            String customer_phone;
            String seat_id;

            while (rs.next())
            {
                reservationID = rs.getString("reservation_id");
                customer_name = rs.getString("name");
                customer_phone = rs.getString("phone");
                seat_id = rs.getString("seat_id");

                //Creates reservationArrayModel objects and store them in an array
                list.add(new ReservationObject(reservationID, customer_name, customer_phone, seat_id));
            }
        } catch (SQLException e)
        {
            e.printStackTrace();
        }
        //--------- SQL data acquired and stored as ShowObject in ArrayList list

        //Returns the list
        return list;
    }

    /**
     * Get the current highest key value in reservations
     * To avoid many connections - use to increment reservation_id offline
     * @return
     */
    public int getNewstReservationKey()
    {
        int key = -1;
        try
        {
            String query = "SELECT MAX(reservation_id) FROM reservations";
            ResultSet rs = databaseController.getResultSet(query);
            while (rs.next())
            {
                key = rs.getInt("MAX(reservation_id)");
            }
        }
        catch (SQLException e) {e.printStackTrace();}

        return key;
    }


}
