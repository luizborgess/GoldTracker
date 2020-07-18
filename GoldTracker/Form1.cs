using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
namespace GoldTracker
{
    public partial class Form1 : Form
    {
        long g_tpsell, g_tpbuy, g_marketsell, g_marketbuy;

        private  Stopwatch stopwatch;
        public Form1()
        {
            InitializeComponent();
        }        
        private void Form1_Load(object sender, EventArgs e)
        {
            stopwatch = new Stopwatch();
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(textBox1.Text))
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://api.guildwars2.com/v2/");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", textBox1.Text);
                HttpResponseMessage response = client.GetAsync("characters").Result;

                string res = await response.Content.ReadAsStringAsync();


                //CHARACTERS PARSED
                List<string> stringList = res.Split(',').ToList();
                List<string> stringList2 = new List<string>();
                foreach (string i in stringList)
                {
                    stringList2.Add(i.Replace("\"", "").Replace("[", "").Replace("]", "").TrimStart().TrimEnd());
                }
                comboBox1.DataSource = stringList2;
                button2.Enabled = true;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.guildwars2.com/v2/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", textBox1.Text);

            HttpResponseMessage inventory = client.GetAsync(Uri.EscapeUriString("characters/" + comboBox1.Text + "/inventory")).Result;
            string inventoryResponse = await inventory.Content.ReadAsStringAsync();

            var items = Items.FromJson(inventoryResponse);

            //INVENTORY PARSED
            List<long> tp = new List<long>();
            List<long> vendor = new List<long>();
            List<long> count_tp = new List<long>();
            List<long> count_vendor = new List<long>();

            //PERCORRE INVENTARIO
            foreach (Bag bag1 in items.Bags)
            {
                foreach (Inventory inv in bag1.Inventory)
                {
                    if (inv != null)
                    {
                        //trading post items
                        if (inv.Binding == null && inv.Id != null)
                        {
                            tp.Add(inv.Id);
                            count_tp.Add(inv.Count);
                        }
                        //vendor items
                        if (inv.Binding == "Account" && inv.Id != null)
                        {
                            vendor.Add(inv.Id);
                            count_vendor.Add(inv.Count);
                        }

                    }
                }
            }

            //Vault PARSED
            List<long> vault_items = new List<long>();
            List<long> vault_count = new List<long>();
            HttpResponseMessage vault_response = client.GetAsync("account/materials").Result;
            string vault_value = await vault_response.Content.ReadAsStringAsync();
            var vault = Vault.FromJson(vault_value);

            foreach (Vault i in vault)
            {
                if (i.Binding == null && i.Count != 0)
                {
                    vault_items.Add(i.Id);
                    vault_count.Add(i.Count);
                    //Console.WriteLine(i.Id);
                }
            }

            using (WebClient web = new WebClient())
            {
                web.DownloadFile(new Uri("http://api.gw2tp.com/1/bulk/items.csv"), Environment.CurrentDirectory + "\\items.csv");
            }
            //MARKET PARSED
            List<long> market_id = new List<long>();
            List<long> market_buy = new List<long>();
            List<long> market_sell = new List<long>();
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

            //MARKET CALC
            long market_value_sell = 0;
            long market_value_buy = 0;
            int j = 0;
            foreach (long item in vault_items)
            {
                int i = 0;
                foreach (long id in market_id)
                {
                    if (item == id)
                    {
                        market_value_sell += market_sell[i] * vault_count[j];
                        market_value_buy += market_buy[i] * vault_count[j];
                    }
                    i++;
                }
                j++;
            }

            //INVENTORY CALC
            j = 0;
            long tp_sell = 0;
            long tp_buy = 0;
            foreach (long item in tp)
            {
                int i = 0;
                foreach (long id in market_id)
                {
                    if (item == id)
                    {
                        tp_sell += market_sell[i] * count_tp[j];
                        tp_buy += market_buy[i] * count_tp[j];
                    }
                    i++;
                }
                j++;
            }


            //display value
            int[] value = Monetary(market_value_sell);
            VaultValue.Text = value[0].ToString() + "g" + value[1].ToString() + "s" + value[2].ToString() + "c";
            VaultValue.Visible = true;
            value = Monetary(tp_sell);
            InventoryValue.Text= value[0].ToString() + "g" + value[1].ToString() + "s" + value[2].ToString() + "c";
            InventoryValue.Visible = true;
            g_tpsell = tp_sell;
            g_tpbuy = tp_buy;
            g_marketbuy = market_value_buy;
            g_marketsell = market_value_sell;

            button6.Enabled = true;
        }
        public int[] Monetary(long money)
        {
            int[] value = new int[3];
            value[0] = (int)(money / 10000);   //gold
            value[1]=(int)(money % 10000) / 100;   //silver
            value[2]= (int)(money % 100);  //copper
            return value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            stopwatch.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            stopwatch.Stop();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            stopwatch.Reset();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            this.label7.Text = string.Format("{0:hh\\:mm\\:ss\\:fff}", stopwatch.Elapsed);
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.guildwars2.com/v2/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", textBox1.Text);

            HttpResponseMessage inventory = client.GetAsync(Uri.EscapeUriString("characters/" + comboBox1.Text + "/inventory")).Result;
            string inventoryResponse = await inventory.Content.ReadAsStringAsync();

            var items = Items.FromJson(inventoryResponse);

            //INVENTORY PARSED
            List<long> tp = new List<long>();
            List<long> vendor = new List<long>();
            List<long> count_tp = new List<long>();
            List<long> count_vendor = new List<long>();

            //PERCORRE INVENTARIO
            foreach (Bag bag1 in items.Bags)
            {
                foreach (Inventory inv in bag1.Inventory)
                {
                    if (inv != null)
                    {
                        //trading post items
                        if (inv.Binding == null && inv.Id != null)
                        {
                            tp.Add(inv.Id);
                            count_tp.Add(inv.Count);
                        }
                        //vendor items
                        if (inv.Binding == "Account" && inv.Id != null)
                        {
                            vendor.Add(inv.Id);
                            count_vendor.Add(inv.Count);
                        }

                    }
                }
            }

            //Vault PARSED
            List<long> vault_items = new List<long>();
            List<long> vault_count = new List<long>();
            HttpResponseMessage vault_response = client.GetAsync("account/materials").Result;
            string vault_value = await vault_response.Content.ReadAsStringAsync();
            var vault = Vault.FromJson(vault_value);

            foreach (Vault i in vault)
            {
                if (i.Binding == null && i.Count != 0)
                {
                    vault_items.Add(i.Id);
                    vault_count.Add(i.Count);
                    //Console.WriteLine(i.Id);
                }
            }

            using (WebClient web = new WebClient())
            {
                web.DownloadFile(new Uri("http://api.gw2tp.com/1/bulk/items.csv"), Environment.CurrentDirectory + "\\items.csv");
            }
            //MARKET PARSED
            List<long> market_id = new List<long>();
            List<long> market_buy = new List<long>();
            List<long> market_sell = new List<long>();
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

            //MARKET CALC
            long market_value_sell = 0;
            long market_value_buy = 0;
            int j = 0;
            foreach (long item in vault_items)
            {
                int i = 0;
                foreach (long id in market_id)
                {
                    if (item == id)
                    {
                        market_value_sell += market_sell[i] * vault_count[j];
                        market_value_buy += market_buy[i] * vault_count[j];
                    }
                    i++;
                }
                j++;
            }

            //INVENTORY CALC
            j = 0;
            long tp_sell = 0;
            long tp_buy = 0;
            foreach (long item in tp)
            {
                int i = 0;
                foreach (long id in market_id)
                {
                    if (item == id)
                    {
                        tp_sell += market_sell[i] * count_tp[j];
                        tp_buy += market_buy[i] * count_tp[j];
                    }
                    i++;
                }
                j++;
            }

            int[] value = Monetary(market_value_sell+tp_sell-g_tpsell-g_marketsell);
            label9.Text = value[0].ToString() + "g" + value[1].ToString() + "s" + value[2].ToString() + "c";
            label9.Visible = true;

            value = Monetary(market_value_buy+tp_buy-g_tpbuy-g_marketbuy);
            label10.Text = value[0].ToString() + "g" + value[1].ToString() + "s" + value[2].ToString() + "c";
            label10.Visible = true;

            label11.Text = ((market_value_sell + tp_sell - g_tpsell - g_marketsell) / (stopwatch.Elapsed.TotalMinutes / 60)).ToString();
            label11.Visible = true;

            //corrigir profit, zerando, estabelecer track como padrao so resetar no track novamente, deixar o botao profit somente para atualização do lucro.
        }
    }
}
