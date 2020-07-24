package itu.mmad.dttn.tingle;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.List;

import itu.mmad.dttn.tingle.Model.Thing;

public class TingleActivity extends AppCompatActivity {
    // GUI variables
    private Button addThing;
    private Button lookUpThing;
    private TextView lastAdded;
    private TextView whatField, whereField;

    //fake database
    private List<Thing> thingsDB;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tingle);
        thingsDB = new ArrayList<>();
        fillThingsDB();

        //Accessing GUI elements
        lastAdded = (TextView) findViewById(R.id.last_thing);
        updateUI();

        //Button
        setButtons();

        //TextFields
        setTextFields();


    }

    private void setButtons() {
        addThing = (Button) findViewById(R.id.add_button);
        addThing.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if ((whatField.getText().length() > 0) && (whereField.getText().length() > 0)) {
                    thingsDB.add(makeThing());
                    whatField.setText("");
                    whereField.setText("");
                    updateUI();
                }
            }
        });


        lookUpThing = (Button) findViewById(R.id.lookUp_button);
        lookUpThing.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (whatField.getText().length() > 0) {
                    String result = SearchThing(whatField.getText().toString());

                    if (result != null)
                        makeToast(getString(R.string.item_found_toast) + " " + result);
                    else
                        makeToast(getString(R.string.item_NotFound_toast));
                }
            }
        });


    }

    private void makeToast(String string) {
        Toast.makeText(TingleActivity.this, string, Toast.LENGTH_SHORT).show();
    }

    private String SearchThing(String item) {
        String searchItem = item.toLowerCase().trim();
        for (Thing thing : thingsDB) {
            if (thing.getWhat().equals(searchItem)) {
                return thing.getWhere();
            }
        }
        return null;
    }

    private void setTextFields() {
        whatField = (TextView) findViewById(R.id.what_text);
        whereField = (TextView) findViewById(R.id.where_text);
    }

    private Thing makeThing() {
        return new Thing(whatField.getText().toString().toLowerCase().trim(),
                whereField.getText().toString().toLowerCase().trim());
    }

    private void updateUI() {
        int size = thingsDB.size();
        if (size > 0) {
            lastAdded.setText(thingsDB.get(size - 1).toString());
        }
    }

    private void fillThingsDB() {
        thingsDB.add(new Thing("Android Pnone", "Desk"));
        thingsDB.add(new Thing("Big Nerd book", "Desk"));
        thingsDB.add(new Thing("Big Nerd book2", "Work"));
        thingsDB.add(new Thing("Big Nerd book3", "Car"));

    }
}
