using System.Collections.Generic;
using UnityEngine;

public static class OrderUtils
{
    private static readonly string[] streetNames = new string[]
    {
        "Maple St", "Oak Ave", "Pine Dr", "Cedar Ln", "Elm St", "Main St", "Sunset Blvd", "Hillcrest Rd", "River Rd", "Forest Ave",
        "Washington St", "Lakeview Dr", "Cherry St", "Broadway", "Highland Ave", "Meadow Ln", "Valley Rd", "Birch St", "Sycamore Dr", "Lincoln Blvd"
    };

    private static readonly string[] stateCodes = new string[]
    {
        "NY" // Only using New York
    };

    private static readonly string[] firstNames = new string[]
    {
        "Liam", "Noah", "Oliver", "Elijah", "James", "William", "Benjamin", "Lucas", "Henry", "Alexander",
        "Mason", "Michael", "Ethan", "Daniel", "Jacob", "Logan", "Jackson", "Levi", "Sebastian", "Mateo",
        "Jack", "Owen", "Theodore", "Aiden", "Samuel", "Joseph", "John", "David", "Wyatt", "Matthew",
        "Luke", "Asher", "Carter", "Julian", "Grayson", "Leo", "Jayden", "Gabriel", "Isaac", "Lincoln",
        "Anthony", "Hudson", "Dylan", "Ezra", "Thomas", "Charles", "Christopher", "Jaxon", "Maverick", "Josiah",
        "Isaiah", "Andrew", "Elias", "Joshua", "Nathan", "Caleb", "Ryan", "Adrian", "Miles", "Eli",
        "Nolan", "Christian", "Aaron", "Cameron", "Ezekiel", "Colton", "Luca", "Landon", "Hunter", "Jonathan",
        "Santiago", "Axel", "Easton", "Cooper", "Jeremiah", "Angel", "Roman", "Connor", "Jameson", "Robert",
        "Greyson", "Jordan", "Ian", "Carson", "Jaxson", "Leonardo", "Nicholas", "Dominic", "Austin", "Everett",
        "Brooks", "Xavier", "Kai", "Jose", "Parker", "Adam", "Jace", "Wesley", "Kayden", "Silas"
    };

    private static readonly string[] lastNames = new string[]
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
        "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
        "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
        "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
        "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts",
        "Gomez", "Phillips", "Evans", "Turner", "Diaz", "Parker", "Cruz", "Edwards", "Collins", "Reyes",
        "Stewart", "Morris", "Morales", "Murphy", "Cook", "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper",
        "Peterson", "Bailey", "Reed", "Kelly", "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson",
        "Watson", "Brooks", "Chavez", "Wood", "James", "Bennett", "Gray", "Mendoza", "Ruiz", "Hughes",
        "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers", "Long", "Ross", "Foster", "Jimenez"
    };

    private const float _defaultShoppingTimer = 240f;
    private const float _defaultDeliveryTimer = 240f;
    private const float _timeBufferOnEachItem = 5f;

    /// <summary>
    /// Returns a random address in New York.
    /// Format: [Number] [Street], New York, NY [Zip]
    /// </summary>
    public static string GetRandomAddress()
    {
        int number = Random.Range(100, 9999);
        string street = streetNames[Random.Range(0, streetNames.Length)];
        string state = "NY";
        int zip = Random.Range(10000, 14999); // NY zip codes range (rough)

        return $"{number} {street}, New York, {state} {zip}";
    }

    /// <summary>
    /// Returns a random Order ID (8-character alphanumeric)
    /// </summary>
    public static string GetRandomOrderID()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] id = new char[8];
        for (int i = 0; i < id.Length; i++)
        {
            id[i] = chars[Random.Range(0, chars.Length)];
        }
        return new string(id);
    }

    /// <summary>
    /// Returns a random tip between $1 and $20 with 10% probability, otherwise returns $0
    /// </summary>
    public static int GetRandomTip()
    {
        if (Random.value <= 0.10f)
        {
            return Random.Range(1, 20);
        }
        return 0;
    }

    /// <summary>
    /// Returns a random Name ID (First Name + Last Name)
    /// </summary>
    public static string GetRandomName
    {
        get
        {
            string firstName = firstNames[Random.Range(0, firstNames.Length)];
            string lastName = lastNames[Random.Range(0, lastNames.Length)];
            return $"{firstName} {lastName}";
        }
    }

    /// <summary>
    /// Returns timers (Shopping & Delievery) as multiples of 5 (e.g., 125, 140), no decimal points.
    /// </summary>
    public static (int shoppingTime, int deliveryTime) GetTimers(int itemCount)
    {
        float shopping = (itemCount * Random.Range(0f, _timeBufferOnEachItem)) + _defaultShoppingTimer;
        float delivery = (itemCount * Random.Range(0f, _timeBufferOnEachItem)) + _defaultDeliveryTimer;

        // Round to nearest multiple of 5
        int shoppingFinal = Mathf.RoundToInt(shopping / 5f) * 5;
        int deliveryFinal = Mathf.RoundToInt(delivery / 5f) * 5;

        return (shoppingFinal, deliveryFinal);
    }

    /// <summary>
    /// Returns a random item count
    /// </summary>
    public static int GetRandomItemCount(int totalItemsInOrder)
    {
        int max = 11 - totalItemsInOrder;
        
        if (max <= 1)
            return 1;

        return Random.Range(1, max);
    }


    /// <summary>
    /// Distributes a total count fairly across a number of items, ensuring each gets at least 1.
    /// </summary>
    public static List<int> DistributeTotalItemCount(int total, int itemTypes)
    {
        List<int> counts = new List<int>();
        int remaining = total;

        for (int i = 0; i < itemTypes; i++)
        {
            // Leave at least 1 for each remaining item
            int maxForThis = remaining - (itemTypes - i - 1);
            int count = Random.Range(1, maxForThis + 1);
            counts.Add(count);
            remaining -= count;
        }

        return counts;
    }

}

[System.Serializable]
public class CustomerInfoData
{
    public ClientData CustomerAvatar;
    public string CustomerName;
    public string DeliveryAddress;
    public string OrderId;
    public int TipAmount;
}

[System.Serializable]
public class OrderItemInfoData
{
    public ItemData Item;
    public int ItemCount;
}

[System.Serializable]
public class OrderData
{
    #region Setters/Prvivate Variables


    [Header("Order Info")]
    [SerializeField] List<OrderItemInfoData> _currentOrder = new List<OrderItemInfoData>();

    [Space]
    [SerializeField] private OrderStatus _orderStatus = OrderStatus.Idle;
    [SerializeField] private float _targetShoppingTime = 5f;
    [SerializeField] private float _targetDeliverTime = 2f;
    [SerializeField] private float _totalOrderTime = 2f;
    [SerializeField] private string _totalOrderTime_MS;

    [Header("Customer Info")]
    [SerializeField] private CustomerInfoData _customerInfo;

    public float RemainingTime = -1f;
    #endregion

    #region Getters/Public Variables
    public OrderItemInfoData[] GetOrderInfo => _currentOrder.ToArray();
    public OrderStatus GetOrderStatus => _orderStatus;

    public CustomerInfoData GetCustomerInfo => _customerInfo;
    
    public string GetTotalOrderTime_MS => _totalOrderTime_MS;
    public float GetTotalOrderTime => _totalOrderTime;
    public float GetTargetShoppingTime => _targetShoppingTime;
    public float GetTargetDeliverTime => _targetDeliverTime;
    public int GetTotalItemsInOrder => _currentOrder.Count;

    public float GetBill
    {
        get
        {
            float price = 0;

            foreach (var orderItem in _currentOrder)
            {
                price += (orderItem.ItemCount * orderItem.Item.itemCost);
            }

            return price;
        }
    }

    #endregion

    public void RegisterOrder(OrderItemInfoData[] item, float shoppingTimer, float deliveryTimer, CustomerInfoData customerInfo)
    {
        CleanCurrentOrder();

        //_orderStatus = OrderStatus.OrderAccepted;
        _orderStatus = OrderStatus.Idle;
        _targetShoppingTime = shoppingTimer;
        _targetDeliverTime = deliveryTimer;
        _totalOrderTime = deliveryTimer + shoppingTimer;

        int minutes = Mathf.RoundToInt(_totalOrderTime / 60f);
        int seconds = Mathf.RoundToInt(_totalOrderTime % 60f);
        _totalOrderTime_MS = $"{minutes:00}:{seconds:00} MIN";

        _customerInfo = customerInfo;

        _currentOrder.AddRange(item);

        if (RemainingTime <= 0f)
            RemainingTime = Random.Range(180f, 240f);
    }
    public void CleanCurrentOrder()
    {
        _currentOrder.Clear();
        _orderStatus = OrderStatus.Idle;
        _customerInfo = new CustomerInfoData();

        _targetShoppingTime = 0;
        _targetDeliverTime = 0;
        _totalOrderTime = 0;

        _totalOrderTime_MS = string.Empty;
        RemainingTime = -1f;
    }
    public OrderData Clone()
    {
        OrderData cloned = new OrderData();

        List<OrderItemInfoData> clonedItems = new List<OrderItemInfoData>();
        foreach (var item in _currentOrder)
        {
            clonedItems.Add(new OrderItemInfoData
            {
                Item = item.Item,
                ItemCount = item.ItemCount
            });
        }
        cloned._currentOrder = clonedItems;
        cloned._orderStatus = _orderStatus;
        cloned._targetShoppingTime = _targetShoppingTime;
        cloned._targetDeliverTime = _targetDeliverTime;
        cloned._totalOrderTime = _totalOrderTime;
        cloned._totalOrderTime_MS = _totalOrderTime_MS;

        cloned._customerInfo = new CustomerInfoData
        {
            CustomerAvatar = _customerInfo.CustomerAvatar,
            CustomerName = _customerInfo.CustomerName,
            DeliveryAddress = _customerInfo.DeliveryAddress,
            OrderId = _customerInfo.OrderId,
            TipAmount = _customerInfo.TipAmount
        };
        cloned.RemainingTime = RemainingTime;
        return cloned;
    }

    public void SetOrderState(OrderStatus state)
    {
        _orderStatus = state;
    }
}