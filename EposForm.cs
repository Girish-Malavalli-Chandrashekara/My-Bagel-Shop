using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace MyBagelShop
{
    public partial class MyBagelShop : Form
    {
        public static string[] BagelNames = { "Plain Bagel", "Cinnamon Raisin Bagel", "Blueberry Bagel", "Chocolate Chip Bagel", "Onion Bagel", "Asiago Bagel", "Chicken Cajun Bagel", "Poppy Seed Bagel", "French Toast Bagel", "Egg Bagel", "Whole Wheat Bagel", "Garlic Sesame Bagel", "Exotic Chilli Bagel" };
        public static string[] BagelSize = { "Small", "Medium", "Regular", "Large", "Extra Large" };
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

        public static int[,] BagelStocks = new int[13, 5];
        int Quantity, RemainingQuantity;
        decimal Total = 0, Price, PriceOf1Bagel;
        int BagelNameIndex = 0, BagelSizeIndex = 0;
        string SEPARATOR = ",", BagelsName = "", BagelsSize = "";
        public static int[,] BagelStocksTemp = new int[13, 5];
        int FIRST_LINE_ITEM_POS = 3, FirstLine=0;
        int TotalBagelsSld, TotalSalesAmt, TotalTranxn, SoldQuantity;
        public MyBagelShop()
        {
            InitializeComponent();
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            SearchForm S2 = new SearchForm();
            S2.Show(this);
        }

        private void SummaryButton_Click(object sender, EventArgs e)
        {
            SummaryForm S3 = new SummaryForm();
            S3.Show(this);
        }

        private bool ReadFromFile(String filename, ref int[,] records)
        {
            int row = 0;
            string[] recordRow, allRecordRows;
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

        private bool WriteToFile(String fileName, int[,] records)
        {
            int row = 0;
            int[] recordRow = new int[records.GetLength(1)];
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

        private void AddToOrderButton_Click(object sender, EventArgs e)
        {
            if (BagelNameIndex == -1 || BagelSizeIndex == -1)
            {
                MessageBox.Show("Please Select Bagel and Size", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    ReadFromFile("BagelStocksTemp.txt", ref BagelStocksTemp);
                    //readFromFile("BagelStocks.txt", ref BagelStocks);
                    //String input = File.ReadAllText(@"BagelStocks.txt");
                    Quantity = int.Parse(QuantityTextBox.Text);
                    BagelNameIndex = BagelListBox.SelectedIndex;
                    BagelSizeIndex = BagelSizeListBox.SelectedIndex;

                    /*int i = 0, j = 0;
                    //int[,] BagelStocks = new int[13, 5];
                    foreach (var row in input.Split('\n'))
                    {
                        j = 0;
                        foreach (var col in row.Trim().Split(','))
                        {
                            BagelStocks[i, j] = int.Parse(col.Trim());
                            j++;
                        }
                        i++;
                    }*/
                    var AvailableQuantity = BagelStocksTemp[BagelNameIndex, BagelSizeIndex];
                    //var AvailableQuantity = BagelStocks[BagelNameIndex, BagelSizeIndex];
                    int RequestQuantity = int.Parse(QuantityTextBox.Text);
                    //String BagelsName = "", BagelsSize = "";
                    BagelsName = BagelListBox.SelectedItem.ToString();
                    BagelsSize = BagelSizeListBox.SelectedItem.ToString();
                    PriceOf1Bagel = BagelPrices[BagelNameIndex, BagelSizeIndex];
                    if (RequestQuantity > AvailableQuantity)
                    {
                        MessageBox.Show("Your requested quantity is not available, We only have  " + AvailableQuantity + " Numbers");
                        QuantityTextBox.Text = Convert.ToString(AvailableQuantity);
                        
                    }
                    else
                    {
                        RunningTotalListBox.Items.Add(BagelsName + ",   " + BagelsSize + ",   " + Quantity + ",   "+ PriceOf1Bagel);
                        Total = (Price * Quantity) + Total;
                        RemainingQuantity = AvailableQuantity - RequestQuantity;
                        BagelStocksTemp[BagelListBox.SelectedIndex, BagelSizeListBox.SelectedIndex] = RemainingQuantity;
                        WriteToFile("BagelStocksTemp.txt", BagelStocksTemp);
                        //writeToFile("BagelStocks.txt", BagelStocks);
                        SoldQuantity += RequestQuantity;
                    }
                    TotalLabel.Text = Total.ToString();
                }
                catch (Exception)
                {
                    MessageBox.Show("Enter numerical values to text","Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ProcessOrderGroupBox_Enter(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void RemoveItemButton_Click(object sender, EventArgs e)
        {
            int SelectedItem = RunningTotalListBox.SelectedIndex;
            if (SelectedItem != -1)
            {
                RunningTotalListBox.Items.RemoveAt(SelectedItem);
                Total = Total - BagelPrices[BagelNameIndex, BagelSizeIndex];
                TotalLabel.Text = Total.ToString();
            }
        }

        private void CompleteOrderButton_Click(object sender, EventArgs e)
        {
            if (RunningTotalListBox.Items.Count > 0)
            {
                Random Number = new Random();
                int UID = Number.Next(100000, 9999999);
                DateTime Today = DateTime.Now;

                string[] Orders = new string[FIRST_LINE_ITEM_POS + RunningTotalListBox.Items.Count];
                string[] Lines = new string[RunningTotalListBox.Items.Count];
                string Msg = "Order Confirmation Details: \n Transaction ID: " + UID + "\n Date & Time:" + Today + "";
                Orders[FirstLine] = UID.ToString();
                Orders[FirstLine + 1] = Today.ToString();
                Orders[FirstLine + 2] = Total.ToString();
                for (int i = 0; i < RunningTotalListBox.Items.Count; i++)
                {
                    Lines[i] = RunningTotalListBox.Items[i].ToString();
                    Msg += "\n" + Lines[i];
                    Orders[FIRST_LINE_ITEM_POS + i] = RunningTotalListBox.Items[i].ToString();
                }
                Msg += "\nTotal " + Total;
                MessageBox.Show(Msg, "Orders Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Order Not Places, No Orders in the selected","No Order",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }

            ReadFromFile("BagelStocksTemp.txt", ref BagelStocks);
            WriteToFile("BagelStocks.txt", BagelStocks);

            ReadFromFile("BagelStocks.txt", ref BagelStocksTemp);
            WriteToFile("BagelStocksTemp.txt", BagelStocksTemp);

            TotalBagelsSld += SoldQuantity;
            TotalSalesAmt += (int)Total;
            TotalTranxn++;
            ClearButton_Click(sender, e);

            
            
        }

        private void QuantityTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            QuantityTextBox.Text = " ";
            PriceLabel.Text = " ";
            TotalLabel.Text = " ";
            BagelListBox.ClearSelected();
            BagelSizeListBox.ClearSelected();
            RunningTotalListBox.Items.Clear();
        }

        private void BagelSizeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
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

        private void BagelListBox_SelectedIndexChanged(object sender, EventArgs e)
        {


        }
    }
}
