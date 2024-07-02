using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace htyö_GUI.Classes
{
    public class ProductList : IEnumerable<InvoiceItem>
    {
        public Dictionary<string, InvoiceItem> productsByName;

        public ProductList()
        {
            productsByName = new Dictionary<string, InvoiceItem>();
        }

        public void AddOrUpdate(InvoiceItem item)
        {
            if (productsByName.ContainsKey(item.ProductName))
            {
                // Product with the same name already exists, update the amount if necessary
                productsByName[item.ProductName].Amount += item.Amount;
            }
            else
            {
                // Add the new product
                productsByName.Add(item.ProductName, item);
            }
        }

        public void Add(InvoiceItem item)
        {
            AddOrUpdate(item);
        }

        //Clears the whole productList after closing the window.
        public void Clear()
        {
            productsByName.Clear(); 
        }

        public void Remove(InvoiceItem item)
        {
            if (productsByName.ContainsKey(item.ProductName))
            {
                productsByName.Remove(item.ProductName);
            }
        }

        //This method calculates the total price of the invoice
        public decimal GetInvoicePrice()
        {
            return productsByName.Values.Sum(item => item.UnitPrice * item.Amount);
        }

        public IEnumerable<InvoiceItem> GetProducts()
        {
            return productsByName.Values;
        }

        public IEnumerator<InvoiceItem> GetEnumerator()
        {
            return productsByName.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


    }
}
