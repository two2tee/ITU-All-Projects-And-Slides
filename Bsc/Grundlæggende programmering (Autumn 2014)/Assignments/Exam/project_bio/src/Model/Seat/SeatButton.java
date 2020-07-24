package Model.Seat;
import javafx.scene.control.ToggleButton;

/**
 * This class holds the input on whether a seat is taken, marked (for reservation) or free within the gui.
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class SeatButton extends ToggleButton{

    private int seat_id;


    public SeatButton(int seat_id)
    {
        super();
        this.seat_id = seat_id;
    }

    /**
     * @return selected seat id.
     */
    public int getSeat_id()
    {
        return seat_id;
    }




}


