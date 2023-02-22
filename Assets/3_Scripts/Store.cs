using HomaGames.Internal.Utilities;
using System;
using _3_Scripts.SO;

public class Store : Singleton<Store>
{
    public StoreItemContainer StoreItems;
    public Action<StoreItem> OnItemSelected;

    public void SelectItem(StoreItem item)
    {
        OnItemSelected?.Invoke(item);
    }
}
