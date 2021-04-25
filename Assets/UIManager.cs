using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private float animationSpeed = 1f;
    [SerializeField]
    private RectTransform healthBar;

    private const int maxHealthPoint = -572;
    private const int minHealthPoint = -708;
    private const int healthOffset = -136;

    private float startHealthOffset;
    private float currentHealthOffset;
    private float targetHealthOffset;

    private void Start()
    {
        currentHealthOffset = maxHealthPoint;
        targetHealthOffset = maxHealthPoint;
        startHealthOffset = maxHealthPoint;
    }

    float animTime = 0;
    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(currentHealthOffset - targetHealthOffset) <= 1f)
        {
            currentHealthOffset = targetHealthOffset;
            animTime = 0f;
        }
        else
        {
            animTime += Time.deltaTime;
            currentHealthOffset = Mathf.Lerp(startHealthOffset, targetHealthOffset, animTime*animationSpeed);
            healthBar.offsetMax = new Vector2(healthBar.offsetMax.x, currentHealthOffset);
        }
    }

    public void SetHealth(float healthPercentage)
    {
        // Just in case we're a 100 type percentage.
        if (healthPercentage > 1)
            healthPercentage /= 100f;
        startHealthOffset = currentHealthOffset;
        targetHealthOffset = maxHealthPoint + (healthOffset - (healthPercentage * healthOffset));
    }
}
