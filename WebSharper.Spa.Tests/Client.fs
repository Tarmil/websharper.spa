namespace WebSharper.Spa.Tests

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Spa.Tests.Paging

[<JavaScript>]
module Client =

    [<SPAEntryPoint>]
    let Main () =
        pager.Doc
        |> Doc.RunPrepend JS.Document.Body
