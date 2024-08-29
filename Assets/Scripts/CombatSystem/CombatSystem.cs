using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class CombatSystem : MonoBehaviour
{
    [SerializeField] private GameObject MissTMP;
    [SerializeField] private GameObject PoorTMP;
    [SerializeField] private GameObject GreatTMP;
    [SerializeField] private GameObject PerfectTMP;
    [SerializeField] private GameObject ComboTMP;

    [SerializeField] private TMP_Text playerHealtchTxt;
    [SerializeField] private TMP_Text enemyHealtchTxt;


    [SerializeField] private NoteController Damage;
    [SerializeField] private NoteController Armor;
    [SerializeField] private UsingBoosters usingBoosters;

    public NoteController noteMovement;
    public NoteController noteMovement2;

    public PlayerController playerController;
    public EnemyController enemyController;
    public int ArmorOrDamage; //0 damage, 1 armor

    public Slider ComboSlider;
    [SerializeField] private Slider playerHealtchBar;
    [SerializeField] private Slider enemyHealtchBar;

    [SerializeField] private Transform transformObj;

    public void HelthBar()
    {
        var PlayerHealtch = playerController.Health;
        var EnemyHealtch = enemyController.Health;

        playerHealtchTxt.text = PlayerHealtch.ToString();
        enemyHealtchTxt.text = EnemyHealtch.ToString();

        playerHealtchBar.maxValue = PlayerHealtch;
        enemyHealtchBar.maxValue = EnemyHealtch;

        playerHealtchBar.value = PlayerHealtch;
        enemyHealtchBar.value = EnemyHealtch;


        Debug.Log("ss");
    }

    private float _timer;
    public bool Miss;
    private bool _armorOrDamage;

    private void Start()
    {
        Miss = false;
        _timer = 4f;
    }
    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 2 && _armorOrDamage == false)
        {
            CreatingNotes(Damage);
            _armorOrDamage = true;
        }
        else if (_timer <= 0)
        {
            CreatingNotes(Armor);
            _timer = 4;
            _armorOrDamage = false;
        }

        if (ComboSlider.value == ComboSlider.maxValue)
        {
            Combo();
        }

        if (playerHealtchBar.value == 0 || enemyHealtchBar.value == 0)
        {
            Time.timeScale = 0;
        }

    }
    private void Combo()
    {
        ComboTMP.SetActive(true);
        StartCoroutine(CountSeconds(ComboTMP));
        ComboSlider.value = 0;
        var WhatComboTrue = Random.Range(0, 2); //0 - �������� �����, 1 - ������ �� �����, 2 - �������������� ��������;

        if (WhatComboTrue == 0)
        {
            usingBoosters.AttacBoost();
            Debug.Log("����� �����");
        }
        else if (WhatComboTrue == 1)
        {
            usingBoosters.ShieldBoost();
            Debug.Log("����� ������");
        }
        else if (WhatComboTrue == 2)
        {
            usingBoosters.RegenBoots();
            Debug.Log("����� ������������� ��");
        }
    }

    private void CreatingNotes(NoteController note)
    {
        var Obj = Instantiate(note, transformObj.parent);
        Obj.SystemPanel = this;
        Obj.HealthEnemy = enemyHealtchBar;
    }
    
    public void Accuracy(int Level, int ArmorOrDamage)
    {
        if (Level == 3)
        {
            ComboSlider.value += 3;
            PerfectTMP.SetActive(true);
            StartCoroutine(CountSeconds(PerfectTMP));
            if (ArmorOrDamage == 0) 
            {
                DamageNote(5);
            }
            else
            {
                ArmorNote(100);
            }
        }
        else if (Level == 2)
        {
            ComboSlider.value += 1;
            GreatTMP.SetActive(true);
            StartCoroutine(CountSeconds(GreatTMP));
            if (ArmorOrDamage == 0)
            {
                DamageNote(2);
            }
            else
            {
                ArmorNote(5);
            }
        }
        else if (Level == 1)
        {
            ComboSlider.value = 0;
            PoorTMP.SetActive(true);
            StartCoroutine(CountSeconds(PoorTMP));
            if (ArmorOrDamage == 0)
            {
                DamageNote(1);
            }
            else
            {
                ArmorNote(1);
            }
        }
        else if (Level == 0)
        {
            ComboSlider.value = 0;
            MissTMP.SetActive(true);
            StartCoroutine(CountSeconds(MissTMP));
            ArmorNote(0);
        }
    }
    private void DamageNote(int Str)
    {
        var damag = playerController.Damage * Str - enemyController.Armor;
        enemyHealtchBar.value -= damag;
        enemyHealtchTxt.text = enemyHealtchBar.value.ToString();
        Debug.Log(damag + "���� ������");
    }
    private void ArmorNote(int Armor)
    {
        var damage = enemyController.Damage - playerController.Armor * Armor;
        if (damage < 0)
        {
            damage = 0;
        }
        playerHealtchBar.value -= damage;
        playerHealtchTxt.text = playerHealtchBar.value.ToString();
        Debug.Log(damage + "���� �����");

    }
    IEnumerator CountSeconds(GameObject TmpPanel)
    {
        yield return new WaitForSeconds(1f);
        TmpPanel.SetActive(false);
    }
}
