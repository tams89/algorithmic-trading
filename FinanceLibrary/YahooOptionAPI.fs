namespace FinanceLibrary.YahooFinanceAPI.Option

module YahooOptionAPI = 

 open System
 open System.IO
 open System.Net
 open System.Xml
 open System.Xml.Linq 
 open System.Globalization
 open System.Text.RegularExpressions
 open Microsoft.FSharp.Collections
 
 let private makeUrlOptions ticker =
     new Uri("http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.options%20where%20symbol%20in%20('" 
      + ticker + "')&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys")
 
 type OptionsData = 
  { Symbol:string; 
    ExpiryDate:DateTime; 
    DaysToExpiry:string; 
    StrikePrice:Decimal; 
    LastPrice:Decimal; 
    ChangeDirection:string; 
    Change:Decimal; 
    Bid:Decimal; 
    Ask:Decimal; 
    Vol:int; 
    OpenInt:int; 
    InTheMoney:bool; 
    AtTheMoney:bool }
 
 let private xn s = XName.Get(s)
 let private (?) (el:XElement) name = el.Element(xn name).Value

 /// Converts a string to a decimal.
 let private cd value =
  match Decimal.TryParse(value) with
  | (false, _) -> 0M
  | (true, n) -> n

 /// Converts a string to an integer.
 let private ci value =
  match Int32.TryParse(value) with
  | (false, _) -> 0
  | (true, n) -> n
 
 /// Calculates expiry date by whether option is mini-option or ordinary.
 let private expiryDate (optionTicker:string) =
     let symbolLength = Regex.Match(optionTicker, @"[^[0-9]*]*").Value.Length // selects symbol from the option ticker.
     if optionTicker.Substring(4,1) = "7" then // mini-option ticker.
        DateTime.ParseExact(String.Format("{0}/{1}/{2}", optionTicker.Substring(8 + 1, 2), optionTicker.Substring(6 + 1, 2), optionTicker.Substring(4 + 1, 2)), "dd/MM/yy", CultureInfo.InvariantCulture)
     elif symbolLength = 4 then // standard 4 char symbol
        DateTime.ParseExact(String.Format("{0}/{1}/{2}", optionTicker.Substring(8, 2), optionTicker.Substring(6, 2), optionTicker.Substring(4, 2)), "dd/MM/yy", CultureInfo.InvariantCulture)
     elif symbolLength = 3 then // 3 char symbol
        DateTime.ParseExact(String.Format("{0}/{1}/{2}", optionTicker.Substring(8 - 1, 2), optionTicker.Substring(6 - 1, 2), optionTicker.Substring(4 - 1, 2)), "dd/MM/yy", CultureInfo.InvariantCulture) 
     elif symbolLength = 2 then // 2 char symbol
        DateTime.ParseExact(String.Format("{0}/{1}/{2}", optionTicker.Substring(8 - 2, 2), optionTicker.Substring(6 - 2, 2), optionTicker.Substring(4 - 2, 2)), "dd/MM/yy", CultureInfo.InvariantCulture) 
     elif symbolLength = 1 then // 1 char symbol
        DateTime.ParseExact(String.Format("{0}/{1}/{2}", optionTicker.Substring(8 - 3, 2), optionTicker.Substring(6 - 3, 2), optionTicker.Substring(4 - 3, 2)), "dd/MM/yy", CultureInfo.InvariantCulture) 
     else // unknown option ticker type.
         failwith "Invalid expiry date parsing operation..."
 
 let private daysToExpiry (expiryDate:DateTime) : string = 
     (expiryDate - DateTime.Today).TotalDays |> string
 
 let private inTheMoney symbol strikePrice marketPrice =    
     // Call option is in the money when the strike price is below the market price.
     if symbol.ToString().[10] = 'C' then strikePrice < marketPrice
     // Put option is in the money when the strike price is above the market price.
     elif symbol.ToString().[10] = 'P' then strikePrice > marketPrice
     else false;
 
 /// Call or Put option is at the money when the strike price is equal to the market price.
 let private atTheMoney strikePrice marketPrice =    
     if strikePrice = marketPrice then true
     else false
 
 /// Gets SOAP data and deserialises into optionsData type.
 let private downloadOptionsFeed (url:Uri) =
   let req = HttpWebRequest.Create(url)
   use resp = req.GetResponse()
   use stream = resp.GetResponseStream()
   use reader = new IO.StreamReader(stream)
   let result = reader.ReadToEnd()
   result

 let private serializeSoapData read = 
  let feed = XDocument.Parse(read)
  let elementToOptionData (e: XElement) =
   {
    Symbol = e.Attribute(xn "symbol").Value
    ExpiryDate = expiryDate(e.Attribute(xn "symbol").Value)
    DaysToExpiry = daysToExpiry (expiryDate(e.Attribute(xn "symbol").Value))
    StrikePrice = cd e?strikePrice
    LastPrice = cd e?lastPrice
    ChangeDirection = e?changeDir
    Change = cd e?change
    Bid = cd e?bid
    Ask = cd e?ask
    Vol = ci e?vol
    OpenInt = ci e?openInt
    InTheMoney = inTheMoney (e.Attribute(xn "symbol").Value) (cd e?strikePrice) (cd e?lastPrice) 
    AtTheMoney = atTheMoney (cd e?strikePrice) (cd e?lastPrice)
   }
  let listOfElems = feed.Root.Element(xn "results").Elements(xn "optionsChain").Elements(xn "option")
  listOfElems
  |> PSeq.ordered
  |> PSeq.map elementToOptionData
  |> PSeq.toList

 /// Example: GetOptionsData "MSFT";;
 // TODO make this execute async by queuing requests in to a list of tickers then execute together see Expert F#
 let GetOptionsData ticker =
  ticker |> makeUrlOptions |> downloadOptionsFeed |> serializeSoapData