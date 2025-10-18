using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public int maxHealth = 2;
        
        public float moveSpeed = 8f;
        public float jumpForce = 8f;
        public float jumpDuration = 0.5f;

        public float shootCooldown = .2f;

        public bool acquiredJetpack = false;
        public float jetpackForce = 10f;
    }
}