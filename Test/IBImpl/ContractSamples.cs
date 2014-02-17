//using IBApi;
///* Copyright (C) 2013 Interactive Brokers LLC. All rights reserved.  This code is subject to the terms
// * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */
//using System.Collections.Generic;

//namespace Test.UnitTest.IBImpl
//{
//    public class ContractSamples
//    {
//        public static Contract getOption()
//        {
//            var contract = new Contract();
//            contract.Symbol = "BAYN";
//            contract.SecType = "OPT";
//            contract.Exchange = "DTB";
//            contract.Currency = "EUR";
//            contract.Expiry = "201512";
//            contract.Strike = 100;
//            contract.Right = "CALL";
//            contract.Multiplier = "100";
//            return contract;
//        }

//        public static Contract GetWrongContract()
//        {
//            var contract = new Contract();
//            contract.Symbol = " IJR ";//note the spaces in the symbol!
//            contract.ConId = 9579976;
//            contract.SecType = "STK";
//            contract.Exchange = "SMART";
//            contract.Currency = "USD";
//            return contract;
//        }

//        public static Contract GetSANTOption()
//        {
//            var contract = new Contract();
//            contract.Symbol = "SANT";
//            contract.SecType = "OPT";
//            contract.Exchange = "MEFFRV";
//            contract.Currency = "EUR";
//            contract.Expiry = "20131220";
//            contract.Strike = 6;
//            contract.Right = "CALL";
//            contract.Multiplier = "100";
//            //this contract for example requires the trading class too in order to prevent any ambiguity.
//            contract.TradingClass = "SANEU";
//            return contract;
//        }

//        public static Contract GetbyIsin()
//        {
//            var contract = new Contract();
//            //contract.SecIdType = "ISIN";
//            //contract.SecId = "XS0356687219";
//            //contract.Exchange = "EURONEXT";
//            contract.Currency = "EUR";
//            contract.Symbol = "IBCID127317301";
//            contract.SecType = "BOND";
//            return contract;
//        }

//        public static Contract getOptionForQuery()
//        {
//            var contract = new Contract();
//            contract.Symbol = "FISV";
//            contract.SecType = "OPT";
//            contract.Exchange = "SMART";
//            contract.Currency = "USD";
//            return contract;
//        }

//        public static Contract GetBondForQuery()
//        {
//            var contract = new Contract();
//            contract.Symbol = "FISV";
//            contract.SecType = "BOND";
//            contract.Currency = "USD";
//            contract.Exchange = "SMART";
//            return contract;
//        }

//        public static Contract getEurUsdForex()
//        {
//            var contract = new Contract();
//            //contract.Symbol = "EUR";
//            contract.SecType = "CASH";
//            //contract.Currency = "USD";
//            //we can also give the conId instead of the whole description
//            //but we still need the exchange though
//            contract.ConId = 12087792;
//            contract.Exchange = "IDEALPRO";
//            return contract;
//        }

//        public static Contract getEurGbpForex()
//        {
//            var contract = new Contract();
//            contract.Symbol = "EUR";
//            contract.SecType = "CASH";
//            contract.Currency = "GBP";
//            contract.Exchange = "IDEALPRO";
//            return contract;
//        }

//        public static Contract getEuropeanStock()
//        {
//            var contract = new Contract();
//            contract.Symbol = "SMTPC";
//            contract.SecType = "STK";
//            contract.Currency = "EUR";
//            contract.Exchange = "SMART";
//            return contract;
//        }

//        public static Contract GetUSStock()
//        {
//            var contract = new Contract();
//            contract.Symbol = "AMZN";
//            contract.SecType = "STK";
//            contract.Currency = "USD";
//            contract.Exchange = "SMART";
//            return contract;
//        }

//        public static Contract getComboContract()
//        {
//            var contract = new Contract();
//            contract.Symbol = "MCD";
//            contract.SecType = "BAG";
//            contract.Currency = "USD";
//            contract.Exchange = "SMART";

//            var leg1 = new ComboLeg();
//            leg1.ConId = 109385219;//Burger King's stocks
//            leg1.Ratio = 1;
//            leg1.Action = "BUY";
//            leg1.Exchange = "SMART";

//            var leg2 = new ComboLeg();
//            leg2.ConId = 9408;//McDonald's stocks
//            leg2.Ratio = 1;
//            leg2.Action = "SELL";
//            leg2.Exchange = "SMART";

//            contract.ComboLegs = new List<ComboLeg>();
//            contract.ComboLegs.Add(leg1);
//            contract.ComboLegs.Add(leg2);

//            return contract;
//        }

//        public static Contract getVixComboContract()
//        {
//            var contract = new Contract();
//            contract.Symbol = "VIX";
//            contract.SecType = "BAG";
//            contract.Currency = "USD";
//            contract.Exchange = "CFE";

//            var leg1 = new ComboLeg();
//            leg1.ConId = 122385422;
//            leg1.Ratio = 1;
//            leg1.Action = "BUY";
//            leg1.Exchange = "CFE";

//            var leg2 = new ComboLeg();
//            leg2.ConId = 124992961;
//            leg2.Ratio = 1;
//            leg2.Action = "SELL";
//            leg2.Exchange = "CFE";

//            contract.ComboLegs = new List<ComboLeg>();
//            contract.ComboLegs.Add(leg1);
//            contract.ComboLegs.Add(leg2);

//            return contract;
//        }
//    }
//}
