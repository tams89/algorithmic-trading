namespace FinanceLibrary.YahooFinanceAPI.Stock

module YahooStockAPI =

 open System
 open System.Net
 
 type Tick = 
  { Date:DateTime
    Open:decimal
    High:decimal
    Low:decimal
    Close:decimal
    Volume:decimal
    AdjClose:decimal }

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
            yield { Date = DateTime.Parse infos.[0]
                    Open = decimal infos.[1]
                    High = decimal infos.[2]
                    Low = decimal infos.[3]
                    Close = decimal infos.[4]
                    Volume = decimal infos.[5]
                    AdjClose = decimal infos.[6] } }
   |> Seq.take count |> Array.ofSeq |> Array.rev