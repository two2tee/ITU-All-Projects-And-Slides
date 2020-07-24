package itu.mmad.dttn.tingle.model;

import java.io.Serializable;
import java.util.Date;

/**
 * Used to store uncommitted changes
 */
public class TempThingToStore implements Serializable {
    private String mWhat;
    private String mWhere;
    private String mDescription;
    private Date mDate;
    private String mBarcode;
    private boolean hasChanged;


    public TempThingToStore() {
        mWhat = null;
        mWhere = null;
        mDescription = null;
        mDate = null;
        mBarcode = null;
        hasChanged = false;

    }


    public String getBarcode() {
        return mBarcode;
    }

    public void setBarcode(String barcode) {
        mBarcode = barcode;
        hasChanged = true;
    }

    public String getWhat() {
        return mWhat;
    }

    public void setWhat(String what) {
        mWhat = what;
        hasChanged = true;
    }

    public String getWhere() {
        return mWhere;
    }

    public void setWhere(String where) {
        mWhere = where;
        hasChanged = true;

    }

    public String getDescription() {
        return mDescription;
    }

    public void setDescription(String description) {
        mDescription = description;
        hasChanged = true;
    }

    public Date getDate() {
        return mDate;
    }

    public void setDate(Date date) {
        mDate = date;
        hasChanged = true;

    }

    public boolean isHasChanged() {
        return hasChanged;
    }
}

