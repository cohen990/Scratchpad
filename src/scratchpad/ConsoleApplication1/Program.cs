namespace ConsoleApplication1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Program
    {
        private static void Main(string[] args)
        {
        }
    }

    public class StockPricer
    {
        private List<SpecialOffer> _specialOffers { get; set; }

        public StockPricer()
        {
            SetSpecialOffers();
        }

        public decimal AppraiseList(params Product[] products)
        {
            if (!products.Any())
            {
                return 0;
            }

            foreach (SpecialOffer specialOffer in _specialOffers)
            {
                SpecialOffer offer = specialOffer;
                specialOffer.SetCost(products.Where(x => offer.AffectedProductNames.Any(x.Name.Equals)).ToList());
            }

            return products.Sum(x => x.Cost);
        }

        private void SetSpecialOffers()
        {
            _specialOffers = new List<SpecialOffer>
                {
                    new SpecialOffer {AffectedProductNames = new List<string> {"2for1product"}}
                };
        }
    }

    public class Product
    {
        public Product(decimal price, string name)
        {
            Price = price;
            Name = name;
            Cost = price;
        }

        public decimal Price;

        public decimal Cost;

        public string Name;
    }

    public class SpecialOffer
    {
        public List<string> AffectedProductNames;

        public void SetCost(params Product[] product)
        {
            SetCost(new List<Product>(product));
        }

        public void SetCost(List<Product> products)
        {
            if (products == null || !products.Any())
                return;
            if(!products.All(x => AffectedProductNames.Contains(x.Name)))
                throw new ArgumentException("The products given are not in the affected products list.", "products");

            for (int i = 0; i < products.Count - 1; i += 2)
            {
                products[i].Cost = 0;
            }
        }
    }
}

