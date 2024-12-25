using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private Transform _turretTransform;

    [SerializeField] private InputReader _inputReader;

    private void LateUpdate() 
    {
        if(!IsOwner) return;

        Vector2 _cursorPosition = Camera.main.ScreenToWorldPoint(_inputReader.AimPosition);

        _turretTransform.up = _cursorPosition - (Vector2)_turretTransform.position;
    }    
}
