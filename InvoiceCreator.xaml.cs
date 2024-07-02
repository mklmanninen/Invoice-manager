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
    /// Interaction logic for InvoiceCreator.xaml
    /// </summary>
    public partial class InvoiceCreator : Window
    {
        private ProductList productList;
        private Customer Customer;
        private List<InvoiceItem> invoiceItemsList;
        private ConnectionDb connectionDb;
        public InvoiceCreator(ProductList productList)
        {
            InitializeComponent();
            // Access to product list created on previous page
            this.productList = productList;
            connectionDb = new ConnectionDb();
            UpdateInvoiceList();
        }

        //To update invoiceList. Same method as in last window.
        private void UpdateInvoiceList()
        {
            InvoiceItems.Items.Clear();
            foreach (var item in productList)
            {
                InvoiceItems.Items.Add(item.ToString());
            }
        }


        private void finish_btn_Click(object sender, RoutedEventArgs e)
        {
            bool check = CheckIfEmpty();
            if (check) 
            {
                MessageBox.Show("Fill out the customer information first");
                return;
            }
            
            invoiceItemsList = new List<InvoiceItem>();
            foreach (var item in productList.productsByName)
            {
                invoiceItemsList.Add(item.Value);
            }

            // Initialize customer variables
            Customer customer = null;
            int customerId = 0;

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
            customerId = connectionDb.GetCustomerId(customer);

            DateTime selectedDateTime = ExpDatePicker.SelectedDate.Value;
            Invoice invoice = new Invoice(invoiceItemsList)
            {
                IssueDate = DateOnly.FromDateTime(DateTime.Now),
                ExpirationDate = DateOnly.FromDateTime(selectedDateTime.Date),
                AddressBiller = tbAddressBusiness.Text,
                AddressBilling = tbAddressCustomer.Text,
                Zipcode_biller = tbZipcodeBusiness.Text,
                Zipcode_customer = tbZipCodeCustomer.Text,
                City_biller = tbCityBusiness.Text,
                City_customer = tbCityCustomer.Text,
                Country_customer = tbCountryBusiness.Text,
                CustomerID = customerId
            };
            connectionDb.CreateInvoice(invoice);
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private bool CheckIfEmpty()
        {
            bool ifEmpty = false;

            if (string.IsNullOrEmpty(tbNameCustomer.Text))
            {
                ifEmpty = true;
            }
            if (string.IsNullOrEmpty(tbAddressCustomer.Text))
            {
                ifEmpty = true;
            }
            if (string.IsNullOrEmpty(tbZipCodeCustomer.Text))
            {
                ifEmpty = true;
            }
            if (string.IsNullOrEmpty(tbCityCustomer.Text))
            {
                ifEmpty = true;
            }
            if (ExpDatePicker.SelectedDate == null)
            {
                ifEmpty = true;
            }

            return ifEmpty;
        }
    }
}
