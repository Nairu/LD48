using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bat : Enemy
{
    private Quaternion _lookRotation;
    private Vector3 _direction;
    [SerializeField]
    protected float chargeActivateDistance = 3f;
    [SerializeField]
    protected float chargeCooldownTime = 1f;
    [SerializeField]
    protected float sleepTime = 0.5f;
    [SerializeField]
    protected float chargeSpeed = 3f;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (currentHealth <= 0)
            return;

        currentHealth -= damage;
        SetDamageEffect(Color.red);
        Aggro();

        if (currentHealth <= 0)
        {
            SetupSink();
        }
    }


    private enum BatState { SLEEPING, CHARGING, MOVING }
    private BatState state = BatState.SLEEPING;

    float sleepCooldownTimer = 0f;
    private void HandleSleep(float distance)
    {
        if (sleepCooldownTimer > 0)
        {
            sleepCooldownTimer -= Time.deltaTime;
        }
        else
        {
            chargeCooldownTimer = chargeCooldownTime;
            chargeActive = true;
            state = BatState.MOVING;
        }
    }

    float chargeCooldownTimer = 0f;
    bool chargeActive;
    Vector3 chargeDirection = Vector3.forward;
    private void HandleCharge(float distance)
    {
        if (chargeCooldownTimer <= 0)
        {
            sleepCooldownTimer = sleepTime;
            state = BatState.SLEEPING;
        }
        else
        {
            transform.Translate(chargeDirection * Time.deltaTime * speed * chargeSpeed);
            base.Attack();
            chargeCooldownTimer -= Time.deltaTime;
            if (CanAttack())
            {
                Attack();
            }
        }
    }

    private void HandleMoving(float distance)
    {
        if (distance < chargeActivateDistance && chargeActive)
        {
            state = BatState.CHARGING;
            agent.ResetPath();
            chargeActive = false;
            chargeDirection = Vector3.forward;
        }
        else
        {
            PathToPoint(player.transform.position);
        }
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            TickSink();
            return;
        }

        if (!aggrod)
        {
            if (CanSeePlayer())
                Aggro();
        }

        if (aggrod)
        {
            float playerDistance = Vector3.Distance(transform.position, player.transform.position);

            switch (state)
            {
                case BatState.SLEEPING:
                    transform.LookAt(player.transform);
                    HandleSleep(playerDistance);
                    break;
                case BatState.MOVING:
                    transform.LookAt(player.transform);
                    HandleMoving(playerDistance);
                    break;
                case BatState.CHARGING:
                    HandleCharge(playerDistance);
                    break;
            }
        }

        //rotate us over time according to speed until we are in the required rotation
        DamageEffect();
        HandleAnimation();
    }

    public override void Attack()
    {
        base.Attack();
        agent.ResetPath();
        player.TakeDamage(Random.Range(damage * 0.66f, damage * 1.25f));
    }
}
