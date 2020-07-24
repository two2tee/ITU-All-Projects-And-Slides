package itu.mmad.dttn.tingle.Model;

import java.io.Serializable;

/**
 * This class represents a thing to be added
 */
public class Thing implements Serializable{
    private String mWhat;
    private String mWhere;

    public Thing(String what, String where) {
        mWhat = what;
        mWhere = where;
    }


    @Override
    public String toString() {
        return oneLine("Item: ", "\nLocation: ");
    }

    public String getWhat() {
        return mWhat;
    }

    public void setWhat(String what) {
        mWhat = what;
    }

    public String getWhere() {
        return mWhere;
    }

    public void setWhere(String where) {
        mWhere = where;
    }

    public String oneLine(String pre, String post) {
        return pre + mWhat + " " + post + mWhere;

    }
}
