using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    public string name = "Glock";

    public int damage = 10;
    public float range = 100f;

    public float fireRate = 0f;

    public int maxBullets = 30;
    [HideInInspector]
    public int bullets;

    public float reloadTime = 2f;

    public GameObject graphics;

    public PlayerWeapon()
    {
        bullets = maxBullets;
    }
}
