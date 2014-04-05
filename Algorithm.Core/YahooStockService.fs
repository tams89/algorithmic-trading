namespace Algorithm.Core.YahooFinanceService

module Stock = 
    open System
    open System.Net
    open Algorithm.Core.Records
    open Algorithm.Core.Interfaces
    
    type StockDataService() = 
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
            seq { 
                for line in dataLines |> Seq.skip 1 do
                    let infos = line.Split(',')
                    yield { Date = DateTime.Parse infos.[0]
                            Open = decimal infos.[1]
                            High = decimal infos.[2]
                            Low = decimal infos.[3]
                            Close = decimal infos.[4]
                            Volume = decimal infos.[5]
                            AdjClose = decimal infos.[6] }
            }
            |> Seq.take count
            |> Array.ofSeq
            |> Array.rev

        /// Returns the last traded price of a stock.
        /// CSV [ symbol, ask, bid, chamge, last trade date, last trade time, market cap ]
        let getRealtimePrice symbol = 
         let url = "http://finance.yahoo.com/d/quotes.csv?s=" + symbol + "&f=sb2b3c6d1t1j3"
         let wc = new WebClient()
         let data = wc.DownloadString(url)
         let infos = data.Split(',')
         { Date = DateTime.Parse infos.[0]
           Open = decimal infos.[1]
           High = decimal infos.[2]
           Low = decimal infos.[3]
           Close = decimal infos.[4]
           Volume = decimal infos.[5]
           AdjClose = decimal infos.[6] }
        
        interface IStockService with
         member this.GetPreviousStockPrices symbol daysBack = getStockPrices symbol daysBack
         member this.GetStockPrices symbol daysBack = getStockPrices symbol daysBack
         member this.GetRealTimePrice symbol = getRealtimePrice symbol