//using IBApi;
//using NUnit.Framework;
//using System.Collections.Generic;
//using System.Threading;
//using TradingCore.IBImpl;

//namespace Test.UnitTest.IBImpl
//{
//    [TestFixture]
//    public class EWrapperImplTests
//    {
//        private IBClient IBClient;

//        [SetUp]
//        public void ClientConnection()
//        {
//            IBClient = new IBClient();
//            IBClient.ClientSocket.eConnect("127.0.0.1", 7496, 0, false);

//            // One good way of knowing if we can proceed is by monitoring the order's nextValidId reception which comes down automatically after connecting. */
//            while (IBClient.NextOrderId <= 0) { }
//        }

//        [TearDown]
//        public void Disconnect()
//        {
//            IBClient.ClientSocket.eDisconnect();
//        }

//        [Test]
//        public void RealTimeMarketData()
//        {
//            /*** Real time market data operations  - Tickers ***/
//            /*** Requesting real time market data ***/
//            IBClient.ClientSocket.reqMarketDataType(2);
//            IBClient.ClientSocket.reqMktData(1001, ContractSamples.getEurUsdForex(), "", false, GetFakeParameters(3));
//            IBClient.ClientSocket.reqMktData(1002, ContractSamples.getOption(), "", false, GetFakeParameters(3));
//            IBClient.ClientSocket.reqMktData(1003, ContractSamples.getEuropeanStock(), "", false, GetFakeParameters(3));
//            Thread.Sleep(2000);
//            /*** Canceling the market data subscription ***/
//            IBClient.ClientSocket.cancelMktData(1001);
//            IBClient.ClientSocket.cancelMktData(1002);
//            IBClient.ClientSocket.cancelMktData(1003);
//        }

//        [Test]
//        public void RealTimeMarketDataOperations_MarketDepth()
//        {
//            /*** Real time market data operations  - Market Depth ***/
//            /*** Requesting the Deep Book ***/
//            //wrapper.ClientSocket.reqMarketDepth(2001, ContractSamples.getEurGbpForex(), 5, GetFakeParameters(2));
//            //Thread.Sleep(2000);
//            /*** Canceling the Deep Book request ***/
//            //wrapper.ClientSocket.cancelMktDepth(2001);
//        }

//        [Test]
//        public void RealTimeMarketDataOperations_RealTimeBars()
//        {
//            /*** Real time market data operations  - Real Time Bars ***/
//            /*** Requesting real time bars ***/
//            //wrapper.ClientSocket.reqRealTimeBars(3001, ContractSamples.getEurGbpForex(), -1, "MIDPOINT", true, GetFakeParameters(4));
//            //Thread.Sleep(2000);
//            /*** Canceling real time bars ***/
//            //wrapper.ClientSocket.cancelRealTimeBars(3001);
//        }

//        [Test]
//        public void RealTimeMarketDataOperations_StreamedOrFrozen()
//        {
//            /*** Real time market data operations  - Streamed or Frozen ***/
//            /*** Switch to frozen or streaming***/
//            //wrapper.ClientSocket.reqMarketDataType(1);
//        }

//        [Test]
//        public void HistoricalDataOperations()
//        {
//            /*** Historical Data operations ***/
//            /*** Requesting historical data ***/
//            //wrapper.ClientSocket.reqHistoricalData(4001, ContractSamples.getEurGbpForex(), "20130722 23:59:59", "1 D", "1 min", "MIDPOINT", 1, 1, GetFakeParameters(4));
//            //wrapper.ClientSocket.reqHistoricalData(4002, ContractSamples.getEuropeanStock(), "20131009 23:59:59", "10 D", "1 min", "TRADES", 1, 1);
//            /*** Canceling historical data requests ***/
//            //wrapper.ClientSocket.cancelHistoricalData(4001);
//            //wrapper.ClientSocket.cancelHistoricalData(4002);
//        }

//        [Test]
//        public void OptionSpecific()
//        {
//            /*** Options Specifics ***/
//            /*** Calculating implied volatility ***/
//            //wrapper.ClientSocket.calculateImpliedVolatility(5001, ContractSamples.getOption(), 5, 85, GetFakeParameters(6));
//            /*** Canceling implied volatility ***/
//            //wrapper.ClientSocket.cancelCalculateImpliedVolatility(5001);
//            /*** Calculating option's price ***/
//            //wrapper.ClientSocket.calculateOptionPrice(5002, ContractSamples.getOption(), 0.22, 85, GetFakeParameters(1));
//            /*** Canceling option's price calculation ***/
//            //wrapper.ClientSocket.cancelCalculateOptionPrice(5002);
//            /*** Exercising options ***/
//            //wrapper.ClientSocket.exerciseOptions(5003, ContractSamples.GetSANTOption(), 1, 1, null, 1);
//        }

//        [Test]
//        public void ContractInformation()
//        {
//            /*** Contract information ***/
//            //wrapper.ClientSocket.reqContractDetails(6001, ContractSamples.GetbyIsin());
//            //wrapper.ClientSocket.reqContractDetails(210, ContractSamples.getOptionForQuery());
//            //wrapper.ClientSocket.reqContractDetails(211, ContractSamples.GetBondForQuery());
//        }

//        [Test]
//        public void MarketScanners()
//        {
//            /*** Market Scanners ***/
//            /*** Requesting all available parameters which can be used to build a scanner request ***/
//            //wrapper.ClientSocket.reqScannerParameters();
//            /*** Triggering a scanner subscription ***/
//            //wrapper.ClientSocket.reqScannerSubscription(7001, ScannerSubscriptionSamples.GetScannerSubscription(), GetFakeParameters(5));
//            /*** Canceling the scanner subscription ***/
//            //wrapper.ClientSocket.cancelScannerSubscription(7001);
//        }

//        [Test]
//        public void ReutersFundamentals()
//        {
//            /*** Reuter's Fundamentals ***/
//            /*** Requesting Fundamentals ***/
//            //wrapper.ClientSocket.reqFundamentalData(8001, ContractSamples.GetUSStock(), "snapshot", GetFakeParameters(4));
//            /*** Camceling fundamentals request ***/
//            //wrapper.ClientSocket.cancelFundamentalData(8001);
//        }

//        [Test]
//        public void IBBulletin()
//        {
//            /*** IB's Bulletins ***/
//            /*** Requesting Interactive Broker's news bulletins */
//            //wrapper.ClientSocket.reqNewsBulletins(true);
//            /*** Canceling IB's news bulletins ***/
//            //wrapper.ClientSocket.cancelNewsBulletin();
//        }

//        [Test]
//        public void AccountManagement()
//        {
//            /*** Account Management ***/
//            /*** Requesting managed accounts***/
//            //wrapper.ClientSocket.reqManagedAccts();
//            /*** Requesting accounts' summary ***/
//            //wrapper.ClientSocket.reqAccountSummary(9001, "All", AccountSummaryTags.GetAllTags());
//            /*** Subscribing to an account's information. Only one at a time! ***/
//            //wrapper.ClientSocket.reqAccountUpdates(true, "U150462");
//            /*** Requesting all accounts' positions. ***/
//            //wrapper.ClientSocket.reqPositions();
//        }

//        [Test]
//        public void OrderHandling()
//        {
//            /*** Order handling ***/
//            /*** Requesting the next valid id ***/
//            //wrapper.ClientSocket.reqIds(-1);
//            /*** Requesting all open orders ***/
//            //wrapper.ClientSocket.reqAllOpenOrders();
//            /*** Taking over orders to be submitted via TWS ***/
//            //wrapper.ClientSocket.reqAutoOpenOrders(true);
//            /*** Requesting this API client's orders ***/
//            //wrapper.ClientSocket.reqOpenOrders();
//            /*** Placing/modifying an order - remember to ALWAYS increment the nextValidId after placing an order so it can be used for the next one! ***/
//            //Order order = OrderSamples.LimitOrder();
//            //order.OrderMiscOptions = GetFakeParameters(3);
//            //wrapper.ClientSocket.placeOrder(wrapper.NextOrderId++, ContractSamples.getComboContract(), order);
//            //wrapper.ClientSocket.placeOrder(wrapper.NextOrderId++, ContractSamples.getComboContract(), OrderSamples.LimitOrderForComboWithLegPrice());
//            //wrapper.ClientSocket.placeOrder(wrapper.NextOrderId++, ContractSamples.getVixComboContract(), OrderSamples.LimitOrder());
//            /*** Cancel all orders for all accounts ***/
//            //wrapper.ClientSocket.reqGlobalCancel();
//            /*** Request the day's executions ***/
//            //wrapper.ClientSocket.reqExecutions(10001, new ExecutionFilter());
//        }

//        [Test]
//        public void FinancialAdvisorOperations()
//        {
//            /*** Financial Advisor Exclusive Operations ***/
//            /*** Requesting FA information ***/
//            //wrapper.ClientSocket.requestFA(Constants.FaAliases);
//            //wrapper.ClientSocket.requestFA(Constants.FaGroups);
//            //wrapper.ClientSocket.requestFA(Constants.FaProfiles);
//            /*** Replacing FA information - Fill in with the appropriate XML string. ***/
//            //wrapper.ClientSocket.replaceFA(Constants.FaGroups, "");
//        }

//        [Test]
//        public void RequestTWSCurrentTime()
//        {
//            /*** Request TWS' current time ***/
//            //wrapper.ClientSocket.reqCurrentTime();
//        }

//        [Test]
//        public void SetTWSLoggingLevel()
//        {
//            /*** Setting TWS logging level  ***/
//            //wrapper.ClientSocket.setServerLogLevel(1);
//        }

//        [Test]
//        public void Linking()
//        {
//            /*** Linking ***/
//            //wrapper.ClientSocket.verifyRequest("a name", "9.71");
//            //wrapper.ClientSocket.verifyMessage("apiData");
//            //wrapper.ClientSocket.queryDisplayGroups(123);
//            //wrapper.ClientSocket.subscribeToGroupEvents(124, 1);
//            //wrapper.ClientSocket.updateDisplayGroup(125, "contract info");
//            //wrapper.ClientSocket.unsubscribeFromGroupEvents(124);
//        }

//        private static List<TagValue> GetFakeParameters(int numParams)
//        {
//            var fakeParams = new List<TagValue>();
//            for (var i = 0; i < numParams; i++)
//                fakeParams.Add(new TagValue("tag" + i, i.ToString()));
//            return fakeParams;
//        }

//    }
//}
