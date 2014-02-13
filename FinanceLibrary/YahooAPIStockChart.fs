namespace FinanceLibrary.YahooFinance.Option

module YahooAPIStockChart =

 open System
 open System.Net
 open System.Drawing
 open FSharp.Charting
 
 let url = "http://ichart.finance.yahoo.com/table.csv?s="
 
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
            yield float infos.[1], float infos.[2], 
                  float infos.[3], float infos.[4] }
   |> Seq.take count |> Array.ofSeq |> Array.rev
 
 let chart numDays ticker = 
  [ for o,h,l,c in getStockPrices ticker numDays do yield h,l,o,c ]
  |> Chart.Candlestick