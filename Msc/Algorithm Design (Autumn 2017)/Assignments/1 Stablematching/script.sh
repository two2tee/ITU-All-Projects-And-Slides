#!/bin/sh
for FILE in *-in.txt

do
	echo $FILE
	base=${FILE%-in.txt}
    time java GS < $FILE > $base.results.out.txt # replace with your command!
	sort $base-out.txt > file1.sorted
	sort $base.results.out.txt > file2.sorted
	diff -b file1.sorted file2.sorted
	
done