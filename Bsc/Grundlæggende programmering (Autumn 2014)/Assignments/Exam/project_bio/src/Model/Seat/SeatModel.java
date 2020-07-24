package Model.Seat;

import Controller.DatabaseController;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;

/**
 * Seat class contains a single method which will create and add seatObjects that contains
 * seat details from database in an arraylist
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class SeatModel {

    /**
     * Stores all information about seats in Wrapped object.
     * Each SeatObject contains detailed information about the seat placement and relations.
     * Info is accessible through get methods.
     * @param showName - name of show
     * @param starTime  - time of start
     * @param TheaterName - theater name
     * @return ArrayList of seat objects
     */
    public ArrayList<SeatObject> getSeatDataforshow(String TheaterName, String show, java.sql.Timestamp starTime    )
    {
        DatabaseController databaseController = new DatabaseController();
        ArrayList<SeatObject> list = new ArrayList<>();
        ResultSet rs;
        try
        {
            String query =  "SELECT * "+
                    "FROM seats "  +
                    "LEFT JOIN shows " +
                    "ON seats.show_id = shows.showID " +
                    "LEFT JOIN theaters "+
                    "on shows.theater_id = theaters.theaterID "+

                    "WHERE theaterName = '"+TheaterName+"' AND showStart ='"+starTime+"' "+
                    "ORDER BY seat_id ";

            rs = databaseController.getResultSet(query);
            //For each row in the ResultSet are the element wrapped in SeatObject for later use
            // Local variables are declared outside the loop to avoid ram "overload"
            int     seat_id;
            Boolean isAvailable;
            int     seatNumber;
            int     rowNumber;
            int     theater_id;
            int     show_id;
            int     rowsInTheater;
            int     seatsInRow;

            while (rs.next())
            {
                seat_id         = rs.getInt("seat_id");
                isAvailable     = rs.getBoolean("isAvailable");
                seatNumber         = rs.getInt("seatNumber");
                rowNumber       = rs.getInt("rowNumber");
                theater_id      = rs.getInt("theater_id");
                show_id         = rs.getInt("show_id");
                rowsInTheater   = rs.getInt("theaterRows");
                seatsInRow      = rs.getInt("theaterSeatsInRow");

                list.add(new SeatObject(seat_id,isAvailable,seatNumber,rowNumber,theater_id,show_id,rowsInTheater,seatsInRow));
            }
        }
        catch (SQLException e) {
            e.printStackTrace();
        }

        return list;
    }
}
