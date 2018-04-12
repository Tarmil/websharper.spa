namespace WebSharper.Spa.Resources

open WebSharper

type PagerCss() = inherit Resources.BaseResource("page.css")

[<WebResource("page.css", "text/css")>]
do()
