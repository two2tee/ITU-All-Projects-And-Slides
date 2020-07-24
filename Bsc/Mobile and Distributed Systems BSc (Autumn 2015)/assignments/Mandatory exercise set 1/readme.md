# Mandatory hand-in exercise set 1

You must submit your solutions to these exercises in groups of at least 4, the same groups you used for the mini-project. (Contact the course manager if you have compelling reasons to reconfigure groups.) Approval of your submission is a prerequisite for attending the examination. Your submission must be a single .txt or .pdf file. Submissions not on this form will be rejected without further review. 

## Exercise A. 
A client attempts to synchronise with a time server. It records the round-trip times and timestamps returned by the server in the table below. Which of these times should it use to set its clock? To what time should it set it? Estimate the accuracy of the setting with respect to the server's clock. If it is known that the time between sending and receiving a message in the system concerned is at least 8 ms, do your answers change?

Round-trip (ms)   Time (hr:min:sec)

22                        10:54:23.674

25                        10:54:25.450

20                        10:54:28.342

## Exercise B. 
An NTP server B receives server A's message at 16:34:23.480 bearing a timestamp 16:34:13.430 and replies to it. A receives the message at 16:34:15.725, bearing B's timestamp 16:34:25.7. Estimate the offset between B and A and the accuracy of the estimate.

## Exercise C. 
Consider diagram picture (diagram.png).

Diagram for Set 1, Exercise C.

- Write out the happens-before relation in the following diagram. (Immediate successors is fine; you don't have to write the entire transitive closure.)
- Identify 4 consistent cuts.
- Identify 4 inconsistent cuts.
- Write 2 different linearisations of the events in this diagram.

## Exercise D. 
Consider this distributed system: There are 3 processes; each process has a state consisting of a single number k. Whenever
a process receives a message, it updates its state k to a randomly chosen new number. If the state k of a process is even, it will transmit messages at random intervals; if odd, it does nothing.

1. Explain how this system may deadlock.
2. Explain how this system may deadlock even if you can make no assumptions on the initial state.
3. Explain how the snapshot algorithm can be used to discover if the system has deadlocked.
NB! Assume that a process in odd state will nonetheless send markers as required by the algorithm. Moreover, a process which receives a message containing only a marker does not update its state k.
4. Summarise the steps of the snapshot algorithm when executed on the system with initial global states 3, 4, 7 and no messages in transit. Will the resulting snapshot allow you to conclude that the system is deadlocked?
5. Summarise the steps of the snapshot algorithm when executed on the system with initial global states 3, 5, 7 and no messages in transit. Will the resulting snapshot allow you to conclude that the system is deadlocked?