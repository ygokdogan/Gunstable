using Interfaces;
using Player;
using UnityEngine;

public class JetpackUnlock : MonoBehaviour, IInteractable
{
    public void Interact(PlayerController player)
    {
        player.UnlockJetpack();
        Destroy(gameObject);
    }
}