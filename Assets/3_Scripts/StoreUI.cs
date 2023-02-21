using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreUI : MonoBehaviour
{
    [SerializeField]
    UIStoreItem itemUIPrefab;
    void Start()
    {
        foreach (StoreItem item in Store.Instance.StoreItems.storeItemList)
        {
            UIStoreItem listItem = Instantiate(itemUIPrefab, transform);
            listItem.Initialize(item);
        }
    }
}
