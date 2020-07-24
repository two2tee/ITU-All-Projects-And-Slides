import Interfaces.ISink;

import java.io.DataInputStream;
import java.io.IOException;
import java.net.ConnectException;
import java.net.Socket;
import java.net.SocketException;
import java.net.UnknownHostException;


/**
 * This class is responsible for receiving incoming messages from a given server,
 * through subscription.
 */
public class Sink implements ISink {

    private Socket sinkSocket;

    private Sink()
    {
        setOnTerminateEvent();
        initiate();
    }

    public static void main(String[] args) {
        System.out.println("Ready to sync... Enter Server IP Address...");
        Sink sink = new Sink();
        sink.listen();
    }

    /**
     * Initialize a connection to a given host (IP-address and port)
     */
    private void initiate(){
        try {
            String ipAddress = System.console().readLine();
            sinkSocket = new Socket(ipAddress, 7000); //auto-subscribe when new connection is made.
            System.out.println("Connected to "+ ipAddress);
        }
        catch(UnknownHostException l)
        {
            System.out.println("Invalid IP. Please try again");
            initiate();
        }
        catch(ConnectException c)
        {
            System.out.println("No server found with the specified ip");
            initiate();
        }
        catch(IOException m){
            m.printStackTrace();
        }
    }

    /**
     * Display a given message in the console.
     * @param message
     */
    public void display(String message) {
        System.out.println(message);
    }

    /**
     * Listens for any messages sent from a host that the sink is subscribed to.
     */
    @Override
    public void listen() {
        System.out.println("Listening...");
        DataInputStream input;
        try {
            while(true)
            {
                input = new DataInputStream(sinkSocket.getInputStream());
                String s = input.readUTF();
                display(s);
            }
        }
        catch( SocketException m){
            System.out.println("Disconnected from server.");
            initiate();
        }
        catch (IOException e) {
            e.printStackTrace();
        }
        finally {
            if(sinkSocket != null){
                try {
                    sinkSocket.close();
                } catch (IOException e) {
                    //nothing to do
                }
            }
        }
    }

    /**
     * EventHandler that closes the connection to a host when the program is terminated
     */
    private void setOnTerminateEvent()
    {
        Runtime.getRuntime().addShutdownHook(new Thread(() -> {
            System.out.println("Sink terminated");
        }));
    }
}
