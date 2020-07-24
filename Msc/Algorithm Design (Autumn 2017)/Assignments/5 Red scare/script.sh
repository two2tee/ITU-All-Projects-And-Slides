#!/bin/sh
rm output.txt
for FILE in data/*.txt
do
	echo $'\n' $FILE >> output.txt
	java Redscare < $FILE >> output.txt
done
