package itu.mmad.dttn.tingle.model.database.repositories.sqlSchema;

import android.annotation.SuppressLint;
import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.UUID;

import itu.mmad.dttn.tingle.model.Interfaces.IRepository;
import itu.mmad.dttn.tingle.model.Thing;
import itu.mmad.dttn.tingle.model.database.Entity;

/**
 * This class represents a SQL repository
 */
public class SQLRepository implements IRepository {
    private final SQLiteDatabase mDatabase;
    private Context mContext;

    public SQLRepository(Context context) {
        mContext = context.getApplicationContext();
        mDatabase = new ThingBaseHelper(mContext).getWritableDatabase();

    }

    private static ContentValues getContentValues(Entity thing) {
        Thing toStore = (Thing) thing;
        ContentValues values = new ContentValues();

        values.put(ThingTable.Cols.UUID, toStore.getId().toString());
        values.put(ThingTable.Cols.WHAT, toStore.getWhat());
        values.put(ThingTable.Cols.WHERE, toStore.getWhere());
        values.put(ThingTable.Cols.DESCRIPTION, toStore.getDescription());
        values.put(ThingTable.Cols.BARCODE, toStore.getBarcode());
        values.put(ThingTable.Cols.DATE, toStore.getDate().getTime());

        return values;
    }

    @Override
    public Entity get(UUID id) {
        ThingCursorWrapper cursor = queryThings(ThingTable.Cols.UUID + "= ?", new String[]{id.toString()});

        try {
            if (cursor.getColumnCount() == 0) return null;
            cursor.moveToFirst();
            return cursor.getThing();
        } finally {
            cursor.close(); // closing cursor to release all resources
        }
    }

    @Override
    public Iterator getAll() {
        List<Entity> items = new ArrayList<>();
        ThingCursorWrapper cursor = queryThings(null, null);

        try {
            cursor.moveToFirst();
            while (!cursor.isAfterLast()) {
                items.add(cursor.getThing());
                cursor.moveToNext();
            }
        } finally {
            cursor.close();
        }

        return items.iterator();
    }

    @Override
    public boolean put(Entity entity) {
        ContentValues values = getContentValues(entity);
        if (values == null) return false;
        mDatabase.insert(ThingTable.NAME, null, values);
        return true;
    }

    @Override
    public boolean update(Entity toStore) {
        ContentValues values = getContentValues(toStore);
        String UUID = toStore.getId().toString();

        if (values == null) return false;
        mDatabase.update(ThingTable.NAME, values, ThingTable.Cols.UUID + " = ?", new String[]{UUID});

        return true;
    }

    @Override
    public boolean delete(UUID id) {
        return mDatabase.delete(ThingTable.NAME, ThingTable.Cols.UUID + "=?", new String[]{id.toString()}) > 0;
    }

    private ThingCursorWrapper queryThings(String whereClause, String[] whereArgs) {
        @SuppressLint("Already closed later") Cursor cursor = mDatabase.query(ThingTable.NAME, null, //null selects all columns
                whereClause, whereArgs, null, // groupBy
                null, // having
                null // orderBy
        );

        return new ThingCursorWrapper(cursor);
    }
}
