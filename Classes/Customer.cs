using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htyö_GUI.Classes
{
    public class Customer
    {
        public int ID { get; set; }  
        public string Name { get; set; }
        public HashSet<Invoice> Invoices = new HashSet<Invoice>(); // Store customer's invoices

        public Customer () { }

        public string PrintOnlyIds() // This method is for testing. Prints only the IDs.
        {
            string message = "Henkilön " + Name + " laskut: ";
            foreach (var l in Invoices)
            {
                message += "\n" + "Laskun ID: " + l.ID;
            }
            return message;
        }

        public override string ToString()
        {
            string message = "Henkilön " + Name + " laskut: ";
            foreach (Invoice l in Invoices)
            {
                message += l.ToString();
            }
            return message;
        }
    }
}
