module App

open Giraffe
open Microsoft.AspNetCore.Http
open FSharp.Text.Lexing
open System.Threading.Tasks

open FunLex
open FunPar

open Spectre.Console
open Absyn

// A recursive function to print the AST
let rec printAst (expr: expr) (indent: string) : string =
    match expr with 
    | CstI i -> sprintf "%sCstI(%d)" indent i 
    | CstB b -> sprintf "%sCstB(%b)" indent b
    | Var v -> sprintf "%sVar(%s)" indent v
    | Let (v, e1, e2) -> 
        sprintf "%sLet(%s)\n%s\n%s" indent v (printAst e1 (indent + " ")) (printAst e2 (indent + " "))

    | Letfun (f, arg, e1,e2) -> 
        sprintf "%sLetfun(%s, %s)\n%s\n%s" indent f arg (printAst e1 (indent + " ")) (printAst e2 (indent + " "))
    
    | If (cond, thenExpr, elseExpr) ->
        sprintf "%sIf\n%s\n%s\n%s" indent (printAst cond (indent + " ")) (printAst thenExpr (indent + " ")) (printAst elseExpr (indent + " "))

    | Prim (op, e1, e2) -> 
        sprintf "%sPrim(%s)\n%s\n%s" indent op (printAst e1 (indent + " ")) (printAst e2 (indent + " "))

    | Call (e1, e2) ->
        sprintf "%sCall\n%s\n%s" indent (printAst e1 (indent + " ")) (printAst e2 (indent + " "))

// function to print the AST as a tre
let printTree (ast: expr) =
    printAst ast ""

//Parses the given MicroML code string into an AST
let parseCode (code: string) =
    try
        let lexbuf = LexBuffer<char>.FromString(code)   //create a lexical buffer fromt the input string
        let ast = FunPar.Main FunLex.Token lexbuf   //run the parser starting from the Main rule
        
        printTree ast
    with ex ->
        sprintf "Error parsing code: %s" ex.Message

//HTTP POST handler reads the submitted code from the form, parses it, and returns the result as plain text.
let parseHandler : HttpHandler =
    fun next ctx ->
        task {
            // Read the form collection
            let! form = ctx.Request.ReadFormAsync()

            // Retrieve the code field from the form collection
            let codeText = ctx.Request.Form.["code"].ToString()

            let result = parseCode codeText
            return! text result next ctx
        }

//This contains the form where users input their MicroMl code
let indexPageHandler : HttpHandler =
    htmlFile "wwwroot/Index.html"

//The web app router that maps HTTP routes to their corresponding handlers. 
let webApp ()=
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

