package sentient

import (
	"fmt"
	"cloud.google.com/go/bigquery" //go get -u cloud.google.com/go/bigquery to install lib dependencies
	"google.golang.org/api/iterator"
	"google.golang.org/appengine"
	"net/http"
)

// query returns a slice of the results of a query.
func query(proj, mgrs string, r *http.Request) (*bigquery.RowIterator, error) {
	ctx := appengine.NewContext(r) //context.Background()

	client, err := bigquery.NewClient(ctx, proj)
	if err != nil {
		fmt.Println("No Client Error!!!")
		return nil, err
	}
	fmt.Println("Client created")

	query := client.Query("SELECT base_url FROM `bigquery-public-data.cloud_storage_geo_index.sentinel_2_index` WHERE `mgrs_tile` = '" + mgrs + "';")
	query.QueryConfig.UseStandardSQL = true
	return query.Read(ctx)
}

//dttn-178408 //TODO this should not be here on real production code
//nisch-179108 //TODO this should not be here on real production code
func GetImages(mgrs string,r *http.Request) []string {
	res, err := query("dttn-178408", mgrs,r)
	if err != nil {
		panic(err)
	}

	return getResults(res)
}

func getResults(iter *bigquery.RowIterator) []string {
	var urls []string
	for {
		var row []bigquery.Value

		err := iter.Next(&row)
		if err == iterator.Done {
			return urls
		}
		if err != nil {
			panic(err)
		}

		ts := row[0].(string)
		urls = append(urls, ts)
	}
}
