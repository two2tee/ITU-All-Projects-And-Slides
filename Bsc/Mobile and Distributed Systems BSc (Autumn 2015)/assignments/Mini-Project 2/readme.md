# Mini project 2
A system for communication. 

You must implement in Java two kinds of processes: a source and a sink.

A source process must repeatedly input lines of text from System.in.

Whenever some source process reads a line, every sink process must write that line on System.out.

It is up to you to find a way to communicate lines read from sources to sinks. 

Your system must have this functionality no matter the number of sources and sinks. That is, if you run two sources and five sinks, and one of the sources reads the line "Hello!", then all five sinks must write the line "Hello!". 

Your system must support both sources and sinks entering and exiting the system. E.g., this should be a valid series of events:

- Source process "A1" starts
- Source process "A2" starts
- Sink process "B1" starts
- A1 inputs "Message 1" [i.e., you type "Message 1"]
- B1 outputs "Message 1"
- Sink process "B2" starts
- A2 inputs "Message 2"
- B1 outputs "Message 2"
- B2 outputs "Message 2"
- A2 terminates
- Source process "A3" starts. 
- B1 terminates
- Sink process B3 starts
- A3 inputs "Message 3"
- B2 outputs "Message 3"
- B3 outputs "Message 3"

Note that B2 is not expected to write the message "Message 1". Output of the same message is in principle concurrent, so the order of B1 and B2 outputting "Message 2" can be reversed.

It is not okay to give every starting process the IP and port of every other process on the command line. It is okay to always give some fixed number, say, always give 2 IP/ports.

## Questions

Besides implementing such processes, you must also answer the following questions (justify your answers, 1-2 paragraphs each; shorter is better). 

Are your processes Web Services?
Is your system time and/or space de-coupled?
Is your system a message queue?
Is your system a publish/subscribe system?
What are the failure modes of your system?
Rules

Submit in groups as in the previous projects. Your expected load is 16 hours/person (10 hours pr. person pr. week for two weeks; substracting lectures and reading).

Your submission should comprise a single .zip-file containing:

Source code. You can use languages different from Java, but must then expect less support from teacher/TAs. Please clear exotic languages with course manager before beginning. 
A file feedback.txt very briefly reporting your major obstacles and a summary of your time consumption (hours pr. group member, anonymized).
A file answers.txt answering the above questions.
Hints.

Sections 6.3-6.4 in the book might help.
Feel free to add more kinds of processes--my solution has a third kind of process. 
This exercise has a really simple solution. If in doubt, yours is too complicated.
Use only the Java Socket API. Anything more complicated (in particular various Java EE libraries) will likely require you to spend time learning the library and, potentially, understanding JavaBeans and the Java EE programming model. That's not the point of this project. 