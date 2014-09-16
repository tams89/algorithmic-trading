namespace Algorithm.Core.YahooFinanceService

/// Obtains data from Yahoo's YQL service as XML and parses to record type.
/// This can be used to obtain near realtime info by constantly pinging yahoo.
module Query = 
    open System
    open System.Net
    open System.Xml
    open System.Xml.Linq
    open Microsoft.FSharp.Collections
    open Algorithm.Core.Records
    open Algorithm.Core.Interfaces

    type QueryService(symbol) = 

        // HTML padded query string returns latest market info for symbol.
        let url =  "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20('" 
                     + symbol + "')&env=store://datatables.org/alltableswithkeys"

        let xn s = XName.Get(s)
        let (?) (el : XElement) name = el.Element(xn name).Value
        
        /// Converts a string to a decimal.
        let cd value = 
            match Decimal.TryParse(value) with
            | (true, n) -> n
            | (false,_) -> 0M
        
        /// Converts a string to an integer.
        let ci value = 
            match Int32.TryParse(value) with
            | (true, n) -> n
            | (false,_) -> 0


        /// Gets SOAP data and deserialises into optionsData type.
        let getStockPriceXml = 
            let req = HttpWebRequest.Create(url)
            use resp = req.GetResponse()
            use stream = resp.GetResponseStream()
            use reader = new IO.StreamReader(stream)
            let result = reader.ReadToEnd()
            result
        
        let parseData read = 
            let feed = XDocument.Parse(read)
            
            let elementToStockData (e : XElement) = 
                { Date = DateTime.Now
                  Open = cd e?Open
                  High = cd e?DaysHigh
                  Low = cd e?DaysLow
                  Close = cd e?PreviousClose
                  Volume = cd e?Volume
                  AdjClose = 0M
                  Ask = (cd e?Ask) |> option.Some
                  Bid = (cd e?Volume) |> option.Some }
            
            feed.Root.Element(xn "results").Elements(xn "quote") 
            |> PSeq.ordered
            |> PSeq.map elementToStockData
            |> PSeq.toList

        /// Example: GetOptionsData "MSFT";;
        // TODO make this execute async by queuing requests in to a list of tickers then execute together see Expert F#
        member x.GetStockPrice = 
            getStockPriceXml
            |> parseData