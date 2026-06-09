using UnityEngine;

// Sits on a child GameObject of the enemy.
// The enemy's attack animation toggles this GameObject active during parryable frames.
// AttackType identifies which kind of attack this is - the player must use the matching parry direction.
public class ParryableWindow : MonoBehaviour
{
    public enum AttackType { High, Mid, Low }

    [Tooltip("Which parry direction is needed to counter this attack")]
    public AttackType attackType = AttackType.Mid;
}