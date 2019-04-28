using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";
    public PlayerWeapon weapon;

    [SerializeField]
    private Camera camera;
    [SerializeField]
    private LayerMask mask;

    void Start()
    {
        if (camera == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced");
            this.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }
    
    [Client]
    void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, weapon.range, mask))
        {
            if (hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(hit.collider.name, weapon.damage);
            }
        }
    }

    [Command]
    void CmdPlayerShot(string playerID, int damage)
    {
        Debug.Log(playerID + " has been shot.");

        Player player = GameManager.GetPlayer(playerID);
        player.TakeDamage(damage);
    }
}
