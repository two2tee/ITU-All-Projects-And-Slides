package itu.mmad.dttn.tingle.controller;

import android.content.res.Configuration;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v7.app.AppCompatActivity;

import itu.mmad.dttn.tingle.R;
import itu.mmad.dttn.tingle.TingleApplication;
import itu.mmad.dttn.tingle.model.Networking.NetworkHandler;
import itu.mmad.dttn.tingle.model.database.ThingsDatabase;

/**
 * Generic fragment activity used to reduce code redundancy
 */
public abstract class BaseActivity extends AppCompatActivity {
    //Tags
    protected static final String TAG_LEFTFRAGMENT = "leftFragment";
    protected static final String TAG_RIGHTFRAGMENT = "rightFragment";
    protected static final String TAG_PORTRAITFRAGMENT = "leftFragment";


    //Portrait mode
    protected abstract Fragment createPortraitFragment();

    //Landscape mode
    protected abstract Fragment createLeftFragment();

    protected abstract Fragment createRightFragment();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tingle);
        setFragment();
    }

    /**
     * Sets the current fragment for portrait mode with a given fragment.
     * NOTE: Support library of fragments are used.
     *
     * @param fragment Fragment
     */
    protected void replacePortraitFragment(Fragment fragment) {
        Fragment prevFragment = getSupportFragmentManager().findFragmentById(R.id.fragment_container);
        getSupportFragmentManager().beginTransaction()
                .addToBackStack(prevFragment.getTag())
                .replace(R.id.fragment_container, fragment, TAG_PORTRAITFRAGMENT)
                .commit();
    }


    protected void replaceLeftFragmentLandscape(Fragment fragment) {
        if (getResources().getConfiguration().orientation == Configuration.ORIENTATION_LANDSCAPE) {
            getSupportFragmentManager().beginTransaction()
                    .replace(R.id.fragment_container_left, fragment, TAG_LEFTFRAGMENT)
                    .commit();
        }
    }

    protected void replaceRightFragmentLandscape(Fragment fragment) {
        if (getResources().getConfiguration().orientation == Configuration.ORIENTATION_LANDSCAPE) {
            getSupportFragmentManager().beginTransaction()
                    .replace(R.id.fragment_container_right, fragment, TAG_RIGHTFRAGMENT)
                    .commit();
        }
    }

    protected void setFragment() {

        //Landscape mode
        if (getResources().getConfiguration().orientation == Configuration.ORIENTATION_LANDSCAPE) {
            Fragment fragmentLeft = createLeftFragment();
            Fragment fragmentRight = createRightFragment();

            getSupportFragmentManager().beginTransaction()
                    .replace(R.id.fragment_container_left, fragmentLeft, TAG_LEFTFRAGMENT)
                    .replace(R.id.fragment_container_right, fragmentRight, TAG_RIGHTFRAGMENT)
                    .commit();
        }

        //Portrait mode
        else if (getResources().getConfiguration().orientation == Configuration.ORIENTATION_PORTRAIT) {
            Fragment fragment = createPortraitFragment();

            getSupportFragmentManager().beginTransaction()
                    .replace(R.id.fragment_container, fragment, TAG_PORTRAITFRAGMENT)
                    .commit();
        }
    }

    /**
     * Gets the database
     *
     * @return database
     */
    public ThingsDatabase getDatabase() {
        return TingleApplication.getDatabase();
    }

    /**
     * Returns the network manager
     *
     * @return Network Manager
     */
    public NetworkHandler getNetworkHandler() {
        return TingleApplication.getNetworkManager();
    }
}
