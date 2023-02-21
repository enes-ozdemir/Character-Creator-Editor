using HomaGames.Internal.Utilities;
using System;
using System.Collections.Generic;
using _3_Scripts;

public class Store : Singleton<Store>
{
    public StoreItemContainer StoreItems;
    public Action<StoreItem> OnItemSelected;

    public void SelectItem(StoreItem item)
    {
        OnItemSelected?.Invoke(item);
    }
}
