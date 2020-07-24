package Model.Show;

import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;

import java.time.LocalDateTime;

/**
 * Wrapper class for database query.
 * Used in an arrayList storing ShowTableRow objects.
 * This class makes it possible to retrieve all (database) shows information and store it in an arrayList of
 * objects containing a single row of information each.
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */
public class ShowTableRow {

    private StringProperty show = new SimpleStringProperty();
    private ObjectProperty<LocalDateTime> time = new SimpleObjectProperty<>();
    private StringProperty hall = new SimpleStringProperty();

    public ShowTableRow(String show, LocalDateTime time, String hall)
    {
        this.show.set(show);
        this.time.set(time);
        this.hall.set(hall);
    }

    /**
     * @return hall where show is being shown.
     */
    public StringProperty hallProperty()
    {
        return hall;
    }

    /**
     * @return specific show.
     */
    public StringProperty showProperty()
    {
        return show;
    }

    /**
     * @return time of show time.
     */
    public ObjectProperty<LocalDateTime> timeProperty()
    {
        return time;
    }

}
