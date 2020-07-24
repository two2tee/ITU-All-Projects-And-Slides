package Model;

import Controller.DatabaseController;

/**
 * CustomerModel class will handle all customer related tasks.
 * The class can create, delete and retrieve customer details within the database
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class CustomerModel {

    //Fields
    private String customerName;
    private String customerPhoneID;
    private final DatabaseController databaseController;

    //Constructor

    public CustomerModel()
    {
            databaseController = new DatabaseController();
    }


    //Methods


    /**
     * getNameByID will return a costumer's phone number based on their name.
     * @param customerPhoneID - a costumer's phone number
     * @return - a customer's name
     */
    public String getNameByID(String customerPhoneID)
    {

        String elementToRetrieve = "name";

        String queryCommand ="SELECT name FROM customers WHERE phone="+customerPhoneID;
        customerName = databaseController.retrieveEntry(queryCommand, elementToRetrieve);

        return customerName;

    }


    /**
     * getPhoneByName will return a costumer's phone number based on their name.
     * @param customerName - a costumer's name
     * @return - a customer's phone number
     */
    public String getPhoneByName(String customerName)
    {
        String elementToRetrieve = "phone";

        String queryCommand  = "SELECT phone " +
                               "FROM customers " +
                               "WHERE name = '" + customerName +"'";

        customerPhoneID = databaseController.retrieveEntry(queryCommand, elementToRetrieve);
        return customerPhoneID;

    }


    /**
     * getUniqueCustomerID will retrieve a unique ID of a customer from the database.
     * @param customerName - name of customer
     * @param customerPhone - phone number of customer
     * @return - unique ID of customer
     */
    public int getUniqueCustomerID(String customerName, String customerPhone)
    {
        String elementToRetrieve = "customer_id";

        String queryCommand  = "SELECT customer_id " +
                                "FROM customers " +
                                "WHERE name = '" + customerName +"'" +
                                "AND phone ='" + customerPhone + "'";

        int customerID = databaseController.retrieveEntryInt(queryCommand , elementToRetrieve);
        return customerID;

    }

    /**
     * newCustomer method will create a new costumer entry inside our costumers database.
     * The method will define a costumer based on their name and phone(ID)
     *
     * @param name - A costumers name as parameter - String
     * @param phone - A costumers phone number - String
     */

    public void newCustomer(String name, String phone)
    {

        String query =  "INSERT INTO customers (name, phone) " +
                "SELECT * FROM (SELECT '"+name+"', '"+phone+"') AS tmp " +
                "WHERE NOT EXISTS (" +
                "SELECT name FROM customers WHERE name = '"+name+"' AND phone = '"+phone+"' " +
                ")";


        databaseController.injectQuery(query);
    }



    /**
     * USED ONLY FOR TESTING PURPOSE
     * deleteCustomer, deletes a customer through use of customerPhoneID
     * @param customerPhoneID - Customer's phone number
     * @param customerName - Customer's name
     */
    public void deleteCustomer(String customerName, String customerPhoneID)
    {
        int customerID = getUniqueCustomerID(customerName,customerPhoneID);

                // Deletes name & phone on existing user
                databaseController.injectQuery("DELETE FROM customers " +
                        "WHERE customer_id = '" + customerID + "'");


    }
}
