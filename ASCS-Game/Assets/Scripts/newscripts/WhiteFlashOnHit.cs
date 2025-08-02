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
            Debug.Log($"{gameObject.name} - Material {i} assigned: {_materials[i].name}");
        }

        _damageable = GetComponent<Damageable>();
        if (_damageable != null)
        {
            _damageable.damageableHit.AddListener(OnHit);
            Debug.Log($"{gameObject.name} - Subscribed to damageableHit");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} - No Damageable component found!");
        }
    }

    private void OnHit(int damage, Vector2 knockback)
    {
        Debug.Log($"{gameObject.name} - OnHit triggered");
        StartCoroutine(DamageFlasher());
    }

    private IEnumerator DamageFlasher()
    {
        Debug.Log($"{gameObject.name} - Flash started");

        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i].HasProperty("_Flash"))
            {
                _materials[i].SetColor("_Flash", _flashColor);
                Debug.Log($"{gameObject.name} - Set _Flash color on material {i}");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} - Material {i} missing _Flash property");
            }

            if (_materials[i].HasProperty("_FlashAmount"))
            {
                _materials[i].SetFloat("_FlashAmount", 1f);
                Debug.Log($"{gameObject.name} - Set _FlashAmount to 1 on material {i}");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} - Material {i} missing _FlashAmount property");
            }
        }

        yield return new WaitForSeconds(_flashTime);

        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i].HasProperty("_FlashAmount"))
            {
                _materials[i].SetFloat("_FlashAmount", 0f);
                Debug.Log($"{gameObject.name} - Reset _FlashAmount on material {i}");
            }
        }

        Debug.Log($"{gameObject.name} - Flash ended");
    }

    private void OnDestroy()
    {
        if (_damageable != null)
        {
            _damageable.damageableHit.RemoveListener(OnHit);
            Debug.Log($"{gameObject.name} - Unsubscribed from damageableHit");
        }
    }
}
