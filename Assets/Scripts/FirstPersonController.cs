using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    // For headbob.
    private GameObject camera;
    private CharacterController controller;
    [SerializeField]
    private float moveSpeed = 1;
    [SerializeField]
    private float rotationSpeed = 1;
    [SerializeField]
    private float timer = 1f;
    [SerializeField]
    private Color bulletColor = Color.red;

    [SerializeField]
    private Hand leftHand;
    [SerializeField]
    private Hand rightHand;

    [SerializeField]
    private Weapon currentWeapon;
    [SerializeField]
    private Weapon[] weapons;
    [SerializeField]
    private int weaponIndex = 0;
    [SerializeField]
    private float maxHealth = 100f;
    private float currentHealth;

    [SerializeField]
    private UIManager manager;

    [SerializeField]
    private float bobAmount;
    [SerializeField]
    private float bobFrequency;

    private float originalYPos;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        leftHand.SetWeapon(currentWeapon);
        rightHand.SetWeapon(currentWeapon);
        leftHand.SetHandTime(timer);
        rightHand.SetHandTime(timer);
        currentHealth = maxHealth;
        camera = Camera.main.gameObject;
        originalYPos = camera.transform.position.y;
    }

    float anim = 0f;
    Vector3 movement = Vector3.zero;
    private void HandleMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            movement.z = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.z = -1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            movement.x = 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            movement.x = -1;
        }

        Vector3 pos = camera.transform.position;
        if (movement == Vector3.zero)
        {
            // Lerp towards the original pos.
            pos.y = originalYPos;
        }
        else
        {
            pos.y = originalYPos + Mathf.Sin(Time.time* bobFrequency) * bobAmount;
        }
        camera.transform.position = pos;
    }

    private void HandleRotation()
    {
        //Get the Screen positions of the object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

        //Get the angle between the points
        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);

        //Ta Daaa
        transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    void HandleRotationManual()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        if (Input.GetKey(KeyCode.A))
        {
            rot.y -= rotationSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rot.y += rotationSpeed;
        }

        transform.rotation = Quaternion.Euler(rot);
    }

    void NextWeapon()
    {
        weaponIndex++;
        if (weaponIndex > weapons.Length-1)
        {
            weaponIndex = 0;
        }
        currentWeapon = weapons[weaponIndex];
    }

    void PreviousWeapon()
    {
        weaponIndex--;
        if (weaponIndex < 0)
        {
            weaponIndex = weapons.Length - 1;
        }
        currentWeapon = weapons[weaponIndex];
    }

    void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NextWeapon();
            leftHand.SetWeapon(currentWeapon);
            rightHand.SetWeapon(currentWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            PreviousWeapon();
            leftHand.SetWeapon(currentWeapon);
            rightHand.SetWeapon(currentWeapon);
        }
    }

    Vector3 transformedPoint;
    private void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward) * 50);
        RaycastHit info;
        Physics.Raycast(ray, out info);
        transformedPoint = info.point;
        transformedPoint.y = leftHand.BulletSpawnPositionWorld().position.y;
    }

    void Update()
    {
        ChangeWeapon();
        HandleRotationManual();
        HandleMovement();

        float curMoveSpeed = movement.z > 0 ? moveSpeed * movement.z : 0;
        leftHand.BulletSpawnPositionWorld().LookAt(transformedPoint);
        rightHand.BulletSpawnPositionWorld().LookAt(transformedPoint);
        leftHand.Tick(curMoveSpeed, Input.GetMouseButton(0));
        rightHand.Tick(curMoveSpeed, Input.GetMouseButton(1));

        controller.Move(transform.TransformDirection(movement.normalized) * moveSpeed * Time.deltaTime);
        movement = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(Random.Range(10f, 50f));
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }

        manager.SetHealth(currentHealth / maxHealth);
    }
}
