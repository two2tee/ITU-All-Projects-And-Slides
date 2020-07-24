package Model.Reservation;

/**
 * ReservationObject will create an object that represents a reservation.
 * The object of this class will be created within Reservation class and is stored in an ArrayList.
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class ReservationObject {

    //-------------Fields----- -------------
    private final String reservation_id;
    private final String customer_name;
    private final String customer_phone;
    private final String reserved_seats;

    /**
     * A reservation object will be created by having the following input as parameters
     * @param reservation_id  - ID of reservation
     * @param customer_name - name of customer
     * @param customer_phone - phone number of customer
     * @param reserved_seats - Seats reserved by customer
     */
    //-------------Constructor -------------
    public ReservationObject(String reservation_id, String customer_name, String customer_phone, String reserved_seats)
    {
        this.reservation_id = reservation_id;
        this.customer_name = customer_name;
        this.customer_phone = customer_phone;
        this.reserved_seats = reserved_seats;
    }


    //-------------get methods -------------


    /**
     * @return reservation ID
     */
    public String getReservation_id() {
        return reservation_id;
    }

    /**
     * @return return name of customer
     */
    public String getCustomer_name() {
        return customer_name;
    }

    /**
     * @return - phone number of customer
     */
    public String getCustomer_phone() {
        return customer_phone;
    }

    /**
     * @return - seats reserved by customer
     */
    public String getReserved_seats() {
        return reserved_seats;
    }
}
