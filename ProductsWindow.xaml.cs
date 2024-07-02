using htyö_GUI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ProductsWindow.xaml
    /// </summary>
    public partial class ProductsWindow : Window
    {
        private ConnectionDb ConnectionDb;
        private List<Product> Products;
        private decimal ShowTotalPrice;
        private Product selectedProduct;
        string selectedProductName;
        public ProductList ProductList = new ProductList();

        //For enter double tap. Check AmountBox functions -->
        private DateTime lastEnterPressTime = DateTime.MinValue;
        private const double DoubleTapIntervalMilliseconds = 1500;

        public ProductsWindow()
        {
            InitializeComponent();
            //Connection to DB
            ConnectionDb = new ConnectionDb();
            RefreshChooseProductBox();
        }

        private void RefreshChooseProductBox()
        {
            //Product information
            Products = ConnectionDb.GetProducts();

            //Product names
            List<string> productNames = Products.Select(p => p.ProductName).ToList();
            ChooseProductBox.ItemsSource = productNames;
        }

        //Method for calculating total price.
        public decimal CalculateTotalPrice()
        {
            int multiplier = int.Parse(AmountBox.Text);
            decimal unitPrice = decimal.Parse(UnitPriceBox.Text);
            decimal totalPrice = multiplier * unitPrice;
            decimal roundedPrice = (decimal)Math.Round(totalPrice, 2);
            return roundedPrice;
        }

        private void AmountBox_PrewviewInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow numeric input (digits 0-9)
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ChooseProductBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Shows the price of a selected product in the Unit Price -box. 
            selectedProductName = ChooseProductBox.SelectedItem as string;
            selectedProduct = Products.FirstOrDefault(p => p.ProductName == selectedProductName);
            UnitPriceBox.Text = selectedProduct.UnitPrice.ToString();

            if (selectedProduct == null) 
            {
                UnitPriceBox.Text = string.Empty;
            }
        }

        private void AmountBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrWhiteSpace(AmountBox.Text) || AmountBox.Text == "0")
                {
                    MessageBox.Show("Please enter amount.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //If user double taps enter in a short time interval, the 2nd tap acts as 'Add_btn_click'.
                if ((DateTime.Now - lastEnterPressTime).TotalMilliseconds <= DoubleTapIntervalMilliseconds)
                {
                    // Double tap detected, simulate Add_btn click
                    Add_btn_Click(sender, e);
                }

                //Calculate Total Price
                ShowTotalPrice = CalculateTotalPrice();

                //Update the interface
                TotalPriceBox.Text = "Price: " + ShowTotalPrice.ToString();
                lastEnterPressTime = DateTime.Now;
            }
        }

        private void Add_btn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AmountBox.Text) || AmountBox.Text == "0")
            {
                MessageBox.Show("Can't save an item with an Amount of 0 (zero)", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            AddPrice();
        }

        private void AddPrice()
        {
            ShowTotalPrice = CalculateTotalPrice();
            InvoiceItem invoiceItem = new InvoiceItem();
            invoiceItem.Amount = int.Parse(AmountBox.Text);
            invoiceItem.ProductName = selectedProductName;
            invoiceItem.UnitPrice = ShowTotalPrice;

            // Retrieve the ProductID corresponding to the selected product
            var productIds = ConnectionDb.GetProductIds();
            if (productIds.ContainsKey(selectedProductName))
            {
                invoiceItem.ProductId = productIds[selectedProductName];
            }
            else
            {
                // Handle the case where the selected product name is not found
                MessageBox.Show("Selected product not found in database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ProductList.Add(invoiceItem);

            UpdateInvoiceList();
        }

        //Method to refresh the ProductList. Called after Adding or Deleting an item.
        private void UpdateInvoiceList()
        {
            InvoiceList.Items.Clear();
            foreach (var item in ProductList)
            {
                InvoiceList.Items.Add(item.ToString());
            }
        }


        //Delete operation. It removes the item directly from ProductList and then updates the interface.
        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
            // Check if an item is selected
            if (InvoiceList.SelectedItem != null)
            {
                string selectedItemText = InvoiceList.SelectedItem as string;
                string productName = selectedItemText.Split('\t')[0];

                // Find and remove the corresponding item from ProductList
                InvoiceItem itemToRemove = ProductList.FirstOrDefault(item => item.ProductName == productName);
                if (itemToRemove != null)
                {
                    ProductList.Remove(itemToRemove);

                    // Update the InvoiceList
                    UpdateInvoiceList();
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.", "Delete Item", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        

        private void Next_btn_Click(object sender, RoutedEventArgs e)
        {
            if (InvoiceList.Items.Count < 1)
            {
                MessageBox.Show("Add items to invoice before continuing");
                return;
            }
            //Next button clicked -- opens invoiceCreator window, where rest of the information can be filled.
            //Also the productList is passed on to the next page.
            InvoiceCreator invoiceCreator = new InvoiceCreator(ProductList);
            invoiceCreator.Show();
            Close();
        }

        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            // Clear the ProductList if the creation of invoice is interrupted.
            ProductList.Clear();

            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void Product_edit_btn_Click(object sender, RoutedEventArgs e)
        {
            ProductEditor productEditor = new ProductEditor();
            productEditor.Show();
            Close();
        }
    }
}
