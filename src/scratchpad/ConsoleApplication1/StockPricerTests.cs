namespace ConsoleApplication1
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    public class StockPricerTests
    {
        [Test]
        public void AppraiseList_givennolist_returns0()
        {
            var pricer = new StockPricer();

            var result = pricer.AppraiseList();

            Assert.AreEqual(0m, result);
        }

        [Test]
        public void AppraiseList_givenoneproduct_returnsproductprice()
        {
            var pricer = new StockPricer();

            var result = pricer.AppraiseList(new Product(10m, "someproduct"));

            Assert.AreEqual(10m, result);
        }

        [Test]
        public void AppraiseList_giventwoproducts_returnssumofprices()
        {
            var pricer = new StockPricer();

            var result = pricer.AppraiseList(new Product(10m, "someproduct"),
                new Product(5m, "someotherproduct"));

            Assert.AreEqual(15m, result);
        }

        [Test]
        public void AppraiseList_giventwo2for1products_returnsdiscountedprice()
        {
            var pricer = new StockPricer();

            var result = pricer.AppraiseList(new Product(10m, "2for1product"),
                new Product(10m, "2for1product"));

            Assert.AreEqual(10m, result);
        }

        [Test]
        public void AppraiseList_givenone2for1productandoneregularproduct_returnsfullprice()
        {
            var pricer = new StockPricer();

            var result = pricer.AppraiseList(new Product(10m, "2for1product"),
                new Product(15m, "otherproduct"));

            Assert.AreEqual(25m, result);
        }

        [Test]
        public void AppraiseList_givenone3for1productandoneregularproduct_returnsfullprice()
        {
            var pricer = new StockPricer();

            var result = pricer.AppraiseList(
                new Product(10m, "2for1product"),
                new Product(10m, "2for1product"),
                new Product(10m, "2for1product"));

            Assert.AreEqual(20m, result);
        }

        [Test]
        public void AppraiseList_givenone5for1productandoneregularproduct_returnsfullprice()
        {
            var pricer = new StockPricer();

            var result = pricer.AppraiseList(
                new Product(10m, "2for1product"),
                new Product(10m, "2for1product"),
                new Product(10m, "2for1product"),
                new Product(10m, "2for1product"),
                new Product(10m, "2for1product"));

            Assert.AreEqual(30m, result);
        }
    }

    [TestFixture]
    public class SpecialOfferTests
    {
        private SpecialOffer _offer;
        private Product _productNotInList;
        private Product _productInList;
        private string _inListIdentifier;
        private string _notInListIdentifier;

        [SetUp]
        public void SetUp()
        {
            _inListIdentifier = "inlist";
            _notInListIdentifier = "notinlist";
            _offer = new SpecialOffer { AffectedProductNames = new List<string> { _inListIdentifier } };
            _productNotInList = new Product(10m, _notInListIdentifier);
            _productInList = new Product(10m, _inListIdentifier);
        }

        [Test]
        public void SetCost_givennothing_doesntthrowexception()
        {
            Assert.DoesNotThrow(() => _offer.SetCost(new List<Product>()) );
        }

        [Test]
        public void SetCost_GivenProductNotInAffectedList_Throws()
        {
            Assert.Throws<ArgumentException>(() => _offer.SetCost(new List<Product> {_productNotInList}));
        }

        [Test]
        public void SetCost_Given1Product_ChangesNothing()
        {
            _offer.SetCost(_productInList);

            Assert.AreEqual(10m, _productInList.Cost);
        }

        [Test]
        public void SetCost_Given2Products_ChangesFirstProductCostToZero()
        {
            var inList1 = new Product(10m, _inListIdentifier);
            var inList2 = new Product(10m, _inListIdentifier);

            _offer.SetCost(inList1, inList2);

            Assert.AreEqual(0, inList1.Cost);
        }

        [Test]
        public void SetCost_Given2Products_DoesntChangeSecondProductCost()
        {
            var inList1 = new Product(10m, _inListIdentifier);
            var inList2 = new Product(10m, _inListIdentifier);

            _offer.SetCost(inList1, inList2);

            Assert.AreEqual(10m, inList2.Cost);
        }

        [Test]
        public void SetCost_Given4Products_ChangesFirstProductCostToZero()
        {
            var inList1 = new Product(10m, _inListIdentifier);
            var inList2 = new Product(10m, _inListIdentifier);
            var inList3 = new Product(10m, _inListIdentifier);
            var inList4 = new Product(10m, _inListIdentifier);

            _offer.SetCost(inList1, inList2, inList3, inList4);

            Assert.AreEqual(0, inList1.Cost);
        }

        [Test]
        public void SetCost_Given4Products_DoesntChangeSecondProduct()
        {
            var inList1 = new Product(10m, _inListIdentifier);
            var inList2 = new Product(10m, _inListIdentifier);
            var inList3 = new Product(10m, _inListIdentifier);
            var inList4 = new Product(10m, _inListIdentifier);

            _offer.SetCost(inList1, inList2, inList3, inList4);

            Assert.AreEqual(10m, inList2.Cost);
        }

        [Test]
        public void SetCost_Given4Products_ChangesThirdProductCostToZero()
        {
            var inList1 = new Product(10m, _inListIdentifier);
            var inList2 = new Product(10m, _inListIdentifier);
            var inList3 = new Product(10m, _inListIdentifier);
            var inList4 = new Product(10m, _inListIdentifier);

            _offer.SetCost(inList1, inList2, inList3, inList4);

            Assert.AreEqual(0, inList3.Cost);
        }

        [Test]
        public void SetCost_Given4Products_DoesntChangeFourthProductCost()
        {
            var inList1 = new Product(10m, _inListIdentifier);
            var inList2 = new Product(10m, _inListIdentifier);
            var inList3 = new Product(10m, _inListIdentifier);
            var inList4 = new Product(10m, _inListIdentifier);

            _offer.SetCost(inList1, inList2, inList3, inList4);

            Assert.AreEqual(10m, inList4.Cost);
        }
    }
}