package UnitTests;

import Model.CustomerModel;
import junit.framework.TestCase;
import org.junit.BeforeClass;
import org.junit.Test;

/**
 * CustomerTest will will be used to test:
 * - New customer creation in the database
 * - Delete a customer from database
 * - test customer exist checking functionality
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class CustomerTest extends TestCase {

    CustomerModel customerModel;
    String name = "TestCustomer";
    String phone = "0000000000";

    @BeforeClass
    public void setUp()
    {
        customerModel = new CustomerModel();
    }

    @Test
    public void testNewCustomer() throws Exception
    {
        customerModel.newCustomer(name,phone);

        String retrievedPhone = customerModel.getPhoneByName(name);
        String retrievedName = customerModel.getNameByID(phone);

        assertEquals(name, retrievedName);
        assertEquals(phone, retrievedPhone);

    }



    @Test
    public void getUniqueCustomerID()
    {
        int customerId = customerModel.getUniqueCustomerID(name,phone);
        boolean result = customerId > 0;
        assertTrue(result);
    }


    /**
     * The method will test the delete functionality of deleteCustomer method in the customerModel class
     */
    @Test
    public void testDeleteCustomer() throws Exception
    {
        // First checks whether a new customer has been registered with correct info


        int customerId = customerModel.getUniqueCustomerID(name,phone);
        boolean result = customerId > 0;
        assertTrue(result);


        customerModel.deleteCustomer(name, phone);

        // Secondly checks whether the customer info is deleted properly in the serverDatabase
        customerId = customerModel.getUniqueCustomerID(name,phone);
        result = customerId > 0;
        assertFalse(result);
    }

}