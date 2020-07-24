package sentient

import (
	"strings"
	"io/ioutil"
	"regexp"
	"net/http"
	"log"
	"google.golang.org/appengine/urlfetch"
	"google.golang.org/appengine"
)

func GetMGRS(lat string, lng string, r *http.Request) string {

	url := "http://legallandconverter.com/cgi-bin/shopmgrs3.cgi"
	contentType := "application/x-www-form-urlencoded"

	requestBody := "detail=0&signup=http%3A%2F%2Flegallandconverter.com%2Fp21.html&reorder=http%3A%2F%2Flegallandconverter.com%2Fcgi%2Fshoppay.prg%3Fgrp%3D2%26prod%3D22&cmd=gps&latitude=" + lat + "&longitude=" + lng + "&xcmd=Calc"
	reader := strings.NewReader(requestBody)

	context := appengine.NewContext(r)
	client := urlfetch.Client(context)

	res, err := client.Post(url,contentType,reader)
	if err != nil {
		log.Fatalf("http request failed: %v", err)
	}

	//Print body
	defer res.Body.Close()
	responseBody, _ := ioutil.ReadAll(res.Body)
	pattern := "<title>(.+)</title>"

	var mgrsRegex = regexp.MustCompile(pattern)
	matches := mgrsRegex.FindStringSubmatch(string(responseBody))

	if len(matches) != 0 {
		substrings := strings.Split(matches[1], " ")
		return substrings[0] + substrings[1]
	}
	return errNoRes
}


