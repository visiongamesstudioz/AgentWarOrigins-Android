using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

public class ExplosiveForce : MonoBehaviour
{

    public float ExplosionForce = 4f;
    public float Multiplier = 1f;
    public float DamageAtCenter = 50f;
    public float ExplosionRadius=10f;
    private List<Rigidbody> explodedObjects;
    public List<Rigidbody> Explode(int layerMask)
    {
        var cols = Physics.OverlapSphere(transform.position, ExplosionRadius);
        explodedObjects = new List<Rigidbody>();
        foreach (var col in cols)
        {
            if (col.attachedRigidbody != null && !explodedObjects.Contains(col.attachedRigidbody) && col.attachedRigidbody.gameObject.layer==layerMask )
            {
                explodedObjects.Add(col.attachedRigidbody);
            }
        }
        foreach (var rb in explodedObjects)
        {
            rb.AddExplosionForce(ExplosionForce * Multiplier, transform.position, ExplosionRadius);            
        }
        return explodedObjects;
    }

    public List<Rigidbody> Explode()
    {
        foreach (var rb in explodedObjects)
        {
            rb.AddExplosionForce(ExplosionForce * Multiplier, transform.position, ExplosionRadius);
        }
        return explodedObjects;
    }

  


}
