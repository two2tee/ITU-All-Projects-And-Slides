# Mini project 1

1. Implement an RFC862 server adhering to the RFC 862 specification. Use port 7007 instead of 7. You need only implement the UDP part ("UDP Based ...”). 
Submit your solution as a single Java-file RFC862.java.

2. Write a drop-in replacement for DatagramSocket which randomly discards, duplicates or reorders a given percentage of sent datagrams. Inherit from DatagramSocket. Submit your solution as a single Java-file QuestionableDatagramSocket.java.

3. Write programs to estimate UDP datagram loss. Your programs must accept as input (a) datagram size, (b) number of datagrams sent and (c) interval between transmissions. The program must output (1) absolute number and percentage of lost datagrams and (2) absolute number and percentage of duplicated datagrams. It is acceptable if your estimate cannot distinguish between losing a single reply, losing a single response, or losing both a reply and a response.

Use your loss estimator to demonstrate:

- Datagram loss on a local connection on a single machine (i.e., no physical net).
- Datagram loss on Wifi. 
- Datagram loss on ethernet.
- Datagram loss on the Internet (i.e., transmitting across multiple physical nets).
- Indicate for each of these four cases the parameters (a-c above) you used to elicit the loss and the observed lossage (i-ii above). Explain where and why you expect the loss to be happening. 

Submit java source file(s) and a .txt file briefly summarising your findings.

4. Write programs to reliably communicate over UDP. Using only DatagramSocket, write programs A,B which together reliably transmits a string of length less than 255 characters. Program A accepts at startup a destination ip and port and a string, then transmits the string, somehow ensures that the string correctly arrived, says so and terminates. Program B repeatedly receives such strings and prints them. Your transmission mechanism must guarantee that for each invocation of Program A with string S, Program B prints S exactly once. The server must handle multiple concurrent clients.

If you decide this is impossible, explain why and make your best approximation.

Submit java source files and optionally a .txt file briefly summarising your impossibility argument.

## Rules.

You must submit in groups of size at least 4; your submission must be in the form of single .zip archive. Note that approval of your submission is a prerequisite for attending the examination. 

If you like, write your programs in a different language. If you do, you must of course translate "using only DatagramSocket” appropriately. If your chosen language is not one of C, C++, C#, F#, Java, Scala, Ruby, Python, OCaml or Haskell, please contact me before beginning.

Your submission must take the form of a single zip-archive  named "group-X.zip" where X is your group number; the archive must contain a single directory "group-X" containing the above-mentioned source- and text-files. Submissions not on this form will be rejected without further review. 

## Hints.

Design your solutions as a group, then split out to implement. Re-convene as a group when your design comes apart.
Don't do anything with threads requiring synchronisation if you can at all avoid it.
An echo-server is running at tiger.itu.dk:7. 
One quick and dirty way to achieve serialisation is using ByteBuffer (see wrap).
Google and Stackoverflow are your friends when you are wondering how to do something specific in Java, say, how to convert a string to a byte array or how to interrupt a thread. Check dates, though: Some popular answers on stackoverflow have been obsoleted by subsequent java releases.