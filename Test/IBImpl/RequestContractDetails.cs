﻿/* Copyright (C) 2013 Interactive Brokers LLC. All rights reserved.  This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using IBApi;
using System;
using System.Threading;
using TradingCore.IBImpl;

namespace Test.UnitTest.IBImpl
{
    public class RequestContractDetails : IBClient
    {
        private bool isFinished;

        public bool IsFinished
        {
            get { return isFinished; }
            set { isFinished = value; }
        }

        public static int Main(string[] args)
        {
            var testImpl = new RequestContractDetails();
            testImpl.ClientSocket.eConnect("127.0.0.1", 7496, 0);
            while (testImpl.NextOrderId <= 0) { }

            //We can request the whole option's chain by giving a brief description of the contract
            //i.e. we only specify symbol, currency, secType and exchange (SMART)
            Contract optionContract = ContractSamples.getOptionForQuery();

            testImpl.ClientSocket.reqContractDetails(1, optionContract);

            while (!testImpl.isFinished) { }
            Thread.Sleep(10000);
            Console.WriteLine("Disconnecting...");
            testImpl.ClientSocket.eDisconnect();
            return 0;
        }

        public override void contractDetailsEnd(int reqId)
        {
            Console.WriteLine("Finished receiving all matching contracts.");
            isFinished = true;
        }

        public override void contractDetails(int reqId, ContractDetails contractDetails)
        {
            Console.WriteLine("/*******Incoming Contract Details - RequestId " + reqId + "************/");
            Console.WriteLine(contractDetails.Summary.Symbol + " " + contractDetails.Summary.SecType + " @ " + contractDetails.Summary.Exchange);
            Console.WriteLine("Expiry: " + contractDetails.Summary.Expiry + ", Right: " + contractDetails.Summary.Right);
            Console.WriteLine("Strike: " + contractDetails.Summary.Strike + ", Multiplier: " + contractDetails.Summary.Multiplier);
            Console.WriteLine("/*******     End     *************/\n");
        }

    }
}