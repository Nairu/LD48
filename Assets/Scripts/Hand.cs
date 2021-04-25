using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField]
    private Material idleTexture;
    [SerializeField]
    private Material fireTexture;
    [SerializeField]
    private Renderer handRenderer;
    [SerializeField]
    private Transform bulletSpawnPoint;
    [SerializeField]
    private bool flipHand = false;

    float handTime = 1f;
    float lastFiredTime = 0f;
    float animationTime = 0f;
    bool fired = false;
    bool loading = false;
    float parentSpeed = 0f;
    Weapon currentWeapon;
    Bullet currentBullet;

    public Transform BulletSpawnPositionWorld()
    {
        return bulletSpawnPoint.transform;
    }

    private void Start()
    {
        idleTexture = new Material(idleTexture);
        fireTexture = new Material(fireTexture);

        if (flipHand)
        {
            Vector2 scale = new Vector2(-1, 1);
            idleTexture.mainTextureScale = scale;
            fireTexture.mainTextureScale = scale;
            handRenderer.material = idleTexture;
        }
    }

    void HandleClickLeft()
    {
        if (animationTime <= 0 && fired == false)
        {
            if (currentBullet != null)
            {
                currentBullet.transform.parent = null;
                currentBullet.transform.rotation = bulletSpawnPoint.transform.rotation;
                currentBullet.GetComponent<SphereCollider>().enabled = true;
                currentBullet.gameObject.layer = LayerMask.NameToLayer("Default");
                currentBullet.SetSpeed(currentBullet.GetMaxSpeed() + parentSpeed);
            }

            animationTime += handTime;
            handRenderer.material = fireTexture;
            fired = true;
        }
    }

    void HandleHandAnimation()
    {
        if (animationTime <= 0)
        {
            animationTime = 0;
            handRenderer.material = idleTexture;
        }
        else
        {
            animationTime -= Time.deltaTime;
        }
    }

    void LoadBullet()
    {
        GameObject b = Instantiate(currentWeapon.GetBullet().gameObject, bulletSpawnPoint.position, Quaternion.identity, bulletSpawnPoint);
        currentBullet = b.GetComponent<Bullet>();
        currentBullet.GetComponent<SphereCollider>().enabled = false;
        currentBullet.SetSpeed(0);
        currentBullet.growSpeed = currentWeapon.GetFireRate();
        currentBullet.maxScale = currentWeapon.GetBulletSize();
        currentBullet.SetColor(currentWeapon.GetBulletColor());
        loading = true;
        lastFiredTime = Time.time + currentWeapon.GetFireRate();
    }

    void HandleWeapon()
    {
        if (animationTime <= 0 && fired == true && loading == false)
        {
            LoadBullet();
        }

        HandleHandAnimation();

        if (lastFiredTime <= Time.time && loading == true && fired == true)
        {
            loading = false;
            fired = false;
        }
    }

    public void Tick(float parentMoveSpeed, bool firing)
    {
        parentSpeed = parentMoveSpeed;

        if (firing)
        {
            HandleClickLeft();
        }
        HandleWeapon();
    }

    public void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        if (currentBullet)
        {
            Destroy(currentBullet.gameObject);
            currentBullet = null;
        }

        LoadBullet();
        // Cheap hack.
        fired = true;
    }

    public void SetHandTime(float time)
    {
        handTime = time;
    }
}
