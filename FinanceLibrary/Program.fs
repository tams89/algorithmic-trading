﻿namespace FinanceLibrary
    module Main =
        [<EntryPoint>]
        let main args =
            printfn "ans %A" (AlgorithmicTrading.AlgoStockTrader.system)
            0