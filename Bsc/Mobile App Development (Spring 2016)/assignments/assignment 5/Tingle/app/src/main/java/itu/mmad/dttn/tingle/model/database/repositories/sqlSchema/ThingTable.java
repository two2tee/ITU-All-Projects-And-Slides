package itu.mmad.dttn.tingle.model.database.repositories.sqlSchema;

/**
 * This class represents a sql table for thing
 * The class exists to define the String constants needed to describe the moving pieces
 * of the table definition.
 */
class ThingTable {
    public static final String NAME = "Things";

    public static final class Cols {
        public static final String UUID = "uuid";
        public static final String WHAT = "whatItem";
        public static final String WHERE = "whereItem";
        public static final String DESCRIPTION = "description";
        public static final String BARCODE = "barcode";
        public static final String DATE = "date";

    }
}
