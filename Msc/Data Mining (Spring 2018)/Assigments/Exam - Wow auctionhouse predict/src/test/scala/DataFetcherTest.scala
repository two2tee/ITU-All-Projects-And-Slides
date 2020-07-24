import org.scalatest.FunSuite

/**
  * DataFetcher Test Suite used to test that the API can fetch content online and retrieve auction items.
  */
class DataFetcherTest extends FunSuite {

  test("ItemFetcher.getByUrl") {
    // Arrange
    val url = "http://www.google.com"

    // Act
    val response = DataFetcher.getByUrl(url)

    // Assert
    assert(response != "")
  }

  test("ItemFetcher.getItemName") {
    // Arrange
    val itemID = 82800

    // Act
    val response = DataFetcher.getItemName(itemID)

    // Assert
    assert(response == Some("Pet Cage"))
  }


}
