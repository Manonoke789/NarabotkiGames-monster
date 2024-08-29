using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoteController : MonoBehaviour
{
    private bool _stop;
    private bool _transparencyCheck;

    private int _armorOrDamage; //0 damage, 1 armor

    private RectTransform rectTransform;

    public CombatSystem SystemPanel;
    [SerializeField] private NoteController noteMovement;
    [SerializeField] private Image image;
    public Slider HealthEnemy;

    public float speedController;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2 (1100, 200);

        if(gameObject.tag == "Armor")
        {
            _armorOrDamage = 1;
        }
    }

    private void MovementNote()
    {
        var defaultSpeed = 200f;
        var maxSpeed = 800f;
        var speedCoefficient = (HealthEnemy.maxValue - HealthEnemy.value) / HealthEnemy.maxValue ;
        var speed = defaultSpeed + maxSpeed * speedCoefficient;
        rectTransform.position += new Vector3(speed * Time.deltaTime * -1, 0, 0);
        speedController = speed;
    }
    private void Update()
    {
       MovementNote();

        if (rectTransform.anchoredPosition.x >= -100 && rectTransform.anchoredPosition.x <= 100)
        {
            SystemPanel.noteMovement = noteMovement;
            SystemPanel.Miss = true;
            SystemPanel.ArmorOrDamage = _armorOrDamage;
        }
        if (rectTransform.anchoredPosition.x < 200 && rectTransform.anchoredPosition.x > 100)
        {
            SystemPanel.noteMovement2 = noteMovement;
        }

        if (rectTransform.anchoredPosition.x <= -100)
        {
            if(_transparencyCheck == false)
            { 
                SystemPanel.Accuracy(0, _armorOrDamage);
            }
            SystemPanel.Miss = false;
            Destroy(gameObject);
        }
    }
    public void PressingtappingTheScreen()
    {
        if (rectTransform.anchoredPosition.x >= -100 && rectTransform.anchoredPosition.x <= 100 && _stop == false && Time.timeScale != 0)
        {


            if (rectTransform.anchoredPosition.x > 25 || rectTransform.anchoredPosition.x < -25)
            {
                Debug.Log("Poor");
                SystemPanel.Accuracy(1, _armorOrDamage);
                _stop = true;
            }
            else if (rectTransform.anchoredPosition.x > 5 || rectTransform.anchoredPosition.x < -5)
            {
                Debug.Log("Great");
                SystemPanel.Accuracy(2, _armorOrDamage);
                _stop = true;
            }
            else if (rectTransform.anchoredPosition.x < 5 || rectTransform.anchoredPosition.x > -5)
            {
                Debug.Log("Perfect");
                SystemPanel.Accuracy(3, _armorOrDamage);
                _stop = true;
            }
            SystemPanel.Miss = false;
            Destroy(gameObject);
        }
    }
    public void HidingObject()
    {
        Debug.Log("Прозрачный");
        _stop = true;
        StartCoroutine(ColorChange());
    }
    IEnumerator ColorChange()
    {
        _transparencyCheck = true;
        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); 

        for (float t = 0.0f; t <= 1.0f; t += Time.deltaTime)
        {
            image.color = Color.Lerp(startColor, endColor, t); 
            yield return null;
        }
    }

}
