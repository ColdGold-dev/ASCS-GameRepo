using UnityEngine;

[RequireComponent(typeof(Capescript))]
public class CapeOffsetController : MonoBehaviour
{
    private Capescript cape;
    private Playermovement player;
    private TouchingDirections touching;

    [Header("Cape Offsets")]
    public Vector2 idleOffset = Vector2.zero;
    public Vector2 walkOffset = new Vector2(-0.05f, 0f);
    public Vector2 jumpOffset = new Vector2(0.05f, -0.05f);
    public Vector2 fallOffset = new Vector2(-0.05f, -0.1f);

    private void Awake()
    {
        cape = GetComponent<Capescript>();
        player = GetComponentInParent<Playermovement>();
        touching = player.GetComponent<TouchingDirections>();
    }

    private void LateUpdate()
{
    if (player == null || touching == null) return;

    Vector2 baseOffset;

    if (touching.IsGrounded)
    {
        baseOffset = player.IsMoving ? walkOffset : idleOffset;
    }
    else
    {
        baseOffset = player.GetComponent<Rigidbody2D>().linearVelocity.y > 0.1f ? jumpOffset : fallOffset;
    }

    // Flip the offset if the player is facing left
    if (!player.IsFacingRight)
    {
        baseOffset.x *= -1;
    }

    cape.partOffset = baseOffset;
}

}
