using System.Collections.Generic;

namespace GoldTracker
{
    internal class SearchInventory
    {
        public List<long> tp { get; set; }
        public List<long> vendor { get; set; }
        public List<long> count_tp { get; set; }
        public List<long> count_vendor { get; set; }
        public List<long> vault_items { get; set; }
        public List<long> vault_count { get; set; }
        public List<long> junk { get; set; }
        public List<long> junk_count { get; set; }

        public Items items;
        public Vault[] vault;
        public Vendor[] vendor_value;

        public SearchInventory Inventory()
        {
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
                        if (inv.Binding == "Character" && inv.Id != null)
                        {
                            vendor.Add(inv.Id);
                            count_vendor.Add(inv.Count);
                        }
                    }
                }
            }
            return new SearchInventory { tp = tp, vendor = vendor, count_tp = count_tp, count_vendor = count_vendor };
        }

        public SearchInventory Vault()
        {
            List<long> vault_items = new List<long>();
            List<long> vault_count = new List<long>();

            foreach (Vault i in vault)
            {
                if (i.Binding == null && i.Count != 0)
                {
                    vault_items.Add(i.Id);
                    vault_count.Add(i.Count);
                }
            }
            return new SearchInventory { vault_items = vault_items, vault_count = vault_count };
        }

        public SearchInventory Vendor()
        {
            List<long> junk = new List<long>();
            List<long> junk_count = new List<long>();
            foreach (Vendor item in vendor_value)
            {
                if (item.Rarity == "Junk")
                {
                    junk.Add(item.VendorValue);
                    junk_count.Add(item.Id);
                }
            }
            return new SearchInventory { junk = junk, junk_count = junk_count };
        }
    }
}