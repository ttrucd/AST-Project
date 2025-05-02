module App 

open Giraffe
open System.IO
open Microsoft.AspNetCore.Http
open FSharp.Text.Lexing
open System.Threading.Tasks

open FunLex
open FunPar
open System.Text.Json
open Newtonsoft.Json
open Absyn

// Parse the MicroML code into an AST
let parse (input: string) : expr =
    let lexbuf = LexBuffer<char>.FromString input
    try
        FunPar.Main FunLex.Token lexbuf
    with
    | ex -> 
        printfn "Parsing failed: %s" ex.Message
        raise ex

// Convert the AST to a JSON string
let exprToJson (expr: expr) : string =
    JsonConvert.SerializeObject(expr)

// HTTP POST handler reads the submitted code from the form, parses it, and returns the result as JSON.
let parseHandler : HttpHandler =
    fun next ctx -> 
        task {
            try 
                // Read the form collection
                let! form = ctx.Request.ReadFormAsync()

                // Retrieve the code field from the form collection
                let codeText = ctx.Request.Form.["code"].ToString()

                // Parse the input into AST
                let ast = parse codeText
                
                // Convert the AST to JSON
                let jsonResult = exprToJson ast
                
                // Return the result as JSON response
                return! json jsonResult next ctx
            with 
            | ex -> 
                //Log the exception and return error response
                printfn "Error: %s" ex.Message
                return! json (sprintf "An error: %s" ex.Message) next ctx
        }

// This contains the form where users input their MicroMl code
let indexPageHandler : HttpHandler =
    htmlFile "wwwroot/Index.html"

// The web app router that maps HTTP routes to their corresponding handlers. 
let webApp () =
    choose [
        GET >=>
            choose [
                route "/" >=> indexPageHandler
            ]
        POST >=>
            choose [
                route "/parse" >=> parseHandler
            ]
        setStatusCode 404 >=> text "Not Found"
    ]


// A recursive function to print the AST
// let rec printAst (expr: expr) (indent: string) : string =
//     match expr with 
//     | CstI i -> sprintf "%sCstI(%d)" indent i 
//     | CstB b -> sprintf "%sCstB(%b)" indent b
//     | Var v -> sprintf "%sVar(%s)" indent v
//     | Let (v, e1, e2) -> 
//         sprintf "%sLet(%s)\n%s\n%s" indent v (printAst e1 (indent + " ")) (printAst e2 (indent + " "))

//     | Letfun (f, arg, e1,e2) -> 
//         sprintf "%sLetfun(%s, %s)\n%s\n%s" indent f arg (printAst e1 (indent + " ")) (printAst e2 (indent + " "))
    
//     | If (cond, thenExpr, elseExpr) ->
//         sprintf "%sIf\n%s\n%s\n%s" indent (printAst cond (indent + " ")) (printAst thenExpr (indent + " ")) (printAst elseExpr (indent + " "))

//     | Prim (op, e1, e2) -> 
//         sprintf "%sPrim(%s)\n%s\n%s" indent op (printAst e1 (indent + " ")) (printAst e2 (indent + " "))

//     | Call (e1, e2) ->
//         sprintf "%sCall\n%s\n%s" indent (printAst e1 (indent + " ")) (printAst e2 (indent + " "))

// // function to print the AST as a tre
// let printTree (ast: expr) =
//     printAst ast ""