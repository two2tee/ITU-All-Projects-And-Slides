package sentient

import (
	"encoding/json"
	"fmt"
	"net/http"
	"strings"
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
	fmt.Println("Retrieveing by lat lng")
	r.ParseForm()
	lat := r.URL.Query().Get("lat")
	lng := r.URL.Query().Get("lng")
	mgrs := GetMGRS(lat, lng,r)

	getData(w,mgrs, r)

}

func getByAddressHandler(w http.ResponseWriter, r *http.Request) {
	fmt.Println("Retrieveing by address")
	r.ParseForm()
	input := strings.Replace(r.URL.Query().Get("address")," ","+",-1)

	if input == ""{
		fmt.Fprintf(w,"No address specified")
	}

	mgrs := FetchAddressMgrs(input,r)

	getData(w,mgrs ,r)

}

func mainPageHandler(w http.ResponseWriter, _ *http.Request){
	fmt.Fprintln(w,"Welcome, please use either links: \n http://127.0.0.1:8080/imagesByAddress?address= \n http://127.0.0.1:8080/images?lat= & lng=")
}

func getData(w http.ResponseWriter,mgrs string, r *http.Request){
	if mgrs == errNoRes {
		fmt.Fprintf(w, "No results found")
	} else if mgrs == errInv {
		fmt.Fprintf(w, "Invalid input")
	} else {
		images := GetImages(mgrs,r)
		enc := json.NewEncoder(w)
		err := enc.Encode(images)
		if err != nil {
			// if encoding fails, create an error page with code 500.
			http.Error(w, err.Error(), http.StatusInternalServerError)
		}

	}

}

func init() {
	fmt.Println("Started!")
	http.HandleFunc("/", mainPageHandler)
	http.HandleFunc("/images", getImagesHandler)
	http.HandleFunc("/imagesByAddress", getByAddressHandler)
}
