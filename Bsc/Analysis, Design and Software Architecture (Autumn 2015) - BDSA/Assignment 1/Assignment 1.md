This week's assignment constitutes some short questions to verify your understanding of chapter 1 of the [OOSE] book, and a more extensive C# exercise to get you up and running with Visual Studio, C#, and unit testing. Hand-in a text file with the answers to the listed questions, and a zip file containing the full C# solution. Follow the instructions specified on the assignments page on how to hand in the assignment.

Introduction to Software Engineering:

Solve the following exercises in the [OOSE] book: 1-6, 1-8

C# Unit Testing:

Install Visual Studio Enterprise 2015 on Windows 10, which you can find on DreamSpark. Your DreamSpark account is your ITU email and password. If you're on a Mac, run Windows in either Bootcamp or Parallels/VMware Fusion.

Download the C# solution attached to this assignment.

For each of the projects within the solution, identify whether it is a (1) plain class library, (2) application (executable), or (3) test project.
Modify the code in the class libraries so that all tests in the test projects pass. Start with 'Introduction', and then move on to 'IsPowerOf'. Use recursion to implement the IsPowerOf method, thus IsPowerOf should call itself as part of the implementation. Recall the modulo operator, which provides you with the remainder after division. Hint: 8 is a power of 2 since 8/2 = 4 and 4/2=2 and 2/2 = 1.
Next, you will have to write your own unit tests for existing code. The code you will be testing is an early implementation of a parser, part of the project later down the course. It allows you to convert BibTex string entries (a format to describe publications) into a strongly-typed data model. You can reuse and extend on this (but are not required to) as part of the project.

Create a new test project for BibliographyParser, and write unit tests providing 100% coverage for DefaultFieldChecker, DefaultItemChecker, FieldValidator, ItemValidator, and BibTexParser. Code coverage can be checked within the test explorer. Think about which tests are most meaningful (where could an implementation fail most likely). When writing tests for a class, only test for behavior implemented within that class. Thus, test the different 'layers' of code independently. Add the "bibtex.bib" file as a resource to the test project, and load it as part of the unit tests. A code snippet to help you with this is provided here.
Stick to a structure and naming convention to organize your tests. (e.g. the one used in the other testing projects).
 
Assignment 1 - Week35.zip Assignment 1 - Week35.zip
