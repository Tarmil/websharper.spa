namespace WebSharper.Spa.Tests

open WebSharper
open WebSharper.JavaScript
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.UI.Notation
open WebSharper.Spa

[<JavaScript>]
module Paging =

    type EndPoint =
        | Home
        | About of string * string
        | Unknown of list<string>

    let route =
        Router.Create
            <| function
                | Home -> ["home"]
                | About (id, s) -> ["about"; id; s]
                | Unknown ("unknown" :: l | l) -> "unknown" :: l
            <| function
                | [] | ["home"] -> Home
                | ["about"; id; s] -> About (id, s)
                | l -> Unknown l
        |> Router.InstallHash Home

    let HomePage = Page.Create(keepInDom = false, render = fun () ->
        let rvId = Var.Create "1"
        let rv = Var.Create "a"
        Doc.Concat [
            p [] [text ("Rendered at " + Date().ToTimeString())]
            p [] [
                Doc.Input [] rvId
                Doc.Input [] rv
                Doc.Button "About" [] <| fun () ->
                    route := About (!rvId, !rv)
            ]
            p [] [a [attr.href "#/foo/bar"] [text "Go to unknown URL foo/bar"]]
        ]
    )

    let AboutPage = Page.Reactive(fst, fun id s ->
        Doc.Concat [
            p [] [text ("Rendered at " + Date().ToTimeString())]
            p [] [text id]
            p [] [textView (s.Map snd)]
            p [] [
                Doc.Button "Home" [] <| fun () ->
                    route := Home
            ]
        ]
    )

    let UnknownPage = Page.Single(fun (v: View<list<string>>) ->
        Doc.Concat [
            p [] [text ("Rendered at " + Date().ToTimeString())]
            v.Doc(List.map (fun x -> div [] [text x] :> Doc) >> p [])
            p [] [
                Doc.Button "Home" [] <| fun () ->
                    route := Home
            ]
        ]
    )

    let pager =
        Pager.Create(
            route = route,
            attrs = [Attr.Style "background" "#beccfa"],
            render = function
            | Home -> HomePage ()
            | About (id, p) -> AboutPage (id, p)
            | Unknown path -> UnknownPage path
        )
