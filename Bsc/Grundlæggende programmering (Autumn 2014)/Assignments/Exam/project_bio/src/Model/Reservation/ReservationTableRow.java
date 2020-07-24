package Model.Reservation;

import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;

/**
 * This class holds the table data displayed in the reservation tab in the GUI.
 * Wrapper class for database query.
 * Is used in an arrayList storing ReservationTable objects.
 * This class makes it possible to retrieve all (database) reservation information and store it in an arrayList of
 * objects containing a single row of information each.
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */

public class ReservationTableRow {

    // Information associated with a reservation
    private StringProperty reservation_id = new SimpleStringProperty();
    private StringProperty customer_name = new SimpleStringProperty();
    private StringProperty customer_phone = new SimpleStringProperty();
    private StringProperty reserved_seats = new SimpleStringProperty();

    //Constructor
    public ReservationTableRow(String reservation_id, String customer_name, String customer_phone, String reserved_seats)
    {

        this.reservation_id.set(reservation_id);
        this.customer_name.set(customer_name);
        this.customer_phone.set(customer_phone);
        this.reserved_seats.set(reserved_seats);
    }

    // getter and setter for reservation_id
    public String getReservation_id()
    {
        return reservation_id.get();
    }

    public StringProperty getReservation_id_Property()
    {
        return reservation_id;
    }

    public void setReservation_id(String reservation_id)
    {
        this.reservation_id.set(reservation_id);
    }



    // getter and setter for customer_name
    public String customer_name()
    {
        return customer_name.get();
    }

    public StringProperty getCustomer_name_Property()
    {
        return customer_name;
    }

    public void setCustomer_name(String customer_name)
    {
        this.customer_name.set(customer_name);
    }



    // getter and setter for customer_phone
    public String customer_phone()
    {
        return customer_phone.get();
    }

    public StringProperty getCustomer_phone_Property()
    {
        return customer_phone;
    }

    public void setCustomer_phone(String customer_phone)
    {
        this.customer_phone.set(customer_phone);
    }



    // getter and setter for reserved_seats
    public String reserved_seats()
    {
        return reserved_seats.get();
    }

    public StringProperty getReserved_seats_Property()
    {
        return reserved_seats;
    }

    public void setReserved_seats(String reserved_seats)
    {
        this.reserved_seats.set(reserved_seats);
    }

}
