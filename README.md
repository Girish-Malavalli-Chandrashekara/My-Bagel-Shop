# My-Bagel-Shop
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BagelShop
{
    public partial class BagelShopForm : Form
    {
        //Declaration of Public string Array variable
        public static string[] BagelNames = { "Plained Bagel", "Raisin Bagel", "Sesame Bagel", "Orange Bagel", "Onions Bagel", "Asiago Bagel", "Chicken Bagel", "Cajjun Bagel", "French Bagel", "British Bagel", "W-Wheat Bagel", "Garlic Bagel", "Exotic Bagel" };
        public static string[] BagelSize = { "Small", "Medium", "Regular", "Large", "XL" };
        public static decimal[,] BagelPrices =
        {
            {1.50M,2.50M,3.50M,4.50M,5.50M},
            {2.50M,3.50M,4.50M,5.50M,6.50M},
            {1.50M,2.50M,3.50M,4.50M,5.50M},
            {3.50M,4.50M,5.50M,6.50M,7.50M},
            {1.00M,2.00M,3.00M,4.00M,5.00M},
            {2.55M,3.55M,4.55M,5.55M,6.55M},
            {1.50M,2.50M,3.50M,4.50M,5.50M},
            {3.50M,4.50M,5.50M,6.50M,7.50M},
            {1.50M,2.50M,3.50M,4.50M,5.50M},
            {2.50M,3.50M,4.50M,5.50M,6.50M},
            {2.55M,3.55M,4.55M,5.55M,6.55M},
            {2.75M,3.75M,4.75M,5.75M,6.75M},
            {3.50M,4.50M,5.50M,6.50M,7.50M},
        };
        
        public  int[,] BagelStocks = new int[13, 5];
        
        //Declaration of Public Variables.
        
        int Quantity, RemainingQuantity;
        decimal Total = 0, Price, PriceOf1Bagel, SubTot=0;
        int BagelNameIndex = 0, BagelSizeIndex = 0;
        string SEPARATOR = ",", BagelsName = "", BagelsSize = "";
        public static int[,] BagelStocksTemp = new int[13, 5];
        int FIRST_LINE_ITEM_POS = 3, FirstLine = 0, SecondLine = 1, ThirdLine = 2;
        
        //Declaring public variables using Get/Set Methods to access variables across the form.
        public static int TotalBagelsSld { get; private set; } 
        public static int TotalSalesAmt { get; private set; }
        public static int TotalTranxn { get; private set; } 
        public static int TotalAvgValue { get; private set; }
        public static int SoldQuantity{ get; private set; }
        
        public int[,] Starting = new int[13, 5];

        ListBox ListBoxSearchResults;
       
        //Searchunique Index Methos is called in Search Function to fetch Unique Transaction number in the Transaction number 
        private int SearchUniqueIndex(String SearchValue, string[] ArraytoSearch)
        {
            //Iterating into array to Search Transaction ID
            for(int i = 0; i < ArraytoSearch.Length; i++)
            {
                if (ArraytoSearch[i].Contains(SearchValue))
                {
                    return i;
                }
            }
            return -1;
        }
        //SearchAll Methos is called in Search Function to fetch Date and Time in the Transaction number
        private List<string> SearchAll(string SearchValue, String[] ArrayToSearch)
        {
            List<string> ResultList = new List<string>();
            foreach(string Line in ArrayToSearch)
            {
                if (Line.Contains(SearchValue))
                {
                    ResultList.Add(Line);
                }
                else
                {
                    MessageBox.Show("Date not found", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
                }
            }
            return ResultList;
        }

        // Summary function used Multiple Form
        private void SummaryButton_Click(object sender, EventArgs e)
        {
            SummaryForm S2 = new SummaryForm();
            S2.Show(this);
        }

        //AddFileRecordToListBox  Method is called in Search Function to join Transaction file with Unique Transaction number, Date / time and total Sales of Bagels.
        private void AddFileRecordToListBox(string FileLines, ListBox ListBoxSearchResults)
        {
            string[] Orders = FileLines.Split(',');
            string TxNo = Orders[FirstLine];
            string Date= Orders[SecondLine];
            string TotalSale = Orders[ThirdLine];

            SearchListBox.Items.Add(TxNo);
            SearchListBox.Items.Add(Date);
            SearchListBox.Items.Add(TotalSale);

            for (int i = FIRST_LINE_ITEM_POS; i < Orders.Length; i++)
            {
                SearchListBox.Items.Add(Orders[i]);
            }
            SearchListBox.Items.Add("\n");
        }

        //Search Function to check Transaction file and fetch search results.
        private void SearchButton_Click(object sender, EventArgs e)
        {
            string[] AllFileLines;
            String Search= SearchTextBox.Text;
            
            //Checking the Search text box if it has any null values 
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
                MessageBox.Show("Please Enter Value to Search ","Empty Search Value",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    //reading transaction file into 'AllFileLines' array
                    AllFileLines = File.ReadAllLines("TransactionFile.txt");
                    
                    //checks transaction radio button is checked and executes the search function for it.
                    if (TransactionRadioButton.Checked)
                    {
                        //calling Search uniqueIndex method to search for Transaction ID in file.
                        int Index = SearchUniqueIndex(Search, AllFileLines);
                        
                        //if transaction found the record is displayed 
                        if (Index != -1)
                        {
                            AddFileRecordToListBox(AllFileLines[Index], ListBoxSearchResults);
                        }
                        else
                        {
                            MessageBox.Show("Transaction Not found ", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    //Checks if date radio button is checked and search for search results of Date in file.
                    else if (DateRadioButton.Checked)
                    {
                        //Calling SearchAll method to find date in file nd placing it in list string
                        List<string> SalesByDate = SearchAll(Search, AllFileLines);
                        for (int i = 0; i < SalesByDate.Count; i++)
                        {
                            AddFileRecordToListBox(SalesByDate[i], ListBoxSearchResults);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Choose between any two Radio buttons to Search ", "Radio buttons not checked", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                // Catches exception if file is unable to be read.
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public BagelShopForm()
        {
            InitializeComponent();
            
        }
        // function to clear search button
        private void SearchClearButton_Click(object sender, EventArgs e)
        {
            SearchTextBox.Text = "";
            SearchListBox.Items.Clear();
            TransactionRadioButton.Checked = false;
            DateRadioButton.Checked = false;
        }

        //function to read the string of array from the file
        private bool ReadFromFile(String filename, ref int[,] records)
        {
            //declaring local variables
            int row = 0;
            string[] recordRow, allRecordRows;

            // try catch block to handle exceptions from file.
            try
            {
                allRecordRows = File.ReadAllLines(filename);
                for (row = 0; row < allRecordRows.Length; row++)
                {
                    recordRow = allRecordRows[row].Split(',');
                    for (int col = 0; col < records.GetLength(1); col++)
                    {
                        records[row, col] = int.Parse(recordRow[col]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }
        
        // function to write the file to an string of array.
        private bool WriteToFile(String fileName, int[,] records)
        {
            //declaration of local variables.
            int row = 0;
            int[] recordRow = new int[records.GetLength(1)];

            // try catch block to handle exceptions from file.
            try
            {
                StreamWriter stream = File.CreateText(fileName);
                for (row = 0; row < records.GetLength(0); row++)
                {
                    for (int col = 0; col < records.GetLength(1); col++)
                    {
                        recordRow[col] = (int)records[row, col];
                    }
                    stream.WriteLine(String.Join(SEPARATOR.ToString(), recordRow));
                }
                stream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

        //add to order function to order the bageltype, bagel size, quantity which will move the order into the cart.
        private void AddToOrderButton_Click(object sender, EventArgs e)
        {
            //if loop to check bagel type and bagel size selected from the listbox
            if ( BagelListBox.SelectedIndex == -1 || BagelSizeListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Bagel and Size", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //// try catch block to handle exceptions from file.
                try
                {
                    ReadFromFile("BagelStocksTemp.txt", ref BagelStocksTemp);
                    
                    // inputing values to variables and logic of selecting values from listbox into variable using array.  
                    Quantity = int.Parse(QuantityTextBox.Text);
                    BagelNameIndex = BagelListBox.SelectedIndex;
                    BagelSizeIndex = BagelSizeListBox.SelectedIndex;
                    var AvailableQuantity = BagelStocksTemp[BagelNameIndex, BagelSizeIndex];
                    int RequestQuantity = int.Parse(QuantityTextBox.Text);
                    BagelsName = BagelListBox.SelectedItem.ToString();
                    BagelsSize = BagelSizeListBox.SelectedItem.ToString();
                    PriceOf1Bagel = BagelPrices[BagelNameIndex, BagelSizeIndex];
                    
                    // checks the Quantity available in the stocks and requested quantity.
                    if (RequestQuantity > AvailableQuantity)
                    {
                        MessageBox.Show("Your requested quantity is not available, We only have  " + AvailableQuantity + " Numbers");
                        QuantityTextBox.Text = Convert.ToString(AvailableQuantity);

                    }
                    else
                    {
                        RunningTotalListBox.Items.Add(BagelsName + ",   " + BagelsSize + ",   " + Quantity + ",   " + PriceOf1Bagel);
                        SubTot = Price * RequestQuantity;
                        Total = SubTot + Total;
                        RemainingQuantity = AvailableQuantity - RequestQuantity;
                        BagelStocksTemp[BagelListBox.SelectedIndex, BagelSizeListBox.SelectedIndex] = RemainingQuantity;
                        WriteToFile("BagelStocksTemp.txt", BagelStocksTemp);
                        
                        SoldQuantity += RequestQuantity;
                    }
                    TotalLabel.Text = Total.ToString();
                }
                catch (Exception)
                {
                    MessageBox.Show("Enter numerical values to text", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //Complete order button function completes the order by printing the receipt and writing the order details to the Transaction file.
        private void CompleteOrderButton_Click(object sender, EventArgs e)
        {
            if (RunningTotalListBox.Items.Count > 0)
            {
                //generating unique transaction id
                Random Number = new Random();
                int UID = Number.Next(100000, 9999999);

                // generates date and time from system.
                DateTime Today = DateTime.Now;

                //array of string to concat transaction ID, date/time, and Total sales into the string of Array.
                string[] Orders = new string[FIRST_LINE_ITEM_POS + RunningTotalListBox.Items.Count];
                string[] Lines = new string[RunningTotalListBox.Items.Count];
                string Msg = "MY BAGEL SHOP \n\n Order Confirmation Details: \n\n Transaction ID: " + UID + "\n\n Date & Time:" + Today + "\n";
                
                Orders[FirstLine] = UID.ToString();
                Orders[FirstLine + 1] = Today.ToString();
                Orders[FirstLine + 2] = Total.ToString();
                
                // for loop to join the the concated Id Date/time, totalsales to order details in the cart like bagel type/size, price 
                for (int i = 0; i < RunningTotalListBox.Items.Count; i++)
                {
                    Lines[i] = RunningTotalListBox.Items[i].ToString();
                    Msg += "\n\n" + Lines[i];
                    Orders[FIRST_LINE_ITEM_POS + i] = RunningTotalListBox.Items[i].ToString();
                }
                Msg += "\n\n\nTotal " + Total;
                MessageBox.Show(Msg, "Orders Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                //try catch to handle file handling exception
                try
                {
                    StreamWriter WriteFile = File.AppendText("TransactionFile.txt");
                    using (WriteFile)
                    {
                        WriteFile.WriteLine(String.Join(SEPARATOR.ToString(), Orders));
                        WriteFile.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Order Not Places, No Orders in the selected", "No Order", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //reading/writing the order details into file in the debug folder to maintain stock availability using temp file.

            ReadFromFile("BagelStocksTemp.txt", ref BagelStocks);
            WriteToFile("BagelStocks.txt", BagelStocks);

            ReadFromFile("BagelStocks.txt", ref BagelStocksTemp);
            WriteToFile("BagelStocksTemp.txt", BagelStocksTemp);

            //Summary function calculation. 
            TotalBagelsSld += SoldQuantity;
            TotalSalesAmt += (int)Total;
            TotalTranxn++;
            TotalAvgValue = TotalSalesAmt / TotalTranxn;
            ClearButton_Click(sender, e);

        }
        //Remove button to Remove Item from the Cart by selecting the index 
        private void RemoveItemButton_Click_1(object sender, EventArgs e)
        {
            int SelectedItem = RunningTotalListBox.SelectedIndex;
            if (SelectedItem != -1)
            {
                RunningTotalListBox.Items.RemoveAt(SelectedItem);
                Total = Total - SubTot;
                TotalLabel.Text = Total.ToString();
            }
        }

        // clear button to clear the form after transaction of the order placed or in mid-way of order.
        private void ClearButton_Click(object sender, EventArgs e)
        {
            QuantityTextBox.Text = " ";
            PriceLabel.Text = " ";
            TotalLabel.Text = " ";
            Total = 0;
            BagelListBox.ClearSelected();
            BagelSizeListBox.ClearSelected();
            RunningTotalListBox.Items.Clear();
        }
        //Exit button to exit the application from the form
        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Bagel Size listbox to fetch price of selected index and display the value of price in the price label.
        private void BagelSizeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //try catch to handle exception
            try
            {
                // if loop to check index 
                if (BagelListBox.SelectedIndex != -1)
                {
                    if (BagelSizeListBox.SelectedIndex != -1)
                    {
                        BagelNameIndex = BagelListBox.SelectedIndex;
                        BagelSizeIndex = BagelSizeListBox.SelectedIndex;
                        Price = BagelPrices[BagelNameIndex, BagelSizeIndex];
                        PriceLabel.Text = BagelPrices[BagelNameIndex, BagelSizeIndex].ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Sales report button to generate sales report and show sales per item amount & value of each bagel type/size sold.
        private void SalesReportButton_Click(object sender, EventArgs e)
        {
            //if loop to check number of transaction
            if (TotalTranxn >= 0)
            {
                SalesReport SR = new SalesReport();
                ReadFromFile("StartingStock.txt", ref Starting);

                //Load report is a method which fetches the sales report. 
                SR.Loadreport(BagelNames, BagelSize, Starting, BagelStocks, BagelPrices);
                SR.ShowDialog();  
            }
            else
            {
                MessageBox.Show("No transaction to be displayed"," no transaction",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }

        }
        //Stockavailability button to check stocks of the bagel type and size.
        private void StocksAvailableButton_Click(object sender, EventArgs e)
        {
            ReadFromFile("BagelStock.txt", ref BagelStocks);
            
            //try catch to handle exceptions
            try
            {
                StreamWriter Output = File.CreateText(@"StockReport.txt");
                
                //formating/ allingning the report.

                Output.WriteLine("\t\t\tReport Generated on:" + DateTime.Now);
                Output.WriteLine();
                Output.WriteLine("\t\t\t\tAvailable Quantity\t\t\t");
                Output.WriteLine();
                Output.WriteLine("\t\t\t\t\tS\t\t\t\tM\t\t\t\tR\t\t\t\tL\t\t\t\tXL");

                // for loop to iterate the values of stock
                for (int i = 0; i < BagelNames.Length; i++)
                {
                    Output.Write(BagelNames[i] + "\t");
                    for (int j = 0; j < BagelSize.Length; j++)
                    {
                        Output.Write("\t\t" + BagelStocks[i, j] + "\t\t");
                    }
                    Output.Write("\n");
                }
                Output.Close();
                MessageBox.Show("Stock Report Generated,\n\n Check your Debug File named: 'StockReport.txt' ", "Stock Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        
    }
}
