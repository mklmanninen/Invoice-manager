using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace htyö_GUI.Classes
{
    public class DbInitializer
    {
        private const string localWithoutDb = @"Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1;";
        private const string localWithDb = @"Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=invoicedbfinal;";

        public void InitializeDatabase()
        {
            string createDatabaseQuery = @"
    CREATE DATABASE IF NOT EXISTS `invoicedbfinal` /*!40100 DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci */;";

            string createTablesAndInsertsQuery = @"
                USE `invoicedbfinal`;

                -- Create Customer table
                CREATE TABLE IF NOT EXISTS Customer (
                  customer_id INT PRIMARY KEY AUTO_INCREMENT,
                  customer_name VARCHAR(50) NOT NULL,
                  UNIQUE (customer_name)
                );

                -- Create Product table
                CREATE TABLE IF NOT EXISTS Product (
                  product_id INT PRIMARY KEY AUTO_INCREMENT,
                  product_name VARCHAR(255) NOT NULL,
                  billingType VARCHAR(50),
                  unitprice DECIMAL(10,2) NOT NULL,
                  UNIQUE (product_name)
                );

                -- Create Invoice table
                CREATE TABLE IF NOT EXISTS Invoice (
                  invoice_id INT PRIMARY KEY AUTO_INCREMENT,
                  customer_id INT NOT NULL,
                  issue_date DATE,
                  expiration_date DATE,
                  zipcode_customer VARCHAR(20),
                  country_customer VARCHAR(100) NOT NULL,
                  city_customer VARCHAR(50) NOT NULL,
                  address_billing VARCHAR(100) NOT NULL,
                  city_biller VARCHAR(50) NOT NULL,
                  address_biller VARCHAR(100) NOT NULL,
                  zipcode_biller VARCHAR(20) NOT NULL,
                  FOREIGN KEY (customer_id) REFERENCES Customer(customer_id)
                );

                CREATE TABLE IF NOT EXISTS InvoiceItems (
                  invoiceitem_id INT PRIMARY KEY AUTO_INCREMENT,
                  invoice_id INT NOT NULL,
                  product_id INT NOT NULL,
                  amount INT NOT NULL,
                  unit_price DECIMAL(10,2) NOT NULL,
                  FOREIGN KEY (invoice_id) REFERENCES Invoice(invoice_id),
                  FOREIGN KEY (product_id) REFERENCES Product(product_id),
                  UNIQUE (invoice_id, product_id, amount, unit_price)
                );

                -- INSERTS

                -- Customers
                INSERT INTO Customer (customer_id, customer_name) VALUES (1, 'Matti')
                ON DUPLICATE KEY UPDATE customer_name=customer_name;
                INSERT INTO Customer (customer_id, customer_name) VALUES (2, 'Jaakko')
                ON DUPLICATE KEY UPDATE customer_name=customer_name;

                -- Products
                INSERT INTO Product (product_id, product_name, billingType, unitprice) VALUES (1, 'Työ', 'Per hour', 60)
                ON DUPLICATE KEY UPDATE product_name=product_name;
                INSERT INTO Product (product_id, product_name, billingType, unitprice) VALUES (2, 'Silikoni', 'Per unit', 5.95)
                ON DUPLICATE KEY UPDATE product_name=product_name;
                INSERT INTO Product (product_id, product_name, billingType, unitprice) VALUES (3, 'Tiiviste', 'Per unit', 1)
                ON DUPLICATE KEY UPDATE product_name=product_name;
                INSERT INTO Product (product_id, product_name, billingType, unitprice) VALUES (4, 'Kattopaneeli', 'Per unit', 1.95)
                ON DUPLICATE KEY UPDATE product_name=product_name;
                INSERT INTO Product (product_id, product_name, billingType, unitprice) VALUES (5, 'Tiili', 'Per unit', 2)
                ON DUPLICATE KEY UPDATE product_name=product_name;

                -- Invoices
                INSERT INTO Invoice (invoice_id, customer_id, issue_date, expiration_date, address_biller, address_billing, country_customer, city_customer, zipcode_customer, city_biller, zipcode_biller)
                VALUES (1, 1, CURDATE(), DATE_ADD(CURDATE(), INTERVAL 30 DAY), 'Business Address 1', 'Billing Address 1', 'FI Suomi', 'Joensuu', '80100', 'Joensuu', '80100')
                ON DUPLICATE KEY UPDATE issue_date=issue_date;

                INSERT INTO InvoiceItems(invoiceitem_id, invoice_id, product_id, amount, unit_price)
                VALUES (1, 1, 1, 5, 60.00)
                ON DUPLICATE KEY UPDATE amount=amount;
                INSERT INTO InvoiceItems(invoiceitem_id, invoice_id, product_id, amount, unit_price)
                VALUES (2, 1, 2, 1, 5.95)
                ON DUPLICATE KEY UPDATE amount=amount;
                INSERT INTO InvoiceItems(invoiceitem_id, invoice_id, product_id, amount, unit_price)
                VALUES (3, 1, 3, 2, 1)
                ON DUPLICATE KEY UPDATE amount=amount;
                INSERT INTO InvoiceItems(invoiceitem_id, invoice_id, product_id, amount, unit_price)
                VALUES (4, 1, 4, 80, 1.95)
                ON DUPLICATE KEY UPDATE amount=amount;

                INSERT INTO Invoice (invoice_id, customer_id, issue_date, expiration_date, address_biller, address_billing, country_customer, city_customer, zipcode_customer, city_biller, zipcode_biller)
                VALUES (2, 2, CURDATE(), DATE_ADD(CURDATE(), INTERVAL 30 DAY), 'Business Address 1', 'Rakennuskatu 1', 'FI Suomi', 'Helsinki', '00280', 'Joensuu', '80100')
                ON DUPLICATE KEY UPDATE issue_date=issue_date;

                INSERT INTO InvoiceItems(invoiceitem_id, invoice_id, product_id, amount, unit_price)
                VALUES (5, 2, 1, 5, 60.00)
                ON DUPLICATE KEY UPDATE amount=amount;
                INSERT INTO InvoiceItems(invoiceitem_id, invoice_id, product_id, amount, unit_price)
                VALUES (6, 2, 2, 1, 5.95)
                ON DUPLICATE KEY UPDATE amount=amount;
                INSERT INTO InvoiceItems(invoiceitem_id, invoice_id, product_id, amount, unit_price)
                VALUES (7, 2, 3, 2, 1)
                ON DUPLICATE KEY UPDATE amount=amount;
                INSERT INTO InvoiceItems(invoiceitem_id, invoice_id, product_id, amount, unit_price)
                VALUES (8, 2, 4, 80, 1.95)
                ON DUPLICATE KEY UPDATE amount=amount;
            ";

            using (MySqlConnection connection = new MySqlConnection(localWithoutDb))
            {
                MySqlCommand command = new MySqlCommand(createDatabaseQuery, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Database created successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                    return;
                }
            }

            using (MySqlConnection connection = new MySqlConnection(localWithDb))
            {
                MySqlCommand command = new MySqlCommand(createTablesAndInsertsQuery, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Tables created successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }
    }
}

