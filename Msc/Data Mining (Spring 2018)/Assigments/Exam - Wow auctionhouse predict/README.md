**Predicting prices and trends in the virtual economy of World of Warcraft**

**The program has two dependencies that must be downloaded before running:**

	1) sbt (scala build tools) -> https://www.scala-sbt.org/download.html
	
	2) spark -> https://spark.apache.org/downloads.html 

**How to run the program:**

	1) 	Navigate to the folder including README.md (this file) and build.sbt in terminal
	
	2) 	Use "sbt run" to run the program. All other dependencies in the code are 
		downloaded automaticly

	
**Fetching new data**

	1)	Delete all the data within the "data" folder without deleting the folder itself
	
	2)	Run the program as usual
	
**What does the program do**

	Main Entry in AuctionApp.scala
	
	1)	Downloads auction data
	
	2)	Preprocess the auctions
	
	3)	Runs Apriori
	
	4)	Runs ANN
	
	5)	Runs Decision Tree
	
	6)	Runs Random Forest
	
