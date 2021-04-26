using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Gates : MonoBehaviour
{
    bool openGates = false;
    [SerializeField]
    public GameObject leftGate;
    [SerializeField]
    public GameObject rightGate;
    [SerializeField]
    public List<Enemy> enemiesToKill;

    private float checkTime = 0.5f;
    private void FixedUpdate()
    {
        if (openGates)
            return;

        // Check if we've killed all the enemies.
        checkTime -= Time.fixedDeltaTime;
        if (checkTime <= 0)
        {
            openGates = enemiesToKill.All(x => x == null);
            if (openGates)
            {
                leftGateTargetPos = leftGate.transform.position;
                leftGateTargetPos.z -= 1;
                rightGateTargetPos = rightGate.transform.position;
                rightGateTargetPos.z += 1;
            }
            checkTime += 0.5f;
        }
    }

    Vector3 leftGateTargetPos = Vector3.zero;
    Vector3 rightGateTargetPos = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        if (openGates)
        {
            if (leftGateTargetPos.z < leftGate.transform.position.z)
            {
                Vector3 leftGatePos = leftGate.transform.position;
                leftGatePos.z -= Time.deltaTime;
                leftGate.transform.position = leftGatePos;
            }

            if (rightGateTargetPos.z > rightGate.transform.position.z)
            {
                Vector3 rightGatePos = rightGate.transform.position;
                rightGatePos.z += Time.deltaTime;
                rightGate.transform.position = rightGatePos;
            }

            if (leftGateTargetPos.z > leftGate.transform.position.z && rightGateTargetPos.z < rightGate.transform.position.z)
                Destroy(gameObject);
        }
    }
}
