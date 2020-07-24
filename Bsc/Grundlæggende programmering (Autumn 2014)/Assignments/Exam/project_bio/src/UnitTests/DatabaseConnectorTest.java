package UnitTests;


import Model.DatabaseModel.DatabaseInjector;
import Model.DatabaseModel.DatabaseRetriever;
import junit.framework.TestCase;
import org.junit.Test;

/**
 * DatabaseControllerTest will perform test regarding
 *  - query injection from DatabaseInjecter
 *  - Data retrieval from DatabaseRetriever
 *  - Simple connection test
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class DatabaseConnectorTest extends TestCase {

    /**
     * Test a simple connection to DB where we retrieve every names from the database
     * @throws Exception
     */

    /**
     * Direct test of Retriever class and injection class
     * The DatabaseController class is not used.
     * @throws Exception
     */
    @Test
    public void testInjectorRetrieveConnection() throws Exception
    {
        DatabaseInjector injector = new DatabaseInjector();
        DatabaseRetriever retriever = new DatabaseRetriever();

        injector.injectQuery(
                "INSERT INTO customers (name,phone) " +
                "VALUES('TestName','12345678765')");


        String result = retriever.retrieveEntry("SELECT name FROM customers WHERE phone='12345678765'","name");

        assertEquals("TestName", result);

        injector.injectQuery("DELETE FROM customers " +
                "WHERE phone= '12345678765' " +
                "AND name= 'TestName'"); //Removes entry


        //Check if thee test entry was deleted .
        result = retriever.retrieveEntry("SELECT name FROM customers WHERE phone='12345678765'", "name");
        assertNull(result);

    }


}