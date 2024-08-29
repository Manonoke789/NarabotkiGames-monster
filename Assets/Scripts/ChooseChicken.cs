using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HutongGames.PlayMaker;

public class ChooseChicken : MonoBehaviour
{
    public Image chickenPhoto;
    public TextMeshProUGUI chickenName;
    public TextMeshProUGUI chickenLevel;
    public Slider DamageSlider;
    public Slider SpeedSlider;
    public Slider HPSlider;
    public SuperkickSlider superkickSlider;
    public Image RarePhoto;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI TotalLevelText;
    public int TotalLevel;
    public Sprite timer_mask;
    private FsmGameObject selectedChickenVariable;

    void Start()
    {
        selectedChickenVariable = FsmVariables.GlobalVariables.FindFsmGameObject("selected_chicken");
        if (selectedChickenVariable == null)
        {
            Debug.LogError("���������� ���������� selected_chicken �� �������!");
            return;
        }

        if (selectedChickenVariable.Value != null)
        {
            UpdateLastChicken(selectedChickenVariable.Value.GetComponent<ChickenStats>());
        }
        else
        {
            Debug.Log("������ selected_chicken �� ����������. ���� ������� ���������� ������ � ����� Resources/Player.");

            GameObject[] players = Resources.LoadAll<GameObject>("Player");
            GameObject bestCandidate = null;
            int highestLevel = -1;

            foreach (var playerPrefab in players)
            {
                ChickenStats stats = playerPrefab.GetComponent<ChickenStats>();
                if (stats != null && stats.TotalLevel > highestLevel)
                {
                    bestCandidate = playerPrefab;
                    highestLevel = stats.TotalLevel;
                }
            }

            if (bestCandidate != null)
            {
                selectedChickenVariable.Value = bestCandidate;
                UpdateLastChicken(selectedChickenVariable.Value.GetComponent<ChickenStats>());
                Debug.Log("������ ������ � ��������� TotalLevel �� ����� Resources/Player.");
            }
            else
            {
                Debug.LogError("�� ������� ����� ���������� ������ � ����� Resources/Player.");
            }
        }
    }




    public void UpdateLastChicken(ChickenStats chickenStats)
    {
        if (chickenStats == null)
        {
            Debug.LogWarning("��������������� ������ �� �������� ��������� ChickenStats");
            return;
        }

        if (chickenPhoto != null && chickenStats.Photo != null)
            chickenPhoto.sprite = chickenStats.Photo;

        if (RarePhoto != null && chickenStats.RarePhoto != null)
            RarePhoto.sprite = chickenStats.RarePhoto;

        if (chickenName != null)
            chickenName.text = chickenStats.Name;

        if (chickenLevel != null)
            chickenLevel.text = "�������: " + chickenStats.Level.ToString();

        if (NameText != null)
            NameText.text = chickenStats.Name;

        if (TotalLevelText != null)
        {
            TotalLevel = chickenStats.DamageLevel + chickenStats.SpeedLevel + chickenStats.HPLevel;
            TotalLevelText.text = "����� �������: " + TotalLevel;
        }

        if (DamageSlider != null)
            DamageSlider.value = chickenStats.DamageLevel;

        if (SpeedSlider != null)
            SpeedSlider.value = chickenStats.SpeedLevel;

        if (HPSlider != null)
            HPSlider.value = chickenStats.HPLevel;

        if (superkickSlider != null)
            superkickSlider.UpdateSlider(chickenStats);

        selectedChickenVariable.Value = chickenStats.gameObject;
    }

}
