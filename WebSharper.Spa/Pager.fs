namespace WebSharper.Spa

#nowarn "40" // let rec container

open System.Collections.Generic
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html

[<JavaScript>]
type RenderedPage =
    {
        Elt : Elt
        IsRoot : bool
        KeepInDom : bool
    }

[<Require(typeof<Resources.PagerCss>)>]
[<JavaScript>]
[<Sealed>]
type Page<'T> internal (render: 'T -> Elt, isRoot: bool, keepInDom: bool) =

    member __.IsRoot = isRoot

    member __.KeepInDom = keepInDom

    member __.Render(x) =
        { Elt = render x; IsRoot = isRoot; KeepInDom = keepInDom }

[<JavaScript>]
[<Sealed>]
type Pager<'T> internal (route: Var<'T>, render: 'T -> RenderedPage, attrs: seq<Attr>) =
    let mutable toRemove = None : option<Elt>

    let rec container : EltUpdater =
        let elt =
            div [
                attr.``class`` "ws-page-container"
                on.viewUpdate route.View (fun el r ->
                    let page = render r
                    let elt = page.Elt.Dom
                    let children = el.ChildNodes
                    for i = 0 to children.Length - 1 do
                        if children.[i] !==. elt then
                            (children.[i] :?> Dom.Element).SetAttribute("aria-hidden", "true")
                            |> ignore
                    elt.RemoveAttribute("aria-hidden")
                    match toRemove with
                    | None -> ()
                    | Some toRemove ->
                        el.RemoveChild toRemove.Dom |> ignore
                        container.RemoveUpdated toRemove
                    if not (el.Contains elt) then
                        el.AppendChild elt |> ignore
                        container.AddUpdated page.Elt
                    toRemove <- if page.KeepInDom then None else Some page.Elt
                )
                Attr.Concat attrs
            ] []
        elt.ToUpdater()

    member __.Doc = container :> Doc

[<JavaScript>]
[<Sealed>]
type Page =

    static member private Wrap doc =
        div [attr.``class`` "ws-page"] [doc]

    static member Reactive
        (
            key: 'T -> 'K,
            render: 'K -> View<'T> -> #Doc,
            ?isRoot: bool,
            ?keepInDom: bool
        ) =
        let dic = Dictionary()
        let getOrRender (route: 'T) =
            let k = key route
            match dic.TryGetValue k with
            | true, (var, doc) ->
                Var.Set var route
                doc
            | false, _ ->
                let var = Var.Create route
                let doc = render k var.View |> Page.Wrap
                dic.[k] <- (var, doc)
                doc
        Page<'T>(getOrRender, defaultArg isRoot false, defaultArg keepInDom true)
            .Render

    static member Create(render, ?isRoot, ?keepInDom) =
        Page<'T>(render >> Page.Wrap, defaultArg isRoot false, defaultArg keepInDom true)
            .Render

    static member Single(render, ?isRoot, ?keepInDom) =
        Page.Reactive(ignore, (fun () -> render), ?isRoot = isRoot, ?keepInDom = keepInDom)

type Pager<'T> with

    [<Inline>]
    static member Create (route: Var<'T>, render: 'T -> RenderedPage, attrs: seq<Attr>) =
        Pager<'T>(route, render, attrs)

    [<Inline>]
    static member Create (route: Var<'T>, render: 'T -> RenderedPage) =
        Pager<'T>(route, render, Seq.empty)
