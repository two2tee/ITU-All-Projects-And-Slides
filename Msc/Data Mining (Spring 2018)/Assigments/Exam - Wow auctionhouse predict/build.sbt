
// Project information
name := "DataMiningProject"
version := "0.1"
scalaVersion := "2.11.12"

// Test dependencies
libraryDependencies += "org.scalatest" %% "scalatest" % "3.0.5" % "test"
libraryDependencies += "org.scalacheck" %% "scalacheck" % "1.13.4" % "test"

// Spark dependencies
// TODO: downgraded from 2.3.0 due to compile exception: https://issues.apache.org/jira/browse/SPARK-23986
val sparkVersion = "2.2.1"

libraryDependencies ++= Seq(
  "org.apache.spark" %% "spark-core" % sparkVersion,
  "org.apache.spark" %% "spark-sql" % sparkVersion,
  "org.apache.spark" %% "spark-mllib" % sparkVersion,
  "org.apache.spark" %% "spark-streaming" % sparkVersion,
  "org.apache.spark" %% "spark-hive" % sparkVersion
)

// Simplified Http dependencies: https://github.com/scalaj/scalaj-http
libraryDependencies +=  "org.scalaj" %% "scalaj-http" % "2.3.0"

// Increase Java Heap memory
val buildSettings = Defaults.defaultConfigs ++ Seq(
  javaOptions += "-Xmx8G"
)

// Display warnings
scalacOptions ++= Seq("-unchecked", "-deprecation", "-feature")