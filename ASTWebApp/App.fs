module App

open Giraffe
open System.IO
open Microsoft.AspNetCore.Http
open FSharp.Text.Lexing
open System.Threading.Tasks

open FunLex
open FunPar

open Spectre.Console
open Absyn

let parse (input: string) : expr =
    let lexbuf = LexBuffer<char>.FromString input
    FunPar.Main FunLex.Token lexbuf

// Recursively add expression nodes to a Spectre.Console TreeNode
let rec addToTree (parent: TreeNode) (expr: expr) =
    match expr with
    | CstI i ->
        parent.AddNode(sprintf "CstI(%d)" i) |> ignore
    | CstB b ->
        parent.AddNode(sprintf "CstB(%b)" b) |> ignore
    | Var v ->
        parent.AddNode(sprintf "Var(%s)" v) |> ignore
    | Let (v, e1, e2) ->
        let node = parent.AddNode(sprintf "Let(%s)" v)
        addToTree node e1
        addToTree node e2
    | Prim (op, e1, e2) ->
        let node = parent.AddNode(sprintf "Prim(%s)" op)
        addToTree node e1
        addToTree node e2
    | If (cond, thenExpr, elseExpr) ->
        let node = parent.AddNode("If")
        addToTree node cond
        addToTree node thenExpr
        addToTree node elseExpr
    | Letfun (f, x, fBody, letBody) ->
        let node = parent.AddNode(sprintf "Letfun(%s, %s)" f x)
        addToTree node fBody
        addToTree node letBody
    | Call (fExpr, argExpr) ->
        let node = parent.AddNode("Call")
        addToTree node fExpr
        addToTree node argExpr

// Draw the AST tree using Spectre.Console and return as string
let drawTree (expr: expr) : string =
    let tree = Tree("AST")  // This creates the root node labeled "AST"

    // Now get the root node (safe in 0.50)
    let rootNode = tree.AddNode("Root")

    // Recursively add AST structure to this root node
    addToTree rootNode expr

    use writer = new StringWriter()

    let settings = AnsiConsoleSettings()
    settings.Out <- AnsiConsoleOutput(writer)

    let console = AnsiConsole.Create(settings)
    console.Write(tree)

    writer.ToString()




// Main run function: parse input and display tree
let run input =
    let ast = parse input
    drawTree ast

//HTTP POST handler reads the submitted code from the form, parses it, and returns the result as plain text.
let parseHandler : HttpHandler =
    fun next ctx ->
        task {
            // Read the form collection
            let! form = ctx.Request.ReadFormAsync()

            // Retrieve the code field from the form collection
            let codeText = ctx.Request.Form.["code"].ToString()

            let result = run codeText
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