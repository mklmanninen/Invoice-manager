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
    /// Interaction logic for InvoiceBrowser.xaml
    /// </summary>
    public partial class InvoiceBrowser : Window
    {
        private ConnectionDb connectionDb = new ConnectionDb();
        private List<Invoice> invoices = new List<Invoice>();
        private Invoice selectedInvoice;
        public InvoiceBrowser()
        {
            InitializeComponent();
            RefreshListView();
            
            
        }
        //Method to refresh the view incase of changes
        public void RefreshListView()
        {
            invoices = connectionDb.GetInvoices();
            lbInvoices.ItemsSource = invoices;
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow productWindow = new ProductsWindow();
            productWindow.Show();


            //CC.Content = new ProductsUserControl();
            //MessageBox.Show("LOL");
        }

        private void laskut_btn_Click(object sender, RoutedEventArgs e)
        {
            InvoiceBrowser invoiceBroswer = new InvoiceBrowser();
            invoiceBroswer.Show();

        }
        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void EditInvoice_btn_Click(object sender, RoutedEventArgs e)
        {
            selectedInvoice = (Invoice)lbInvoices.SelectedItem;
            if (selectedInvoice != null)
            {
                
                InvoiceEditor invoiceEditor = new InvoiceEditor(selectedInvoice);
                invoiceEditor.Show();
            } 
            else
            {
                MessageBox.Show("Choose an invoice to edit");
            }
            Close();
        }

        private void CreateInvoice_btn_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow productsWindow = new ProductsWindow();
            productsWindow.Show();
            Close();

        }

        private void DeleteInvoice_btn_Click(object sender, RoutedEventArgs e)
        {
            selectedInvoice = (Invoice)lbInvoices.SelectedItem;
            if (selectedInvoice != null)
            {
                connectionDb.DeleteInvoice(selectedInvoice);
            }
            else
            {
                MessageBox.Show("Please select an invoice to delete!");
            }
            RefreshListView();
        }
    }
}
