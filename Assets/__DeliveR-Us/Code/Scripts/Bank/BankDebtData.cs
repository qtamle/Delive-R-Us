using UnityEngine;

[CreateAssetMenu(fileName = "BankDebtData", menuName = "Bank/Bank Debt Data")]
public class BankDebtData : ScriptableObject
{
    public float currentDebt;
    public int borrowDay;

    public void AddDebt(float amount, int currentDay)
    {
        currentDebt += amount;
        borrowDay = currentDay;
        SaveDebt();
    }

    public void RepayDebt(float amount)
    {
        currentDebt = Mathf.Max(currentDebt - amount, 0);
        SaveDebt();
    }

    public void SaveDebt()
    {
        PlayerPrefs.SetFloat("BankDebt_CurrentDebt", currentDebt);
        PlayerPrefs.Save();
    }

    public void LoadDebt()
    {
        if (PlayerPrefs.HasKey("BankDebt_CurrentDebt"))
            currentDebt = PlayerPrefs.GetFloat("BankDebt_CurrentDebt");
        else
            currentDebt = 0;
    }

    public void ResetDebt()
    {
        currentDebt = 0;
        SaveDebt();
    }

    //[ContextMenu("Add Debt")]
    //void AddDebtTest()
    //{
    //    AddDebt(50f);
    //}

    [ContextMenu("Reset Debt")]
    void ResetDebtTest()
    {
        ResetDebt();
        borrowDay = 0;
    }
}
