import Interfaces.ISource;

import java.io.DataOutputStream;
import java.io.EOFException;
import java.io.IOException;
import java.net.Socket;
import java.net.UnknownHostException;

/**
 * Source is responsible for publishing messages
 */
public class Source implements ISource {

    private String ipAddress;
    private int portNumber;
    private boolean connected;


    private Socket socket;
    private DataOutputStream outputStream;

    private Source() {
        System.out.println("Please input IP-address:");
        ipAddress = System.console().readLine();
        portNumber = 7001;
        run();
    }

    public static void main(String[] args) {
        Source program = new Source();
    }

    void run()
    {
        System.out.println(
                "Following options are: \n" +
                        "Enter your message and press Enter:\n" +
                        "type 'exit' without the quotes to terminate the program \n" +
                        "type 'connect' without the quotes to connect to another server");
        try
        {
            while (true)
            {

                String userInput = System.console().readLine();
                inputInterpreter(userInput);
            }

        }
        catch (Exception e){System.out.println(e.getStackTrace());}
    }

    /**
     * Can take commands that can reconnect to another server, exit the program or a message to send to the server.
     *
     * @param message
     */
    private  void inputInterpreter(String message)
    {
        message = message.trim().toLowerCase();

        switch (message) {

            case "exit":
                terminate();
                disconnectFromServer();
                break;

            case "connect":
                //Disconnecting from old server
                disconnectFromServer();

                //New setup
                getNewConnectionSetup();

                //Connect to new server
                connectToServer();
                break;

            default:
                connectToServer();
                sendMessage(message);
                disconnectFromServer();
                break;
        }


    }

    /**
     * Get new server address and port from user
     */
    private void getNewConnectionSetup() {
        System.out.println("Change IP address to: ");
        ipAddress = System.console().readLine();
        System.out.println("Input port new portNumber¬l");
        portNumber = Integer.parseInt(System.console().readLine());
    }

    /**
     * Sends a user message to the server.
     * @param message string
     */
    public  void sendMessage(String message)
    {
        if(connected)
            try {
                outputStream.writeUTF(message);          // UTF is a string encoding see Sn. 4.4
                outputStream.flush();
                System.out.println("sending message: "+message);
            }catch (UnknownHostException e){System.out.println("Socket:"+e.getMessage());
            }catch (EOFException e){System.out.println("EOF:"+e.getMessage());
            }catch (IOException e){System.out.println(e.getMessage());
            }
        else
            System.out.println("Program is not connected, connect before sending messages!");
    }

    /**
     * Creates a connection to the server.
     */
    public  void connectToServer()
    {
        socket = null;
        try{
            socket = new Socket(ipAddress, portNumber);
            outputStream = new DataOutputStream(socket.getOutputStream());

            connected = true;
        }catch (UnknownHostException e){System.out.println("Socket:"+e.getMessage());
        }catch (EOFException e){System.out.println("EOF:"+e.getMessage());
        }catch (IOException e){System.out.println("readline:"+e.getMessage());
        }
    }

    /**
     * Disconnect from from a server
     */
    public  void disconnectFromServer()
    {
        try {
            outputStream.writeUTF("0");
            socket.close();
            socket = null;
            outputStream = null;
            connected = false;
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    /**
     * Shut down program
     */
    private void terminate() {
        System.exit(0);
    }
}