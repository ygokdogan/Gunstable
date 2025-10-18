using Interfaces;
using Player;
using UnityEngine;

public class JetpackUnlock : MonoBehaviour, IInteractable
{
    public GameObject interactPrompt;
    
    public void Interact(PlayerController player)
    {
        player.UnlockJetpack();
        Destroy(gameObject);
    }
}