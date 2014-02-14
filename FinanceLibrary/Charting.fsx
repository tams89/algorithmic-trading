 #r "bin/debug/FSharp.Charting.dll"
 #load "FSharp.Charting.fsx"

 open System
 open System.Net
 open System.Drawing
 open FSharp
 open FSharp.Charting

 let url = "http://ichart.finance.yahoo.com/table.csv?s="

 type Tick = 
  { Date:DateTime
    Open:decimal
    High:decimal
    Low:decimal
    Close:decimal
    Volume:decimal
    AdjClose:decimal }
 
 /// Returns prices (as tuple) of a given stock for a 
 /// specified number of days (starting from the most recent)
 let getStockPrices stock count =
  // Download the data and split it into lines
  let wc = new WebClient()
  let data = wc.DownloadString(url + stock)
  let dataLines = 
      data.Split([| '\n' |], StringSplitOptions.RemoveEmptyEntries) 
 
  // Parse lines of the CSV file and take specified
  // number of days using in the oldest to newest order
  seq { for line in dataLines |> Seq.skip 1 do
            let infos = line.Split(',')
            yield { Date = DateTime.Parse infos.[0]
                    Open = decimal infos.[1]
                    High = decimal infos.[2]
                    Low = decimal infos.[3]
                    Close = decimal infos.[4]
                    Volume = decimal infos.[5]
                    AdjClose = decimal infos.[6] } }
   |> Seq.take count |> Array.ofSeq |> Array.rev
 
 let chart ticker numDaysBack = 
  [ for x in getStockPrices ticker numDaysBack do yield x.Date, x.High, x.Low, x.Close, x.AdjClose ]
  |> Chart.Candlestick

  /// Volume Weighted Average Price
  /// Calculated using mean high low close.
 let vwap (prices: Tick []) period = 
   let ticksInPeriod =
    prices 
    |> Array.filter (fun x -> x.Date <= DateTime.Today.AddDays(-period) || x.Date >= DateTime.Today.AddDays(+period)) // get data within period relative to now.
   (ticksInPeriod |> Array.sumBy (fun x-> ((x.High + x.Low + x.Close)/3.0M) * x.Volume)) // Sum price times volume per trade
    / (ticksInPeriod |> Array.sumBy (fun x -> x.Volume)) // Sum volume over whole period

 let getVwapYahooFinance ticker period = 
  let prices = (getStockPrices ticker period) 
  vwap prices (float period)