package itu.mmad.dttn.tingle.controller;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.res.Configuration;
import android.support.v4.app.Fragment;
import android.util.Log;

import itu.mmad.dttn.tingle.R;
import itu.mmad.dttn.tingle.controller.Fragments.ListFragment;
import itu.mmad.dttn.tingle.controller.Fragments.TingleFragment;

/**
 * This class represents the launcher activity for tingle
 */
public class TingleActivity extends BaseActivity implements ListFragment.ListFragmentEventListener, TingleFragment.TingleFragmentEventListener {

    @Override
    protected Fragment createPortraitFragment() {
        return new TingleFragment();
    }

    @Override
    protected Fragment createLeftFragment() {
        return new TingleFragment();
    }

    @Override
    protected Fragment createRightFragment() {
        return new ListFragment();
    }


    /**
     * Returns to tingle main page.
     */
    public void goBack() {
        replacePortraitFragment(new TingleFragment());
    }

    /**
     * Goes to list page
     */
    public void onShowAllPressed() {
        replacePortraitFragment(new ListFragment());
    }

    /**
     * When add is pressed update list
     */
    @Override
    public void onItemAdded() {
        //To update list when landscape mode
        if (getResources().getConfiguration().orientation == Configuration.ORIENTATION_LANDSCAPE) {
            setFragment();
        }

    }

    @Override
    public void onBackPressed() {
        if (getResources().getConfiguration().orientation == Configuration.ORIENTATION_PORTRAIT) {

            //Check if there is no item left in back stack (used to exit app if back stack is empty)
            int backStackEntryCount = getSupportFragmentManager().getBackStackEntryCount();
            if (backStackEntryCount == 0) {
                //Exit dialog
                new AlertDialog.Builder(this).setIcon(android.R.drawable.ic_dialog_alert).setTitle(R.string.warning).setMessage(R.string.exit_question).setPositiveButton(R.string.yes, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        finish();
                    }

                }).setNegativeButton(R.string.no, null).show();
            } else getSupportFragmentManager().popBackStackImmediate(); //pop latest fragment
        } else //Exit app if screen is on landscape mode
        {
            Log.d("CDA", "onBackPressed Called");
            finish();
        }
    }

}
