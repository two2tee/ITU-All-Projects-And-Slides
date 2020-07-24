package Controller;

import Model.*;
import Model.Reservation.ReservationModel;
import Model.Reservation.ReservationTableRow;
import Model.Seat.SeatButton;
import Model.Seat.SeatModel;
import Model.Seat.SeatObject;
import Model.Show.ShowModel;
import Model.Show.ShowTableRow;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.fxml.FXML;
import javafx.scene.Node;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.layout.BorderPane;
import javafx.scene.layout.GridPane;
import javafx.scene.layout.VBox;
import javafx.stage.Stage;

import java.sql.Timestamp;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.ArrayList;


/**
 * GUI controller will handle and parse every inputs from the GUI to appropriate model classes.
 * Created by Dennis Thinh Tan Nguyen - William Diedrichsen Marstrand - Thor Valentin Olesen
 */

public class GUIController {

        //Fields
        DatabaseController databaseController;

        ArrayList<SeatObject> seatInfoList;

        private final ReservationModel reservationModel;

        //Show table Fields
        @FXML
        private TableView<ShowTableRow> bioTable;

        @FXML
        private TableColumn<ShowTableRow, String> showColumn;

        @FXML
        private TableColumn<ShowTableRow, LocalDateTime> timeColumn;

        @FXML
        private TableColumn<ShowTableRow, String> hallColumn;


        //Reservation Tab table fields
        @FXML
        private TableView<ReservationTableRow> reservationTableView;

        @FXML
        private TableColumn<ReservationTableRow, String> reservationIdColumn;

        @FXML
        private TableColumn<ReservationTableRow, String> customerNameColumn;

        @FXML
        private TableColumn<ReservationTableRow, String> customerPhoneColumn;

        @FXML
        private TableColumn<ReservationTableRow, String> reservedSeatColumn;

        //Show hall tab fields
        @FXML
        private BorderPane showBorderPane;

        // Show hall gridPane for seats
        @FXML
        private GridPane showHallGrid;


        @FXML
        private  ObservableList<ShowTableRow> showGuiData = FXCollections.observableArrayList();





        //Constructor

        /**
         * Creates an instance of DatabaseController and Reservation for use in the class methods.
         */
        public GUIController()
        {
                /*Running before initialize*/
                databaseController = new  DatabaseController();
                reservationModel = new ReservationModel();
        }


        /**
         * @return An ObserverableList with objects of type ShowTableRow used to set show data in MainView().
         */
        public ObservableList<ShowTableRow> getShowGuiData()
        {
                return showGuiData;
        }

        //Methods

        /**
         * Sets data for the columns in reservationTable and bioTable and setting actionListener for the DatePicker.
         * The data is inserted in the tableview; show, time and hallcollumn.
         */
        @FXML
        private void initialize()
        {
                showColumn.setCellValueFactory(cellData -> cellData.getValue().showProperty());
                timeColumn.setCellValueFactory(cellData -> cellData.getValue().timeProperty());
                hallColumn.setCellValueFactory(cellData -> cellData.getValue().hallProperty());

                reservationIdColumn.setCellValueFactory(cellData -> cellData.getValue().getReservation_id_Property());
                customerNameColumn.setCellValueFactory(cellData -> cellData.getValue().getCustomer_name_Property());
                customerPhoneColumn.setCellValueFactory(cellData -> cellData.getValue().getCustomer_phone_Property());
                reservedSeatColumn.setCellValueFactory(cellData -> cellData.getValue().getReservation_id_Property());

        }

        /**
         * Event handler for bioTable On Mouse Clicked Event which will load the seats in the theater connected to the
         * chosen show and the reservationTable.
         */
        @FXML
        private void setShowBorderPane()
        {
                setShowHall(); //Sets the Show halls seats
                setReservationTable(); // Loads the connected reservationTables reservation data

        }

        /**
         * Deletes a marked reservation in the GUI's reservation table.
         */
        @FXML
        private void deleteReservarionClick()
        {
                ReservationController reservationController = new ReservationController();

                ReservationTableRow selectedItem = reservationTableView.getSelectionModel().getSelectedItem(); // Selects the item in the clicked table cell
                int a = reservationTableView.getSelectionModel().getSelectedIndex(); // Gets the index of the chosen table column which is reservationID column and stores it

                reservationController.deleteReservation(Integer.parseInt(selectedItem.getReservation_id())); // Deletes the chosen row in the database depending on the reservation ID
                reservationTableView.getSelectionModel().clearSelection(); // Clears the selection, so no row is chosen/marked

                ObservableList<ReservationTableRow> items = reservationTableView.getItems(); // Updates the reservation table with the existing values in the observable list
                items.remove(a); //Removes the item in the earlier stored index so the GUI view is updated accordingly.

                reservationTableView.setItems(items); //Sets the table to the remaining items
                setShowHall(); // Refresh
        }

        /**
         * Event handler for GUI Home Button component On Mouse Clicked, which resets the bioTable data to the current date.
         */
        @FXML
        private void showReset()
        {
                ShowModel showModel= new ShowModel();

                showGuiData.clear(); //Clear the previous list

                showModel.getShowData(showGuiData);//Add the correct shows to the time table

                setShowData(showGuiData);//Update display
        }

        /**
         * Sets the data for the bioTable, used in showReset.
         * @param data
         */
        public void setShowData(ObservableList<ShowTableRow> data)
        {
                bioTable.setItems(data);
        }

        /**
         * Will be called by the event handler in setSeats() in TheaterModel. It will make non reserved seats become
         * blue and thereby chosen when clicked once and green (not chosen) when clicked again.
         * @param button
         */
        public void onSeatPressEvent(ToggleButton button)
        {

                if(button.isSelected() && button.getStyleClass().contains("free"))
                {
                        button.getStyleClass().clear();
                        button.getStyleClass().add("chosen");
                }
                else if(!button.isSelected() && button.getStyleClass().contains("chosen"))
                {
                        button.getStyleClass().clear();
                        button.getStyleClass().add("free");
                }
        }

        /**
         * Event handler for reserved Button in the reservation section
         * Creates a pop up window where customer name and phone number can be written
         */
        @FXML
        public void clickOnReserved()
        {
                Stage newStage = new Stage();
                VBox comp = new VBox();
                TextField nameField = new TextField("Name");
                TextField phoneNumber = new TextField("Phone Number");
                Button acceptButton = new Button("Place reservation");
                comp.getChildren().add(nameField);
                comp.getChildren().add(phoneNumber);
                comp.getChildren().add(acceptButton);

                /*Pop up window where name and phone number can be typed
                 */
                Scene stageScene = new Scene(comp, 300, 300);
                newStage.setScene(stageScene);
                newStage.show();
                //Add action listener for place reservation button in pop up window
                placeReservationActionListener(acceptButton,nameField,phoneNumber,newStage);
        }

        /**
         * Action listener to  place reservation pop Up window.
         * Uses the name and phone number from the text fields above to place a reservation for user on selected seats.
         * @param button
         */
        private void placeReservationActionListener(Button button,TextField name, TextField phoneNumber,Stage newStage)
        {
                button.setOnAction(new EventHandler<ActionEvent>()
                {
                        @Override
                        public void handle(ActionEvent event)
                        {

                                CustomerModel customerModel = new CustomerModel();
                                //seatButtons wrapped as java FX Nodes. Contains css style class (is the seat marked)
                                ObservableList<Node> nodes = showHallGrid.getChildren();
                                //SeatButtons. Contains seat_id. Needed for reservation query
                                ObservableList<SeatButton> seatButtons = (ObservableList<SeatButton>) (ObservableList<?>) nodes;
                                ReservationController reservationController = new ReservationController();

                                //Defining variables for database reservation
                                String customerName = name.getText();
                                String phone = phoneNumber.getText();
                                String customerPhone  = phoneNumber.getText();
                                //Create user if user wasn't registered before
                                reservationController.createUser(customerName, customerPhone);
                                int customer_id = customerModel.getUniqueCustomerID(customerName, customerPhone);
                                int show_id = seatInfoList.get(0).getShow_id();

                                int reservation_id = reservationController.getNewstReservationKey();


                                //String builder for for-loop. SQL update queries
                                ArrayList<String> queryList = new ArrayList<String>();


                                // Run through the entire Button grid and reserve each seat which has been marked
                                for (int i = 0; i < nodes.size(); i++)
                                {
                                        ObservableList elementClassList = nodes.get(i).getStyleClass();
                                        if (containString(elementClassList, "chosen"))
                                        {       //Update key before insert
                                                reservation_id++;
                                                int seat_id = seatButtons.get(i).getSeat_id();
                                                reservationController.newReservationQuery(queryList, customer_id, show_id, seat_id, reservation_id);

                                        }
                                }
                                for (String s : queryList)
                                        System.out.println(s);
                                databaseController.injectMany(queryList);

                                ;
                                //Reload GUI with the new data from database
                                setShowBorderPane();
                                //Close pop up window
                                newStage.close();
                        }
                }); // End of setAction method
        }


        /**
         * Help Function for clickOnReserved.
         */

        private boolean containString(ObservableList<String> list,String desiredElement)
        {
                Boolean result = false;
                for(String s: list)
                {
                        if(s==desiredElement)
                                result = true;
                }
                return result;
        }


        /**
         * Sets up view under GUI's Show hall tab.
         */
        private void setShowHall()
        {

                TheaterModel theaterModel = new TheaterModel();


                int index = bioTable.getSelectionModel().getSelectedIndex();
                //storing information from tableRow
                String hallColumnValue = hallColumn.getCellData(index);
                String showColumnValue = showColumn.getCellData(index);
                LocalDateTime timeColumnValue = timeColumn.getCellData(index);
                Timestamp ts = java.sql.Timestamp.valueOf(timeColumnValue); // Parses the value of timeColumnValue to Timestamp

                //Retrieve all seat information from database
                SeatModel seatModel = new SeatModel();
                seatInfoList = seatModel.getSeatDataforshow(hallColumnValue,showColumnValue,ts);

                //GridPane which store all seatButton in GUI
                showHallGrid = theaterModel.setSeats(seatInfoList);
                //Adds the seat to the GUI pane ShowHall
                showBorderPane.setCenter(showHallGrid);
        }


        /**
         * Sets up view under GUI's Reservations tab.
         */
        private void setReservationTable()
        {
                ObservableList<ReservationTableRow> reservations = FXCollections.observableArrayList();
                ShowModel showModel = new ShowModel();
                ReservationModel reservationModel = new ReservationModel();
                int show_id;
                int theater_id;

                int index = bioTable.getSelectionModel().getSelectedIndex();
                //storing information from tableRow
                String hallColumnValue = hallColumn.getCellData(index);
                String showColumnValue = showColumn.getCellData(index);
                LocalDateTime timeColumnValue = timeColumn.getCellData(index);

                theater_id = showModel.getTheaterID(hallColumnValue);
                Timestamp ts = java.sql.Timestamp.valueOf(timeColumnValue); // Parses the value of timeColumnValue to Timestamp

                //Timestamp ts = new Timestamp();
                show_id = showModel.getShowID(ts, theater_id);


                reservationModel.getReservationData(reservations, show_id);

                reservationTableView.setItems(reservations);
        }


    }

