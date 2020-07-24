import java.io.File
import java.net.{URL, UnknownHostException}

import scalaj.http.{Http, HttpRequest}

import scala.language.postfixOps
import scala.sys.process._

/**
  * Trait (interface) responsible for fetching auction data online.
  */
sealed trait DataFetcher[A] {
  val key = "hjwsp3v5qnz3kb8agbyn5cfu5ppebdvs" //API KEY TODO HIDE
  val base= "https://eu.api.battle.net"        //Base URL
  val postUrl= s"?locale=en_GB&apikey=$key"

  /**
    * Fetches data from a url using a HTTP client.
    * @param url of the target address.
    * @return content of result.
    */
  def getByUrl(url: String): Option[A]

  /**
    * Checks whether an internet connection is present.
    * @param url URL to service used.
    * @return True if service is available.
    */
  def isConnected(url: String): Boolean
}

/**
  * Implementation of [[DataFetcher]] responsible for fetching item data from blizzards API.
  */
object DataFetcher extends DataFetcher[String] {

  /**
    * Fetches a given item name based on its id.
    * @param id of item.
    * @return name of item.
    */
  def getItemName(id: Int): Option[String] = {
    val api = s"$base/wow/item/$id$postUrl"
    val jsonResponse = getByUrl(api)

    // Match with regex
    val namePattern = "\"name\":\"([a-z-A-Z0-9_ ]+)\"".r
    jsonResponse match {
      case Some(response) => namePattern.findFirstMatchIn(response) match {
        case Some(name) => Some(name.group(1))
        case _ => None
      }
      case _ => None
    }

  }

  /**
    * Fetches data from a url using a HTTP client.
    * @param url of the target address.
    * @return content of result.
    */
  override def getByUrl(url: String): Option[String] = {
    val request : HttpRequest = Http(url)
    try {
      val response = request.asString
      Some(response.body)
    } catch {
      case _: UnknownHostException => None
      case _: Exception => None
    }
  }

  /**
    * Checks whether an internet connection is present.
    *
    * @param url URL to service used.
    * @return True if service is available.
    */
  override def isConnected(url: String): Boolean = {
    try {
      val URL = new URL(url)
      val connection = URL.openConnection()
      connection.connect()
      connection.getInputStream.close()
      true
    } catch {
      case _: Exception => false
      }
  }

  /**
    * Helper function used to download auction JSON in data folder.
    * URL: http://www.appnow.dk/api/datamining/fetch.php
    *
    * @param url Path to URL containing json data.
    */
  def downloadAuctionJSON(url: String, filename: String, shouldDownload: Boolean): Unit = {
    if(shouldDownload)
    new URL(url) #> new File(s"./data/$filename") !!
  }
}

