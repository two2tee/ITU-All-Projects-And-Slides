package FileHandler;

import model.DataObject;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

/**
 * The CSVFileReader class is used to load a csv file
 *
 */
public class CSVFileReader {
    /**
     * The read method reads in a csv file as a two dimensional string array.
     * This method is utilizes the string.split method for splitting each line of the data file.
     * String tokenizer bug fix provided by Martin Marcher.
     * @param csvFile File to load
     * @param seperationChar Character used to seperate entries
     * @param nullValue What to insert in case of missing values
     * @return Data file content as a 2D string array
     * @throws IOException
     */
    private static String[][] readDataFile(String csvFile, String seperationChar, String nullValue, boolean skipHeaderRow) throws IOException
    {

        List<String[]> lines = new ArrayList<>();
        BufferedReader bufRdr = new BufferedReader(new FileReader(new File(csvFile)));

        // read the header
        String line = bufRdr.readLine();

        while ((line = bufRdr.readLine()) != null)
        {
            String[] arr = line.split(seperationChar);

            for(int i = 0; i < arr.length; i++)
            {
                if(arr[i].equals(""))
                {
                    arr[i] = nullValue;
                }
            }
            if(!skipHeaderRow)
            {
                lines.add(arr);
            }
        }

        String[][] ret = new String[lines.size()][];
        bufRdr.close();
        return lines.toArray(ret);
    }


    /**
     * This method parses the raw data read from a file and selects a subset of it
     */
    public static List<DataObject> ParseMasterData(String[][] data){
        ArrayList<DataObject> parsed = new ArrayList<>();
        for (String[] aData : data) {
            String age = aData[1], gender = aData[2], shoe = aData[3], degree = aData[5], programingLanguage = aData[7],
                    height = aData[4], commute = aData[21], phoneOs=aData[8],whyCourse=aData[6],playedGames=aData[20];
            DataObject entry = new DataObject(age,gender,shoe,degree, height, programingLanguage, commute, phoneOs,playedGames,whyCourse);
            System.out.println(entry.toString());
            parsed.add(entry);

        }
        return parsed;
    }

    //
    public static String[][] readInputFile(String path){
        try
        {
            String[][] data = readDataFile(path,"\",\"", "-",false);

            //Print all the data
            for (String[] line : data)
            {
                System.out.println(Arrays.toString(line));
            }
            System.out.println(data[0][0].length());

            //Print a specific entry in the data
            //System.out.println(Arrays.toString(data[1]));
            System.out.println("Number of tuples loaded: "+data.length);

            return data;
        }
        catch (IOException e)
        {
            System.err.println(e.getLocalizedMessage());
        }
        return null;
    }
}
