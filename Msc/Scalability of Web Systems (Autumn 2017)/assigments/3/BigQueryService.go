package sentient

import (
	"context"
	"fmt"
	"log"
	"net/http"
	"strconv"
	"cloud.google.com/go/bigquery" //go get -u cloud.google.com/go/bigquery to install lib dependencies
	"cloud.google.com/go/storage"
	"google.golang.org/api/iterator"
	"google.golang.org/api/option"
	"google.golang.org/appengine"
	"sync/atomic"
)

type Granule struct {
	baseUrl   string
	granuleId string
}

type Coords struct{
	lat1 string
	lat2 string
	lng1 string
	lng2 string
}



func GetPointImages(mgrs string, r *http.Request, id string, outChannel chan string) {
	user := getId(id)
	var querySql = "SELECT base_url, granule_id FROM `bigquery-public-data.cloud_storage_geo_index.sentinel_2_index` WHERE `mgrs_tile` = '" + mgrs + "';"
	res, err := query(user, querySql, r)
	if err != nil {
		panic(err)
	}
	getImageResults(res, r, outChannel)
}

func GetAreaImages(coords Coords, r *http.Request, id string, outChannel chan string) {
	granulesIter := GetGranulesIterator(coords,r,id)
	getImageResults(granulesIter, r, outChannel)
}

func GetGranulesIterator(coords Coords, r *http.Request, id string) *bigquery.RowIterator {
	var user = getId(id)

	upperLat, lowerLat := getUpperLower(coords.lat1, coords.lat2)
	upperLon, lowerLon := getUpperLower(coords.lng1, coords.lng2)

	var querySql = "SELECT base_url, granule_id " +
		"FROM `bigquery-public-data.cloud_storage_geo_index.sentinel_2_index` " +
		"WHERE north_lat <= " + upperLat +
		" AND south_lat >= " + lowerLat +
		" AND west_lon >= " + lowerLon +
		" AND east_lon <= " + upperLon
	res, err := query(user, querySql, r)
	if err != nil {
		log.Println("Query: " + querySql + ", upperLat: " + upperLat + ", lowerLat: " + lowerLat + ", upperLon: " + upperLon + ", lowerLon: " + lowerLon)
	}
	return res
}


func GetImageCount(iter *bigquery.RowIterator, r *http.Request, total *int32) {
	inChannel := make(chan *Granule, 52)
	go getGranuleResults(inChannel, iter) //Fetch granules from iterator

	for n := 0; n < 4; n++ {
		go countImagesForGranule(inChannel, total)
	}
}

func countImagesForGranule(inChannel chan *Granule, total *int32){
	countDelta := int32(0)
	for {
		_, isChannelOpen := <-inChannel
		if !isChannelOpen {
			atomic.StoreInt32(total,atomic.AddInt32(total,countDelta))
			return
		}
		countDelta = countDelta + 13
	}

}




func getId(id string) string {
	var user = ""
	if id == "dttn" {
		user = "dttn-178408"
	} else if id == "nisch" {
		user = "nisch-179108"
	} else {
		panic("invalid user")
	}

	return user
}

func query(proj, querySql string, r *http.Request) (*bigquery.RowIterator, error) {
	ctx := appengine.NewContext(r) //context.Background()

	log.Println("Running for ", proj)
	log.Println("Executed query: ", querySql)

	client, err := bigquery.NewClient(ctx, proj)
	if err != nil {
		log.Println("No Client Error!!!")
		return nil, err
	}
	log.Println("Client created")

	query := client.Query(querySql)
	query.QueryConfig.UseStandardSQL = true
	return query.Read(ctx)
}

func getUpperLower(coord1 string, coord2 string) (upper string, lower string) {
	parsed1, err1 := strconv.ParseFloat(coord1, 64)
	parsed2, err2 := strconv.ParseFloat(coord2, 64)

	if err1 != nil || err2 != nil {
		panic("Provided numbers are not coordinates")
	}

	if parsed1 > parsed2 {
		return coord1, coord2
	} else {
		return coord2, coord1
	}
}

func getImageResults(iter *bigquery.RowIterator, r *http.Request, outChannel chan string) {
	inChannel := make(chan *Granule, 10)
	go getGranuleResults(inChannel, iter)
	ctx := appengine.NewContext(r)
	client, err := storage.NewClient(ctx, option.WithoutAuthentication())
	if err != nil {
		panic(err)
	}
	for n := 0; n <= 4; n++ {
		go getImageUrlsFromGranules(inChannel, ctx, outChannel, client)
	}
}

func getGranuleResults(inChannel chan *Granule, iter *bigquery.RowIterator) {
	//var granules []Granule
	for {
		var row []bigquery.Value

		err := iter.Next(&row)
		if err == iterator.Done {
			close(inChannel)
			return
		}
		if err != nil {
			panic(err)
		}

		inChannel <- &Granule{row[0].(string), row[1].(string)}
	}
}

func getImageUrlsFromGranules(inChannel chan *Granule, ctx context.Context, outChannel chan string, storageClient *storage.Client) {
	for {
		granule, isChannelOpen := <-inChannel
		if !isChannelOpen {
			close(outChannel)
			return
		}
		url := fmt.Sprintf("%s/GRANULE/%s/IMG_DATA/",
			granule.baseUrl[32:], granule.granuleId)

		bucketName := "gcp-public-data-sentinel-2"
		bucket := storageClient.Bucket(bucketName)

		objects := bucket.Objects(ctx, &storage.Query{
			Prefix: url,
		})

		for {
			objAttrs, err := objects.Next()
			if err == iterator.Done {
				break
			}
			if err != nil {
				panic(err)
			}
			outChannel <- objAttrs.Name
		}
	}
}

