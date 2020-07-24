namespace RayTracer

(*
    Signature file used to give a short summary of the PLY Parser and its public interface 
    The module is used to parse PLY files (polygon file format) storing graphical objects described as a collection of polygons.
    Specifically, we use it to store vertices that are referenced by multiple triangle meshes. 
    The vertices and faces are used in the MeshBuilder module to construct triangle meshes.
    Note that the Ply Parser does not take calculate normals for the vertices without, which is instead done in the MeshBuilder.
    Also note that the length of the properties are currently hard coded to conform with the format of e.g. the "Stanford Bunny". 
*)

module PlyParser = 

    type Point = Point.Point
    type Vector = Vector.Vector
    type Vertex = SceneAssets.Vertex // Vertex composed of a Point, normal vector and (u,v) coordinates 
    type Face = SceneAssets.Face

    val readLines : fileName:string -> seq<string>
    val (|ParseRegex|_|) : regex:string -> str:string -> string list option 
    val parseHeader : header:seq<string> -> unit 
    val stringToVertex : s:string -> Vertex
    val parseVertices : vertices:seq<string> -> Vertex[]
    val stringToFace : s:string -> Face
    val parseFaces : vertices:seq<string> -> Face[]
    val parsePLY : fileName:string -> Vertex[]*Face[] 

    