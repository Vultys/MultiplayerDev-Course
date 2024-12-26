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

    [SerializeField] private GameObject _muzzleFlash;

    [SerializeField] private Collider2D _playerCollider;

    [Header("Settings")]
    [SerializeField] private float _projectileSpeed = 30f;

    [SerializeField] private float _fireRate = 0.75f;

    [SerializeField] private float _muzzleFlashDuration = 0.075f;

    private bool _shouldFire;

    private float _previousFireTime;

    private float _muzzleFlashTimer;

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
        if(_muzzleFlashTimer > 0f)
        {
            _muzzleFlashTimer -= Time.deltaTime;

            if(_muzzleFlashTimer <= 0f)
            {
                _muzzleFlash.SetActive(false);
            }
        }

        if(!IsOwner) return;

        if(!_shouldFire) return;

        if(Time.time < (1 / _fireRate) + _previousFireTime) return;

        PrimaryFireServerRpc(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
        SpawnDummyProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
        
        _previousFireTime = Time.time;
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        _shouldFire = shouldFire;
    }

    private void SpawnDummyProjectile(Vector3 spawnPosition, Vector3 direction)
    {
        _muzzleFlash.SetActive(true);
        _muzzleFlashTimer = _muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(_clientProjectilePrefab, spawnPosition, Quaternion.identity);
        projectileInstance.transform.up = direction;
        
        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        if(projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = rigidbody.transform.up * _projectileSpeed;
        }
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(_serverProjectilePrefab, spawnPosition, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        if(projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = rigidbody.transform.up * _projectileSpeed;
        }

        SpawnDummyProjectileClientRpc(spawnPosition, direction);
    }
    
    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPosition, Vector3 direction)
    { 
        if(IsOwner) return;

        SpawnDummyProjectile(spawnPosition, direction);
    }
}
