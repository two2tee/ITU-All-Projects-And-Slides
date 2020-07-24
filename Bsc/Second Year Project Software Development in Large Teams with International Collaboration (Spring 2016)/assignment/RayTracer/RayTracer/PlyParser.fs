namespace RayTracer
 
(*
This module is used to parse PLY files (polygon file format) storing graphical objects described as a collection of polygons.
Specifically, we use it to store vertices that are referenced by multiple triangle meshes. 
The vertices and faces are used in the MeshBuilder module to construct triangle meshes.
Note that the Ply Parser does not take calculate normals for the vertices without, which is instead done in the MeshBuilder.
Also note that the length of the properties are currently hard coded to conform with the format of e.g. the "Stanford Bunny". 

Typical structure of PLY file:
- Header: describes remainder of file including element type, name, amount and a list of properties associated with each element.
- Vertex List: list of triples (x,y,z) for vertices used to build different elements in Face list
- Face List: list of other elements composed of vertices referenced in the Vertex List 

A typical PLY object definition is simply a list of (x,y,z) triples for vertices
and a list of faces that are described by indices into the list of vertices. 
See link: http://paulbourke.net/dataformats/ply/ 
*) 

module PlyParser = 

    // Libraries used for parsing including regular expression support 
    open System
    open System.IO
    open System.Collections.Generic
    open System.Text.RegularExpressions 
    open RayTracer.SceneAssets 
    
    // Types used to represent vertices consisting of 3 points and face polygons consisting of 3 sides for triangle meshes 
    type Vector = Vector.Vector  
    type Point = Point.Point
    type Vertex = SceneAssets.Vertex
    type Face = SceneAssets.Face

    // Exception used if malformed lines occur in the PLY file 
    exception ParseError of string

    /// Read lines from a PLY file into a sequence of trimmed strings.
    /// Takes the filename of the file to be parsed as argument. 
    /// Note that the path should be located within the 'Resource' folder located in the project files. 
    /// Returns a sequence of trimmed lines from the file. 
    let readLines (fileName : string) = 
       seq { use streamReader = new StreamReader (fileName)
             while not streamReader.EndOfStream do
             yield streamReader.ReadLine().Trim() } // Yield each trimmed line 

    // Mutable values used during parsing 
    // TODO: replace vertexCount and faceCount with a tuple 
    let mutable vertexCount = 0
    let mutable faceCount = 0
    let mutable (vertices: seq<Vertex>) = Seq.empty
    let mutable (faces: seq<Face>) = Seq.empty
    let mutable vertexPropertyMap = Dictionary<string,int>() // Property map (name, position) for fast lookups 

    
    /// Reinitializes all the mutable values to allow parsing a sequence of PLY files 
    let initialize () = 
        vertexCount <- 0
        faceCount <- 0
        vertices <- Seq.empty
        faces <- Seq.empty
        vertexPropertyMap <- Dictionary<string,int>()

    // Retrieves the position of a given property in a map composed of property names and positions. 
    // Returns -1 if the property does not exist in the map and thus does not have a position. 
    let getPropertyMapPosition (propertyName : string) (properties : Dictionary<string,int>) = 
        if (properties.ContainsKey(propertyName))
        then  
            let propertyPosition = properties.Item propertyName
            propertyPosition
        else
            -1 // No position 
    
    // Retrieves a property from an array of properties extracted from property lines in the parseVertices and parseFaces methods.
    // Returns the property on a given position, else returns 0.0 which is evaluated in the MeshBuilder for e.g. computing normals. 
    let getProperty (position : int) (properties : string[]) = 
        if position = -1 
        then 0.0 
        else
            if (position < properties.Length) // Check that property position is within amount of properties 
            then (float) (Array.get properties position) 
            else 0.0

    /// ParseRegex parses a regular expression 
    /// and returns a list of the strings that match each group in the regular expression.
    /// List.tail is called to eliminate the first element in the list, 
    /// which is the full matched expression, since only the matches for each group are wanted.
    /// Active patterns always takes one argument but may take additional arguments to specialize pattern (e.g. match string).
    /// This active pattern uses regular expressions to parse strings including extra parameter. 
    /// The method is used to match on strings that use regular expressions for various PLY formats
    /// in the parseHeader method that customize the general ParseRegex active pattern.
    /// See link (Active Patterns: https://msdn.microsoft.com/en-us/library/dd233248.aspx#Anchor_3) 
    let (|ParseRegex|_|) regex input =
        let m = Regex(regex).Match(input)
        if m.Success
        then Some (List.tail [ for x in m.Groups -> x.Value ])
        else None 



    /// Parses a sequences of lines from the header of the PLY file omitting the "end_header".
    /// Specifically, the amount of vertices and faces are used to determine the mutable counter values. 
    /// Lines that are not Exception 'ParseError' is raised if input is not recognized. 
    /// \d stands for a single character from the set [0..9]
    /// [A-Za-z0-9\-]+ is one or more alphanumeric characters (e.g. float)
    let parseHeader (header: seq<string>) = 
        initialize()

        let counter = ref 0
        for line in header do 
            match line with
            | ParseRegex "element vertex (\d+)" [amount] -> // Amount of vertices 
                vertexCount <- int (amount) 
            | ParseRegex "element face (\d+)" [amount] -> // Amount of face elements (polygons)       
                faceCount <- int (amount)                                                                                     
            | ParseRegex "property ([A-Za-z0-9]+) ([A-Za-z0-9]+)" [datatype; name] -> // Vertex properties 
                  if not (vertexPropertyMap.ContainsKey(name)) then do
                    vertexPropertyMap.Add(name, counter.Value)
                    counter := counter.Value + 1
            | _ -> ()

    /// Converts a string representation of a vertex to a vertex as represented in the program.
    /// Exception if thrown when length of input is less than three (no vertex). 
    /// Note that the property values are determined by the actual appearance of them in the given PLY file.
    /// If the property is not found a default value of 0 is set for further validation in the MeshBuilder. 
    let stringToVertex (s: string) : Vertex =
       let properties = s.Split[|' '|]
       let x =  getProperty (getPropertyMapPosition "x" vertexPropertyMap) properties
       let y =  getProperty (getPropertyMapPosition "y" vertexPropertyMap) properties
       let z =  getProperty (getPropertyMapPosition "z" vertexPropertyMap) properties
       let nx = getProperty (getPropertyMapPosition "nx" vertexPropertyMap) properties
       let ny = getProperty (getPropertyMapPosition "ny" vertexPropertyMap) properties
       let nz = getProperty (getPropertyMapPosition "nz" vertexPropertyMap) properties
       let u =  getProperty (getPropertyMapPosition "u" vertexPropertyMap) properties
       let v =  getProperty (getPropertyMapPosition "v" vertexPropertyMap) properties
       let normal = Vector.mkVector nx ny nz
       mkVertex (Point.mkPoint x y z) normal v u

    /// Converts a a sequences of strings to an array of vertices as represented in the program.
    /// Note that the counter is a ref type since you cannot use mutable values inside a closure.
    let parseVertices (vertices: seq<string>) =
       let counter = ref 0
       let vertexarr = Seq.fold (fun array a -> // Accumulator function 
                                    (Array.set array counter.Value (stringToVertex a)) // Set array index value 
                                    counter := counter.Value + 1
                                    array) 
                                    (Array.zeroCreate vertexCount) // Initial array with size of vertex amount 
                                    vertices // Sequence to convert 
       vertexarr

    /// Convert a string PLY representation of a face to a face as represented in the program.
    /// Throws a 'ParseError' exception when length of inpus is less than 3 (polygon should atleast have 3 sides). 
    /// Note that the property length is hard coded to be three as used in the triangle meshes. 
    /// The property length should not be hard coded but be determined by the actual amount of properties in any given file.
    let stringToFace (s: string) : Face =
       let faceProperties = s.Split[|' '|]
       if (int) (Array.get faceProperties 0) <> 3 
       then failwith "Face polygon is not a triangle" 
       else
       match faceProperties with
       | face when face.Length < 3 -> 
                                      System.Console.WriteLine(s)
                                      raise (ParseError("Malformed polygon"))
       | _                         -> // Face with triple (x,y,z) reference 
                                      // Position is incremented by 1 to skip amount of face sides 
                                      let x =  getProperty ((getPropertyMapPosition "x" vertexPropertyMap) + 1) faceProperties
                                      let y =  getProperty ((getPropertyMapPosition "y" vertexPropertyMap) + 1) faceProperties
                                      let z =  getProperty ((getPropertyMapPosition "z" vertexPropertyMap) + 1) faceProperties
                                      F(int x, int y, int z) 

    /// Converts a sequence of string containing PLY face information to an array of faces as represented in the program.
    /// Note that the counter is a ref type since you cannot use mutable values inside a closure.
    let parseFaces (faces: seq<string>) =
          let counter = ref 0
          let facearr = Seq.fold (fun array a -> // Accumulator function 
                                    (Array.set array counter.Value (stringToFace a)) // Set array index value 
                                    counter := counter.Value + 1
                                    array) 
                                    (Array.zeroCreate faceCount) // Initial array with size of vertex amount  
                                    faces // Sequence to convert 
          facearr

    /// Main function of the PLY parser used to parse the specified PLY file.
    /// The parsing is done using the three helper functions; parseHeader, parseVertices and parseFaces.
    /// Returns an array of vertices and faces used to construct triangle meshes in the Mesh Builder. 
    let parsePLY fileName =
       let lines = readLines fileName 

       // Find header and parse until end ('end_header') 
       let bodyPosition = lines |> Seq.findIndex(fun f -> f = "end_header") 
       let header = lines |> Seq.take bodyPosition 
       parseHeader header 

       // Find and parse vertices and faces based on appearance in line numbers using vertexCount and faceCount 
       let vertexPart = lines |> Seq.skip (bodyPosition + 1) |> Seq.take vertexCount 
       let facePart = (lines |> Seq.skip (bodyPosition + vertexCount + 1) |> Seq.take faceCount)
       let vertices = parseVertices vertexPart
       let faces = parseFaces facePart 
       (vertices, faces)