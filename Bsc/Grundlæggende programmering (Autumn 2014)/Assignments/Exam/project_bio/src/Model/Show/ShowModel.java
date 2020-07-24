package Model.Show;

import Controller.DatabaseController;
import Model.DatabaseModel.DatabaseConnector;
import Model.DatabaseModel.DatabaseRetriever;
import javafx.collections.ObservableList;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Timestamp;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.util.ArrayList;

/**
 * ShowModel class will handle every show related tasks.
 * The class can retrieve show details within the database.
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class ShowModel {

    //Constructor

    public void showModel()
    {

    }

    //Methods

    /**
     * Inset all relevant show info from database (MovieName, StartTime, Theater) into given ObservableList.
     * @param list
     */
    public void getShowData(ObservableList<ShowTableRow> list)
    {

    //Load show details : MovieName, StartTime, Theater
    ArrayList<ShowObject> sqlData = getShows();

    //Insert all shows MovieName, StartTime and Theater into given ObservableList (Used for reflections)
        for (ShowObject aSqlData : sqlData) //foreach
        {
            String movieName = aSqlData.getMovie();
            String theaterName = aSqlData.getTheaterName();
            LocalDateTime startTime = aSqlData.getStartTime().toInstant().atZone(ZoneId.systemDefault()).toLocalDateTime();

            list.add(new ShowTableRow(movieName, startTime, theaterName));

        }

    }

    /**
     * Calendar- Refresh show list with current shows for picked day.
     * @param list
     * @param timestamp
     */
    public void getShowData(ObservableList<ShowTableRow> list,Timestamp timestamp)
    {

        //Load show details : MovieName, StartTime, Theater
        ArrayList<ShowObject> sqlData = getShows(timestamp);

        //Insert all shows MovieName, StartTime and Theater into given ObservableList (Used for reflections)
        for (ShowObject aSqlData : sqlData) //foreach
        {
            String movieName = aSqlData.getMovie();
            String theaterName = aSqlData.getTheaterName();
            LocalDateTime startTime = aSqlData.getStartTime().toInstant().atZone(ZoneId.systemDefault()).toLocalDateTime();

            list.add(new ShowTableRow(movieName, startTime, theaterName));

        }

    }

    /**
     * Returns the theater id of a chosen theater by using the theater_name.
     * @param theater_name
     * @return
     */
    public int getTheaterID(String theater_name)
    {
        int theaterID = 0;

        try
        {
            DatabaseRetriever databaseRetriever = new DatabaseRetriever();

            String query = "SELECT theaterID " +
                    "FROM theaters " +
                    "WHERE theaterName = '" + theater_name + "'";

            String DBcolumnName = "theaterID";

            theaterID = Integer.parseInt(databaseRetriever.retrieveEntry(query, DBcolumnName)); // Parses string input containing int (showID) into integer value because DBRetriever requires a string input


            databaseRetriever.closeConnection();
        } catch (SQLException e)
        {
            e.printStackTrace();
        }

        return theaterID;

    }


    /**
     * Returns the id of a show with the specified start time and theater.
     * @param show_start
     * @param theater_id
     * @return
     */
    public int getShowID(Timestamp show_start, int theater_id)
    {
        int show_id = 0;

        try
        {
        DatabaseRetriever databaseRetriever= new DatabaseRetriever();

        String query = "SELECT showID " +
                        "FROM shows " +
                        "WHERE showStart ='"+show_start+"' " +
                        "AND theater_id ='"+theater_id+"'";

        String DBcolumnName = "showID";

        show_id = Integer.parseInt(databaseRetriever.retrieveEntry(query, DBcolumnName)); // Parses string input containing int (showID) into integer value because DBRetriever requires a string input


            databaseRetriever.closeConnection();


        } catch (SQLException e)
        {
            e.printStackTrace();
        }

        return show_id;

    }


    /**
     * Fetch all current show details: MovieName, MovieStartTime, TheaterHall as objects in an arrayList.
     * Each ShowObject contains a MovieName, MovieStartTime and TheaterHall.
     * @return
     */
    private ArrayList<ShowObject> getShows()
    {
        DatabaseController databaseController = new DatabaseController();
        ArrayList<ShowObject> list = new ArrayList<>();

        try
        {//--------- Open database connection and inject query ---------
            String query =  "SELECT  movieTitle, theaterName, showStart " +
                    "FROM shows " +
                    "INNER JOIN theaters " +
                    "ON theaterID= theater_id " +
                    "INNER JOIN movieList " +
                    "ON movie_id = movieID " +
                    "WHERE showStart>=NOW()";

            ResultSet rs = databaseController.getResultSet(query);

            String  movieName;
            String  theaterName;
            Timestamp startTime;

            while (rs.next())
            {
                movieName   = rs.getString("movieTitle");
                theaterName = rs.getString("theaterName");
                startTime   = rs.getTimestamp("showStart");

                list.add(new ShowObject(movieName,startTime,theaterName));
            }
        }
        catch(SQLException e){e.printStackTrace();}
        //--------- SQL data acquired and stored as ShowObject in ArrayList list

        return  list;
    }

    /**
     * Used to retrieve shows which is playing at a specific time related to the given Timestamp and the refreshes showList.
     * @param timestamp
     * @return
     */
    private ArrayList<ShowObject> getShows(Timestamp timestamp)
    {
        DatabaseConnector databaseConnector = new DatabaseConnector();
        DatabaseController databaseController = new DatabaseController();
        ArrayList<ShowObject> list = new ArrayList<>();

        try
        {//--------- Open database connection and inject query ---------
            String query =  "SELECT  movieTitle, theaterName, showStart " +
                    "FROM shows " +
                    "INNER JOIN theaters " +
                    "ON theaterID= theater_id " +
                    "INNER JOIN movieList " +
                    "ON movie_id = movieID " +
                    "WHERE showStart >'"+timestamp+"'";
            System.out.println(query);

            ResultSet rs = databaseController.getResultSet(query);

            String  movieName;
            String  theaterName;
            Timestamp startTime;

            while (rs.next())
            {
                movieName   = rs.getString("movieTitle");
                theaterName = rs.getString("theaterName");
                startTime   = rs.getTimestamp("showStart");

                list.add(new ShowObject(movieName,startTime,theaterName));
            }
            databaseConnector.closeConnection();
        }
        catch(SQLException e){e.printStackTrace();}
        //--------- SQL data acquired and stored as ShowObject in ArrayList list

        return  list;
    }

    /**
     * Creates seats in the database for given show ID and theater ID.
     * @param show_id
     * @param theater_id
     * @return
     */
    public boolean createShowSeats(int show_id, int theater_id)
    {
        DatabaseController databaseController = new DatabaseController();

        int theaterRows=0; // Not initialized yet
        int seatsInRow=0;  // Not initialized yet
        String query;

        //Is show already created

        //Get Amount of rows and seats in each row
        query = "SELECT theaterRows " +
                "FROM theaters " +
                "WHERE theaterID ="+theater_id;

        theaterRows = Integer.parseInt(databaseController.retrieveEntry(query,"theaterRows"));

        query = "SELECT theaterSeatsInRow " +
                "FROM theaters " +
                "WHERE theaterID ="+theater_id;

        seatsInRow = Integer.parseInt(databaseController.retrieveEntry(query, "theaterSeatsInRow"));

        //Injecting seats into database
        query = "INSERT INTO seats " +
                "(seatNumber, rowNumber, theater_id, show_id) " +
                "VALUES ";
        StringBuilder stringbuilder = new StringBuilder(query);

        for(int row = 1; row<=theaterRows; row++)
            for(int seat= 1; seat<=seatsInRow; seat++)
            {

                stringbuilder.append("("+seat+", "+ row+", "+theater_id+", "+show_id+")");
                if(row != theaterRows || seat != seatsInRow)
                {
                    stringbuilder.append(", ");
                }
            }


        query = stringbuilder.toString();
        System.out.println(query);

        databaseController.injectQuery(query); //Injecting query.

        return true;
    }

}

