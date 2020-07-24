package UnitTests;

import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;
import junit.framework.TestCase;
import org.junit.Test;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;

public class ShowTableRowTest extends TestCase {

    @Test
    public void testHallProperty() throws Exception
    {
        String hall = "Hall 1";
        StringProperty hallProperty = new SimpleStringProperty();
        hallProperty.set("Hall 1"); // Wraps a string
        assertEquals("Hall 1", hallProperty.get());
    }

    @Test
    public void testShowProperty() throws Exception
    {
        String show = "Movie 1";
        StringProperty showProperty = new SimpleStringProperty();
        showProperty.set("Movie 1");
        assertEquals("Movie 1", showProperty.get());
    }

}