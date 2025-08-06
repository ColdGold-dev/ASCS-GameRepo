using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;
    public Canvas gameCanvas;

    private void Awake()
    {
        gameCanvas = FindObjectOfType<Canvas>();
    }

    public void CharecterTookDamage(GameObject charecter, int damageReccived)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(charecter.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
            .GetComponent<TMP_Text>();

        tmpText.text = damageReccived.ToString();
    }

    private void OnEnable()
    {
        
        CharecterEvents.charecterDamaged += CharecterTookDamage;
        //  CharecterEvents.CharecterHealed += CharecterHealed;
    }

    private void OnDisable()
    {



        CharecterEvents.charecterDamaged -= CharecterTookDamage;
    //   CharecterEvents.CharecterHealed -= CharecterHealed;
    }

    public void CharecterHealed(GameObject charecter, int healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(charecter.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
            .GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }
}
