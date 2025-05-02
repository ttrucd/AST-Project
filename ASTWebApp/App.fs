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

let printTokens lexbuf =
    let rec loop () =
        match FunLex.Token lexbuf with
        | EOF -> printfn "End of file"; ()
        | token -> printfn "Token: %A" token; loop ()
    loop ()

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
