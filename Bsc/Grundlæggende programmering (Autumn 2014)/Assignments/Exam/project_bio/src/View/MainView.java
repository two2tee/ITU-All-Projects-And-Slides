package View;

import Controller.GUIController;
import Model.Show.ShowModel;
import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

import java.io.IOException;

/**
 * Created by William on 08/12/14.
 */
public class MainView extends Application {

//Fields
GUIController guiController = new GUIController();

   //Constructor
    /**
     * Creates an instance of ShowModel used to get the data which is then set via the controller.
     */
    public MainView()
    {
        ShowModel showModel = new ShowModel();
        showModel.getShowData(guiController.getShowGuiData());//Load showData into reflection (GUI)
    }

        @Override
        public void start(Stage primaryStage) throws Exception  //Setting the Stage for the application view
        {
            showBioView(primaryStage);
        }

    /**
     * Method for setting up controller, stage, and scene and making them visible containing data.
     * @param stage
     */
        public void showBioView(Stage stage)
        {
            try
            {
                FXMLLoader loader = new FXMLLoader(); //Creating instance of FXMLLoader()
                loader.setLocation(getClass().getResource("bioView.fxml")); //Setting the path for the loader to find fxml-file
                Parent bioView = loader.load(); //Loading resources into application view
                stage.setTitle("Bio"); //Setting title for the view
                stage.setScene(new Scene(bioView, 1280, 920)); //Setting the scene into the stage

            //Get the controller from the fx:controller attribute of our FXML
                GUIController controller = loader.getController();
                controller.setShowData(guiController.getShowGuiData()); //Setting data for show table

                stage.show(); //Making visible
            }
            catch (IOException e)
            {
                e.printStackTrace();
            }


        }

        public static void main(String[] args)
        {
            launch(args); // main method launching
        }
}

