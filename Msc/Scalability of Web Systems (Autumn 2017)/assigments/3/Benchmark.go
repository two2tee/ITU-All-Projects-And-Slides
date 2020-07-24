package sentient

import (
	"strings"
	"testing"
)

var res []string

func BenchmarkFields(b *testing.B) {
	for i := 0; i < b.N; i++ {
		res = strings.Fields("hello, dear friend")
	}
}
