package Task_3;

import java.io.IOException;
import java.net.*;
import java.util.HashSet;

/**
 * The estimator  is used to estimate the amount of datagrams, received, lost and duplicated.
 * It will also show the percentage of the anomalies
 */
public class Estimator
{
    static HashSet<String> receivedPackets = new HashSet<>(); //Received messages. Used to keep count on duplicate messages
    String[] messageList; //Stores all sent messages.
    int receivedPacketCounter;

    SendingThread sender;         //Sending thread. Splitting receiver and sender in two different threads
    String fillerData;            //Filler data used to append package data to adjust package size
    boolean SendingThreadIsStillActive = true;

    DatagramSocket sendingSocket; //Sending socket
    String IPDestination = "localhost";
    int sendingToPort = 7;

    public static void main(String[] args)
    {
        System.out.println("Opgave 3 start");
        System.out.println("Transmitting packages");
        Estimator program = new Estimator(1,200,5, true);

        program.calculateAll();
    }

    /**
     * Constructer of estimator class.
     * @param datagramSize
     * @param amountOfDatagramsSent
     * @param transmissionInterval
     * @param sendingToLocalHost
     */
    public Estimator(int datagramSize, int amountOfDatagramsSent, int transmissionInterval, boolean sendingToLocalHost)
    {
        fillerData = createFillerData(datagramSize);

        try
        {
            sendingSocket = new DatagramSocket();
            sender = new SendingThread(sendingSocket, transmissionInterval,amountOfDatagramsSent);
            Thread senderThread   = new Thread(sender);
            senderThread.start();

            if (sendingToLocalHost)
            {
                IPDestination = "localhost";
                receivePackets(new DatagramSocket(sendingToPort));
            }
            else{
                receivePackets(sendingSocket);}

        }
        catch (SocketException e) {e.printStackTrace();}
        catch (IOException e) {e.printStackTrace();}
    }


    //Packet methods

    /**
     * Constantly receives packets while thread is still alive.
     * It will also update the number of received messages
     * @param socket
     */
    private void receivePackets(DatagramSocket socket)
    {
        try
        {
            socket.setSoTimeout(5000);
            byte[] buffer = new byte[1000];
            while(SendingThreadIsStillActive)
            {
                DatagramPacket reply = new DatagramPacket(buffer, buffer.length);

                socket.receive(reply);
                String message = new String(reply.getData());
                updateOccurrenceRatings(message);
            }
        }
        catch (SocketTimeoutException e ){}
        catch (SocketException e) {e.printStackTrace();}
        catch (IOException e) {e.printStackTrace();}
    }

    /**
     * Returns the number of received packets
     * @return interger
     */
    public int getReceivedPacketCounter() {return receivedPacketCounter;}

    /**
     * increment number of received packet and store the data in the packet
     * @param reply
     */
    private void updateOccurrenceRatings(String reply)
    {
        receivedPacketCounter++;
        receivedPackets.add(reply);
    }

    /**
     * create  filler data to be used when sending packet
     * @param memorySize
     * @return
     */
    private String createFillerData(int memorySize)
    {
        //Minimum String memory usage (bytes) = 8 * (int) ((((no chars) * 2) + 45) / 8)
        //int memorySize = 16+ ((((x) * 2) + 45) / 8);

        int x = ((memorySize-16)*8-45)/2 ;
        String _filerData = "";

        if(x>0)
        {
            char[] fillerData = new char[x];
            for(int i =0; i<fillerData.length; i++)
            {
                fillerData[i] = 'a';
            }

            _filerData = new String(fillerData);
        }

        return _filerData;
    }



    //Calculate methods

    /**
     * Calculate amount of datagram, received, lost and duplicated and print the results
     */
    private void calculateAll(){
        System.out.println("Amount of datagrams received: " + receivedPackets.size());
        System.out.println("Amount of datagrams lost: " + amountOfLostDatagrams());
        System.out.println("Amount of datagrams lost in percentage: " + amountOfLostDatagramsInPercentage() + " %");
        System.out.println("Amount of datagrams duplicates: " + amountOFDuplicateDatagram());
        System.out.println("Amount of datagrams duplicates in percentage: " + amountOFDuplicateDatagramInPercentage() + " %");
    }

    /**
     * Returns the number of lost packets
     * @return integer
     */
    public int amountOfLostDatagrams()
    {
        return messageList.length- receivedPackets.size();
    }

    /**
     * Returns the percentage of lost packets
     * @return float
     */
    public double amountOfLostDatagramsInPercentage()
    {
        if(receivedPackets.size() != 0)
            return ((((double)amountOfLostDatagrams())/(double)messageList.length)*100);
        else
            return 100; // percent
    }

    /**
     * Returns the number of duplicated packets
     * @return integer
     */
    public int amountOFDuplicateDatagram()
    {
        return receivedPacketCounter - receivedPackets.size();
    }

    /**
     * Returns the percentage of duplicated packets
     * @return double
     */
    public double amountOFDuplicateDatagramInPercentage()
    {
        if(receivedPackets.size()!= 0)
            return (((double) amountOFDuplicateDatagram())/ receivedPackets.size())*100;
        else
            return 0;
    }



//Inner class

/**
     * Innerclass that represents a sending thread, that allows packets to be sent in parallel with other processes
     */
public class SendingThread implements Runnable{

        DatagramSocket socket;
        int _timeOut;


        public SendingThread(DatagramSocket socket, int timeOut, int amountOfMessagesTOBeSent)
        {
            this.socket = socket;
            messageList  = new String[amountOfMessagesTOBeSent];
            _timeOut = timeOut;

            //Create filler data used to append package data to adjust package size
            for(int i = 0; i<messageList.length; i++)
            {
                if(i>9)
                    messageList[i]=i+fillerData;
                else
                    messageList[i] ="0"+i+fillerData;
            }

        }

        public String[] getMessageList()
        {
            return messageList;
        }


        @Override
        public void run()
        {

            try {Thread.sleep(1000);}   //Initial sleep. Gives the receiver time to start
            catch (InterruptedException e) {e.printStackTrace();}

            byte[] message;
            String _message;

            for (int i = 0; i<messageList.length; i++)
            {
                message = messageList[i].getBytes();
                _message = messageList[i];

                try
                {
                    InetAddress host = InetAddress.getByName(IPDestination);
                    DatagramPacket packet = new DatagramPacket(message, _message.length(), host, sendingToPort);
                    socket.send(packet);
                    Thread.sleep(_timeOut);
                }
                catch (SocketException e) {e.printStackTrace();}
                catch (UnknownHostException e) {e.printStackTrace();}
                catch (IOException e) {e.printStackTrace();} catch (InterruptedException e) {
                    e.printStackTrace();
                }
                try {Thread.sleep(_timeOut*10);}
                catch (InterruptedException e) {e.printStackTrace();}
            }

            //All messages has been sent. Signal listener to close
            SendingThreadIsStillActive = false;

        }
    }

}
