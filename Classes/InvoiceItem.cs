using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htyö_GUI.Classes
{
    public class InvoiceItem
    {
        public int InvoiceItemId { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public decimal UnitPrice { get; set; }
        public Product Product { get; set; }
        public string ProductName { get; set; }

        public override string ToString()
        {
            return $"{ProductName}\t{UnitPrice:C}\tpcs: {Amount}";
        }

    }
}
