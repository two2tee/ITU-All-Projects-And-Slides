package itu.mmad.dttn.tingle.model.database.repositories.sqlSchema;

import android.database.Cursor;
import android.database.CursorWrapper;

import java.util.Date;
import java.util.UUID;

import itu.mmad.dttn.tingle.model.Thing;

/**
 * ThingCursorWrapper lets you wrap a Cursor you received from another place and
 * add new methods on top of it.
 */
class ThingCursorWrapper extends CursorWrapper {

    /**
     * Creates a cursor wrapper.
     *
     * @param cursor The underlying cursor to wrap.
     */
    public ThingCursorWrapper(Cursor cursor) {
        super(cursor);
    }

    public Thing getThing() {
        String uuidString = getString(getColumnIndex(ThingTable.Cols.UUID));
        String what = getString(getColumnIndex(ThingTable.Cols.WHAT));
        String where = getString(getColumnIndex(ThingTable.Cols.WHERE));
        String description = getString(getColumnIndex(ThingTable.Cols.DESCRIPTION));
        String barcode = getString(getColumnIndex(ThingTable.Cols.BARCODE));
        long date = getLong(getColumnIndex(ThingTable.Cols.DATE));

        Thing thing = new Thing(what, where, UUID.fromString(uuidString));
        thing.setDescription(description);
        thing.setDate(new Date(date));
        thing.setBarcode(barcode);

        return thing;

    }
}
