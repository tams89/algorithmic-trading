namespace FinanceLibrary.AlgorithmicTrading

module AlgoCalculation = 
    
    open FinanceLibrary.Records
    
    /// Financial Calculations
    type Calculation() = 
        class
            // Calculated using mean high low close.
            let volumeWeightedAvgPrice (prices : array<Tick>) = 
             if Seq.isEmpty prices then 0M 
             else
              let tickPrices = prices |> Array.map (fun tick -> (tick.High + tick.Low + tick.Close) / 3.0M) 
              let tickVols = prices |> Array.map (fun tick -> tick.Volume) 
              Array.foldBack2 (fun acc vol price -> acc + (price / vol)) tickPrices tickVols 0M

            /// Volume Weighted Average Price
            member this.VWAP(prices) = volumeWeightedAvgPrice prices
        end