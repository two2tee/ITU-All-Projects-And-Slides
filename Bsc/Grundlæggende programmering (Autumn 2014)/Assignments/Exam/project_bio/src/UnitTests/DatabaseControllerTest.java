package UnitTests;

import Controller.DatabaseController;
import junit.framework.TestCase;
import org.junit.Before;
import org.junit.Test;

import java.sql.ResultSet;
import java.util.ArrayList;

/**
 * DatabaseControllerTest will perform test regarding injection and retrieving data
 * by using the DatabaseController.
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class DatabaseControllerTest extends TestCase {

    DatabaseController controller = new DatabaseController();

    @Before
    public void setUp()
    {
        controller = new DatabaseController();
    }

    /**
     * Simple injection and retrieval test with databaseController.
     * The test will try to insert a new customer entry into the database customer table.
     * If injection was successful, the test will perform another injection to remove the entry that was added
     * by the test.
     * @throws Exception
     */
    @Test
    public void testInjectRetrievalQuery() throws Exception
    {

       //Create a new test entry in customer table
        controller.injectQuery("INSERT INTO customers (name,phone) VALUE('TestName','1234567876')");
        System.out.println("Successfully inserted test entry");

       //Retrieve test entry from customer table
        String result = controller.retrieveEntry("SELECT name " +
                                                 "FROM customers " +
                                                 "WHERE phone=1234567876",
                                                 "name"); //column name from customers table;
        assertEquals("TestName", result);
        System.out.println("Successfully retrieved test entry");

        //Delete test entry
        controller.injectQuery("DELETE FROM customers " +
                               "WHERE phone= '1234567876' " +
                               "AND name= 'TestName'");

        result = controller.retrieveEntry("SELECT name " +
                        "FROM customers " +
                        "WHERE phone=1234567876",
                        "name"); //column name from customers table;

        assertNull(result);
    }

    /**
     * Test retrieving a Result set with name entries from customer table in database.
     * The test will check whether the returned Result set is empty or not
     * @throws Exception
     */
    @Test
    public void testRetrieveResultSet()throws Exception
    {
        String query = "SELECT  name " +
                "FROM customers";

        ResultSet testRs = controller.getResultSet(query);

        while(testRs.next())
        {
            System.out.println("Retrieved result: " + testRs.getString("name"));
            assertNotNull(testRs.getString("name"));

        }
    }

    /**
     * Test to inject many queries at once.
     */
    @Test
    public void testInjectManyQueries()
    {
        //Inject new entries
        ArrayList<String> queryList = new ArrayList<>();
                queryList.add("INSERT INTO customers (name,phone) VALUE('TestName1','1234567876')");
            queryList.add("INSERT INTO customers (name,phone) VALUE('TestName2','2234567876')");
            queryList.add("INSERT INTO customers (name,phone) VALUE('TestName3','3234567876')");

        controller.injectMany(queryList);

        String result = controller.retrieveEntry("SELECT name " +
                        "FROM customers " +
                        "WHERE phone=2234567876",
                        "name"); //column name from customers table;

        assertEquals("TestName2", result);


    }

    /**
     * Remove every entries created by testInjectQueries
     */
    @Test
    public void testRemovalManyQueries()
    {
        //Remove created entries

        controller.injectQuery("DELETE FROM customers WHERE phone= '1234567876'  AND name = 'TestName1'");

        controller.injectQuery("DELETE FROM customers WHERE phone= '2234567876' AND name = 'TestName2'");

        controller.injectQuery("DELETE FROM customers WHERE phone= '3234567876'  AND name = 'TestName3'");

        //Check if entries are removed
        String result = controller.retrieveEntry("SELECT name " +
                        "FROM customers " +
                        "WHERE phone=2234567876",
                "name"); //column name from customers table;

        assertNull(result);
    }



}