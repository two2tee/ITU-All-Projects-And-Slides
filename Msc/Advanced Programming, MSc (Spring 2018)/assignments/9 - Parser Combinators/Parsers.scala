package fpinscala.parsing

// GROUP 7 : tvao@itu.dk, dttn@itu.dk, daro@itu.dk

// I used Scala's standard library lists, and scalacheck Props in this set,
// instead of those developed by the book.

import java.util.regex._
import scala.util.matching.Regex
import language.higherKinds // we need this for higher kinded polymorphism
import language.implicitConversions // we need this for introducing internal DSL syntax

/**
  * Combinator library used to build a concrete JSON parser.
  * Contains all combinators as (static) functions transforming or constructing parsers of type Parser[A]
  * @tparam ParseError Type used to represent a parse error.
  * @tparam Parser Higher kinded type constructor takes another generic type parameter to construct a Parser.
  */
trait Parsers[ParseError, Parser[+_]] { self => // self refers to Parser instance used in ParserOps

  def run[A] (p: Parser[A]) (input: String): Either[ParseError,A]
  implicit def char (c: Char): Parser[Char]
  implicit def string(s: String): Parser[String]
  implicit def operators[A] (p: Parser[A]): ParserOps[A] = ParserOps[A](p)
  def or[A] (s1: Parser[A], s2: =>Parser[A]): Parser[A]
  def listOfN[A] (n: Int, p: Parser[A]): Parser[List[A]]

  def map[A,B] (p: Parser[A]) (f: A => B): Parser[B]
  def many[A](p: Parser[A]): Parser[List[A]]
  def succeed[A](a: A): Parser[A] = string("") map (_ => a)
  def slice[A](p: Parser[A]): Parser[String]
  def product[A,B](p: Parser[A], p2: =>Parser[B]): Parser[(A,B)]
  def flatMap[A,B](p: Parser[A])(f: A => Parser[B]): Parser[B]

  implicit def regex(r: Regex): Parser[String]
  implicit def asStringParser[A] (a: A) (implicit f: A => Parser[String]): ParserOps[String] = ParserOps(f(a))

  // JSON parser combinator methods

  // Sequence but ignore left component when building AST
  def seqTakeRight[A, B] (p: Parser[A], p2: Parser[B]): Parser[B] = product(p, p2).map(_._2)

  // Sequence but ignore right side when building AST
  def seqTakeLeft[A,B] (p: Parser[A], p2: Parser[B]): Parser[A] = product(p, p2).map(_._1)

  // Optionally include component when building AST (e.g. whitespace)
  // TODO: is this the same as just p.map(Some(_)) ???
  def zeroOrOne[A] (p: Parser[A]) : Parser[Option[A]] = p.map(Some(_)) or succeed(None)

  // Match zero or more
  def zeroOrMore[A] (p: Parser[A], p2: Parser[Any]) : Parser[List[A]] = oneOrMore(p, p2) or succeed(List())

  // Match one or more
  def oneOrMore[A] (p: Parser[A], p2: Parser[Any]) : Parser[List[A]] = map2 (p, (p2 |* p).many) (_::_)

  /**
    * ParserOps contains helper methods that enable using combinators infix
    * @param p Parser combinator.
    * @tparam A Generic type constructor of parser that is replaced with concrete type.
    */
  case class ParserOps[A](p: Parser[A]) {

    def |[B>:A] (p2: Parser[B]): Parser[B] = self.or (p,p2)
    def or[B>:A] (p2: Parser[B]): Parser[B] = self.or (p,p2)
    def **[B] (p2: Parser[B]): Parser[(A,B)] = self.product (p,p2)
    def product[B] (p2: Parser[B]): Parser[(A,B)] = self.product (p,p2)

    def map[B] (f: A => B): Parser[B] = self.map (p) (f)
    def flatMap[B] (f: A => Parser[B]): Parser[B] = self.flatMap (p) (f)
    def many: Parser[List[A]] = self.many[A] (p)
    def slice: Parser[String] = self.slice (p)

    // Helper methods for JSON parser combinator

    // |* is sequencing & ignore left component when building AST
    // ('x |* y' is syntactic sugar for '(x ** y) map (._2)
    def |* [B] (p2: Parser[B]): Parser[B] = self.seqTakeRight(p, p2) // Same as(p ** p2).map(_._2)


    // *| is sequencing & ignore right component when building AST
    // ('x *| y' is syntactic sugar for '(x ** y) map (._2)
    // def *|[B] (p2: Parser[B]): Parser[A] = self.seqTakeLeft(p, p2)
    def *| [B] (p2: Parser[B]): Parser[A] = self.seqTakeLeft(p, p2) // Same as (p ** p2).map(_._1)

    // ? is an optional parser that matches zero or one
    //def ? [A] : Parser[Option[A]] = self.zeroOrOne (p)
    def ? : Parser[Option[A]] = self.zeroOrOne(p)

    // * matches 0 or more occurrences of input
    def * (p2: Parser[Any]) : Parser[List[A]] = self.zeroOrMore(p, p2)

    // + matches 1 or more
    def + (p2: Parser[Any]) : Parser[List[A]] = self.oneOrMore(p, p2)
  }

  /**
    * Laws represent properties that should always hold for the parser combinators.
    */
  object Laws {

    // Storing the laws in the trait -- the will be instantiated when we have
    // concrete implementation.  Still without a concrete implementation they
    // can be type checked, when we compile.  This tells us that the
    // construction of the laws is type-correct (the first step for them
    // passing).

    import org.scalacheck._
    import org.scalacheck.Prop._

    val runChar = Prop.forAll { (c: Char) => run(char(c))(c.toString) == Right(c) }
    val runString = Prop.forAll { (s: String) => run(string(s))(s) == Right(s) }

    val listOfN1 = Prop.protect (run(listOfN(3, "ab" | "cad"))("ababcad") == Right("ababcad"))
    val listOfN2 = Prop.protect (run(listOfN(3, "ab" | "cad"))("cadabab") == Right("cadabab"))
    val listOfN3 = Prop.protect (run(listOfN(3, "ab" | "cad"))("ababab") == Right("ababab"))
    val listOfN4 = Prop.protect (run(listOfN(3, "ab" | "cad"))("ababcad") == Right("ababcad"))
    val listOfN5 = Prop.protect (run(listOfN(3, "ab" | "cad"))("cadabab") == Right("cadabab"))
    val listOfN6 = Prop.protect (run(listOfN(3, "ab" | "cad"))("ababab") == Right("ababab"))

    def succeed[A] (a: A) = Prop.forAll { (s: String) => run(self.succeed(a))(s) == Right(a) }

    // Not planning to run this (would need equality on parsers), but can write for typechecking:
    def mapStructurePreserving[A] (p: Parser[A]): Boolean = map(p)(a => a) == p
  }

  // Parser of abra og cadabra string yields Right result "abra"
  run ("abra" | "cadabra") ("abra") == Right("abra")

  // Exercise 1

  // Parser combinator that counts 'a' characters
  // Converts char input 'a' implicitly to Parser[Char] using implicit def char (c: Char): Parser[Char]
  // Slice returns portion of input string matched by char parser into a Parser[String]
  // Finally maps Strings to get count of a's in input string using constant time String length operator
  // Example: run (manyA) ("aaa") gives Right(3) and run (manyA) ("b") gives Right(0)
  def manyA : Parser[Int] = many('a').slice.map(_.length)
  def manyATest (input: String) : Either[ParseError, Int] = run (manyA) (input)

  // Exercise 2

  // Use product and map to transform elements using both Parser[A] and Parser[B] into Parser[C]
  // Signature of product: product[A,B](p: Parser[A], p2: =>Parser[B]): Parser[(A,B)]
  // Signature of map: map[A,B] (p: Parser[A]) (f: A => B): Parser[B]
  def map2[A,B,C](p: Parser[A], p2: Parser[B])(f: (A,B) => C): Parser[C] = {
    // Explicit
    val productOutput : Parser[(A,B)] = p ** p2
    val map : Parser[C] = productOutput.map (f.tupled) // map[(A,B),C] (p: Parser(A,B) (f: (A,B) => C): Parser[C]

    // Chained
    (p ** p2).map(f.tupled) // Creates a tupled function instead of two arguments to map (A,B) product elements
    (p ** p2).map(p => f(p._1, p._2)) // Equivalent to explicitly unpacking tuple
    // TODO: Why does f.tupled make map2 non-strict in its second argument ???
  }

  // Recognize one or more 'a' characters using many1 combinator defined in terms of many
  // Many signature: many[A](p: Parser[A]): Parser[List[A]]
  // Parser many recognized 0 or more chars and returns #chars seen
  // many1(p) is equivalent to p followed by many(p) so we run parser p followed by parser many(p) assuming first succeed
  // many(p) will try running p followed by many(p) until attempt to parse p fails
  // result of success runs of p are returned
  // f in map 2 depends on result of first parser => result only added if Parser[A] p succeeds
  def many1[A](p: Parser[A]): Parser[List[A]] = {
    map2 (p, p.many) (_ :: _) // Run parser p followed parser p.many and add result to list
  }

  // Exercise 3

  // Parser parses a single digit using flatMap, and as many occurrences of char 'a' as value of the digit
  // Example: "3aaa" should parse to 3 (as a number) but "2aaa" should not parse
  // parsing implies to consume characters, in which product may help proceed parsing the sequence of a only if it parses the digit
  //implicit def regex(r: Regex): Parser[String] used to promote regular expression to parser
  // Match strings with a digit followed by chars that are matched using slice and counted using String length
  def digitTimesA: Parser[Int] = {
    "[a-zA-Z_][a-zA-Z0-9_]*".r.flatMap(s => s.substring(1)).slice.map(_.length)
  }

  // Exercise 4

  // Define product in terms of flatMap
  // Product is used to run one parser a followed by another parser b (a ** b)
  // Signature: product[A,B](p: Parser[A], p2: =>Parser[B]): Parser[(A,B)]
  def product_[A,B] (p: Parser[A], p2: =>Parser[B]): Parser[(A,B)] = {
    p.flatMap (pa => p2.map(pb => (pa, pb))) // Map parser[A] values and combine with Parser[B]
  }

  // Implement map2 in terms of flatMap
  def map2_[A,B,C] (p: Parser[A], p2: Parser[B])(f: (A,B) => C): Parser[C] = {
    p.flatMap(pa => p2.map(pb => f(pa, pb)))
  }

  // Exercise 5

  // Express map in terms of flatMap and/or other combinators
  def map_[A,B] (p: Parser[A]) (f: A => B): Parser[B] = {
    p.flatMap[B] (pa => succeed(f(pa))) // Map Parser[A] values to B values and wrap in Parser using succeed
  }

}

/**
  * Minimal trait (interface) of JSON parser combinator.
  */
trait JSON

/**
  * Internal JSON abstract syntax [[JSON]] representation and concrete JSON parser created from generic [[Parsers]].
  */
object JSON {
  case object JNull extends JSON
  case class JNumber(get: Double) extends JSON
  case class JString(get: String) extends JSON
  case class JBool(get: Boolean) extends JSON
  case class JArray(get: IndexedSeq[JSON]) extends JSON
  case class JObject(get: Map[String, JSON]) extends JSON

  // JSON parser used to parse JSON example
  // The first argument is the parsers implementation P (that we don't have).
  // We write this code with only having the interface
  // Builds JSON parser combinator language
  // Specifies translations from JSON input to abstract syntax output (JSON terminals)
  def jsonParser[ParseErr,Parser[+_]] (P: Parsers[ParseErr,Parser]): Parser[JSON] = {

    import P._ // Parser implementation
    val spaces = char (' ').many.slice

    /* Exercise 6 */

    // Quotation parser : regex match anything except zero or more quotations
    // // TODO: what does dropRight and substring do? seems like dropping left and right quotation again?
    val QUOTED: Parser[String] = """([^"]*)""".r.map{ _ dropRight 1 substring 1}

    // Double parser : match any multi-digit numbers and convert to doubles
    val DOUBLE: Parser[Double] = """(\+|-)?[0-9]+(\.[0-9]+(e[0-9]+)?)?""".r.map { _.toDouble }

    // Whitespace parser ([\t\n] equivalent to \ss) : ignore whitespace (hence unit)
    val ws: Parser[Unit] = "[\t\n ]+".r map { _ => () }

    // Null parser
    val jnull: Parser[JSON] = "null" |* succeed (JNull)

    // Boolean parser
    val jbool: Parser[JBool] =
      (ws.? |* "true" |* succeed (JBool(true ))) | (ws.? |* "false"|* succeed (JBool(false)))

    // String parser
    val jstring: Parser[JString] = QUOTED map { JString(_) }

    // Number parser
    val jnumber: Parser[JNumber] = DOUBLE map { JNumber(_) }

    // Array parser
//    lazy val jarray: Parser[JArray] =
//      ( ws.? |* "[" |* (ws.? |* json *| ",").*
//        *| ws.? *| "]" *| ws.? )
//        .map { l => JArray (l.toVector) }

    lazy val jarray: Parser[JArray] =
    (ws.? |* "[" |* (ws.? |* json).*(",")
      *| ws.? *| "]" *| ws.?).map(l => JArray(l.toVector))


    // Field parser
    lazy val field: Parser[(String, JSON)] =
      ws.? |* QUOTED *| ws.? *| ":" *| ws.? ** json *| ","

    // Object parser
//    lazy val jobject: Parser[JObject] =
//      (ws.? |* "{" |* field.* *| ws.? *| "}" *| ws.?)
//        .map { l => JObject (l.toMap) }

    lazy val jobject: Parser[JObject] =
      (ws.? |* "{" |* field.*(",") *| "}" *| ws.?).map(l => JObject(l.toMap))

    // Complex JSON parser combinator composed from multiple simpler parsers to output JSON AST (abstract syntax)
    lazy val json : Parser[JSON] =
      (jstring | jobject | jarray | jnull | jnumber | jbool) *| ws.?

    json *| "\\z" // Match end of file
  }

  // Input string representation
  val jsonInput = """{
                        "Company name" : "Microsoft",
                        "Ticker" : "MSFT",
                        "Active" : true,
                        "Price" : 30.66,
                        "Shares outstanding" : 8.38e9,
                        "Related companies" :
                          [ "HPQ", "IBM", "YHOO", "DELL", "GOOG", ],
                     }
                 """

  // Output abstract representation
  val jsonOutput = JObject(Map(
    "Shares outstanding" -> JNumber(8.38E9),
    "Price" -> JNumber(30.66),
    "Company name" -> JString("Microsoft"),
    "Related companies" -> JArray(
      Vector(JString("HPQ"), JString("IBM"),
        JString("YHOO"), JString("DELL"),
        JString("GOOG"))),
    "Ticker" -> JString("MSFT"),
    "Active" -> JBool(true)))

}
