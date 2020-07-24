package Task_2;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.SocketException;
import java.util.ArrayList;
import java.util.Random;

/**
 * This class is a modified version of DatagramSocket. It inherent from the DatagramsSocket class, but adds extra
 * functionality to simulate packet anomalies such as corrupted, duplicated or missing packets.
 * These anomalies are all randomly chosen.
 */
public class QuestionableDatagramSocket extends DatagramSocket {

    ArrayList<DatagramPacket> packages = new ArrayList<>();

    Random random = new Random();
    int transmissionInterval;


    //Constructors
    public QuestionableDatagramSocket() throws SocketException {}

    public QuestionableDatagramSocket(int i, int transmissionInterval) throws SocketException{
        super(i);
        this.transmissionInterval = transmissionInterval;
    }


    //packet methods

    /**
     * Send packet to a given host
     * @param _package
     */
    @Override
    public void send(DatagramPacket _package)
    {
        addPacketTOQueue(_package);

        if(random.nextBoolean())
        {
            while(!packages.isEmpty()) //All stored messages
            {
                try {super.send(getRandomPackage());}
                catch (IOException e) {e.printStackTrace();}

                try {Thread.sleep(transmissionInterval);} //Makes sure the packages are transmitted at the same interval as specified from the sender.
                catch (InterruptedException e) {e.printStackTrace();}
            }
        }
    }

    /**
     * Add packet to queue
     * @param _package
     */
    private void addPacketTOQueue(DatagramPacket _package)
    {
        int action = random.nextInt(100);
        if (action >= 0 && 25 >= action){packages.add(_package);}
        else if (action >= 25 && 50 >= action){packages.add(_package); packages.add(_package);}
        else if (action >= 50 && 75 >= action){packages.add(corruptData(_package));}
        else if (action >= 75 && 100 >= action){ /* Discard message */}

    }



    //Corruption methods

    /**
     * Corrupts the data in packet
     * @param _package original packet
     * @return corrupted packet
     */
    private DatagramPacket corruptData(DatagramPacket _package)
    {
        String input = new String(_package.getData());

        String returnValue;

        if(random.nextBoolean())
            returnValue = disarrange(input);
        else
            returnValue = randomDiscard(input);

        byte[] changedData = returnValue.getBytes();

        _package.setData(changedData);
        return _package;
    }


    /**
     * Rearrange the data inside of the packet
     * Corrupts it.
     * @param _input
     * @return
     */
    private String disarrange(String _input)
    {
        char[] input = _input.toCharArray();
        int swapIndex;

        for(int i = 0; i<input.length; i++)
        {
            swapIndex = random.nextInt(input.length);
            char swapTemp = input[i];
            input[i]= input[swapIndex];
            input[swapIndex] = swapTemp;
        }

        return String.valueOf(input);
    }

    /**
     * Discard some of the data inside the packet
     * @param input original packet
     * @return corrupted packet
     */
    private String randomDiscard(String input)
    {
        int interval = input.length();
        int substringStart = random.nextInt(interval);
        int substringEnd = random.nextInt(interval);

        if(substringStart>substringEnd)
        {
            int swapTemp = substringStart;
            substringStart = substringEnd;
            substringEnd = swapTemp;
        }
        //Avoid 100% deletion

        return input.substring(substringStart, substringEnd);
    }

    /**
     * Returns a random packet from the queue
     * @return
     */
    private DatagramPacket getRandomPackage()
    {
        int randomIndex = random.nextInt(packages.size());
        DatagramPacket _package = packages.get(randomIndex);
        packages.remove(randomIndex);
        return _package;
    }
}
