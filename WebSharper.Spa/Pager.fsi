namespace WebSharper.Spa

open WebSharper.UI

type RenderedPage =
    {
        Elt : Elt
        IsRoot : bool
        KeepInDom : bool
    }

[<Sealed>]
type Pager<'T> =
    member Doc : Doc

    static member Create
        : route: Var<'T>
        * render: ('T -> RenderedPage)
        -> Pager<'T>

    static member Create
        : route: Var<'T>
        * render: ('T -> RenderedPage)
        * attrs: seq<Attr>
        -> Pager<'T>

[<Sealed>]
type Page =
    static member Create
        : render: ('T -> #Doc)
        * ?isRoot: bool
        * ?keepInDom: bool
        -> ('T -> RenderedPage)

    static member Single
        : render: (View<'T> -> #Doc)
        * ?isRoot: bool
        * ?keepInDom: bool
        -> ('T -> RenderedPage)

    static member Reactive
        : key: ('T -> 'K)
        * render: ('K -> View<'T> -> #Doc)
        * ?isRoot: bool
        * ?keepInDom: bool
        -> ('T -> RenderedPage)
        when 'K : equality
