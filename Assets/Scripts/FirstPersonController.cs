using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private Weapon currentWeaponLeft;
    [SerializeField]
    private Weapon currentWeaponRight;
    [SerializeField]
    private List<Weapon> weapons;
    [SerializeField]
    private int weaponIndexLeft = 0;
    [SerializeField]
    private int weaponIndexRight = 0;
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
        leftHand.SetWeapon(currentWeaponLeft);
        rightHand.SetWeapon(currentWeaponRight);
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
        if (Input.GetKey(KeyCode.D))
        {
            movement.x = 1;
        }
        if (Input.GetKey(KeyCode.A))
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

    void NextWeaponLeft()
    {
        weaponIndexLeft++;
        if (weaponIndexLeft > weapons.Count-1)
        {
            weaponIndexLeft = 0;
        }
        currentWeaponLeft = weapons[weaponIndexLeft];
    }

    void NextWeaponRight()
    {
        weaponIndexRight++;
        if (weaponIndexRight > weapons.Count - 1)
        {
            weaponIndexRight = 0;
        }
        currentWeaponRight = weapons[weaponIndexRight];
    }

    void PreviousWeaponLeft()
    {
        weaponIndexLeft--;
        if (weaponIndexLeft < 0)
        {
            weaponIndexLeft = weapons.Count - 1;
        }
        currentWeaponLeft = weapons[weaponIndexLeft];
    }

    void PreviousWeaponRight()
    {
        weaponIndexRight--;
        if (weaponIndexRight < 0)
        {
            weaponIndexRight = weapons.Count - 1;
        }
        currentWeaponRight = weapons[weaponIndexRight];
    }

    void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            NextWeaponRight();
            rightHand.SetWeapon(currentWeaponRight);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            PreviousWeaponRight();
            rightHand.SetWeapon(currentWeaponRight);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            NextWeaponLeft();
            leftHand.SetWeapon(currentWeaponLeft);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            PreviousWeaponLeft();
            leftHand.SetWeapon(currentWeaponLeft);
        }
    }

    Vector3 transformedPoint;
    private void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward) * 50);
        RaycastHit info;
        Physics.Raycast(ray, out info);
        transformedPoint = info.point;
        //transformedPoint.y = leftHand.BulletSpawnPositionWorld().position.y;
    }

    void Update()
    {
        ChangeWeapon();
        //HandleRotationManual();
        HandleMovement();

        float curMoveSpeed = movement.z > 0 ? moveSpeed * movement.z : 0;
        leftHand.BulletSpawnPositionWorld().LookAt(transformedPoint);
        rightHand.BulletSpawnPositionWorld().LookAt(transformedPoint);
        leftHand.Tick(curMoveSpeed, Input.GetMouseButton(0));
        rightHand.Tick(curMoveSpeed, Input.GetMouseButton(1));

        controller.Move(transform.TransformDirection(movement.normalized) * moveSpeed * Time.deltaTime);
        movement = Vector3.zero;

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void AddWeapon(Weapon weapon)
    {
        weapons.Add(weapon);
        weaponIndexLeft = weapons.Count - 1;
        weaponIndexRight = weapons.Count - 1;
        currentWeaponLeft = weapon;
        currentWeaponRight = weapon;
        leftHand.SetWeapon(currentWeaponLeft);
        rightHand.SetWeapon(currentWeaponRight);
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
