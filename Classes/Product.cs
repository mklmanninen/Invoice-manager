using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htyö_GUI.Classes
{
    public class Product
    {
        public int Id {  get; set; }
        public string ProductName { get; set; }
        public string Type { get; set; }
        public decimal UnitPrice { get; set; }

        //Overloading. First one without ID.
        public Product(string nameOfProduct, string type, decimal singlePrice)
        {
            ProductName = nameOfProduct;
            Type = type;
            UnitPrice = singlePrice;
        }
        public Product(int id, string nameOfProduct, string type, decimal singlePrice)
        {
            Id = id;    
            ProductName = nameOfProduct;
            Type = type;
            UnitPrice = singlePrice;
        } 

       
    }
}

