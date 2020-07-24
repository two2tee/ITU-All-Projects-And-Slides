package sentient

import (
	"github.com/golang/geo/s2"
	"log"
	"fmt"
	"strconv"
	"net/http"
	"bufio"
	"regexp"
	"strings"
	"sync"
)

func GetNumberOfImagesByPSLG(body string ,id string, r *http.Request) int {
	return IterateCover(r,id,getImgCover(parsePoints(body)))
}

func parsePoints(body string) *[]s2.Point {
	var points []s2.Point
	scanner := bufio.NewScanner(strings.NewReader(body))
	pattern := "(-?\\d+.\\d+)"
	re := regexp.MustCompile(pattern)
	for i := 0; i < 2; i++ {
		scanner.Scan()
	}

	for scanner.Scan() {
		line := scanner.Text()
		coordinateStrings := strings.Fields(line)
		if len(coordinateStrings) == 1 {
			continue
		}
		latMatches := re.FindStringSubmatch(coordinateStrings[0])
		lonMatches := re.FindStringSubmatch(coordinateStrings[1])
		lat, latErr := strconv.ParseFloat(latMatches[1], 64)
		lng, lngErr := strconv.ParseFloat(lonMatches[1], 64)

		if latErr != nil || lngErr != nil {
			panic(latErr)
		}

		points = append(points, s2.PointFromLatLng(s2.LatLngFromDegrees(lat, lng)))
	}
	if err := scanner.Err(); err != nil {
		panic(err)
	}
	return &points
}

func getImgCover(points *[]s2.Point) s2.CellUnion {
	l1 := s2.LoopFromPoints(*points)
	loops := []*s2.Loop{l1}
	poly := s2.PolygonFromLoops(loops)

	log.Printf("No. of edges %v\n", poly.NumEdges())

	rc := &s2.RegionCoverer{MaxLevel: 30, MaxCells: 100}
	cover := rc.Covering(poly)
	return cover;
}

func IterateCover(r *http.Request, id string, cover s2.CellUnion) int {

	hiLat, hiLng, lowLat, lowLng := "", "", "", ""
	coords := Coords{}
	total := int32(0)
	var wg sync.WaitGroup
	wg.Add(len(cover))

	for i := 0; i < len(cover); i++ {
		fmt.Printf("Cell %v : ", i)
		c := s2.CellFromCellID(cover[i])

		hiLat = c.RectBound().Hi().Lat.String()
		hiLng = c.RectBound().Hi().Lng.String()
		lowLat = c.RectBound().Lo().Lat.String()
		lowLng = c.RectBound().Lo().Lng.String()
		coords = Coords{hiLat,lowLat,hiLng,lowLng}
		go countImgInCover(coords ,r,id, &total, &wg)
	}

	wg.Wait()
	return int(total)
}

func countImgInCover(coords Coords, r *http.Request , id string, total *int32, wg *sync.WaitGroup) {
	granulesIter := GetGranulesIterator(coords,r,id)
	GetImageCount(granulesIter,r, total)
	wg.Done()
}
