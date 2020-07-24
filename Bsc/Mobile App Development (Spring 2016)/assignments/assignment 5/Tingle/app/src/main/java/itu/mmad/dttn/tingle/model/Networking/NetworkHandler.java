package itu.mmad.dttn.tingle.model.Networking;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.AsyncTask;
import android.util.Log;

import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.Map;
import java.util.concurrent.ExecutionException;

/**
 * This class is responsible for managing network related tasks
 */
public class NetworkHandler {
    private static final String LOG_TAG = "NetworkHandler";

    public FetchBarcodeInfoTask getBarcodeTask() {
        return new FetchBarcodeInfoTask();
    }

    /**
     * Checks if one has an active internet connection
     * We use a stable well known site (google) to test the connection
     *
     * @param context context
     * @return boolean boolean
     */
    public boolean hasActiveInternetConnection(Context context) {
        try {

            return new CheckConnectionTask().execute(context).get();

        } catch (InterruptedException | ExecutionException e) {
            Log.e(LOG_TAG, e.getMessage());
        }
        return false;
    }

    /**
     * Checks whether one is connected to a network (This does not check if there is
     * an actual connection to the internet)
     *
     * @param context
     * @return
     */
    private boolean isNetworkAvailable(Context context) {

        ConnectivityManager connectivityManager
                = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo activeNetworkInfo = connectivityManager.getActiveNetworkInfo();
        return activeNetworkInfo != null && activeNetworkInfo.isConnected();
    }

    /**
     * Fetch information of an item from a given barcode
     * It will retrieve information from www.outpan.com by utilizing their public API
     * <p/>
     * GET API - https://api.outpan.com/v2/products/[GTIN]?apikey=[YOUR API KEY]
     */
    public class FetchBarcodeInfoTask extends AsyncTask<String, Void, Map> {
        private static final String APIKEY = "92f9b26d17535cfbe4d228008ad3fea7";

        @Override
        protected Map<String, String> doInBackground(String... params) {
            String barcode = params[0];
            String URL = "https://api.outpan.com/v2/products/" + barcode + "?apikey=" + APIKEY;
            try {
                Map<String, String> result = new OutpanFetcher().fetchItem(URL);
                Log.i(LOG_TAG, "Fetched contents of URL:" + result);
                return result;
            } catch (IOException e) {
                Log.e(LOG_TAG, "Failed to fetch ur:", e);
            }
            return null;
        }
    }

    /**
     * Checks connection availability on another thread
     */
    private class CheckConnectionTask extends AsyncTask<Context, Void, Boolean> {

        @Override
        protected Boolean doInBackground(Context... params) {
            Context context = params[0];

            if (isNetworkAvailable(context)) {
                HttpURLConnection urlc = null;
                try {
                    urlc = (HttpURLConnection) (new URL("http://www.google.com").openConnection());
                } catch (IOException e) {
                    Log.e(LOG_TAG, "Malformed URL : ", e);
                }

                if (urlc == null) return false;

                try {
                    urlc.setRequestProperty("User-Agent", "Test");
                    urlc.setRequestProperty("Connection", "close");
                    urlc.setConnectTimeout(1500);
                    urlc.connect();
                    return (urlc.getResponseCode() == 200);
                } catch (IOException e) {
                    Log.e(LOG_TAG, "Error checking internet connection", e);
                } finally {
                    urlc.disconnect();
                }
            } else {
                Log.d(LOG_TAG, "No network available!");
            }
            return false;
        }
    }
}


