package itu.mmad.dttn.tingle.model.Networking;

import android.util.Log;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.HashMap;
import java.util.Map;

import javax.net.ssl.HttpsURLConnection;

/**
 * This class is responsible for fecthing data from outpan given a
 * barcode
 */
class OutpanFetcher {


    /**
     * This method connects to a given host and retrieves data from it
     *
     * @param urlSpec url to resource
     * @return bytearray
     * @throws IOException if bad connection
     */
    private byte[] getUrlBytes(String urlSpec) throws IOException {
        URL url = new URL(urlSpec);
        HttpsURLConnection connection = (HttpsURLConnection) url.openConnection();

        try {
            ByteArrayOutputStream out = new ByteArrayOutputStream();
            connection.setConnectTimeout(1500); //timeout in case of bad connection
            InputStream in = connection.getInputStream();

            if (connection.getResponseCode() != HttpURLConnection.HTTP_OK) {
                throw new IOException(connection.getResponseMessage() + ": with" + urlSpec);
            }
            int bytesRead;
            byte[] buffer = new byte[1024];
            while ((bytesRead = in.read(buffer)) > 0) {
                out.write(buffer, 0, bytesRead);
            }
            out.close();
            return out.toByteArray();

        } finally {
            connection.disconnect();
        }
    }

    private String getUrlString(String urlSpec) throws IOException {
        return new String(getUrlBytes(urlSpec));
    }

    /**
     * Returns a map containing fetched data
     *
     * @param url to resource
     * @return map with fetched data
     * @throws IOException
     */
    public Map<String, String> fetchItem(String url) throws IOException {
        String JsonString = getUrlString(url);
        Map results = new HashMap();
        try {
            JSONObject jsonBody = new JSONObject(JsonString);

            results.put("barcode", jsonBody.get("gtin").toString());
            results.put("name", jsonBody.get("name").toString());

        } catch (JSONException e) {
            Log.e("OutpanFetcher", "Failed to parse JSON", e);
        }

        return results;
    }

}
