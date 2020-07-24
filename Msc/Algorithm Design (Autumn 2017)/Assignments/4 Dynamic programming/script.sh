#!/bin/sh
javac Sq.java
java Sq < data/HbB_FASTAs-in.txt > our-out.txt
sort our-out.txt > file1.sorted
sort data/HbB_FASTAs-out.txt > file2.sorted
diff -b file1.sorted file2.sorted
	