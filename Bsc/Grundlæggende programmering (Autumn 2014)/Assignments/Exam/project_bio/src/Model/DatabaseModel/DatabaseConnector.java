package Model.DatabaseModel;
import java.sql.*;
/**
 * DatabaseConnector will be responsible for establishing and terminating connections to the database.
 * It won't handle queries.
 *
 * Credits go to professor Claus for the source code.
 * we are using Dennis Thinh Tan Nguyen's database
 *
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class  DatabaseConnector {

    // JDBC driver name and database URL

    private static final String JDBC_DRIVER = "com.mysql.jdbc.Driver";
    private static final String database_name = "twoteebase";
    private static final String DB_URL = "jdbc:mysql://mysql.itu.dk/" + database_name;

    //Connection fields
    protected Connection connection = null;
    protected Statement statement = null;
    protected ResultSet rs = null;

    //Database credentials
    private static final String USER = "twotee";
    private static final String PASS = "thinhtan";

    //Constructor
    public DatabaseConnector()
    {

    }

    /**
     * initConnection method will initialise a connection between the client and the database.
     */
    protected void initConnection()
    {
        int timeOut = 3; //In seconds - used to validate the connection

        System.out.println("Connecting to DB...");

        try
        {
            //Resetting Connection fields
            connection = null;
            statement = null;
            rs = null;

            //Init connection
            DriverManager.registerDriver(new com.mysql.jdbc.Driver()); //Load JDBC driver
            connection = DriverManager.getConnection(DB_URL, USER, PASS); //Establish a connection to DB

            if(connection.isValid(timeOut))  //If connection was successful print success message else print fail message
            {
                System.out.println("Connected to DB.");
            }
            else
            {
                System.out.println("Connection to DB failed");
            }

        }
        catch (Exception e)
        {
            e.printStackTrace();
        }

    }



    /**
     * closeConnection will terminate an existing connection between the client and the database.
     * @throws SQLException - if there isn't a connection, a SQL exception will be thrown and caught.
     *                      No matter what, connection will be terminated "finally block"
     */
    public void closeConnection() throws SQLException
    {
        try
        {
            if(rs != null)
            {
                rs.close();
            }
            statement.close();
            connection.close();

        }
        catch (SQLException e)
        { //Handle error for JDBC driver.
            e.printStackTrace();

        }
        finally
        {
            //finally block used to terminate connection...
            try
            {
                if (statement != null)
                    statement.close();
            }
            catch (SQLException e)
            {
                e.printStackTrace();
            } //We can't do anything
            try
            {
                if (connection != null)
                    connection.close();
            }
            catch (SQLException e)
            {
                e.printStackTrace();
            }

            if(connection.isClosed())  //If connection is terminated
            {
                System.out.println("Connection to DB terminated...");
            }
            else
            {
                System.out.println("Connection to DB could not be terminated...");
            }

        }


    }
}
