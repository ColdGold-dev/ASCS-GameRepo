using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class WhiteFlashOnHit : MonoBehaviour
{
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashTime = 0.25f;

    private Damageable _damageable;
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _materials;

    private void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _materials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _materials[i] = _spriteRenderers[i].material;
           
        }

        _damageable = GetComponent<Damageable>();
        if (_damageable != null)
        {
            _damageable.damageableHit.AddListener(OnHit);
        
        }
    }

    private void OnHit(int damage, Vector2 knockback)
    {
       
        StartCoroutine(DamageFlasher());
    }

    private IEnumerator DamageFlasher()
    {
       

        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i].HasProperty("_Flash"))
            {
                _materials[i].SetColor("_Flash", _flashColor);
              
            }
          

            if (_materials[i].HasProperty("_FlashAmount"))
            {
                _materials[i].SetFloat("_FlashAmount", 1f);
               
            }
           
        }

        yield return new WaitForSeconds(_flashTime);

        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i].HasProperty("_FlashAmount"))
            {
                _materials[i].SetFloat("_FlashAmount", 0f);
               
            }
        }

      
    }

    private void OnDestroy()
    {
        if (_damageable != null)
        {
            _damageable.damageableHit.RemoveListener(OnHit);
          
        }
    }
}
