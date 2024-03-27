using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFov : MonoBehaviour
{
    public float viewRange = 15f;
    [Range(0, 360)]
    public float viewAngle = 120f;

    public Vector3 CirclePoint(float angle) {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public UnitBase isTracePlayer() {
        var colls = Physics.OverlapSphere(transform.position, viewRange, LayerMask.GetMask("Unit"));
        if (colls.Length > 0) {
            foreach (var coll in colls) {
                if (coll.gameObject == gameObject)
                    continue;

                var dir = coll.transform.position - transform.position;
                dir = dir.normalized;
                if (Vector3.Angle(transform.forward, dir) < viewAngle * 0.5f) {
                    int mask = (1 << (int)Define.LayerList.Unit) | (1 << (int)Define.LayerList.Obstacle);
                    Physics.Raycast(transform.position, dir, out var target, float.MaxValue, mask);
                    if(target.collider.gameObject.layer == (int)Define.LayerList.Obstacle) {
                        continue;
                    }
                    else if(target.collider.gameObject.layer == (int)Define.LayerList.Unit) {
                        Debug.Log($"¹ß°ß!{target.collider.name}");
                        return coll.GetComponent<UnitBase>();
                    }
                }
            }
        }
        return null;
    }

}
