// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' ProjectionParameterAttribute
// for more guidance on F# programming.

#load "YahooOptionAPI.fs"
#r "System.Xml.Linq"

open FinanceLibrary
open YahooFinance.Option.YahooOptionAPI
open Microsoft.FSharp.Collections

// Define your library scripting code here

#time "on"                
let res = (GetOptionsData "MSFT")
#time "off"                 