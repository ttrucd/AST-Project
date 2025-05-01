module App

open Giraffe
open Microsoft.AspNetCore.Http
open FSharp.Text.Lexing
open System.Threading.Tasks

open FunLex
open FunPar

let parseCode (code: string) =
    try
        let lexbuf = LexBuffer<char>.FromString(code)
        let ast = FunPar.Main FunLex.Token lexbuf
        sprintf "AST: %A" ast
    with ex ->
        sprintf "Error parsing code: %s" ex.Message

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

let indexPageHandler : HttpHandler =
    htmlFile "wwwroot/Index.html"

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
