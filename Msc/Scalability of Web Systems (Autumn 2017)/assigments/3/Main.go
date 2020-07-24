package sentient

import (
	"encoding/json"
	"io/ioutil"
	"log"
	"net/http"
	"strconv"
	"strings"
	_ "net/http/pprof"
)

/*
Design​ ​and​ ​implement​ ​in​ ​Go​ ​on​ ​the​ ​Google​ ​Cloud​ ​Service​ ​a​ ​webservice​ ​
	- Accepts GET​ ​requests​ ​that​ ​contain​ ​a​ ​location​ ​(latitude,​ ​longitude)​ ​as​ ​query​ ​parameters​ ​as follows:​ ​​
			http://your.app.com/images?lat=37.4224764&lng=-122.0842499
	- Return​ ​a​ ​JSON​ ​array​ ​containing​ ​the​ ​links​ ​to​ ​all​ ​images​ ​for​ ​that​ ​location,​
​		e.g.: [“...2A_OPER_MSI_L1C_TL_EPA__20170529T193446_A002407_T33UUP_B01.jp2”,”... 2A_OPER_MSI_L1C_TL_EPA__20170529T193446_A002407_T33UUP_B02.jp2”...​ ​]
*/

var errInv = "-1"
var errNoRes = "-2"

func getImagesHandler(w http.ResponseWriter, r *http.Request) {
	log.Println("Retrieveing by lat lng")
	r.ParseForm()
	lat := r.URL.Query().Get("lat")
	lng := r.URL.Query().Get("lng")
	id := r.URL.Query().Get("id")

	if !isValidCoord(lat) || !isValidCoord(lng) {
		http.Error(w, "Invalid input", http.StatusBadRequest)
	} else {
		mgrs := GetMGRS(lat, lng, r)
		getDataBySinglePoint(w, mgrs, r, id)
	}

}

func getByAddressHandler(w http.ResponseWriter, r *http.Request) {
	log.Println("Retrieveing by address")
	r.ParseForm()
	input := strings.Replace(r.URL.Query().Get("address"), " ", "+", -1)
	id := r.URL.Query().Get("id")

	if input == "" {
		log.Printf("No address specified")
	}

	mgrs := FetchAddressMgrs(input, r)

	getDataBySinglePoint(w, mgrs, r, id)

}

func getByArea(w http.ResponseWriter, r *http.Request) {
	log.Println("Retrieveing by lat lng")
	r.ParseForm()
	lat1 := r.URL.Query().Get("lat1")
	lng1 := r.URL.Query().Get("lng1")
	lat2 := r.URL.Query().Get("lat2")
	lng2 := r.URL.Query().Get("lng2")
	id := r.URL.Query().Get("id")

	if !isValidCoord(lat1) || !isValidCoord(lat2) || !isValidCoord(lng1) || !isValidCoord(lng2) {
		http.Error(w, "Invalid input", http.StatusBadRequest)
	} else {
		var coords = Coords{lat1:lat1,lat2:lat2,lng1:lng1,lng2:lng2}
		getDataByArea(w,coords,r, id)
	}
}

func mainPageHandler(_ http.ResponseWriter, _ *http.Request) {
	log.Println("Welcome, please use either links: " +
		"/imagesByAddress?address= & id= \n " +
		"/images?lat=&lng=&id=" +
		"/imagesByArea?lat1=&lat2=&lng1=&lng2=&id=")
}

func getDataBySinglePoint(w http.ResponseWriter, mgrs string, r *http.Request, id string) {
	outChannel := make(chan string, 56) //buffered reader to limit amount of memory used

	if mgrs == errNoRes {
		http.Error(w, "No results found", http.StatusNotFound)
	} else if mgrs == errInv {
		http.Error(w, "Invalid input", http.StatusNotFound)

	} else {
		go GetPointImages(mgrs, r, id, outChannel)
		postResults(w, outChannel)
	}
}

//A service that allows the user to specify two longitude and two latitude bands to mark an area of interest.
func getDataByArea(w http.ResponseWriter, coords Coords, r *http.Request, id string) {
	outChannel := make(chan string, 56) //buffered reader to limit amount of memory used
	go GetAreaImages(coords, r, id, outChannel)
	postResults(w, outChannel)

}

func postResults(w http.ResponseWriter, outChannel chan string) {
	isOpen := true
	var img string
	for isOpen {
		img, isOpen = <-outChannel
		if isOpen && img != "" {
			enc := json.NewEncoder(w)
			err := enc.Encode(img)
			if err != nil {
				http.Error(w, err.Error(), http.StatusInternalServerError)
			}
		}
	}

}

func isValidCoord(input string) bool {
	_, err := strconv.ParseFloat(input, 64)
	return input != "" && input != " " && err == nil
}

func getByPSLG(w http.ResponseWriter, r *http.Request) {
	b, err := ioutil.ReadAll(r.Body)
	if err != nil {
		http.Error(w, "Internal Server Error - could not read body", http.StatusInternalServerError)
		return
	}

	if len(b) == 0 {
		http.Error(w, "Bad request - could not read body", http.StatusBadRequest)
	}

	body := string(b)
	if body == "" {
		http.Error(w, "Bad request - please provide a non-empty body", http.StatusBadRequest)
	}
	id := r.URL.Query().Get("id")

	var totalImgs = GetNumberOfImagesByPSLG(body,id, r)

	enc := json.NewEncoder(w)
	enc.Encode(totalImgs)
}

func init() {
	log.Println("Started!")
	http.HandleFunc("/", mainPageHandler)
	http.HandleFunc("/images", getImagesHandler)
	http.HandleFunc("/imagesByAddress", getByAddressHandler)
	http.HandleFunc("/imagesByArea", getByArea)
	http.HandleFunc("/imagesByPSLG", getByPSLG)
}
