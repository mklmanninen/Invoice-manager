using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace htyö_GUI.Classes
{
    public class ConnectionDb
    {
        private const string local = @"Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1;";
        private const string localWithDb = @"Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=invoicedbfinal;";

        // -- PRODCUT OPERATIONS --
        //Method for getting the product information. Used e.g. In ProductsWindow.
        public List<Product> GetProducts()
        {
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                List<Product> products = new List<Product>();
                conn.Open();
                
                MySqlCommand selectProducts = new MySqlCommand("SELECT * FROM product", conn);
                var dr = selectProducts.ExecuteReader();

                while (dr.Read())
                {
                    int id = dr.GetInt32("product_id");
                    string productName = dr.GetString("product_name");
                    string productType = dr.GetString("billingType");
                    decimal SinglePrice = dr.GetDecimal("unitprice");
                    var product = new Product(id, productName, productType, SinglePrice);
                    
                    products.Add(product);
                }
                return products;    
            }
        }

        //To get Product IDs. 
        public Dictionary<string, int> GetProductIds()
        {
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                Dictionary<string, int> productIds = new Dictionary<string, int>();
                conn.Open();

                MySqlCommand selectProducts = new MySqlCommand("SELECT product_id, product_name FROM product", conn);
                var dr = selectProducts.ExecuteReader();

                while (dr.Read())
                {
                    int id = dr.GetInt32("product_id");
                    string productName = dr.GetString("product_name");
                    productIds.Add(productName, id);
                }
                return productIds;
            }
        }

        //Product creation
        public void CreateProduct(Product product)
        {
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();
                
                string query = "INSERT INTO product (product_name, billingType, unitprice) VALUES (@productName, @productType, @unitPrice)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@productName", product.ProductName);
                cmd.Parameters.AddWithValue("@productType", product.Type);
                cmd.Parameters.AddWithValue("@unitPrice", product.UnitPrice);

                cmd.ExecuteNonQuery();
            }
        }

        //Method for updating product info
        public void UpdateProduct(Product product)
        {
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Construct the SQL UPDATE statement
                        string updateProduct = "UPDATE Product SET product_name = @ProductName, billingType = @BillingType, unitprice = @UnitPrice WHERE product_id = @Id";

                        // Create MySqlCommand object
                        MySqlCommand cmd = new MySqlCommand(updateProduct, conn, transaction);

                        // Add parameters
                        cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                        cmd.Parameters.AddWithValue("@BillingType", product.Type);
                        cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                        cmd.Parameters.AddWithValue("@Id", product.Id);

                        // Execute the update query
                        cmd.ExecuteNonQuery();

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of error
                        transaction.Rollback();
                        MessageBox.Show("Error updating product: " + ex.Message);
                    }
                }
            }
        }

        //Method for deleting a product from database
        public void DeleteProduct(Product product) 
        { 
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string deleteProduct = "DELETE FROM product WHERE product_id = @Id";
                        MySqlCommand cmd = new MySqlCommand(deleteProduct, conn, transaction);
                        cmd.Parameters.AddWithValue("@Id", product.Id);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error deleting product. If this Product is already part of an existing invoice, it can't be removed, for the sake of data integrity!");
                    }
                }

            }
        }

        //Get customers
        public List<Customer> GetCustomerList()
        {
            var customerList = new List<Customer>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(localWithDb))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand("SELECT customer_id, customer_name FROM customer;", con);
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var customer = new Customer
                        {
                            ID = dr.GetInt32("customer_id"),
                            Name = dr.GetString("customer_name")
                        };
                        customerList.Add(customer);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or display an error message
                Console.WriteLine($"Error retrieving customer data: {ex.Message}");
                MessageBox.Show($"Error retrieving customer data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return customerList;
        }

        //Create customer
        public void CreateCustomer(Customer customer) 
        {
            using (var conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                string query = "INSERT INTO Customer (customer_name) VALUES (@customer_name);";
                MySqlCommand cmd = new MySqlCommand( query, conn);
                cmd.Parameters.AddWithValue("@customer_name", customer.Name);
                cmd.ExecuteNonQuery();
            }
        }

        public int GetCustomerId(Customer customer)
        {
            int customerId = 0;
            string query = "SELECT customer_id FROM Customer WHERE customer_name = @customerName";

            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customerName", customer.Name);
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    customerId = Convert.ToInt32(result);
                }
            }

            return customerId;
        }

        public string GetCustomerNameById(int customerId)
        {
            string customerName = string.Empty;
            string query = "SELECT customer_name FROM Customer WHERE customer_id = @customerId";

            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    customerName = result.ToString();
                }
            }
            return customerName;
        }


        // ----- INVOICE CRUD METHODS --------
        public List<Invoice> GetInvoices()
        {
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();
                List<Invoice> invoiceList = new List<Invoice>();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM invoice", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Invoice invoice = new Invoice();
                    // Map properties from database columns
                    invoice.ID = dr.GetInt32("invoice_id");
                    invoice.CustomerID = dr.GetInt32("customer_id");
                    invoice.IssueDate = DateOnly.FromDateTime(dr.GetDateTime("issue_date"));
                    invoice.ExpirationDate = DateOnly.FromDateTime(dr.GetDateTime("expiration_date"));
                    invoice.City_biller = dr.GetString("city_biller");
                    invoice.Zipcode_biller = dr.GetString("zipcode_biller");
                    invoice.AddressBiller = dr.GetString("address_biller");
                    invoice.Zipcode_customer = dr.GetString("zipcode_customer");
                    invoice.City_customer = dr.GetString("city_customer");
                    invoice.Country_customer = dr.GetString("country_customer");
                    invoice.AddressBilling = dr.GetString("address_billing");

                    // Add the invoice to the list
                    invoiceList.Add(invoice);
                }
                return invoiceList;
            }
            
        }

        
        //Method for creating an invoice
        public void CreateInvoice(Invoice invoice)
        {
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                // Insert Invoice data
                string query = "INSERT INTO invoice (customer_id, issue_date, expiration_date, address_biller, address_billing, country_customer, city_customer, zipcode_customer, city_biller, zipcode_biller) " +
                    "VALUES (@customer_id, @issueDate, @expirationDate, @addressBiller, @addressBilling, @country_customer, @city_customer, @zipcode_customer, @city_biller, @zipcode_biller)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customer_id", invoice.CustomerID);
                cmd.Parameters.AddWithValue("@issueDate", invoice.IssueDate);
                cmd.Parameters.AddWithValue("@expirationDate", invoice.ExpirationDate);
                cmd.Parameters.AddWithValue("@addressBiller", invoice.AddressBiller);
                cmd.Parameters.AddWithValue("@addressBilling", invoice.AddressBilling);
                cmd.Parameters.AddWithValue("@country_customer", invoice.Country_customer);
                cmd.Parameters.AddWithValue("@city_customer", invoice.City_customer);
                cmd.Parameters.AddWithValue("@zipcode_customer", invoice.Zipcode_customer);
                cmd.Parameters.AddWithValue("@city_biller", invoice.City_biller);
                cmd.Parameters.AddWithValue("@zipcode_biller", invoice.Zipcode_biller);
                cmd.ExecuteNonQuery();

                //To get the invoiceId from DB.
                query = "SELECT LAST_INSERT_ID()";
                cmd = new MySqlCommand(query, conn);
                int invoiceId = Convert.ToInt32(cmd.ExecuteScalar());
               
                // Insert each InvoiceItem into InvoiceItems table
                foreach (InvoiceItem item in invoice.InvoiceItems)
                {
                    query = "INSERT INTO InvoiceItems (invoice_id, product_id, amount, unit_price) VALUES (@invoiceId, @productId, @amount, @unitPrice)";
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@invoiceId", invoiceId); // Use the retrieved invoice ID
                    cmd.Parameters.AddWithValue("@productId", item.ProductId);
                    cmd.Parameters.AddWithValue("@amount", item.Amount);
                    cmd.Parameters.AddWithValue("@unitPrice", item.UnitPrice);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteInvoice(Invoice invoice)
        {
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Delete associated invoice items first
                        string deleteInvoiceItemsQuery = "DELETE FROM InvoiceItems WHERE invoice_id = @invoiceId";
                        MySqlCommand deleteInvoiceItemsCmd = new MySqlCommand(deleteInvoiceItemsQuery, conn, transaction);
                        deleteInvoiceItemsCmd.Parameters.AddWithValue("@invoiceId", invoice.ID);
                        deleteInvoiceItemsCmd.ExecuteNonQuery();

                        // Delete the invoice
                        string deleteInvoiceQuery = "DELETE FROM invoice WHERE invoice_id = @invoiceId";
                        MySqlCommand deleteInvoiceCmd = new MySqlCommand(deleteInvoiceQuery, conn, transaction);
                        deleteInvoiceCmd.Parameters.AddWithValue("@invoiceId", invoice.ID);
                        deleteInvoiceCmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of error
                        transaction.Rollback();
                        Console.WriteLine($"Error deleting invoice: {ex.Message}");
                        MessageBox.Show($"Error deleting invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        public void UpdateInvoice(Invoice invoice)
        {
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Include invoice_id in the WHERE clause to update the specific invoice
                        string updateInvoice = @"UPDATE invoice SET 
                customer_id = @customer_id, 
                issue_date = @issueDate, 
                expiration_date = @expirationDate,
                address_billing = @addressBilling, 
                country_customer = @country_customer, 
                city_customer = @city_customer, 
                zipcode_customer = @zipcode_customer
                WHERE invoice_id = @invoiceId";

                        MySqlCommand cmd = new MySqlCommand(updateInvoice, conn, transaction);

                        cmd.Parameters.AddWithValue("@invoiceId", invoice.ID);
                        cmd.Parameters.AddWithValue("@customer_id", invoice.CustomerID);
                        cmd.Parameters.AddWithValue("@issueDate", invoice.IssueDate);
                        cmd.Parameters.AddWithValue("@expirationDate", invoice.ExpirationDate);
                        cmd.Parameters.AddWithValue("@addressBilling", invoice.AddressBilling);
                        cmd.Parameters.AddWithValue("@country_customer", invoice.Country_customer);
                        cmd.Parameters.AddWithValue("@city_customer", invoice.City_customer);
                        cmd.Parameters.AddWithValue("@zipcode_customer", invoice.Zipcode_customer);

                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error updating invoice: {ex.Message}");
                        MessageBox.Show($"Error updating invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

    }
}