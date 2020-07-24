// Code from Hansen and Rischel: Functional Programming using F#     16/12 2012
// Chapter 5: Collections: Lists, maps and sets. 
// Just from Section 5.3 Maps

// Cash register

type ArticleCode = string;;
type ArticleName = string;;
type NoPieces    = int;;
type Price       = int;;

type Register = Map<ArticleCode, ArticleName*Price>;;

type Info    = NoPieces * ArticleName * Price;;
type Infoseq = Info list;;
type Bill    = Infoseq * Price;;

// Version 1

type Item     = NoPieces * ArticleCode;;
type Purchase = Item list;;

let rec makeBill1 reg = function
    | []           -> ([],0)
    | (np,ac)::pur ->
        match Map.tryFind ac reg with
        | Some(aname,aprice) ->
            let tprice          = np*aprice
            let (infos,sumbill) = makeBill1 reg pur
            ((np,aname,tprice)::infos, tprice+sumbill)
        | None                ->
            failwith(ac + " is an unknown article code");;



let reg1 = Map.ofList [("a1",("cheese",25));
                       ("a2",("herring",4));
                       ("a3",("soft drink",5))];;

let pur = [(3,"a2"); (1,"a1")];;


// Version 2

let makeBill2 reg pur =
    let f (np,ac) (infos,billprice) =
        let (aname, aprice) = Map.find ac reg
        let tprice          = np*aprice
        ((np,aname,tprice)::infos, tprice+billprice)
    List.foldBack f pur ([],0);;

// Version 3

type Purchase3 = Map<ArticleCode,NoPieces>;;

let makeBill3 reg pur =
    let f ac np (infos,billprice) =
        let (aname, aprice) = Map.find ac reg
        let tprice          = np*aprice
        ((np,aname,tprice)::infos, tprice+billprice)
    Map.foldBack f pur ([],0);;

let purMap = Map.ofList [("a2",3); ("a1",1)];;













