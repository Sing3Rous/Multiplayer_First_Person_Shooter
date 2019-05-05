using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public Text killCount;
    public Text deathCount;

    void Start()
    {
        UserAccountManager.instance.GetData(OnReceivedData);
    }

    void OnReceivedData(string data)
    {
        if (killCount == null || deathCount == null)
        {
            return;
        }
        
        killCount.text = DataTranslator.DataToKills(data).ToString() + " kills.";
        deathCount.text = DataTranslator.DataToDeaths(data).ToString() + " deaths.";
    }
}
