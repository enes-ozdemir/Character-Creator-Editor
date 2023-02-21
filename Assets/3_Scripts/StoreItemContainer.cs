using System.Collections.Generic;
using UnityEngine;

namespace _3_Scripts
{
    [CreateAssetMenu(fileName = "StoreItemContainer", menuName = "StoreItemContainer")]
    public class StoreItemContainer : ScriptableObject
    {
        public List<StoreItem> storeItemList;
    }
}