using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int _damage = 5;

    private ulong _ownerClientId;

    public void SetOwner(ulong ownerClientId)
    {
        _ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody == null) return;

        if (other.attachedRigidbody.TryGetComponent(out NetworkObject networkObject) && _ownerClientId == networkObject.OwnerClientId) return;

        if (other.attachedRigidbody.TryGetComponent(out Health health)) 
        {
            health.TakeDamage(_damage);
        }
    }
}
