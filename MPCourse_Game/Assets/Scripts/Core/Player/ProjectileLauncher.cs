using System;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;

    [SerializeField] private Transform _projectileSpawnPoint;

    [SerializeField] private GameObject _serverProjectilePrefab;

    [SerializeField] private GameObject _clientProjectilePrefab;

    [Header("Settings")]
    [SerializeField] private float _projectileSpeed;

    private bool _shouldFire;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;

        _inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) return;

        _inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void Update() 
    {
        if(!IsOwner) return;

        if(!_shouldFire) return;

        PrimaryFireServerRpc(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
        SpawnDummyProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        _shouldFire = shouldFire;
    }

    private void SpawnDummyProjectile(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(_clientProjectilePrefab, spawnPosition, Quaternion.identity);
        projectileInstance.transform.up = direction;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(_serverProjectilePrefab, spawnPosition, Quaternion.identity);
        projectileInstance.transform.up = direction;

       SpawnDummyProjectileClientRpc(spawnPosition, direction);
    }
    
    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPosition, Vector3 direction)
    { 
        if(!IsOwner) return;

        SpawnDummyProjectile(spawnPosition, direction);
    }
}
