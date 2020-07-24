package itu.mmad.dttn.tingle.model.utils;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Point;

/**
 * This class provide utility methods for handling pictures
 */
public class PictureUtils {
    /**
     * This method returns a scaled bitmap image to reduce memory use of the loaded image
     *
     * @param path       to file
     * @param destWidth  width of area of placement
     * @param destHeight height of area of placement
     * @return bitmap image
     */
    public static Bitmap getScaledBitmap(String path, int destWidth, int destHeight) {
        // Read image dimensions from disk
        BitmapFactory.Options options = new BitmapFactory.Options();
        options.inJustDecodeBounds = true;
        BitmapFactory.decodeFile(path, options);

        float srcWidth = options.outWidth;
        float srcHeight = options.outHeight;

        //Compute scale
        int inSampleSize = 1; //Determines how big each sample should be for each pixel
        if (srcHeight > destHeight || srcWidth > destWidth) {
            if (srcWidth > srcHeight) {
                inSampleSize = Math.round(srcHeight / destHeight);
            } else {
                inSampleSize = Math.round(srcWidth / destWidth);
            }
        }

        options = new BitmapFactory.Options();
        options.inSampleSize = inSampleSize;

        //read in and create final bitmap
        return BitmapFactory.decodeFile(path, options);

    }

    /**
     * This method uses a conservative estimate to calculate the size of
     * an image.
     * <p/>
     * Method checks to see how big the screen is and then scales the image down
     * to that size
     *
     * @param path     of file
     * @param activity activity
     * @return bitmap image
     */
    public static Bitmap getScaledBitmap(String path, Activity activity) {
        Point size = new Point();
        activity.getWindowManager().getDefaultDisplay().getSize(size);

        return getScaledBitmap(path, size.x, size.y);
    }

}
