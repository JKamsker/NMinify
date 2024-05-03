package main

import (
	"fmt"
	"os"
	"regexp"
	"strconv"
	"sync"
	"unsafe"

	"github.com/tdewolff/minify/v2"
	"github.com/tdewolff/minify/v2/css"
	"github.com/tdewolff/minify/v2/html"
	"github.com/tdewolff/minify/v2/js"
	"github.com/tdewolff/minify/v2/json"
	"github.com/tdewolff/minify/v2/svg"
	"github.com/tdewolff/minify/v2/xml"
	"github.com/tdewolff/parse/v2/buffer"
)
import "C"

// var m *minify.M

func init() {
}

// func goBytes(str *C.char, length C.longlong) []byte {
// 	return (*[1 << 32]byte)(unsafe.Pointer(str))[:length:length]
// }

// func goBytes(str *C.char, length C.longlong) []byte {
//     return (*[1 << 30]byte)(unsafe.Pointer(str))[:length:length]
// }

// Compiles, but doesn't work
// func goBytes(str *C.char, length C.longlong) []byte {
// 	return (*(*[]byte)(unsafe.Pointer(str)))[:length:length]
// }

// func goBytes(str *C.char, length C.longlong) []byte {
// 	_ = (*(*[]*C.char)(unsafe.Pointer(str)))[:length:length]

// 	return (*[1 << 32]byte)(unsafe.Pointer(str))[:length:length]
// }

// func goStringArray(carr **C.char, length C.longlong) []string {
// 	if length == 0 {
// 		return []string{}
// 	}

// 	strs := make([]string, length)
// 	// arr := (*[1 << 32]*C.char)(unsafe.Pointer(carr))[:length:length]
// 	arr := (*(*[]C.char)(unsafe.Pointer(carr)))[:length:length]
// 	for i := 0; i < int(length); i++ {
// 		strs[i] = C.GoString(arr[i])
// 	}
// 	return strs
// }

func goBytes(str *C.char, length C.longlong) []byte {
	return (*(*[]byte)(unsafe.Pointer(str)))[:length:length]
}

func goStringArray(carr **C.char, length C.longlong) []string {
	if length == 0 {
		return []string{}
	}

	strs := make([]string, length)
	//arr := (*[1 << 32]*C.char)(unsafe.Pointer(carr))[:length:length]
	arr := (*(*[]*C.char)(unsafe.Pointer(carr)))[:length:length]
	for i := 0; i < int(length); i++ {
		strs[i] = C.GoString(arr[i])
	}
	return strs
}

var minifiers = make(map[int]*minify.M)
var lock sync.Mutex

//export allocateMinifier
func allocateMinifier() uintptr {
	minifier := minify.New()
	ptr := unsafe.Pointer(minifier)
	uintPtr := uintptr(ptr)

	lock.Lock()
	defer lock.Unlock()
	minifiers[int(uintPtr)] = minifier

	return uintPtr
}

//export freeMinifier
func freeMinifier(minifierPtr uintptr) {
	lock.Lock()
	defer lock.Unlock()
	delete(minifiers, int(minifierPtr))
}

//export configureMinifier
func configureMinifier(minifierPtr uintptr, ckeys **C.char, cvals **C.char, length C.longlong, output_errorMessage *C.char) *C.char {
	minifier := minifiers[int(minifierPtr)]

	keys := goStringArray(ckeys, length)
	vals := goStringArray(cvals, length)

	cssMinifier := &css.Minifier{}
	htmlMinifier := &html.Minifier{}
	jsMinifier := &js.Minifier{}
	jsonMinifier := &json.Minifier{}
	svgMinifier := &svg.Minifier{}
	xmlMinifier := &xml.Minifier{}

	var err error
	for i := 0; i < len(keys); i++ {
		switch keys[i] {
		case "css-precision":
			var precision int64
			precision, err = strconv.ParseInt(vals[i], 10, 64)
			cssMinifier.Precision = int(precision)
		case "html-keep-comments":
			htmlMinifier.KeepComments, err = strconv.ParseBool(vals[i])
		case "html-keep-conditional-comments":
			htmlMinifier.KeepConditionalComments, err = strconv.ParseBool(vals[i])
		case "html-keep-default-attr-vals":
			htmlMinifier.KeepDefaultAttrVals, err = strconv.ParseBool(vals[i])
		case "html-keep-document-tags":
			htmlMinifier.KeepDocumentTags, err = strconv.ParseBool(vals[i])
		case "html-keep-end-tags":
			htmlMinifier.KeepEndTags, err = strconv.ParseBool(vals[i])
		case "html-keep-whitespace":
			htmlMinifier.KeepWhitespace, err = strconv.ParseBool(vals[i])
		case "html-keep-quotes":
			htmlMinifier.KeepQuotes, err = strconv.ParseBool(vals[i])
		case "js-precision":
			var precision int64
			precision, err = strconv.ParseInt(vals[i], 10, 64)
			jsMinifier.Precision = int(precision)
		case "js-keep-var-names":
			jsMinifier.KeepVarNames, err = strconv.ParseBool(vals[i])
		case "js-no-nullish-operator":
			jsMinifier.NoNullishOperator, err = strconv.ParseBool(vals[i])
		case "json-precision":
			var precision int64
			precision, err = strconv.ParseInt(vals[i], 10, 64)
			jsonMinifier.Precision = int(precision)
		case "json-keep-numbers":
			jsonMinifier.KeepNumbers, err = strconv.ParseBool(vals[i])
		case "svg-keep-comments":
			svgMinifier.KeepComments, err = strconv.ParseBool(vals[i])
		case "svg-precision":
			var precision int64
			precision, err = strconv.ParseInt(vals[i], 10, 64)
			svgMinifier.Precision = int(precision)
		case "xml-keep-whitespace":
			xmlMinifier.KeepWhitespace, err = strconv.ParseBool(vals[i])
		default:
			output_errorMessage = C.CString(fmt.Sprintf("unknown config key: %s", keys[i]))
			return nil
		}
		if err != nil {
			if err.(*strconv.NumError).Func == "ParseInt" {
				err = fmt.Errorf("\"%s\" is not an integer", vals[i])
			} else if err.(*strconv.NumError).Func == "ParseBool" {
				err = fmt.Errorf("\"%s\" is not a boolean", vals[i])
			}
			output_errorMessage = C.CString(fmt.Sprintf("bad config value for %s: %v", keys[i], err))
			return nil
		}
	}

	// minifier := minify.New()
	minifier.Add("text/css", cssMinifier)
	minifier.Add("text/html", htmlMinifier)
	minifier.Add("image/svg+xml", svgMinifier)
	minifier.AddRegexp(regexp.MustCompile("^(application|text)/(x-)?(java|ecma|j|live)script(1\\.[0-5])?$|^module$"), jsMinifier)
	minifier.AddRegexp(regexp.MustCompile("[/+]json$"), jsonMinifier)
	minifier.AddRegexp(regexp.MustCompile("[/+]xml$"), xmlMinifier)

	// return (*C.char)(unsafe.Pointer(minifier))

	// Return a pointer to the minifier while preventing the Go garbage collector from freeing it.
	// C.cgoAllocMap(unsafe.Pointer(minifier))
	return nil
}

//export minifyString
func minifyString(minifierPtr uintptr, cmediatype, cinput *C.char, input_length C.longlong, coutput *C.char, output_length *C.longlong) *C.char {
	// cast the minifier pointer to a minifier
	ptr := unsafe.Pointer(minifierPtr)
	minifier := (*minify.M)(ptr)

	// minifier := minifiers[int(minifierPtr)]
	mediatype := C.GoString(cmediatype) // copy
	input := goBytes(cinput, input_length)
	output := goBytes(coutput, input_length)

	out := buffer.NewStaticWriter(output[:0])
	if err := minifier.Minify(mediatype, out, buffer.NewReader(input)); err != nil {
		return C.CString(err.Error())
	} else if err := out.Close(); err != nil {
		return C.CString(err.Error())
	}
	*output_length = C.longlong(out.Len())
	return nil
}

//export minifyFile
func minifyFile(minifierPtr uintptr, cmediatype, cinput, coutput *C.char) *C.char {
	minifier := minifiers[int(minifierPtr)]

	mediatype := C.GoString(cmediatype) // copy
	input := C.GoString(cinput)
	output := C.GoString(coutput)

	fi, err := os.Open(input)
	if err != nil {
		return C.CString(err.Error())
	}

	fo, err := os.Create(output)
	if err != nil {
		return C.CString(err.Error())
	}

	if err := minifier.Minify(mediatype, fo, fi); err != nil {
		fi.Close()
		fo.Close()
		return C.CString(err.Error())
	} else if err := fi.Close(); err != nil {
		fo.Close()
		return C.CString(err.Error())
	} else if err := fo.Close(); err != nil {
		return C.CString(err.Error())
	}
	return nil
}

func main() {}
