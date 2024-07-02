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
    /// Interaction logic for ProductEditor.xaml
    /// </summary>
    public partial class ProductEditor : Window
    {
        private ConnectionDb ConnectionDb;
        private List<Product> Products;
        public Product selectedProduct;
        public string selectedProductName;
        public ProductEditor()
        {
            InitializeComponent();
            ConnectionDb = new ConnectionDb();
            RefreshProducts();
        }


        //Method for refreshin ChooseProductBox
        private void RefreshProducts()
        {
            Products = ConnectionDb.GetProducts();
            List<string> productNames = Products.Select(p => p.ProductName).ToList();
            ChooseProductBox.ItemsSource = productNames;
        }

        private void Save_btn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProduct != null)
            {
                selectedProduct.ProductName = tbProductName.Text;
                decimal unitPrice = decimal.Parse(UnitPriceBox.Text);
                selectedProduct.UnitPrice = unitPrice;
                ConnectionDb.UpdateProduct(selectedProduct);
                MessageBox.Show("Updated the product " + selectedProduct.ProductName);
                RefreshProducts();
            }
            else
            {
                MessageBox.Show("Unable to update the product data");
            }

        }

        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProduct != null)
            {
                ConnectionDb.DeleteProduct(selectedProduct);
                RefreshProducts();
            }
            else
            {
                MessageBox.Show("Select a product to delete");
            }
            
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow productsWindow = new ProductsWindow();
            productsWindow.Show();
            Close();
        }

        private void CreateItem_btn_Click(object sender, RoutedEventArgs e)
        {
            //Store info
            decimal unitPrice;
            string productName = tbProductName.Text;
            bool parseCheck = decimal.TryParse(UnitPriceBox.Text, out unitPrice);
            string billingType = BillingTypeBox.Text;

            //Check if input is appropriate
            if (parseCheck)
            {
                if (unitPrice <= 0)
                {
                    MessageBox.Show("Please enter a valid unit price greater than zero.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            Product product = new Product(productName, billingType, unitPrice);

            //Create new product
            ConnectionDb.CreateProduct(product);
            MessageBox.Show("New product created");
        }

        private void Done_btn_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow productsWindow = new ProductsWindow();
            productsWindow.Show();
            Close();
        }

        private void ChooseProductBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if an item is selected in the ChooseProductBox
            if (ChooseProductBox.SelectedItem != null)
            {
                // Get the selected product name
                selectedProductName = ChooseProductBox.SelectedItem as string;

                // Find the selected product in the Products list
                selectedProduct = Products.FirstOrDefault(p => p.ProductName == selectedProductName);

                // Check if a product with the selected name exists
                if (selectedProduct != null)
                {
                    // Update the UI with product information
                    tbProductName.Text = selectedProductName;
                    UnitPriceBox.Text = selectedProduct.UnitPrice.ToString();
                    BillingTypeBox.SelectedItem = selectedProduct.Type;
                }
                else
                {
                    // Clear the UI if the selected product is not found
                    tbProductName.Text = string.Empty;
                    UnitPriceBox.Text = string.Empty;
                    BillingTypeBox.SelectedItem = null;
                }
            }
            else
            {
                // Clear the UI if no item is selected in the ChooseProductBox
                tbProductName.Text = string.Empty;
                UnitPriceBox.Text = string.Empty;
                BillingTypeBox.SelectedItem = null;
            }
        }
    }

        
  
}
