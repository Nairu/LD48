using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    public float maxSpeed;
    [SerializeField]
    public float growSpeed;
    [SerializeField]
    public float maxScale = 0.4f;
    [SerializeField]
    public Color color = Color.red;
    [SerializeField]
    public int numBullets = 1;

    [SerializeField]
    private List<GameObject> subBullets;

    [SerializeField]
    private float damage;
    [SerializeField]
    private LayerMask interactableLayers;


    [SerializeField]
    private Renderer bulletRenderer1;
    Material _bulletAnimMaterial1;

    [SerializeField]
    private Renderer bulletRenderer2;
    Material _bulletAnimMaterial2;

    [SerializeField]
    private Renderer fogRenderer;
    Material _bulletFogMaterial;

    [SerializeField]
    private Renderer coreRenderer;
    Material _bulletCoreMaterial;

    private float speed;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (interactableLayers == (interactableLayers | (1 << other.gameObject.layer)))
        {
            if (other.GetComponent<Enemy>() != null)
            {
                other.GetComponent<Enemy>().TakeDamage(damage);
            }

            // Destroy ourselves for now.
            Destroy(gameObject);
        }
    }

    Vector3 startScale = new Vector3(0.01f, 0.01f, 0.01f);
    float time = 0;
    private void HandleGrowing()
    {
        if (time > growSpeed)
            return;

        transform.localScale = Vector3.Lerp(startScale, new Vector3(maxScale, maxScale, maxScale), time / growSpeed);
        time += Time.deltaTime;
    }

    private void Start()
    {
        _bulletAnimMaterial1 = bulletRenderer1.material;
        _bulletAnimMaterial2 = bulletRenderer2.material;
        _bulletCoreMaterial = coreRenderer.material;
        _bulletFogMaterial = fogRenderer.material;

        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

    bool colorChanged = false;
    public void SetColor(Color color)
    {
        Start();
        color.a = 255f / 255f;
        _bulletAnimMaterial1.color = color;
        _bulletAnimMaterial2.color = color;
        Color core = color;
        core.a = 128f / 255f;
        _bulletCoreMaterial.color = core;
        Color fog = color;
        fog.a = 32f / 255f;
        _bulletFogMaterial.color = fog;
        colorChanged = true;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void ResetSpeed()
    {
        this.speed = maxSpeed;
    }

    public void SetMaxSpeed(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
    }

    public float GetMaxSpeed()
    {
        return this.maxSpeed;
    }

    private void FixedUpdate()
    {
        Vector3 dir = transform.TransformDirection(Vector3.forward);
        dir *= speed * Time.deltaTime;
        transform.position += dir;
    }

    Vector2 offset = Vector2.zero;
    private void Update()
    {
        HandleGrowing();

        offset.y += Time.deltaTime;
        if (offset.y >= 1)
            offset.y -= 1;

        _bulletAnimMaterial1.mainTextureOffset = offset;
        _bulletAnimMaterial2.mainTextureOffset = offset;

        bulletRenderer1.material = _bulletAnimMaterial1;
        bulletRenderer2.material = _bulletAnimMaterial2;

        if (colorChanged)
        {
            colorChanged = false;
            fogRenderer.material = _bulletFogMaterial;
            coreRenderer.material = _bulletCoreMaterial;
        }

        if (subBullets.Count > 1)
        {
            int index = 1;
            foreach (GameObject go in subBullets)
            {
                Vector3 pos = Vector3.zero;
                float sin = Mathf.Sin((Time.time * orbRotationSpeed) + orbSeperationDistance * index);
                float cos = Mathf.Cos((Time.time * orbRotationSpeed) + orbSeperationDistance * index);
                float orbit = Mathf.Sin((Time.time / 2 * orbRotationSpeed) + orbSeperationDistance * index);// + Mathf.Cos(Time.time * 3);
                pos.x = sin * offsetDistance;
                pos.y = cos * offsetDistance;
                pos.z = orbit * offsetDistance;
                //pos.x = -Mathf.Sin(Mathf.Cos(Time.time + curSpacing)) / 2 + Mathf.Sin(Mathf.Cos(Time.time + curSpacing));
                //pos.z = -Mathf.Cos(Mathf.Sin(Time.time + curSpacing)) / 2 + Mathf.Cos(Mathf.Sin(Time.time + curSpacing));
                //pos.y = -Mathf.Sin(Mathf.Sin(Time.time + 0.5f + curSpacing)) / 2 + Mathf.Sin(Mathf.Sin(Time.time + 0.5f + curSpacing));
                go.transform.localPosition = pos;
                index += 1;
            }
        }
    }
    public float orbSeperationDistance = 1f;
    public float offsetDistance = 0.2f;
    public float orbRotationSpeed = 1f;

    public Quaternion SlerpAround(Transform orbit, Transform mover, float speed)
    {
        Vector3 relativePos = (orbit.position) - mover.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        Quaternion current = mover.localRotation;

        return Quaternion.Slerp(current, rotation, speed * Time.deltaTime);
    }
}
