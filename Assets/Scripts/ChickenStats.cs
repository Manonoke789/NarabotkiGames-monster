using UnityEngine;
using Gley.DailyRewards.Internal;
using Gley.DailyRewards;
using UnityEngine.UI;
using TMPro;

public class ChickenStats : MonoBehaviour
{
    [Range(1, 10)]
    public int DamageLevel;
    [Range(1, 10)]
    public int SpeedLevel;
    [Range(1, 10)]
    public int HPLevel;
    [Range(1, 3)] public int SuperkickLevel;
    public RareType rareType;
    public Sprite RarePhoto;
    [Space]
    private int[] _levelHPValues = new int[11];
    private float[] SpeedValues_Lvl = new float[10];
    [Space]
    public string Name;
    public Sprite Photo;
    [Space]
    public int HP;
    public int CurrentHP;
    public float Speed;
    private int Damage;
    private int[] _DamageLevel = new int[11];
    public int DamageMin;
    public int DamageMax;
    public int LevelUp_count = 0;
    [Space]
    public int TotalLevel;
    [Space]
    [Space]
    private int maxLevelUpCount = 5;
    [Range(1, 10)]
    public int Default_DamageLevel;
    [Range(1, 10)]
    public int Default_SpeedLevel;
    [Range(1, 10)]
    public int Default_HPLevel;
    private int _level;
    public int Level;
    public int RemainingClicks => maxLevelUpCount - LevelUp_count;
    public bool Rest;
    private int prevHPLevel, prevDamageLevel, prevSpeedLevel;

    public TimerButtonScript recoveryTimer;  
    public TimerButtonIDs buttonID;
    public TextMeshProUGUI recoveryTimeText;


    public void SaveChickenData()
    {
        PlayerPrefs.SetInt($"ChickenData_{Name}_DamageLevel", DamageLevel);
        PlayerPrefs.SetInt($"ChickenData_{Name}_SpeedLevel", SpeedLevel);
        PlayerPrefs.SetInt($"ChickenData_{Name}_HPLevel", HPLevel);
        PlayerPrefs.SetInt($"ChickenData_{Name}_SuperkickLevel", SuperkickLevel);
        PlayerPrefs.Save();
    }


    public int CalculateLevel()
    {
        _level = HPLevel + DamageLevel + SpeedLevel;
        return _level;
    }

    public enum RareType
    {
        Common,
        Rare,
        Legendary
    }

    public float GetCurrentSpeed()
    {
        return Speed;
    }

    private void InitializeLevelHPValues()
    {
        _levelHPValues[0] = 0; 
        _levelHPValues[1] = 8;
        _levelHPValues[2] = 15;
        _levelHPValues[3] = 33;
        _levelHPValues[4] = 140;
        _levelHPValues[5] = 165;
        _levelHPValues[6] = 230;
        _levelHPValues[7] = 365;
        _levelHPValues[8] = 430;
        _levelHPValues[9] = 460;
        _levelHPValues[10] = 540;
    }

    private void InitializeDamageLevelValues()
    {
        for (int i = 1; i <= 10; i++)
        {
            _DamageLevel[i] = 2 * i - 1;
        }
    }

    public void ApplyPrefabLevels()
    {
        SetLevel(HPLevel);
        SetDamageLevel(DamageLevel);
        UpdateDamageLevel();
    }

    public void UpdateHP()
    {
        HP = _levelHPValues[HPLevel];
        SaveChickenData();
    }

    public void UpdateDamageLevel()
    {
        DamageLevel = Mathf.Clamp(DamageLevel, 1, 10);
        Damage = _DamageLevel[DamageLevel];
        DamageMin = Mathf.Max(1, _DamageLevel[DamageLevel] - 2);
        DamageMax = _DamageLevel[DamageLevel] + 2;
        SaveChickenData();
    }

    public void SetLevel(int level)
    {
        HPLevel = level;
        UpdateHP();
    }

    public void SetDamageLevel(int level)
    {
        DamageLevel = level;
        UpdateDamageLevel();
    }

    public void SetInitialLevel(int initialLevel)
    {
        HPLevel = initialLevel;
        UpdateHP();
    }

    public void SetInitialDamageLevel(int initialLevel)
    {
        DamageLevel = initialLevel;
        UpdateDamageLevel();
    }

    public void SetSpeedLevel(int level)
    {
        SpeedLevel = Mathf.Clamp(level, 1, SpeedValues_Lvl.Length);
        UpdateSpeed();
        SaveChickenData();
    }
    public void CalculateAndSetLevel()
    {
        Level = CalculateLevel();
    }
    private void UpdateSpeed()
    {
        Speed = SpeedValues_Lvl[SpeedLevel - 1];
    }

    void Start()
    {

        prevHPLevel = HPLevel;
        prevDamageLevel = DamageLevel;
        prevSpeedLevel = SpeedLevel;

        SpeedValues_Lvl[0] = 2.8f;
        SpeedValues_Lvl[1] = 2.6f;
        SpeedValues_Lvl[2] = 2.4f;
        SpeedValues_Lvl[3] = 2.2f;
        SpeedValues_Lvl[4] = 2f;
        SpeedValues_Lvl[5] = 1.8f;
        SpeedValues_Lvl[6] = 1.6f;
        SpeedValues_Lvl[7] = 1.4f;
        SpeedValues_Lvl[8] = 1.2f;
        SpeedValues_Lvl[9] = 1f;

        TotalLevel = Level;
        InitializeLevelHPValues();
        InitializeDamageLevelValues();

        UpdateHP();
        UpdateDamageLevel();
        UpdateSpeed();
    }



    public void LevelUpdate()
    {
        TotalLevel = _level + 1;
    }
    public void UpdateStats()
    {
        if (HPLevel < 1) HPLevel = 1;
        if (DamageLevel < 1) DamageLevel = 1;
        if (SpeedLevel < 1) SpeedLevel = 1;

        UpdateHP();
        UpdateDamageLevel();
        UpdateSpeed();
    }

    private void OnValidate()
    {
        UpdateStats();
    }

    void Update()
    {
   
        if (prevHPLevel != HPLevel || prevDamageLevel != DamageLevel || prevSpeedLevel != SpeedLevel)
        {
            UpdateStats();
            prevHPLevel = HPLevel;
            prevDamageLevel = DamageLevel;
            prevSpeedLevel = SpeedLevel;
        }
    }

    public void ResetChickenLevel()
    {
        DamageLevel = Default_DamageLevel;
        SpeedLevel = Default_SpeedLevel;
        HPLevel = Default_HPLevel;
    }

}
