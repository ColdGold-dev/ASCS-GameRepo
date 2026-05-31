using UnityEngine;

// Marker for "this enemy is currently parryable."
// Put on a child of the enemy. Their attack animation toggles this GameObject
// active/inactive during their attack's parryable frames.
// No code - the GameObject's activeInHierarchy IS the data.
public class ParryableWindow : MonoBehaviour
{
}