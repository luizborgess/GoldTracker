using CsvHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;

namespace GoldTracker
{
    public partial class Form1 : Form
    {
        private long g_tpsell, g_tpbuy, g_marketsell, g_marketbuy;
        private List<long> market_id = new List<long>();
        private List<long> market_buy = new List<long>();
        private List<long> market_sell = new List<long>();

        private Stopwatch stopwatch;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            stopwatch = new Stopwatch();

            //initial csv parse
            DateTime date = File.GetLastWriteTime(Environment.CurrentDirectory + "\\items.csv");
            label4.Text = "Date: " + date.ToString();

            using (var reader = new StreamReader("items.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.BadDataFound = null;
                var records = csv.GetRecords<Market>();
                foreach (var i in records)
                {
                    market_id.Add(i.id);
                    market_buy.Add(i.buy);
                    market_sell.Add(i.sell);
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)//search
        {
            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                HttpResponseMessage response = Api.ClientConfig(textBox1.Text).GetAsync("characters").Result;
                string res = await response.Content.ReadAsStringAsync();
                var character = Character.FromJson(res);

                List<string> character_list = new List<string>();
                foreach (string item in character)
                {
                    character_list.Add(item);
                }

                comboBox1.DataSource = character_list;
                button2.Enabled = true;
            }

            textBox1.Enabled = false;
        }

        private async void button2_Click(object sender, EventArgs e)//track
        {
            SearchInventory search = new SearchInventory();

            //inventory search
            HttpResponseMessage inventory = Api.ClientConfig(textBox1.Text).GetAsync(Uri.EscapeUriString("characters/" + comboBox1.Text + "/inventory")).Result;
            string inventoryResponse = await inventory.Content.ReadAsStringAsync();
            var items = Items.FromJson(inventoryResponse);
            search.items = items;

            //Vault search
            HttpResponseMessage vault_response = Api.ClientConfig(textBox1.Text).GetAsync("account/materials").Result;
            string vault_value = await vault_response.Content.ReadAsStringAsync();
            var vault = Vault.FromJson(vault_value);
            search.vault = vault;

            //String.Join(",", lst)
            //vendor search
            //var stringue = string.Join(",", search.vendor);
            if (search.vendor != null)
            {
                HttpResponseMessage vendor_response = Api.ClientConfig(textBox1.Text).GetAsync("items?ids=" + string.Join(",", search.vendor)).Result;
                string vendor_response_content = await vendor_response.Content.ReadAsStringAsync();
                var vendor = Vendor.FromJson(vendor_response_content);
                search.vendor_value = vendor;
            }

            //Calculate value
            Calc calc = new Calc();
            var vault_calc = calc.Vault(search, market_id, market_sell, market_buy);
            var inv_calc = calc.Inventory(search, market_id, market_sell, market_buy);
            if (search.vendor != null)
            {
                var junk = calc.Vendor(search);
            }
            else
            {
                calc.junk_value = 0;
            }

            //display value
            int[] value = Monetary(vault_calc.vault_sell);
            VaultValue.Text = value[0].ToString() + "g" + value[1].ToString() + "s" + value[2].ToString() + "c";
            VaultValue.Visible = true;

            value = Monetary(inv_calc.inv_sell);
            InventoryValue.Text = value[0].ToString() + "g" + value[1].ToString() + "s" + value[2].ToString() + "c";
            InventoryValue.Visible = true;

            g_tpsell = inv_calc.inv_sell;
            g_tpbuy = inv_calc.inv_buy;
            g_marketbuy = vault_calc.vault_buy;
            g_marketsell = vault_calc.vault_sell;

            button6.Enabled = true;
        }

        public int[] Monetary(long money)
        {
            int[] value = new int[3];
            value[0] = (int)(money / 10000);   //gold
            value[1] = (int)(money % 10000) / 100;   //silver
            value[2] = (int)(money % 100);  //copper
            return value;
        }

        private void button3_Click(object sender, EventArgs e)//stopwatch start
        {
            stopwatch.Start();
        }

        private void button4_Click(object sender, EventArgs e)//stopwatch stop
        {
            stopwatch.Stop();
        }

        private void button5_Click(object sender, EventArgs e)//stopwatch reset
        {
            stopwatch.Reset();
        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)//Update Database
        {
            using (WebClient web = new WebClient())
            {
                web.DownloadFile(new Uri("http://api.gw2tp.com/1/bulk/items.csv"), Environment.CurrentDirectory + "\\items.csv");
            }
            DateTime date = File.GetLastWriteTime(Environment.CurrentDirectory + "\\items.csv");
            label4.Text = "Date: " + date.ToString();

            //clear market values
            market_id.Clear();
            market_buy.Clear();
            market_sell.Clear();

            //parse csv
            using (var reader = new StreamReader("items.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.BadDataFound = null;
                var records = csv.GetRecords<Market>();
                foreach (var i in records)
                {
                    market_id.Add(i.id);
                    market_buy.Add(i.buy);
                    market_sell.Add(i.sell);
                }
            }
        }

        private async void button6_Click(object sender, EventArgs e)//profit
        {
            SearchInventory search = new SearchInventory();

            //inventory search
            HttpResponseMessage inventory = Api.ClientConfig(textBox1.Text).GetAsync(Uri.EscapeUriString("characters/" + comboBox1.Text + "/inventory")).Result;
            string inventoryResponse = await inventory.Content.ReadAsStringAsync();
            var items = Items.FromJson(inventoryResponse);
            search.items = items;

            //Vault search
            HttpResponseMessage vault_response = Api.ClientConfig(textBox1.Text).GetAsync("account/materials").Result;
            string vault_value = await vault_response.Content.ReadAsStringAsync();
            var vault = Vault.FromJson(vault_value);
            search.vault = vault;

            //Calculate value
            Calc calc = new Calc();
            var vault_calc = calc.Vault(search, market_id, market_sell, market_buy);
            var inv_calc = calc.Inventory(search, market_id, market_sell, market_buy);

            int[] value = Monetary(vault_calc.vault_sell + inv_calc.inv_sell - g_tpsell - g_marketsell);
            label9.Text = value[0].ToString() + "g" + value[1].ToString() + "s" + value[2].ToString() + "c";
            label9.Visible = true;

            value = Monetary(vault_calc.vault_buy + inv_calc.inv_buy - g_tpbuy - g_marketbuy);
            label10.Text = value[0].ToString() + "g" + value[1].ToString() + "s" + value[2].ToString() + "c";
            label10.Visible = true;

            /* CALCULO GOLD POR HORA
            double gold_perhour= (vault_calc.vault_sell + inv_calc.inv_sell - g_tpsell - g_marketsell) / (stopwatch.Elapsed.TotalMinutes / 60);
            if (!double.IsNaN(gold_perhour))
            {
                value = Monetary(Convert.ToInt64(gold_perhour));
                label11.Text = value.ToString();
                label11.Visible = true;
            }

             */
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            this.label7.Text = string.Format("{0:hh\\:mm\\:ss\\:fff}", stopwatch.Elapsed);
        }
    }
}