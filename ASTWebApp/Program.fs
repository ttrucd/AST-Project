open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Microsoft.Extensions.Hosting

// Define a simple handler to return a response
let webApp =
    choose [
        GET >=> route "/" >=> text "Hello, world!"
        GET >=> route "/about" >=> text "About Page"
    ]

// Configure services and application pipeline in one file
let configureApp (app: IApplicationBuilder) =
    app.UseGiraffe(webApp)

let configureServices (services: IServiceCollection) =
    services.AddGiraffe() |> ignore

// Define the entry point for your web app
[<EntryPoint>]
let main _ =
    // Configure and run the web server
    let host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(fun webHostBuilder ->
                    webHostBuilder
                        .Configure(configureApp)
                        .ConfigureServices(configureServices)
                        .UseUrls("http://localhost:5000") |> ignore)
                .Build()

    host.Run()
    0

