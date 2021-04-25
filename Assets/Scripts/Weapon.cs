using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private Bullet bullet;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private Color bulletColor;
    [SerializeField]
    private float bulletSize;
    [SerializeField]
    private float bulletDamage;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private int numBullets = 1;

    public Bullet GetBullet()
    {
        bullet.SetDamage(bulletDamage);
        bullet.SetMaxSpeed(bulletSpeed);
        return bullet;
    }

    public int GetNumBullets()
    {
        return numBullets;
    }

    public float GetFireRate()
    {
        return fireRate;
    }

    public Color GetBulletColor()
    {
        return bulletColor;
    }

    public float GetBulletSize()
    {
        return bulletSize;
    }
}
