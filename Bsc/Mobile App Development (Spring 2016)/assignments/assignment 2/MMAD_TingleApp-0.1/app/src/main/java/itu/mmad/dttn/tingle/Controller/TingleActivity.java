package itu.mmad.dttn.tingle.Controller;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import java.util.Iterator;

import itu.mmad.dttn.tingle.Model.InMemoryRepository;
import itu.mmad.dttn.tingle.Model.Interfaces.IRepository;
import itu.mmad.dttn.tingle.Model.Thing;
import itu.mmad.dttn.tingle.R;

/**
 * This class represents the launcher activity for tingle
 */
public class TingleActivity extends AppCompatActivity {

    // GUI variables
    private Button addThing;
    private Button lookUpThing;
    private Button showAll;
    private TextView lastAdded;
    private TextView whatField, whereField;

    //database
    private IRepository<Thing> repository;

    //Other
    private static boolean isFilled = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tingle);
        repository = InMemoryRepository.getInMemoryRepository();
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
                    repository.put(makeThing());
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
                } else {
                    makeToast(getString(R.string.no_what_specified));
                }
            }
        });

        showAll = (Button) findViewById(R.id.goTOList_button);
        showAll.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent i = new Intent(TingleActivity.this, ListActivity.class);
                startActivity(i);
            }
        });


    }

    private void makeToast(String string) {
        Toast.makeText(TingleActivity.this, string, Toast.LENGTH_SHORT).show();
    }

    private void setTextFields() {
        whatField = (TextView) findViewById(R.id.what_text);
        whereField = (TextView) findViewById(R.id.where_text);
    }

    private String SearchThing(String item) {
        String searchItem = item.toLowerCase().trim();
        Thing thing;
        Iterator<Thing> items = repository.getAll();
        while (items.hasNext()) {
            thing = items.next();
            if (thing.getWhat().equals(searchItem)) {
                return thing.getWhere();
            }
        }
        return null;
    }

    private Thing makeThing() {
        return new Thing(whatField.getText().toString().toLowerCase().trim(),
                whereField.getText().toString().toLowerCase().trim());
    }

    private void updateUI() {
        int lastAdded = repository.returnSize();
        if (lastAdded > 0) {
            this.lastAdded.setText(repository.get(lastAdded).toString());
        } else {
            this.lastAdded.setText(getString(R.string.item_NotFound_toast));
        }
    }

    private void fillThingsDB() {
        if (!TingleActivity.isFilled) {
            repository.put(new Thing("Android Phone", "Desk"));
            repository.put(new Thing("Keys", "Desk"));
            repository.put(new Thing("Child", "Kindergarten"));
            repository.put(new Thing("Groceries", "Car"));
            TingleActivity.isFilled = true;
        }
    }
}
