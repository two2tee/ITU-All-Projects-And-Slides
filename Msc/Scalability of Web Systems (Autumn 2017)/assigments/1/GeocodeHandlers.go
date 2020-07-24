package sentient

import (
	"fmt"
	"net/http"
	"io/ioutil"
	"strings"
	"encoding/json"
	"strconv"
	"google.golang.org/appengine"
	"google.golang.org/appengine/urlfetch"
)

var apiStr = "AIzaSyAoKKxEl2Hdn5X8_DgUdbhbJTRSP_9IAzc" //TODO this should not be here on real production code


//http://json2struct.mervine.net to auto construct struct based on json
type response struct {
	Results []struct {
		AddressComponents []struct {
			LongName  string   `json:"long_name"`
			ShortName string   `json:"short_name"`
			Types     []string `json:"types"`
		} `json:"address_components"`
		FormattedAddress string `json:"formatted_address"`
		Geometry         struct {
			Location struct {
				Lat float64 `json:"lat"`
				Lng float64 `json:"lng"`
			} `json:"location"`
			LocationType string `json:"location_type"`
			Viewport     struct {
				Northeast struct {
					Lat float64 `json:"lat"`
					Lng float64 `json:"lng"`
				} `json:"northeast"`
				Southwest struct {
					Lat float64 `json:"lat"`
					Lng float64 `json:"lng"`
				} `json:"southwest"`
			} `json:"viewport"`
		} `json:"geometry"`
		PlaceID string   `json:"place_id"`
		Types   []string `json:"types"`
	} `json:"results"`
	Status string `json:"status"`
}

/*
Bonus​ ​exercise:​ ​Imagine​ ​that​ ​you​ ​are​ ​building​ ​the​ ​backend​ ​for​ ​a​ ​system​ ​that​ ​needs​ ​to deal​ ​with​ ​end​ ​users​ ​that​ ​just​ ​want​
​to ​specify​ ​a​ ​human-like​ ​location,​ ​such​ ​as “Rued​ ​Langgaards​ ​Vej​ ​7,​ ​2300​ ​København​ ​S”
implement​ ​that​ ​feature​ ​to​ ​your​ ​service​ ​by​ ​connecting​ ​to​ ​​Google’s​ ​Geocoding​ ​API​.
 */

func FetchAddressMgrs(input string, r *http.Request) string {
	addr := strings.Replace(input," ","+",-1)

	if addr == ""{
		return "-1"
	}

	request := "https://maps.googleapis.com/maps/api/geocode/json?address="+addr+"&key="+apiStr
	fmt.Println(request)

	context := appengine.NewContext(r)
	client := urlfetch.Client(context)

	response, err := client.Get(request)
	defer response.Body.Close()
	if err != nil {
		panic(err)
	}
	responseBody, _ := ioutil.ReadAll(response.Body)
	fmt.Println(string(responseBody))

	lat,lng := decodeJson(responseBody)
	fmt.Println(lat)
	fmt.Print(lng)
	fmt.Println("")


	if lat == 0 && lng == 0 { return ""}
	mgrs := GetMGRS(strconv.FormatFloat(lat,'f',6,64,),strconv.FormatFloat(lng,'f',6,64),r)
	return mgrs
}

func decodeJson(rawjson []byte ) (lat float64, lng float64){
	response := response{Status:"not created"}
	json.Unmarshal(rawjson,&response)

	if response.Status == "not created" {
		fmt.Println("json was not converted")
		return 0,0
	}

	fmt.Println("status: " + response.Status)
	if response.Status != "OK" {return 0,0}

	coords := response.Results[0].Geometry.Location
	return coords.Lat,coords.Lng

}


