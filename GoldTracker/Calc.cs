using System.Collections.Generic;

namespace GoldTracker
{
    internal class Calc
    {
        public long vault_sell { get; set; }
        public long vault_buy { get; set; }
        public long inv_sell { get; set; }
        public long inv_buy { get; set; }

        public Calc Vault(SearchInventory search, List<long> market_id, List<long> market_sell, List<long> market_buy)
        {
            long market_value_sell = 0;
            long market_value_buy = 0;
            int j = 0;
            foreach (long item in search.Vault().vault_items)
            {
                int i = 0;
                foreach (long id in market_id)
                {
                    if (item == id)
                    {
                        market_value_sell += market_sell[i] * search.Vault().vault_count[j];
                        market_value_buy += market_buy[i] * search.Vault().vault_count[j];
                    }
                    i++;
                }
                j++;
            }
            return new Calc { vault_buy = market_value_buy, vault_sell = market_value_sell };
        }

        public Calc Inventory(SearchInventory search, List<long> market_id, List<long> market_sell, List<long> market_buy)
        {
            int j = 0;
            long tp_sell = 0;
            long tp_buy = 0;
            foreach (long item in search.Inventory().tp)
            {
                int i = 0;
                foreach (long id in market_id)
                {
                    if (item == id)
                    {
                        tp_sell += market_sell[i] * search.Inventory().count_tp[j];
                        tp_buy += market_buy[i] * search.Inventory().count_tp[j];
                    }
                    i++;
                }
                j++;
            }
            return new Calc { inv_sell = tp_sell, inv_buy = tp_buy };
        }
    }
}