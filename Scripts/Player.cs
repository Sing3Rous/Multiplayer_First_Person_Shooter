using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }

    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    public float GetHealthPct()
    {
        return (float)currentHealth / maxHealth;
    }

    [SyncVar]
    public string username = "Loading...";

    public int kills;
    public int deaths;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            //Switch cameras
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup) 
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; ++i)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }

        SetDefaults();
    }

    // void Update()
    // {
    //     if (!isLocalPlayer)
    //     {
    //         return;
    //     }

    //     if (Input.GetKeyDown(KeyCode.K))
    //     {
    //         RpcTakeDamage(999999);
    //     }
    // }

    [ClientRpc]
    public void RpcTakeDamage(int amount, string sourceID)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if (currentHealth <= 0)
        {
            Die(sourceID);
        }
    }

    private void Die(string sourceID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(sourceID);
        if (sourceID != null)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);
        }

        deaths++;

        //Disable components
        foreach(Behaviour component in disableOnDeath)
        {
            component.enabled = false;
        }

        //Disable GameObjects
        foreach(GameObject gameObj in disableGameObjectsOnDeath)
        {
            gameObj.SetActive(false);
        }

        //Disable the collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        //Spawn a death effect
        GameObject gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gfxIns, 3f);
        
        //Switch cameras
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " is dead!");

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log(transform.name + " respawned.");
    }
    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        //Enable the components
        for (int i = 0; i < disableOnDeath.Length; ++i)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Enable the gameobjects
        foreach(GameObject gameObj in disableGameObjectsOnDeath)
        {
            gameObj.SetActive(true);
        }

        //Enable the collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        //Create spawn effect
        GameObject gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(gfxIns, 3f);
    }
}
