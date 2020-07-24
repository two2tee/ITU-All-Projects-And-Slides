package itu.mmad.dttn.tingle.Controller.Fragments;

import android.content.Context;
import android.content.res.Configuration;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.util.List;

import itu.mmad.dttn.tingle.Controller.TingleActivity;
import itu.mmad.dttn.tingle.Model.Thing;
import itu.mmad.dttn.tingle.Model.ThingsDatabase;
import itu.mmad.dttn.tingle.R;

/**
 * Fragment of main page.
 * NOTE: Support library of fragments are used
 * android.support.v4.app...
 */
public class TingleFragment extends Fragment {

    //EventHandler
    TingleFragmentEventListener mCallBack;

    public interface TingleFragmentEventListener {
        void onShowAllPressed();

        void onAddPressed();
    }

    // GUI variables
    private Button addThing;
    private Button lookUpThing;
    private Button showAll;
    private TextView lastAdded;
    private EditText whatField, whereField;

    //database
    private ThingsDatabase repository;


    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View v = inflater.inflate(R.layout.fragment_tingle, container, false);
        repository = ((TingleActivity) getActivity()).getDatabase();

        setButtons(v);
        setTextFields(v);
        updateUI();
        return v;
    }

    @Override
    public void onAttach(Context context) {
        super.onAttach(context);

        //Checks if parent activity has implemented the
        //callback interface
        try {
            mCallBack = (TingleFragmentEventListener) context;
            repository = ((TingleActivity) getActivity()).getDatabase();
        } catch (ClassCastException e) {
            throw new ClassCastException(context.toString() + " must implement ListFragmentEventListener");
        }
    }


    private void setButtons(View v) {
        addThing = (Button) v.findViewById(R.id.add_button);
        addThing.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if ((whatField.getText().length() > 0) && (whereField.getText().length() > 0)) {
                    repository.put(makeThing());
                    whatField.setText("");
                    whereField.setText("");
                    updateUI();

                    mCallBack.onAddPressed();


                }
            }
        });


        lookUpThing = (Button) v.findViewById(R.id.lookUp_button);
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

        //Portrait mode show go to list button
        if (getResources().getConfiguration().orientation == Configuration.ORIENTATION_PORTRAIT) {
            showAll = (Button) v.findViewById(R.id.goTOList_button);
            showAll.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    mCallBack.onShowAllPressed();
                }
            });
        }

    }

    private void makeToast(String string) {
        Context context = getActivity().getApplicationContext();
        Toast.makeText(context, string, Toast.LENGTH_SHORT).show();
    }

    private void setTextFields(View v) {
        lastAdded = (TextView) v.findViewById(R.id.last_thing);
        whatField = (EditText) v.findViewById(R.id.what_text);
        whereField = (EditText) v.findViewById(R.id.where_text);
    }

    private String SearchThing(String item) {
        String searchItem = item.toLowerCase().trim();
        List<Thing> things = repository.getAll();

        for (Thing t : things) {
            if (t.getWhat().equals(searchItem)) {
                return t.getWhere();
            }
        }

        return null;
    }

    private Thing makeThing() {
        return new Thing(whatField.getText().toString().toLowerCase().trim(),
                whereField.getText().toString().toLowerCase().trim());
    }

    private void updateUI() {
        int lastAdded = repository.getTotalSize() - 1;
        if (lastAdded > 0) {
            this.lastAdded.setText(repository.get(lastAdded).toString());
        } else {
            this.lastAdded.setText(getString(R.string.item_NotFound_toast));
        }
    }

}


