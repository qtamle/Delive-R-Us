using System.Collections.Generic;

public static class GameStrings
{


    public static string OrderNotificationMsg(string customerName, List<OrderItemInfoData> orderItems)
    {
        int totalItems = orderItems.Count;
        string itemSummary;

        if (totalItems == 1)
        {
            var item = orderItems[0];
            itemSummary = $"{item.Item.itemName} x{item.ItemCount}";
        }
        else if (totalItems == 2)
        {
            itemSummary = $"{orderItems[0].Item.itemName} x{orderItems[0].ItemCount}, " +
                          $"{orderItems[1].Item.itemName} x{orderItems[1].ItemCount}";
        }
        else
        {
            itemSummary = $"{orderItems[0].Item.itemName} x{orderItems[0].ItemCount}, " +
                          $"{orderItems[1].Item.itemName} x{orderItems[1].ItemCount} <color=#BBBBBB>+{totalItems - 2}</color>";
        }

        return
            $"<size=100%><b><color=#FEC601>{customerName}</color> <color=#CCCCCC>needs delivery</color></b></size>\n" +
            $"<color=#FFFFFF>Items: {itemSummary}</color>\n" +
            $"<size=85%><color=#AAAAAA>Press <color=#F8D206>P</color> to review & respond.</color></size>";
    }
    public static string GetAlreadyInOrderMsg()
    {
        return "<size=100%><b><color=#FEC601>You're already working on an order!</color></b></size>\n" +
          "<color=#FFFFFF>Complete or cancel it before taking a new one.</color>";
    }

    public static string GetNoInternetInMarketMsg()
    {
        return "<size=100%><b><color=#FEC601>Internet access is limited inside the market.</color></b></size>\n" +
               "<color=#FFFFFF>Please step outside to accept new orders.</color>";
    }
    public static string GetOrderDeclinedMsg()
    {
        return "<size=100%><b><color=#FF6A00>You declined the order.</color></b></size>\n" +
           "<color=#AAAAAA>Waiting for the next one...</color>";
    }
    public static string GetOrderStartedMsg(string customerName, int itemCount) =>
        $"<size=100%><b><color=#00D1FF>Order accepted</color> <color=#CCCCCC>for</color> <color=#FEC601>{customerName}</color>!</b></size>\n" +
        $"<color=#FFFFFF>You need to collect <color=#F8D206>{itemCount}</color> {(itemCount == 1 ? "item" : "items")} and deliver them on time.</color>";
    public static string GetOrderFailedMsg(string customerName) =>
        $"<size=100%><b><color=#FF3B3B>Order failed</color> <color=#CCCCCC>for</color> <color=#FEC601>{customerName}</color></b></size>\n" +
        "<color=#FFFFFF>Delivery was late or incorrect.</color>";
    public static string GetOrderSuccessMsg(string customerName, float bill, float tip)
    {
        float total = bill + tip;

        return tip > 0
            ? $"<b><color=#3A772A>Delivered to</color> <color=#FEC601>{customerName}</color></b>\n" +
              $"<color=#FFFFFF>Delivery: ${bill:F2} | Tip: ${tip:F2}</color>\n" +
              $"<color=#FEC601>Total: ${total:F2}</color>\n" +
              "<color=#AAAAAA>Added to wallet (day end)</color>"
            : $"<b><color=#3A772A>Delivered to</color> <color=#FEC601>{customerName}</color></b>\n" +
              $"<color=#FFFFFF>Delivery: ${bill:F2}</color>\n" +
              $"<color=#FEC601>Total: ${bill:F2}</color>\n" +
              "<color=#AAAAAA>Added to wallet (day end)</color>";
    }
    public static string GetNoEarningsMsg(string playerName = null)
    {
        string nameLine = string.IsNullOrEmpty(playerName) ? "" : $" <color=#CCCCCC>for</color> <color=#FEC601>{playerName}</color>";
        return $"<size=100%><b><color=#3A772A>No earnings</color>{nameLine}</b></size>\n<color=#FFFFFF>You didn’t earn anything yesterday — keep delivering!</color>";
    }
    public static string GetMidnightEarningsMsg(float amount, string playerName = null)
    {
        string nameLine = string.IsNullOrEmpty(playerName) ? "" : $" <color=#CCCCCC>for</color> <color=#FEC601>{playerName}</color>";
        return $"<size=100%><b><color=#3A772A>Daily earnings</color>{nameLine}</b></size>\n<color=#FFFFFF><b><color=#FEC601>${amount}</color></b> has been transferred to your balance.</color>";
    }
    public static string GetAwayEarningsMsg(float amount, string playerName = null)
    {
        string nameLine = string.IsNullOrEmpty(playerName) ? "" : $" <color=#CCCCCC>for</color> <color=#FEC601>{playerName}</color>";
        return $"<size=100%><b><color=#3A772A>While you were away</color>{nameLine}</b></size>\n<color=#FFFFFF>Your <b><color=#FEC601>${amount}</color></b> earnings were auto-transferred.\n<b>Play daily to earn more!</b></color>";
    }

    public static (string header, string description) GetInvalidNameAlert()
    {
        string header = "<color=#AA2E2E><b>Invalid Name</b></color>";
        string description = "<color=#000000>Name cannot be empty and must be at least 3 characters long.</color>";

        return (header, description);
    }

    public static (string header, string description) GetNoActiveOrderAlert()
    {
        string header = "<color=#AA2E2E><b>No Active Order</b></color>";
        string description = "<color=#000000>You need to start an order before\nadding items to the cart.</color>";

        return (header, description);
    }

    public static (string header, string description) GetOrderFailedAlert()
    {
        string header = "<color=#AA2E2E><b>Order Failed</b></color>";
        string description = "<color=#000000>You didn’t complete the order in time or the delivery was incorrect.</color>";

        return (header, description);
    }

    public static (string header, string description) GetOrderSuccessAlert()
    {
        string header = "<color=#3A772A><b>Order Delivered</b></color>";
        string description = "<color=#000000>You’ve successfully completed the delivery.</color>";

        return (header, description);
    }

    public static (string header, string description) GetWrongTargetItemAlert(string targetItem)
    {
        string header = "<color=#AA2E2E><b>Wrong Item</b></color>";
        string description = $"<color=#000000>This is not your target item. Please collect <b>{targetItem}</b> instead.</color>";
        return (header, description);
    }


    public static (string header, string description) GetItemNotInOrderAlert()
    {
        string header = "<color=#AA2E2E><b>Not on the List</b></color>";
        string description = "<color=#000000>Looks like this item isn’t part of the job.</color>";
        return (header, description);
    }

    public static (string header, string description) GetOrderItemAlreadyCollectedAlert()
    {
        string header = "<color=#AA2E2E><b>Item Done</b></color>";
        string description = "<color=#000000>No more of this item is needed in the current order.</color>";
        return (header, description);
    }

    public static (string header, string description) GetCheckoutSuccessAlert()
    {
        string header = "<color=#3A772A><b>Shopping Complete</b></color>";
        string description = "<color=#000000>Begin delivery to finish the order.</color>";
        return (header, description);
    }

    public static (string header, string description) GetNoEarningsAlert()
    {
        string header = "<color=#C1EFB5><b>No Earnings</b></color>";
        string description = "<color=#000000>You didn’t earn anything yesterday — <b>keep delivering!</b></color>";

        return (header, description);
    }

    public static (string header, string description) GetMidnightEarningsAlert(float amount)
    {
        string header = "<color=#C1EFB5><b>Daily Earnings</b></color>";
        string description = $"<color=#000000><b><color=#745A00>${amount}</color></b> has been transferred to your balance.</color>";

        return (header, description);
    }
    
    public static (string header, string description) GetAwayEarningsAlert(float amount)
    {
        string header = "<color=#C1EFB5><b>While You Were Away</b></color>";
        string description = $"<color=#000000>Your <b><color=#745A00>${amount}</color></b> earnings were auto-transferred.<br><b>Play daily to earn more!</b></color>";

        return (header, description);
    }

    public static (string header, string description) GetPurchaseSuccessAlert(string itemName, float price)
    {
        string header = "<color=#C1EFB5><b>Purchase Successful</b></color>";
        string description = $"<color=#000000><b>{itemName}</b> has been added to your apartment for <b>${price}</b>.</color>";
        return (header, description);
    }

    public static (string header, string description) GetPurchaseFailedAlert(string itemName, float price)
    {
        string header = "<color=#B22222><b>Purchase Failed</b></color>";
        string description = $"<color=#000000>You need <b>${price}</b> to buy <b>{itemName}</b>, but you don't have enough funds.</color>";
        return (header, description);
    }

    public static (string header, string description) GetItemSectionHintAlert(string itemName)
    {
        string section = GetSectionName(itemName);

        string header = "<color=#2E8B57><b>Hint</b></color>";
        string description = $"<color=#000000>Find <b>{itemName}</b> in <b>{section}</b> section. Press <b>Esc</b> to close.</color>";

        return (header, description);
    }


    public static (string header, string description) GetFirstItemReminderAlert()
    {
        string header = "<color=#2E8B57><b>Reminder</b></color>";
        string description = "<color=#000000>Press <b>V</b> to view your list. Collect the <b>first item</b> to continue. Press <b>Esc</b> to close.</color>";

        return (header, description);
    }


    public static (string header, string description) GetTutorialCompleteAlert()
    {
        string header = "<color=#2E8B57><b>Congratulations!</b></color>";
        string description = "<color=#000000>You’ve completed the tutorial. Time is running, so hurry up and finish the task!</color>";

        return (header, description);
    }


    private static string GetSectionName(string itemName)
    {
        itemName = itemName.ToLower();

        if (itemName.Contains("chocolate")) return "Chocolates";
        if (itemName.Contains("cookie")) return "Cookies";
        if (itemName.Contains("gum")) return "Gums";
        if (itemName.Contains("nut")) return "Nuts";
        if (itemName.Contains("soup")) return "Canned Goods";
        if (itemName.Contains("soya")) return "Dairy Alternatives";
        if (itemName.Contains("tooth") || itemName.Contains("wash") || itemName.Contains("conditioner"))
            return "Personal Care";
        if (itemName.Contains("clean") || itemName.Contains("fabric")) return "Cleaning Supplies";

        return "General Items";
    }
}
