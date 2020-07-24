package Controller;

import Model.DatabaseModel.DatabaseInjector;
import Model.DatabaseModel.DatabaseRetriever;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;

/**
 * DatabaseController represents the DatabaseRetriever class and the DatabaseInjector class. The class will
 * handle and control every input to the controller by parsing query commands to the retriever or injector.
 *
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class DatabaseController {

    private DatabaseInjector injector;
    private DatabaseRetriever retriever;

    //Constructor
    public DatabaseController()
    {

        try
        {
            injector = new DatabaseInjector();
            retriever = new DatabaseRetriever();
        } catch (SQLException e) {e.printStackTrace();}

    }

    //Retrievers

    /**
     * getResultSet will retrieve a result set which contains various data from the database,
     * e.g. all entries in a table or all entries in a joined table.
     * @param query - Query command
     * @return - resultSet with data that can be iterated
     */
    public ResultSet getResultSet(String query)
    {
        ResultSet rs;
        if (retriever.getResultSet(query) != null)
        {
            successMessage("retrieved entries");
            rs = retriever.getResultSet(query); //activates the getResultSet() method in the DatabaseRetriever
        }
        else
        {
            System.out.println("Failed to retrieve ResultSet");
            rs = null;
        }
        return rs;

    }

    /**
     * retrieveEntry will retrieve a single entry with a single query command.
     *
     * @param queryCommand - A string that contains a single query command
     * @param DBcolumnName - Column name of table
     * @return - returns an entry
     */
    public String retrieveEntry(String queryCommand, String DBcolumnName)
    {
        String retrievedEntry = null;

        try
        {
            retrievedEntry = retriever.retrieveEntry(queryCommand, DBcolumnName); //Retrieve a single entry with retrieveEntry() from DatabaseRetriever
        }
        catch (SQLException e) {e.printStackTrace();}

        successMessage("retrieved entry");
        return retrievedEntry;
    }


    /**
     * retrieveEntryInt will retrieve a single element of type int by a single query command.
     * @param queryCommand - SQL query
     * @param DBcolumnName - column name of table in the database
     * @return int entry
     */
    public int retrieveEntryInt(String queryCommand, String DBcolumnName)
    {
        int retrievedEntry = 0;

        try
        {
            retrievedEntry = retriever.retrieveEntryInt(queryCommand, DBcolumnName); //Retrieve a single entry as an int
        }
        catch (SQLException e) {e.printStackTrace();}

        successMessage("retrieved entry");
        return retrievedEntry;
    }


    /**
     * RetrieveManyEntries method will retrieve several entries from a single SQL-query and store
     * them in a temporary list, which will be returned to the caller.
     * Example "SELECT * FROM customers"
     * @param queryCommand - A query
     * @param DBcolumn - Column name of table
     * @return - Returns an ArrayList of every collected entries
     */
    public ArrayList retrieveEntriesSingleQuery(String queryCommand, String DBcolumn)
    {
        ArrayList retrievedEntries = null;
        try
        {
            retrievedEntries = retriever.RetrieveManyEntries(queryCommand, DBcolumn);
        }
        catch (SQLException e) {e.printStackTrace();}

        successMessage("retrieved entries");
        return retrievedEntries;

    }


    /**
     * retrieveEntriesInt will retrieve several entries from a single SQL-query and wrap them as Integer objects in a temporary list, which will be returned to the caller.
     * This is used due to compatibility issues in java, the Integer objects can be unwrapped and used as integers.
     * @param queryCommand - SQL query
     * @param DBcolumn - column name of table in database
     * @return ArrayList<Integer>
     */
    public ArrayList retrieveEntriesInt(String queryCommand, String DBcolumn)
    {
        ArrayList<Integer> retrievedEntries = null;
        try
        {
            retrievedEntries = retriever.RetrieveManyEntries(queryCommand, DBcolumn);
        } catch (SQLException e) {
            e.printStackTrace();
        }
        successMessage("retrieved entries");
        return retrievedEntries;

    }


    // injectors

    /**
     * injectQuery will inject a single query command into our database.
     *
     * @param queryCommand - A single query command string
     */
    public void injectQuery(String queryCommand)
    {
        try
        {
            injector.injectQuery(queryCommand);
        }
        catch (SQLException e) {e.printStackTrace();}

            successMessage("injected query");
    }


    /**
     * successMessage method will just print a message to the terminal,
     * that indicates a method successfully completed a method.
     * @param s - String message
     */
    private void successMessage(String s)
    {
        System.out.println("Successfully " + s);
    }

    public void injectMany(ArrayList<String> queryList)
    {
            injector.injectMany(queryList);

    }


}
