package Model.DatabaseModel;

import java.sql.SQLException;
import java.util.ArrayList;

/**
 * DatabaseInjector will handle every injection queries. It will not retrieve anything from the database.
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class DatabaseInjector extends DatabaseConnector {

    public DatabaseInjector() throws SQLException
    {

    }

    /**
     * injectQuery will inject a single query command into our database.
     * @param queryCommand - A single query command string
     * @throws SQLException - MANGLER FORKLARING
     */
    public void injectQuery(String queryCommand) throws SQLException
    {
        initConnection();
            statement = connection.createStatement();
            statement.execute(queryCommand);
        closeConnection();
    }


    public void injectMany(ArrayList<String> queryList)
    {

        try
        {
            initConnection();
            connection.setAutoCommit(false); // Doesnt send commits before list is whole/completed, else connection is opened/closed repeatedly
            statement = connection.createStatement();

            for(String query: queryList) // Adds all queries to a batch
                statement.addBatch(query); // Batch is a basket for all queries

            int [] updateCounts = statement.executeBatch();
            connection.commit(); // commits query list
            connection.setAutoCommit(true); // ends commit
            closeConnection();
        }
        catch (SQLException e) {e.printStackTrace();}

    }



}