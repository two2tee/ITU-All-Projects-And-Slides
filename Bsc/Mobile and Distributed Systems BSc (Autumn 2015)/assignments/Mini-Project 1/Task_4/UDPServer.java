package Task_4;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.util.HashMap;

/**
 * This class creates a UDP server using a DatagramSocket to receive packets from a Task_4.UDPClient.
 */
public class UDPServer
{
	private String[] dividedMessage;
	private DatagramPacket request;
	private DatagramSocket aSocket = null;
	private HashMap<InetAddress, Integer> IPMessageMap;
	private final int BUFFER_SIZE = 1000;

	public static void main(String[] args)
	{
		UDPServer server = new UDPServer();
		server.initialize();
	}

	/**
	 * Initialize server and wait for input from other clients.
	 */
	private void initialize(){
		try
		{


			IPMessageMap = new HashMap<>();
			aSocket = new DatagramSocket(7007);
			// create socket at agreed port
			byte[] buffer = new byte[BUFFER_SIZE];
			System.out.println("Server process started...");
			while(true)
			{
				request = new DatagramPacket(buffer, buffer.length);
				aSocket.receive(request);
				dividedMessage = new String(request.getData()).split("\\$");

				if (isRequestValid())
				{
					handleValidMessage();
				}
				else
				{
					String failedMessage = "0";
					aSocket.send(new DatagramPacket(failedMessage.getBytes(), failedMessage.length(),
							request.getAddress(), request.getPort()));
				}
			}
		}
		catch (SocketException e){System.out.println("Socket: " + e.getMessage()); }
		catch (IOException e) {System.out.println("IO: " + e.getMessage());}
		finally {if(aSocket != null) aSocket.close();}
	}

	/**
	 * Checks whether the packet address is valid.
	 * @return true if address is valid.
	 */
	private boolean isRequestValid()
	{
		if(IPMessageMap.get(request.getAddress()) != null)
			return dividedMessage[0].hashCode() == Integer.parseInt(dividedMessage[1])
					&& IPMessageMap.get(request.getAddress()) != request.hashCode();
		else
			return dividedMessage[0].hashCode() == Integer.parseInt(dividedMessage[1]);
	}

	/**
	 * Prints out packet information (address, port number and message) if the message is valid.
	 * A message is sent back to the client if packet is received successfully.
	 */
	private void handleValidMessage()
	{
		try
		{
			IPMessageMap.put(request.getAddress(), request.hashCode());
			System.out.println("UDP packet from:  " + new String(request.getAddress().toString()));
			System.out.println("Port:" + new String(String.valueOf(request.getPort())).toString());
			System.out.println("Message: " + new String(dividedMessage[0]));
			System.out.println("Packet:" + request);
			String approvedMessage = "1";
			aSocket.send(new DatagramPacket(approvedMessage.getBytes(), approvedMessage.length(),
					request.getAddress(), request.getPort()));
		}
		catch(IOException e)
		{
			e.printStackTrace();
			System.out.println("");
		}
	}
}
