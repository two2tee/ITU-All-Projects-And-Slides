package Model;

import Controller.DatabaseController;
import Controller.GUIController;
import Model.Seat.SeatButton;
import Model.Seat.SeatObject;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.control.ToggleButton;
import javafx.scene.layout.GridPane;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;

/**
 * TheaterModel class will retrieve information about seats and show halls from a show within the database.
 * The retrieved information will be used to create a visualization of the show hall and its seats within the gui.
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class TheaterModel {

    DatabaseController databaseController;

    public TheaterModel()
    {
        databaseController = new DatabaseController();
    }

    /**
     * Return all seat information for a given show.
     * SeatObject contains seat_id and seat availability.
     * @param show_id - ID of specific show
     * @return ArrayList of Seat objects
     */
    private ArrayList<SeatObject> getAllSeats(int show_id)
    {
        DatabaseController databaseController = new DatabaseController();
        ArrayList<SeatObject> list = new ArrayList<>();
        try
        {
            String query =
                    "SELECT seat_id, isAvailable " +
                    "FROM seats " +
                    "WHERE show_id = " + show_id +" " +
                    "ORDER BY show_id";
            ResultSet rs = databaseController.getResultSet(query);

            int seat_id;
            boolean isAvailable;

            while(rs.next())
            {
                seat_id         = rs.getInt("seat_id");
                isAvailable     = rs.getBoolean("isAvailable");

                list.add(new SeatObject(seat_id,isAvailable));
            }
        }
        catch (SQLException e){e.printStackTrace();}

        return list;

    }


    /**
     * setSeats will create and return a gridPane that represents every seats in a show hall of a specific show.
     * Integer.parseInt method is used consistently due to the fact that it parses a string input.
     * It parses Integer (e.g. showID) into integer value because the DBRetriever requires a string input.
     */
    public GridPane setSeats(ArrayList<SeatObject> list)
    {


        int rows = list.get(0).getRowsInTheater();


        int seats = list.get(0).getSeatsInEachRow();

        final GridPane root = new GridPane();

        root.setId("TheaterSeats");
        root.setPadding(new Insets(5));
        root.setHgap(5);
        root.setVgap(5);

        root.setAlignment(Pos.CENTER);

        final ToggleButton[][] buttons = new ToggleButton[rows][seats];




        int counter = 0;
        for (int y = 0; y<rows; y++)
            for(int x = 0; x<seats; x++)
            {
                String styleClass;
                if(list.get(counter).getIsAvailable())
                {
                    styleClass = "free";
                }
                else
                {
                    styleClass = "reserved";
                }
                //Add seat_id as property to seatButton
                final SeatButton button = new SeatButton(list.get(counter).getSeat_id());
                button.setText(""+(counter+1));
                buttons[y][x] = button;
                buttons[y][x].getStyleClass().add(styleClass);
                buttons[y][x].setPrefSize(50,50);


                root.add(buttons[y][x], x, y);

                counter++;


                buttons[y][x].setOnAction(event -> {
                    GUIController guiController = new GUIController();
                    guiController.onSeatPressEvent(button);
                });
            }

        return root;
    }


}
