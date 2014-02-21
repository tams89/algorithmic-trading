namespace FinanceLibrary
    module Main =
        [<EntryPoint>]
        let main args =
            printfn "ans %A" (FinanceLibrary.AlgorithmicTrading.AlgoBackTester.backTest())
            0