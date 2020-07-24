package Task_1;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.SocketException;

/**
* This class is a representation of a UDP server
*/
public class UDPServer{
    public static void main(String args[]){ 
    	DatagramSocket aSocket = null;
		try{
	    	aSocket = new DatagramSocket(7007);
					// create socket at agreed port
			byte[] buffer = new byte[1000];
 			while(true){
 				DatagramPacket request = new DatagramPacket(buffer, buffer.length);
  				aSocket.receive(request);     
    			DatagramPacket reply = new DatagramPacket(request.getData(), request.getLength(), 
    				request.getAddress(), request.getPort());
                System.out.println("UDP packet from: " + new String(request.getAddress().toString()));	
    			aSocket.send(reply);
    		}
		}catch (SocketException e){System.out.println("Socket: " + e.getMessage());
		}catch (IOException e) {System.out.println("IO: " + e.getMessage());
		}finally {if(aSocket != null) aSocket.close();}
    }
}
