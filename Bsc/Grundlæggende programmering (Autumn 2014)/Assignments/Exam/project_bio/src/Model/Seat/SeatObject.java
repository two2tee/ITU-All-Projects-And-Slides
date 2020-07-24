package Model.Seat;

/**
 * /**
 * SeatObject will create an object that represents a seat and its status.
 * The object of this class will be created within class and is stored in an arrayList.
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class SeatObject {

    private int seat_id;
    private Boolean isAvailable;
    private int seatNumber;
    private int rowNumber;
    private int theater_id;
    private int show_id;
    private int rowsInTheater;
    private int seatsInRow;


    public SeatObject(int seat_id, Boolean isAvailable, int seatNumber, int rowNumber, int theater_id, int show_id, int rowsInTheater, int seatsInEachRow)
    {
        this.seat_id = seat_id;
        this.isAvailable = isAvailable;
        this.seatNumber = seatNumber;
        this.rowNumber = rowNumber;
        this.theater_id = theater_id;
        this.show_id = show_id;
        this.rowsInTheater = rowsInTheater;
        this.seatsInEachRow = seatsInEachRow;

    }

    public SeatObject(int seat_id, Boolean isAvailable)
    {
        this.seat_id =seat_id;
        this.isAvailable= isAvailable;
    }

    public int getSeatsInEachRow()
    {
        return seatsInEachRow;
    }

    public int getRowsInTheater()
    {
        return rowsInTheater;
    }

    public int getShow_id()
    {
        return show_id;
    }

    public int getTheater_id()
    {
        return theater_id;
    }

    public int getRowNumber()
    {
        return rowNumber;
    }

    public int getSeatNumber()
    {
        return seatNumber;
    }

    private int seatsInEachRow;



    public int getSeat_id()
    {
        return seat_id;
    }

    public void setSeat_id(int seat_id)
    {
        this.seat_id = seat_id;
    }

    public Boolean getIsAvailable()
    {
        return isAvailable;
    }

    public void setIsAvailable(Boolean isAvailable)
    {
        this.isAvailable = isAvailable;
    }


}
