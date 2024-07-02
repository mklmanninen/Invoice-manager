using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htyö_GUI.Classes
{
    public class Invoice
    {
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public DateOnly IssueDate { get; set; }
        public DateOnly ExpirationDate { get; set; }
        public string City_biller {  get; set; }
        public string Zipcode_biller { get; set; }
        public string AddressBiller { get; set; }
        public string Zipcode_customer { get; set; }
        public string City_customer { get; set; }
        public string Country_customer { get; set; }
        public string AddressBilling { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; }
        public decimal InvoiceTotal { get; set; }
        ConnectionDb connectionDb = new ConnectionDb();

        //Constructors (overloaded)
        public Invoice() { }
        public Invoice(List<InvoiceItem> invoiceItems)
        {
            InvoiceItems = invoiceItems;
            InvoiceTotal = InvoiceItems.Sum(item => item.UnitPrice * item.Amount);
        }

        public Invoice(DateOnly dateOfIssue, DateOnly expires, string addressSender, string addressRecipient, List<InvoiceItem> invoiceItems, int customerId)
        {
            IssueDate = dateOfIssue;
            ExpirationDate = expires;
            AddressBiller = addressSender;
            AddressBilling = addressRecipient;
            InvoiceItems = invoiceItems;
            InvoiceTotal = InvoiceItems.Sum(item => item.UnitPrice * item.Amount);
            CustomerID = customerId;
        }

        //The customer name is connected to the invoice by customer Id, the invoice table doesn't techincally contain customerName directly. This is for data normalisation purposes. 
        public string customerName;
        public bool NameChanged = false;
        public string CustomerName
        {
            get
            {
                if (customerName == null || NameChanged == true)
                {
                    customerName = GetCustomerNameById(CustomerID);
                    NameChanged = false;
                }
                return customerName;
            }
        }
        public void RefreshName()
        {
            NameChanged = true;
        }

        //This method connects the customername to invoice via customerID
        private string GetCustomerNameById(int customerId)
        { 
            return connectionDb.GetCustomerNameById(customerId);
        }

        public override string ToString()
        {
            return $"{ID}\t{CustomerName}\t{CustomerID}\t{IssueDate}\t{ExpirationDate}\t{AddressBiller}\t{AddressBilling}\t\t{InvoiceTotal:C}\t{CustomerID}";
        }
    }
}
