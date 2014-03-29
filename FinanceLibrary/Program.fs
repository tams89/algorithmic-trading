namespace FinanceLibrary
    module Main =
        [<EntryPoint>]
        let main args =
//            printfn "ans %A" (FinanceLibrary.AlgorithmicTrading.AlgoBackTester.backTest())
            printfn "ans %A" (FinanceLibrary.FIX.execute())
            0