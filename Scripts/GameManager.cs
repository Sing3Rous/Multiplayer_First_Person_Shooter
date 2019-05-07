using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MatchSettings matchSettings;

    [SerializeField]
    private GameObject sceneCamera;

    public delegate void OnPlayerKilledCallback(string player,string source);
    public OnPlayerKilledCallback onPlayerKilledCallback;

    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("GameManager: More than one GameManager in scene.");
        } 
        else 
        {
            instance = this;
        }
    }

    public void SetSceneCameraActive(bool isActive)
    {
        if (sceneCamera == null)
        {
            return;
        }

        sceneCamera.SetActive(isActive);
    }

    #region Player tracking
    private const string PLAYER_ID_PREFIX = "Player ";
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string netID, Player player)
    {
        string playerID = PLAYER_ID_PREFIX + netID;
        players.Add(playerID, player);
        player.transform.name = playerID;
    }

    public static void UnRegisterPlayer(string playerID)
    {
        players.Remove(playerID);
    }

    public static Player GetPlayer(string playerID)
    {
        return players[playerID];
    }

    public static Player[] GetAllPlayer()
    {
        return players.Values.ToArray();
    }

    // void OnGUI()
    // {
    //     GUILayout.BeginArea(new Rect(200, 200, 200, 500));
    //     GUILayout.BeginVertical();

    //     foreach(string playerID in players.Keys)
    //     {
    //         GUILayout.Label(playerID + "  -  " + players[playerID].transform.name);
    //     }

    //     GUILayout.EndVertical();
    //     GUILayout.EndArea();
    // }
    #endregion
}
