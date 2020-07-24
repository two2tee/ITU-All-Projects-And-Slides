package itu.mmad.dttn.tingle.controller.Fragments;

import android.app.Activity;
import android.content.ActivityNotFoundException;
import android.content.Context;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.EditorInfo;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.ExecutionException;

import itu.mmad.dttn.tingle.R;
import itu.mmad.dttn.tingle.controller.BaseActivity;
import itu.mmad.dttn.tingle.model.Networking.NetworkHandler;
import itu.mmad.dttn.tingle.model.Thing;
import itu.mmad.dttn.tingle.model.database.ThingsDatabase;

/**
 * Fragment of main page.
 * NOTE: Support library of fragments are used
 * android.support.v4.app...
 */
public class TingleFragment extends Fragment {

    private static final int REQUEST_SCAN = 1;

    //EventHandler
    TingleFragmentEventListener mCallBack;
    // GUI variables
    private Button addThing;
    private Button addBarcodeThing;
    private Button showAll;
    private TextView lastAdded;
    private EditText whatField, whereField;
    //database
    private ThingsDatabase repository;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setHasOptionsMenu(true);
    }

    @Override
    public void onResume() {
        super.onResume();
        updateUI();
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View v = inflater.inflate(R.layout.fragment_tingle, container, false);
        repository = ((BaseActivity) getActivity()).getDatabase();

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
            repository = ((BaseActivity) getActivity()).getDatabase();
        } catch (ClassCastException e) {
            throw new ClassCastException(context.toString() + " must implement ListFragmentEventListener");
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {

            case REQUEST_SCAN:
                handleScanResult(data);
                break;
            case Activity.RESULT_CANCELED:
                makeToast(getString(R.string.scan_fail));
                break;
            case Activity.RESULT_OK:
                makeToast(getString(R.string.ok));

            default:
                makeToast(getString(R.string.wrong_resultCode));

        }


    }


    private void handleScanResult(Intent data) {
        String barcode = data.getStringExtra("SCAN_RESULT");
        NetworkHandler manager = ((BaseActivity) getActivity()).getNetworkHandler();
        try {

            Map<String, String> result = manager.getBarcodeTask().execute(barcode).get(); //returns map of data fetched from the internet
            if (result.get("name").equals("null")) {
                makeToast(getString(R.string.item_NotFound_toast));
                return;
            } else if (result == null) {
                makeToast(getString(R.string.something_Went_Wrong));
            }

            //Creating a thing
            Thing toAdd = makeThing(result.get("name"), whereField.getText().toString());
            toAdd.setBarcode(result.get("barcode"));

            repository.put(toAdd);

        } catch (InterruptedException | ExecutionException e) {
            e.printStackTrace();
        }

    }


    @Override
    public void onCreateOptionsMenu(Menu menu, MenuInflater inflater) {
        if (getResources().getConfiguration().orientation == Configuration.ORIENTATION_PORTRAIT) {
            menu.clear();
            setHasOptionsMenu(false);
        }
        super.onCreateOptionsMenu(menu, inflater);
    }


    private void setButtons(View v) {
        addThing = (Button) v.findViewById(R.id.add_button);
        addThing.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                addItem();
            }

        });

        addBarcodeThing = (Button) v.findViewById(R.id.add_button_barcode);
        addBarcodeThing.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (whereField.getText().length() == 0) {
                    makeToast(getString(R.string.where_is_empty));
                    return;
                } else if (((BaseActivity) getActivity())
                        .getNetworkHandler().hasActiveInternetConnection(getActivity()) == false) {
                    makeToast(getString(R.string.no_network));
                    return;
                }

                try {
                    Intent intent = new Intent("com.google.zxing.client.android.SCAN");
                    intent.putExtra("SCAN_MODE", "PRODUCT_MODE");
                    startActivityForResult(intent, REQUEST_SCAN);
                } catch (ActivityNotFoundException e) {
                    Log.e("onCreate", "Scanner Not Found", e);
                    makeToast(getString(R.string.no_scanner_found));
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

    /**
     * Adds a given item from entered data
     */
    private void addItem() {
        if ((whatField.getText().length() > 0) && (whereField.getText().length() > 0)) {
            repository.put(makeThing());
            whatField.setText("");
            whereField.setText("");
            updateUI();

            mCallBack.onItemAdded();
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
        whereField.setOnEditorActionListener(new TextView.OnEditorActionListener() {
            @Override
            public boolean onEditorAction(TextView v, int actionId, KeyEvent event) {
                boolean handled = false;
                if (actionId == EditorInfo.IME_ACTION_DONE) {
                    addItem();
                    handled = true;
                }

                return handled;
            }
        });

    }

    private Thing makeThing() {
        return new Thing(whatField.getText().toString().toLowerCase().trim(), whereField.getText().toString().toLowerCase().trim(), UUID.randomUUID());
    }

    private Thing makeThing(String what, String where) {
        return new Thing(what.toLowerCase().trim(), where.toLowerCase().trim(), UUID.randomUUID());
    }

    private void updateUI() {
        List<Thing> Things = repository.getAll();
        if (Things.size() > 0) {
            this.lastAdded.setText(Things.get(Things.size() - 1).toString());
        } else {
            this.lastAdded.setText(getString(R.string.item_NotFound_toast));
        }
    }

    public interface TingleFragmentEventListener {
        void onShowAllPressed();

        void onItemAdded();
    }

}


