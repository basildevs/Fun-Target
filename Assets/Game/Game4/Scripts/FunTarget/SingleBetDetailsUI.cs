using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
namespace FunTarget
{
    public class SingleBetDetailsUI : MonoBehaviour
    {
        public Transform listParent;
        public SingleBetDetailsItem itemPrefab;
        private List<SingleBetDetailsItem> spawnedItems;

        public void ShowData(string bet, string positions, string amount)
        {
            if (int.Parse(amount) <= 0) return;
            if (spawnedItems == null) spawnedItems = new List<SingleBetDetailsItem>();
            SingleBetDetailsItem singleItem = Instantiate(itemPrefab, listParent);
            singleItem.SetDetails(bet, positions, amount);
            spawnedItems.Add(singleItem);
        }

        public void ClearCurrentList()
        {
            if (spawnedItems == null)
                spawnedItems = new List<SingleBetDetailsItem>();
            if (spawnedItems.Count == 0) return;
            foreach (var item in spawnedItems)
            {
                Destroy(item.gameObject);
            }
            spawnedItems.Clear();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
