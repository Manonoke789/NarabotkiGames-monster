using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UsingBoosters : MonoBehaviour
{
    [SerializeField] private Button boostAttackButton;
    [SerializeField] private Button boostShieldButton;
    [SerializeField] private Button boostRegenButton;

    [SerializeField] private Slider playerHealtchBar;

    [SerializeField] private TMP_Text playerHealtchTxt;
    [SerializeField] private TMP_Text attackTMP;
    [SerializeField] private TMP_Text shieldTMP;
    [SerializeField] private TMP_Text regenTMP;

    [SerializeField] private CombatSystem combatSystem;

    public int BoostAttack;
    public int BoostShield;
    public int BoostRegen;

    private void Start()
    {
        attackTMP.text = BoostAttack.ToString();
        shieldTMP.text = BoostShield.ToString();
        regenTMP.text = BoostRegen.ToString();
    }

    private void OnEnable()
    {
        boostAttackButton.onClick.AddListener(BoostAttackButtonDown);
        boostShieldButton.onClick.AddListener(BoostShieldButtonDown);
        boostRegenButton.onClick.AddListener(BoostRegenButtonDown);
    }
    private void OnDisable()
    {
        boostAttackButton.onClick.RemoveListener(BoostAttackButtonDown);
        boostShieldButton.onClick.RemoveListener(BoostShieldButtonDown);
        boostRegenButton.onClick.RemoveListener(BoostRegenButtonDown);
    }

    private void BoostAttackButtonDown()
    {
        if(BoostAttack >= 1)
        {
            AttacBoost();
        }
    }
    public void AttacBoost()
    {
        combatSystem.playerController.Damage += 10;
        BoostAttack--;
        attackTMP.text = BoostAttack.ToString();
    }
    private void BoostShieldButtonDown()
    {
        if(BoostShield >= 1)
        {
            ShieldBoost();
        }
    }
    public void ShieldBoost()
    {
        StartCoroutine(ShildForTime(10));

    }
    private void BoostRegenButtonDown()
    {
        if (BoostRegen >= 1)
        {
            RegenBoots();
        }
    }
    public void RegenBoots()
    {
        playerHealtchBar.value += 10;
        playerHealtchTxt.text = playerHealtchBar.value.ToString();
        BoostRegen--;
        regenTMP.text = BoostRegen.ToString();
    }
    IEnumerator ShildForTime(int Shild)
    {
        combatSystem.playerController.Armor += Shild;
        BoostShield--;
        shieldTMP.text = BoostShield.ToString();
        yield return new WaitForSeconds(3f);
        combatSystem.playerController.Armor -= Shild;
    }

}
