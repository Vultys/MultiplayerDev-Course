using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(!other.TryGetComponent(out Coin coinComponent)) return;

        int coins = coinComponent.Collect();

        if(!IsServer) return;
        
        TotalCoins.Value += coins;
    }
}
