using htyö_GUI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace htyö_GUI
{
    /// <summary>
    /// Interaction logic for InvoiceEditor.xaml
    /// </summary>
    public partial class InvoiceEditor : Window
    {
        private ConnectionDb connectionDb;
        private Invoice selectedInvoice {  get; set; }
        //Checks if the name gets changed
        private bool isNameCustomerChanged = false;
        public InvoiceEditor(Invoice invoice)
        {
            InitializeComponent();
            connectionDb = new ConnectionDb();
            selectedInvoice = invoice;
            tbAddressCustomer.Text = invoice.AddressBilling;
            tbCityCustomer.Text = invoice.City_customer;
            tbZipCodeCustomer.Text = invoice.Zipcode_customer;
            tbIdCustomer.Text = invoice.CustomerID.ToString();
            tbNameCustomer.Text = invoice.CustomerName;
            
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void finish_btnw_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            int customerId = 0;
            //If the name was changed then..
            if (isNameCustomerChanged)
            {

                //Check existing customers, the idea is that if the customer already exists, we don't create a new customer. One customer can have many Invoices.
                List<Customer> existingCustomers = connectionDb.GetCustomerList();
                string customerNameInput = tbNameCustomer.Text?.Trim();

                if (!string.IsNullOrEmpty(customerNameInput))
                {
                    bool customerExists = existingCustomers.Any(c => c.Name.Equals(customerNameInput, StringComparison.OrdinalIgnoreCase));

                    if (!customerExists)
                    {
                        // Create a new customer if not exists
                        customer = new Customer()
                        {
                            Name = customerNameInput,
                        };
                        connectionDb.CreateCustomer(customer);
                        
                        

                    }
                    else
                    {
                        // Get existing customer
                        customer = existingCustomers.First(c => c.Name.Equals(customerNameInput, StringComparison.OrdinalIgnoreCase));
                        customerId = customer.ID; // Assuming the Customer class has an Id property
                    }
                }
                

            }
            selectedInvoice.RefreshName();
            customerId = connectionDb.GetCustomerId(customer);

            DateTime issueDateNew = dpIssueDate.SelectedDate ?? DateTime.MinValue;
            DateTime expDateNew = dpExpiration.SelectedDate ?? DateTime.MinValue;

            selectedInvoice.CustomerID = customerId;
            selectedInvoice.ID = selectedInvoice.ID;
            selectedInvoice.City_customer = tbCityCustomer.Text;
            selectedInvoice.IssueDate = DateOnly.FromDateTime(issueDateNew);
            selectedInvoice.ExpirationDate = DateOnly.FromDateTime(expDateNew);
            selectedInvoice.AddressBilling = tbAddressCustomer.Text;
            selectedInvoice.Zipcode_customer = tbZipCodeCustomer.Text;
            connectionDb.UpdateInvoice(selectedInvoice);
            
            InvoiceBrowser invoiceBrowser = new InvoiceBrowser();
            invoiceBrowser.Show();
            Close();
        }

        private void tbNameCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            isNameCustomerChanged = true;
        }
    }
}
