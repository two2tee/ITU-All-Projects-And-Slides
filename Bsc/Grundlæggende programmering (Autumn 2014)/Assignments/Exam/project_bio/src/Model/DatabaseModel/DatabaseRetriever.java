package Model.DatabaseModel;

import java.sql.*;
import java.util.ArrayList;

/**
 * DatabaseRetriever will handle every tasks relating to data retrieval from the Database.
 * The class will not handle injection queries where you modify/update the database as that goes to the
 * DatabaseInjector class.
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class DatabaseRetriever extends DatabaseConnector {

    public DatabaseRetriever() throws SQLException{}

    //Methods

     /**
     * RetrieveManyEntries method will retrieve many entries from a single SQL-query and store
     * them in a temporary list which will be returned to the caller.
     * Example "SELECT * FROM customers"
     * @param queryCommand - A query
     * @param DBcolumnName - Column name of table
     * @return - Returns an arraylist of every collected entries
     * @throws SQLException
     */
    public ArrayList RetrieveManyEntries(String queryCommand, String DBcolumnName) throws SQLException
    {
        initConnection();

        ArrayList<String> tmpReturnList = new ArrayList<>(); //Arraylist of stored Strings/entries

            statement = connection.createStatement();
            ResultSet rs = statement.executeQuery(queryCommand); // execute query

            while(rs.next())
            {
                tmpReturnList.add(rs.getString(DBcolumnName));
            }

        closeConnection();

        return tmpReturnList;
    }

    /**
     * retrieveEntry will retrieve a single element by a single query command.
     * @param queryCommand - A string that contains a single query command.
     * @param DBcolumnName - element that the client wants from the database. eg. "name"
     * @return - returns an element from the database
     * @throws SQLException
     */
    public String retrieveEntry(String queryCommand, String DBcolumnName) throws SQLException
    {
        initConnection();
            statement = connection.createStatement(); //Create Statement
            ResultSet rs = statement.executeQuery(queryCommand); // execute query
            String returnElement = null; //Local variable

            if(rs.next()) // Check if Resultset returns an element;
            {
                returnElement = rs.getString(DBcolumnName); //return a given element, such as name

                System.out.println("\nRetrieved element from DB where selected column is: " + DBcolumnName + //Print retrieved element
                        "\nRetrieved element: " + returnElement);
            }
        closeConnection();

        return returnElement;
    }

    /**
     * retrieveEntryInt will retrieve a single element of type int by a single query command.
     * @param queryCommand - sql query
     * @param DBcolumnName - column name of table in database
     * @return int entry
     * @throws SQLException
     */
    public int retrieveEntryInt(String queryCommand, String DBcolumnName) throws SQLException
    {
        initConnection();
        statement = connection.createStatement(); //Create Statement
        ResultSet rs = statement.executeQuery(queryCommand); // execute query
        int returnElement = 0; //Local variable

        if(rs.next()) // Check if Resultset returns an element;
        {
            returnElement = rs.getInt(DBcolumnName); //return a given element, such as name

            System.out.println("\nRetrieved element from DB where selected column is: " + DBcolumnName + //Print retrieved element
                    "\nRetrieved element: " + returnElement);
        }
        closeConnection();

        return returnElement;
    }

    /**
     * Polymorph SQL retriever, connection Must be closed.
     * @param query - sql query
     * @return result set
     */
        public ResultSet getResultSet(String query) {
        ResultSet rs = null;
        try
        {
            initConnection();

            statement = connection.createStatement(); //Create Statement

            rs = statement.executeQuery(query);

        }
        catch (SQLException e){e.printStackTrace();}
        return rs;
    }

}