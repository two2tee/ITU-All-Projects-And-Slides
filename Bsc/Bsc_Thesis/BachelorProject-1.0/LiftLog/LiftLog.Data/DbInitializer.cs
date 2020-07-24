// DbInitializer.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

namespace LiftLog.Data
{
    // EF creates an empty database that is populated with test (seed) data in this class
    public static class DbInitializer
    {
        // Checks if there is any data in db, otherwise assumes db is new and needs to be seeded with test data 
        // Uses arrays instead of List<T> for performance 
        public static async void Initialize(Context context)
        {
            await context.Database.EnsureCreatedAsync();

            // Seed Data 

            //await context.SaveChangesAsync();
        }
    }
}