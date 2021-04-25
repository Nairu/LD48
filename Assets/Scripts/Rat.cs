using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rat : Enemy
{
    private Quaternion _lookRotation;
    private Vector3 _direction;

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
            // Set a path to the player.
            PathToPoint(player.transform.position);

            if (CanAttack())
            {
                Attack();
            }
        }

        //rotate us over time according to speed until we are in the required rotation
        transform.LookAt(player.transform);
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
