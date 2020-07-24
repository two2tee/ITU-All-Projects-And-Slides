// reference the type provider dll
#r "/Users/nh/fsharp/SQLProvider.0.0.11-alpha/lib/FSharp.Data.SqlProvider.dll"

#r "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5/Mono.Data.Sqlite.dll"
open System
open System.Linq
open FSharp.Data.Sql

let [<Literal>] resolutionPath = __SOURCE_DIRECTORY__ + @"/" 
let [<Literal>] connectionString = "Data Source=" + __SOURCE_DIRECTORY__ + @"/parts.db;Version=3"
// create a type alias with the connection string and database vendor settings
type sql = SqlDataProvider< 
              ConnectionString = connectionString,
              DatabaseVendor = Common.DatabaseProviderTypes.SQLITE,
              ResolutionPath = resolutionPath,
              IndividualsAmount = 1000,
              UseOptionTypes = true >
let db = sql.GetDataContext()

let partTable =  db.Main.Part
let partsListTable = db.Main.PartsList
    
let r = Seq.item 2 partTable
let _ = r.PartId
let _ = r.PartName

let q1 = query { for part in db.Main.Part do
                   select (part.PartName) }

let _ = Seq.take 3 q1

let q2 = query { for pl in db.Main.PartsList do
                   join part in db.Main.Part on (pl.PartsListId = part.PartId)
                   select (part.PartName, pl.PartId, pl.Quantity) }

let _ = Seq.take 3 q2

let nextId() = query { for part in db.Main.Part do count }
let _ = nextId()

let getDesc id = query { for part in db.Main.Part do
                           where (part.PartId=id)
                           select (part.PartName)
                           exactlyOne }

let _ = getDesc 0

