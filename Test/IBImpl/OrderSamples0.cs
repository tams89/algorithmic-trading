﻿/* Copyright (C) 2013 Interactive Brokers LLC. All rights reserved.  This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using IBApi;
using System.Collections.Generic;

namespace Test.UnitTest.IBImpl
{
    public class OrderSamples0
    {
        public static Order LimitOrder()
        {
            var order = new Order();
            order.Action = "BUY";
            order.OrderType = "LMT";
            order.TotalQuantity = 100;
            order.Account = "DU74649";
            order.LmtPrice = 0.8;
            return order;
        }

        public static Order MarketOrder()
        {
            var order = new Order();
            order.Action = "BUY";
            order.OrderType = "MKT";
            order.TotalQuantity = 1;
            return order;
        }

        public static Order LimitOrderForComboWithLegPrice()
        {
            var order = new Order();
            order.Action = "BUY";
            order.OrderType = "LMT";
            order.TotalQuantity = 1;

            var ocl1 = new OrderComboLeg();
            ocl1.Price = 5.0;

            var ocl2 = new OrderComboLeg();
            ocl2.Price = 5.90;
            order.OrderComboLegs = new List<OrderComboLeg>();
            order.OrderComboLegs.Add(ocl1);
            order.OrderComboLegs.Add(ocl2);

            order.SmartComboRoutingParams = new List<TagValue>();
            order.SmartComboRoutingParams.Add(new TagValue("NonGuaranteed", "1"));

            return order;
        }

    }
}
