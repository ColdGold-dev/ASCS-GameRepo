using UnityEngine;

public class Capescript : MonoBehaviour
{
    public Vector2 partOffset = Vector2.zero;

    private Transform[] hairParts;
    private Transform hairAnchor;
public float lerpSpeed = 20f;
    private void Awake()
    {
        hairAnchor = GetComponent<Transform>();
        hairParts = GetComponentsInChildren<Transform>(); 

    }

    private void Update()
    {
        Transform pieceToFollow = hairAnchor;

        foreach (Transform hairPart in hairParts)
        {
            if (hairPart != hairAnchor) 
            {

                Vector2 targetPosition = (Vector2)pieceToFollow.position + partOffset;
                Vector2 newPositionLerped = Vector2.Lerp(hairPart.position, targetPosition, Time.deltaTime * lerpSpeed);


                hairPart.position = newPositionLerped;
                pieceToFollow = hairPart;
            }
        }
    }
}
