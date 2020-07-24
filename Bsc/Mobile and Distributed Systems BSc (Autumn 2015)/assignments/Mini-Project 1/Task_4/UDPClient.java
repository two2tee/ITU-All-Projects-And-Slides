package Task_4;

import java.io.IOException;
import java.net.*;
import java.util.Scanner;

/**
 * This class creates a UDPClient sending user input from the terminal to a UDPServer with an agreed port.
 */
public class UDPClient
{

	private static final int BUFFER_SIZE = 1000;
	private static final int TIMEOUT_SIZE = 5000;
	private static final int MAX_SIZE = 255; // chars

	public static void main(String args[])
	{
		UDPClient client = new UDPClient();
	}


	public UDPClient()
	{
		initialize();
	}


	//Client methods

	/**
	 * Initialize the system
	 */
	private void initialize(){
		try
		{
			String[] packet = setPacket();
			if (validatePacket(packet))
			{
				send(packet);
			}
		}
		catch (Exception e)
		{
			System.out.println(e.getMessage());
			System.out.println("Something went wrong...");
			resetClient();
		}
	}

	/**
	 * Method used to restart the client.
	 */
	private void resetClient()
	{
		System.out.println("\n---Resetting client...");
		System.out.println("\n");
		initialize();
	}

	//Packet methods

	/**
	 * Create a packet with host address, port and message (with hashcode) from user input.
	 * @return array with packet message.
	 */
	private String[] setPacket() throws IllegalAccessException
	{

		System.out.println("Please specify receiver address, port and message. Separate the input with |  " +
				"\n Eg: 192.0.0.1|8080|message1|message2 " +
				"\n Waiting for user input:");

		Scanner scanner = new Scanner(System.in);
		if (scanner.hasNext())
		{
			String input = scanner.nextLine();
			String[] packetData = input.split("\\|");

			if (isUserInputValid(packetData))
			{
				for (int i = 2; i < packetData.length; i++)
				{
					String tempMessage = packetData[i];
					String messageWithHashCode = new String(tempMessage + "$" + tempMessage.hashCode()+"$");
					packetData[i] = messageWithHashCode;
				}
				return packetData;
			}
			else throw new IllegalArgumentException("Entered port or address is invalid.");
		}
		else return null;
	}

	/**
	 * Sends packet to host server.
	 * @param inputData including address, port and message.
	 */
	private void send(String[] inputData)
	{
		DatagramSocket aSocket = null;
		String address = inputData[0];
		int port = Integer.parseInt(inputData[1]);
		int tryCount = 3;
		int messageCount = inputData.length-2;

		for(int i = 2; i < inputData.length ; i++)
		{
			System.out.println("\nSending message " + (i-1) + "." );

			String message = inputData[i];

			try
			{
				aSocket = new DatagramSocket();
				byte[] m = message.getBytes();
				InetAddress aHost = InetAddress.getByName(address);
				DatagramPacket request = new DatagramPacket(m, message.length(), aHost, port);

				aSocket.send(request);

				//Check if server has received datagram
				System.out.println("Waiting for host...");
				boolean packetOK = isReceived(aSocket);

				if(!packetOK && tryCount > 0)
				{
					i--;
					tryCount--;
				}

				else if (!packetOK && tryCount == 0)
				{
					if(inputData.length < 3)
					{
						messageCount--;
						System.out.println("Message could not be sent.");
					}
					else if (i < inputData.length)
					{
						messageCount--;
						tryCount = 3;
						System.out.println("\nMessage " + (i - 1) + " could not be sent. Skipping to next message.");
					}
				}
				else if(packetOK) System.out.println("Success.");

			}
			catch (SocketException e) 	{System.out.println("Socket: " + e.getMessage());}
			catch (IOException e)		{System.out.println("IO: " + e.getMessage());}
			finally {
				if (aSocket != null) aSocket.close();
			}
		}

		if(messageCount < inputData.length-2)
		{
			System.out.println("\nSome messages could not be sent.");
		}

		resetClient();

	}



	//Validation methods

	/**
	 * Checks if address and port input is valid.
	 * @param packetData - array with address, port and message.
	 * @return true if address and port is valid.
	 */
	private boolean isUserInputValid(String[] packetData)
	{
		if(packetData.length < 3 ) return false;
		String address = packetData[0];
		int port = Integer.parseInt(packetData[1]);

		int addressPeriodsCount = address.length() - address.replace(".", "").length();

		if (packetData.length < 3)
		{
			return false;
		}
		else if(address.equals("localhost") && (port > 0 || port <= 65535))
		{
			return true;
		}
		else
		{
			return (addressPeriodsCount == 3 ||addressPeriodsCount == 2 ) && (port > 0 || port <= 65535);
		}
	}

	/**
	 * Checking if host has received sent message and also if it was non-corrupt when it arrived
	 * @return
	 * @throws IOException
	 */
	private boolean isReceived(DatagramSocket aSocket) throws IOException
	{
		byte[] buffer = new byte[BUFFER_SIZE];
		DatagramPacket reply = new DatagramPacket(buffer, buffer.length);
		try
		{
			aSocket.setSoTimeout(TIMEOUT_SIZE);
			aSocket.receive(reply);

			if(reply.getAddress() == null) return false;

			int validationNumber = Character.getNumericValue(new String(reply.getData()).charAt(0)); //Get first char and convert to int
			if(validationNumber == 0) System.out.println("Sent message was corrupt.");

			//Packed was successfully sent if host returned 1
			return validationNumber == 1;
		}
		catch (SocketTimeoutException e)
		{
			System.out.println("Timeout.");
			return false;
		}
		catch (IOException e)
		{
			e.printStackTrace();
		}
		return false;
	}

	/**
	 * Validate if a packet is not null and also that it doesn't contains messages with a size greater than allowed.
	 * @param packet
	 * @return boolean
	 */
	private boolean validatePacket(String[] packet)throws NullPointerException
	{

		if(packet == null)
		{
			throw new NullPointerException("Packet was null");
		}

		for(int i = 2 ; i< packet.length ; i++)
		{
			if(packet.length > MAX_SIZE)
			{
				throw new StringIndexOutOfBoundsException("Message " + (i-1)+": " + "'" + packet[i] + "'" + "is to long");
			}
		}
		return true;
	}

}
