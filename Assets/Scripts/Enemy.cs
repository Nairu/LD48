using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float MaxHealth;

    [SerializeField]
    protected float RotationSpeed;

    [SerializeField]
    protected Color renderColor;

    [SerializeField]
    protected float damage;

    [SerializeField]
    protected float sensorRange = 5f;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float allyAggroRange = 2f;

    [SerializeField]
    protected LayerMask layersToPerceive;

    [SerializeField]
    protected float attackSpeed = 1f;

    [SerializeField]
    protected float sinkSpeed = 1f;

    [SerializeField]
    protected float attackRange = 1f;

    [SerializeField]
    protected bool rangedAttack = false;

    [SerializeField]
    protected List<Texture> walkAnimFrames;
    [SerializeField]
    protected float walkAnimSpeed = 0f;

    [SerializeField]
    protected Texture idleAnimFrame;
    // No speed because we're always idle until we're not.

    [SerializeField]
    protected Texture attackAnimFrame;
    [SerializeField]
    protected float attackAnimSpeed = 0f;

    [SerializeField]
    protected Texture hurtAnimFrame;
    [SerializeField]
    protected float hurtAnimSpeed = 0f;

    private enum AnimState { IDLE, MOVE, ATTACK, HURT }
    private AnimState currentState = AnimState.IDLE;

    protected float currentHealth;
    protected NavMeshAgent agent;
    protected FirstPersonController player;
    protected Renderer _renderer;
    protected bool aggrod = false;

    private float sinkAnimationPosition = 0f;
    private float lastAttackTime = 0f;
    private float colorShift = 0f;

    public void Aggro()
    {
        if (aggrod == false)
        {
            aggrod = true;
            AggroAllies();
            currentState = AnimState.MOVE;
        }
    }

    private float currentAnimTimer = 0f;
    private int walkAnimIndex = 0;
    protected void HandleAnimation()
    {
        switch (currentState)
        {
            case AnimState.ATTACK:
                _renderer.material.mainTexture = attackAnimFrame;
                currentAnimTimer -= Time.deltaTime;
                if (currentAnimTimer <= 0f)
                {
                    currentState = AnimState.MOVE;
                    currentAnimTimer = walkAnimSpeed;
                }
                break;
            case AnimState.HURT:
                _renderer.material.mainTexture = hurtAnimFrame;
                currentAnimTimer -= Time.deltaTime;
                if (currentAnimTimer <= 0f)
                {
                    currentState = AnimState.MOVE;
                    currentAnimTimer = walkAnimSpeed;
                }
                break;
            case AnimState.IDLE:
                _renderer.material.mainTexture = idleAnimFrame;
                break;
            case AnimState.MOVE:
                _renderer.material.mainTexture = walkAnimFrames[walkAnimIndex];
                currentAnimTimer -= Time.deltaTime;
                if (currentAnimTimer <= 0f)
                {
                    walkAnimIndex++;
                    if (walkAnimIndex > walkAnimFrames.Count - 1)
                        walkAnimIndex = 0;
                    currentAnimTimer = walkAnimSpeed;
                }
                break;
        }
    }

    // Start is called before the first frame update
    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        _renderer = GetComponentInChildren<Renderer>();
        currentHealth = MaxHealth;
        lastAttackTime = Time.time;
        walkAnimSpeed = 1 / (speed*2);
    }

    private void AggroAllies()
    {
        var colliders = Physics.OverlapSphere(transform.position, allyAggroRange, LayerMask.GetMask("Enemy"));
        foreach (Collider col in colliders)
        {
            if (col.gameObject == this.gameObject)
                continue;
            
            if (CanSeeTarget(col.transform, LayerMask.GetMask("Enemy", "World"), LayerMask.NameToLayer("Enemy")))
            {
                col.GetComponent<Enemy>().Aggro();
            }
        }
    }

    public virtual void TakeDamage(float damage) {
        currentState = AnimState.HURT;
        currentAnimTimer = hurtAnimSpeed;
    }

    public virtual void Attack() {
        currentState = AnimState.ATTACK;
        currentAnimTimer = attackAnimSpeed;
    }

    // Variables for working out where to sink to.
    Vector2 scale = Vector2.zero;
    Vector3 startPos = Vector3.zero;
    Vector3 endPos = Vector3.zero;
    protected void SetupSink()
    {
        scale = _renderer.material.mainTextureScale;
        scale.y = -scale.y;
        agent.ResetPath();
        startPos = transform.position;
        endPos = transform.position;
        endPos.y *= -1;
        transform.GetComponent<BoxCollider>().enabled = false;
        sinkAnimationPosition = 0f;
    }

    protected bool CanAttack()
    {
        bool attack = false;
        if (lastAttackTime + attackSpeed < Time.time)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
            {
                lastAttackTime = Time.time;
                attack = true;
            }
        }
        return attack;
    }

    protected void TickSink()
    {
        sinkAnimationPosition += Time.deltaTime * sinkSpeed;
        _renderer.material.mainTextureScale = scale;
        transform.position = Vector3.Lerp(startPos, endPos, sinkAnimationPosition);
        if (sinkAnimationPosition >= 1)
            Destroy(this.gameObject);
    }

    protected void SetDamageEffect(Color col)
    {
        _renderer.material.color = col;
        colorShift = 0.05f;
    }

    protected void DamageEffect()
    {
        if (colorShift <= 0)
        {
            _renderer.material.color = renderColor;
        }
        else
        {
            colorShift -= Time.deltaTime;
        }
    }

    protected bool CanSeeTarget(Transform target, LayerMask allLayers, int validLayer)
    {
        RaycastHit info;
        Physics.Raycast(transform.position, target.transform.position - transform.position, out info, sensorRange, allLayers);
        if (info.collider == null)
            return false;
        return info.collider.gameObject.layer == validLayer;
    }

    protected bool CanSeePlayer()
    {
        return CanSeeTarget(player.transform, LayerMask.GetMask("Player", "World"), LayerMask.NameToLayer("Player"));
        //// Check if we're aggrod.
        //RaycastHit info;
        //Physics.Raycast(transform.position, player.transform.position - transform.position, out info, sensorRange, layersToPerceive);
        //return (info.transform != null && info.transform.CompareTag("Player"));
    }

    protected void PathToPoint(Vector3 pos)
    {
        float dist = Vector3.Distance(transform.position, pos);
        if (dist >= attackRange)
        {
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(pos, path))
            {
                agent.speed = speed;
                agent.SetPath(path);
            }
        }
    }
}
