using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using TMPro;
using Gley.DailyRewards.Internal;
using Gley.DailyRewards;
using System;
using System.Collections;
public class Barracks : MonoBehaviour
{
    private Dictionary<ChickenStats, TimerButtonScript> timerScripts = new Dictionary<ChickenStats, TimerButtonScript>();

    private List<Sprite> chickenPhotos = new List<Sprite>();
    private List<ChickenStats> chickenStats = new List<ChickenStats>();
    public ScrollRect scrollView;
    private Sprite commonSprite;
    private Sprite rareSprite;
    private Sprite legendarySprite;
    public Button allButton;
    public Button commonButton;
    public Button rareButton;
    public Button legendaryButton;
    public Button AddDamage_btn;
    public Button AddSpeed_btn;
    public Button AddHP_btn;
    public Button Superkick_btn;
    public Button ResetButton;
    public Image Photo;
    public Sprite timer_mask;
    public Sprite selected_chickenSprite;
    public Image RarePhoto;
    public TextMeshProUGUI Name;
    public Slider DamageSlider;
    public Slider SpeedSlider;
    public Slider HPSlider;
    public SuperkickSlider superkickSlider;
    public TextMeshProUGUI chickenLevel;
    private const string DamageLevelKey = "DamageLevel";
    private const string SpeedLevelKey = "SpeedLevel";
    private const string HPLevelKey = "HPLevel";
    private const string SuperkickLevelKey = "SuperkickLevel";
    private const string LevelUpCountKey = "LevelUp_count";
    private const string TotalLevelKey = "TotalLevel";
    private const int MaxTotalLevel = 30;
    private Dictionary<ChickenStats, GameObject> chickenObjects = new Dictionary<ChickenStats, GameObject>();
    private bool levelsChangedSinceLastReset = false;
    private ChickenStats currentChicken;
    private bool firstObjectButtonsInitialized = false;
    public Sprite spritePeso;
    public Sprite spriteGems;
    public TextMeshProUGUI timer_left;
    public TextMeshProUGUI damageUpgradeCostText;
    public TextMeshProUGUI speedUpgradeCostText;
    public TextMeshProUGUI hpUpgradeCostText;
    public TextMeshProUGUI SuperKickUpgradeCostText;
    private FsmGameObject selectedChickenFsmVariable;
    public int[] PesoUpgradeCosts = new int[11];
    public int[] GemsUpgradeCosts = new int[11];
    public int[] SuperkickUpgradeCosts = new int[] { 0, 50, 150 };

    void Start()
    {
        FindChickenPhotos();
        CreateAllImages();
        SetupUIEventListeners();
        UpdateAllButtonsInteractability();
        UpdateUpgradeCostTexts();
        SetupSliders();

        InitializeTimersDirectly();
    }

    void InitializeTimersDirectly()
    {

        if (TimerButtonManager.Instance != null)
        {
            InitializeTimers();
        }
        else
        {
            Debug.LogError("TimerButtonManager is not initialized properly.");
        }
    }

    void SetupUIEventListeners()
    {
        Superkick_btn.onClick.AddListener(UpdateSuperkickSlider);
        ResetButton.onClick.AddListener(ResetChickenLevel);
        allButton.onClick.AddListener(CreateAllImages);
        commonButton.onClick.AddListener(CreateCommonImages);
        rareButton.onClick.AddListener(CreateRareImages);
        legendaryButton.onClick.AddListener(CreateLegendaryImages);
    }

    void SetupSliders()
    {
        DamageSlider.onValueChanged.AddListener(delegate { OnLevelChanged(); });
        SpeedSlider.onValueChanged.AddListener(delegate { OnLevelChanged(); });
        HPSlider.onValueChanged.AddListener(delegate { OnLevelChanged(); });
    }

    private void UpdateUpgradeCostTexts()
    {
        if (currentChicken == null) return;
        int nextDamageLevel = currentChicken.DamageLevel + 1;
        int nextSpeedLevel = currentChicken.SpeedLevel + 1;
        int nextHPLevel = currentChicken.HPLevel + 1;
        int nextSuperkickLevel = currentChicken.SuperkickLevel + 1;

        damageUpgradeCostText.text = currentChicken.DamageLevel >= 10 ? "Max" :
            (new[] { 2, 3, 5, 7, 9 }.Contains(nextDamageLevel) && nextDamageLevel <= PesoUpgradeCosts.Length) ?
                $"{PesoUpgradeCosts[nextDamageLevel - 1]}" :
                (nextDamageLevel <= GemsUpgradeCosts.Length ? $"{GemsUpgradeCosts[nextDamageLevel - 1]}" : "Max");

        speedUpgradeCostText.text = currentChicken.SpeedLevel >= 10 ? "Max" :
            (nextSpeedLevel <= 3 && nextSpeedLevel <= PesoUpgradeCosts.Length) ?
                $"{PesoUpgradeCosts[nextSpeedLevel - 1]}" :
                (nextSpeedLevel <= GemsUpgradeCosts.Length ? $"{GemsUpgradeCosts[nextSpeedLevel - 1]}" : "Max");

        hpUpgradeCostText.text = currentChicken.HPLevel >= 10 ? "Max" :
            (new[] { 2, 3, 4, 5, 7, 9 }.Contains(nextHPLevel) && nextHPLevel <= PesoUpgradeCosts.Length) ?
                $"{PesoUpgradeCosts[nextHPLevel - 1]}" :
                (nextHPLevel <= GemsUpgradeCosts.Length ? $"{GemsUpgradeCosts[nextHPLevel - 1]}" : "Max");
   
        if (currentChicken.SuperkickLevel == 1)
        {
            SuperKickUpgradeCostText.text = $"{SuperkickUpgradeCosts[1]}";
        }
        else if (currentChicken.SuperkickLevel == 2)
        {
            SuperKickUpgradeCostText.text = $"{SuperkickUpgradeCosts[2]}";
        }
        else if (currentChicken.SuperkickLevel == 3)
        {
            SuperKickUpgradeCostText.text = "Max";
        }
    }

    private void OnLevelChanged()
    {
        levelsChangedSinceLastReset = true;
        if (!ResetButton.interactable)
            ResetButton.interactable = true;
    }

    public void UpdateChickenLevel(ChickenStats stats)
    {
        stats.CalculateAndSetLevel();
        chickenLevel.text = stats.Level.ToString();
    }

    void UpdateTimerUI(ChickenStats chicken)
    {
        TimerButtonIDs buttonID = chickenTimers[chicken.Name];
        TimeSpan totalTime = TimerButtonManager.Instance.GetTimeToPass(buttonID);
        TimeSpan remainingTime = TimerButtonManager.Instance.GetRemainingTimeSpan(buttonID);

        timer_left.text = FormatTime(remainingTime);
        if (timerMasks.TryGetValue(chicken, out GameObject timerMaskObject))
        {
            Image timerMaskImage = timerMaskObject.GetComponent<Image>();
            if (timerMaskImage != null)
            {
                float fillAmount = (float)(remainingTime.TotalSeconds / totalTime.TotalSeconds);
                timerMaskImage.fillAmount = fillAmount;
                timerMaskImage.gameObject.SetActive(remainingTime.TotalSeconds > 0);
            }
        }
    }

    private string FormatTime(TimeSpan time)
    {
        return $"/ recuperation: {time.Minutes:D2} m {time.Seconds:D2} s left";
    }


    void Update()
    {
        if (!firstObjectButtonsInitialized && chickenStats.Count > 0)
        {
            InitializeFirstObjectButtonsInteractability();
            firstObjectButtonsInitialized = true;
        }
        foreach (var chicken in chickenStats)
        {
            UpdateTimerUI(chicken);

        }

    }


    private void InitializeFirstObjectButtonsInteractability()
    {
        currentChicken = chickenStats[0];
        if (currentChicken != null)
        {
            LoadChickenData(currentChicken);
            UpdateButtonsInteractability(currentChicken);
        }
    }

    private void UpdateAllButtonsInteractability()
    {
        foreach (ChickenStats stats in chickenStats)
        {
            LoadChickenData(stats);
            UpdateButtonsInteractability(stats);
        }
    }

    private Dictionary<string, TimerButtonIDs> chickenTimers = new Dictionary<string, TimerButtonIDs>();


    private TimerButtonIDs GetButtonIDForCategory(ChickenStats.RareType rareType)
    {
        switch (rareType)
        {
            case ChickenStats.RareType.Common:
                return TimerButtonIDs.CommonRecoveryTime;
            case ChickenStats.RareType.Rare:
                return TimerButtonIDs.RareRecoveryTime;
            case ChickenStats.RareType.Legendary:
                return TimerButtonIDs.LegendaryRecoveryTime;
            default:
                throw new ArgumentOutOfRangeException(nameof(rareType), rareType, null);
        }
    }

    void FindChickenPhotos()
    {
        GameObject[] playerPrefabs = Resources.LoadAll<GameObject>("Player");
        playerPrefabs = playerPrefabs.Where(prefab => prefab.tag == "player").ToArray();
        foreach (GameObject prefab in playerPrefabs)
        {
            ChickenStats stats = prefab.GetComponent<ChickenStats>();
            if (stats != null)
            {
                chickenPhotos.Add(stats.Photo);
                chickenStats.Add(stats);
                LoadChickenData(stats);
                if (stats.rareType == ChickenStats.RareType.Common)
                    commonSprite = stats.Photo;
                else if (stats.rareType == ChickenStats.RareType.Rare)
                    rareSprite = stats.Photo;
                else
                    legendarySprite = stats.Photo;
            }
        }
        UpdateButtonsInteractability(currentChicken);
    }

    private void LoadChickenData(ChickenStats stats)
    {
        stats.DamageLevel = PlayerPrefs.GetInt($"ChickenData_{stats.Name}_{DamageLevelKey}", stats.DamageLevel);
        stats.SpeedLevel = PlayerPrefs.GetInt($"ChickenData_{stats.Name}_{SpeedLevelKey}", stats.SpeedLevel);
        stats.HPLevel = PlayerPrefs.GetInt($"ChickenData_{stats.Name}_{HPLevelKey}", stats.HPLevel);
        stats.SuperkickLevel = PlayerPrefs.GetInt($"ChickenData_{stats.Name}_{SuperkickLevelKey}", stats.SuperkickLevel);
    }

    public enum RareType
    {
        Common = 0,
        Rare = 1,
        Legendary = 2
    }

    public void UpdateCategoryButtons()
    {

        bool hasCommon = chickenStats.Any(chicken => chicken.rareType == ChickenStats.RareType.Common);
        bool hasRare = chickenStats.Any(chicken => chicken.rareType == ChickenStats.RareType.Rare);
        bool hasLegendary = chickenStats.Any(chicken => chicken.rareType == ChickenStats.RareType.Legendary);


        commonButton.interactable = hasCommon;
        rareButton.interactable = hasRare;
        legendaryButton.interactable = hasLegendary;
    }

    public void CreateAllImages()
    {
        ClearContent();
        var sortedStats = chickenStats
            .Select((stats, index) => new { Stats = stats, Index = index })
            .OrderBy(x => x.Stats.rareType)
            .ToList();

        var sortedPhotos = sortedStats.Select(x => chickenPhotos[x.Index]).ToList();
        var finalSortedStats = sortedStats.Select(x => x.Stats).ToList();

        CreateImageObjects(sortedPhotos, finalSortedStats);
        UpdateCategoryButtons();

        if (finalSortedStats.Count > 0)
            ButtonClicked(finalSortedStats[0]);
    }


    public void CreateCommonImages()
    {
        ClearContent();
        var filteredChickens = chickenStats.Where(stats => stats.rareType == ChickenStats.RareType.Common).ToList();
        var commonPhotos = filteredChickens.Select(stats => stats.Photo).ToList();
        CreateImageObjects(commonPhotos, filteredChickens);
        UpdateCategoryButtons();

        if (filteredChickens.Count > 0)
            ButtonClicked(filteredChickens[0]);
        else
            commonButton.interactable = false;
    }

    public void CreateRareImages()
    {
        ClearContent();
        var filteredChickens = chickenStats.Where(stats => stats.rareType == ChickenStats.RareType.Rare).ToList();
        var rarePhotos = filteredChickens.Select(stats => stats.Photo).ToList();
        CreateImageObjects(rarePhotos, filteredChickens);
        UpdateCategoryButtons();

        if (filteredChickens.Count > 0)
            ButtonClicked(filteredChickens[0]);
        else
            rareButton.interactable = false;
    }

    public void CreateLegendaryImages()
    {
        ClearContent();
        var filteredChickens = chickenStats.Where(stats => stats.rareType == ChickenStats.RareType.Legendary).ToList();
        var legendaryPhotos = filteredChickens.Select(stats => stats.Photo).ToList();
        CreateImageObjects(legendaryPhotos, filteredChickens);
        UpdateCategoryButtons();

        if (filteredChickens.Count > 0)
            ButtonClicked(filteredChickens[0]);
        else
            legendaryButton.interactable = false;
    }

    private Dictionary<ChickenStats, GameObject> selectionMasks = new Dictionary<ChickenStats, GameObject>();
    private Dictionary<ChickenStats, GameObject> timerMasks = new Dictionary<ChickenStats, GameObject>();

    void CreateImageObjects(List<Sprite> sprites, List<ChickenStats> stats)
    {
        chickenObjects.Clear();
        timerScripts.Clear();
        selectionMasks.Clear();
        timerMasks.Clear();

        for (int i = 0; i < sprites.Count; i++)
        {
            Sprite sprite = sprites[i];
            ChickenStats prefabStats = stats[i];

            GameObject imageObject = new GameObject(prefabStats.Name);
            imageObject.transform.SetParent(scrollView.content.transform, false);

            Image image = imageObject.AddComponent<Image>();
            image.sprite = sprite;
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);

            Button button = imageObject.AddComponent<Button>();
            button.onClick.AddListener(() => ButtonClicked(prefabStats));

            TextMeshProUGUI textMeshProUGUI = AddTextComponent(imageObject);
            textMeshProUGUI.text = "Initializing...";

            TimerButtonIDs buttonID = GetButtonIDForCategory(prefabStats.rareType);
            TimerButtonScript timerScript = imageObject.GetComponent<TimerButtonScript>() ?? imageObject.AddComponent<TimerButtonScript>();
            timerScript.buttonID = buttonID;
            timerScript.buttonScript = button;
            timerScript.buttonText = textMeshProUGUI;
            timerScripts[prefabStats] = timerScript;
            chickenTimers[prefabStats.Name] = buttonID;

            GameObject maskObject = new GameObject("Mask");
            maskObject.transform.SetParent(imageObject.transform, false);
            Image maskImage = maskObject.AddComponent<Image>();
            maskImage.sprite = selected_chickenSprite;
            maskImage.rectTransform.sizeDelta = new Vector2(170, 170);
            maskImage.gameObject.SetActive(false);
            selectionMasks[prefabStats] = maskObject;

            GameObject timerMaskObject = new GameObject("TimerMask");
            timerMaskObject.transform.SetParent(imageObject.transform, false);
            Image timerMaskImage = timerMaskObject.AddComponent<Image>();
            timerMaskImage.sprite = timer_mask;
            timerMaskImage.type = Image.Type.Filled;
            timerMaskImage.fillMethod = Image.FillMethod.Radial360;
            timerMaskImage.fillClockwise = false;
            timerMaskImage.fillOrigin = (int)Image.Origin360.Top;
            timerMaskImage.rectTransform.sizeDelta = new Vector2(150, 150);
            timerMaskImage.fillAmount = 1.0f;  
            timerMaskImage.gameObject.SetActive(true); 

            timerMasks[prefabStats] = timerMaskObject;
        }
    }


    void ButtonClicked(ChickenStats stats)
    {
        currentChicken = stats;
        LoadChickenData(currentChicken);
        FsmObject barracksCurrentChicken = FsmVariables.GlobalVariables.FindFsmObject("barracks_current_chicken");
        barracksCurrentChicken.Value = currentChicken;
        FsmGameObject selectedChickenFsm = FsmVariables.GlobalVariables.FindFsmGameObject("selected_chicken");
        selectedChickenFsm.Value = currentChicken.gameObject;
        Photo.sprite = currentChicken.Photo;
        Name.text = currentChicken.Name;
        RarePhoto.sprite = currentChicken.RarePhoto;
        DamageSlider.value = currentChicken.DamageLevel;
        SpeedSlider.value = currentChicken.SpeedLevel;
        HPSlider.value = currentChicken.HPLevel;
        UpdateSliders(currentChicken);
        UpdateSuperkickSlider();
        UpdateButtonsInteractability(currentChicken);
        UpdateChickenLevel(stats);
        UpdateUpgradeCostTexts();
        foreach (var entry in selectionMasks)
        {
            entry.Value.SetActive(entry.Key == currentChicken);
        }
          StartCoroutine(UpdateTimerUIWithDelay(currentChicken, 0.5f));
    }

    private IEnumerator UpdateTimerUIWithDelay(ChickenStats chicken, float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateTimerUI(chicken);
    }




    private TextMeshProUGUI AddTextComponent(GameObject parent)
    {
        GameObject textObject = new GameObject("TimerText");
        textObject.transform.SetParent(parent.transform, false);
        var textComponent = textObject.AddComponent<TextMeshProUGUI>();
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.fontSize = 30;
        timer_left.text = textComponent.text;
        return textComponent;

    }



    private void UpdateButtonsInteractability(ChickenStats currentChicken)
    {
        if (currentChicken != null)
        {
            int currentPeso = PlayerPrefs.GetInt("peso", 0);
            int currentGems = PlayerPrefs.GetInt("gems", 0);
            bool canLevelUp = currentChicken.LevelUp_count < 5;
            int nextDamageLevel = currentChicken.DamageLevel + 1;
            int nextSpeedLevel = currentChicken.SpeedLevel + 1;
            int nextHPLevel = currentChicken.HPLevel + 1;
            int nextSuperkickLevel = currentChicken.SuperkickLevel + 1;
            bool canUpgradeDamage = canLevelUp && nextDamageLevel <= 10 &&
                ((new[] { 2, 3, 5, 7, 9 }.Contains(nextDamageLevel) && currentPeso >= PesoUpgradeCosts[nextDamageLevel - 1]) ||
                (!new[] { 2, 3, 5, 7, 9 }.Contains(nextDamageLevel) && currentGems >= GemsUpgradeCosts[nextDamageLevel - 1]));
            bool canUpgradeSpeed = canLevelUp && nextSpeedLevel <= 10 &&
                (nextSpeedLevel <= 3 ? currentPeso >= PesoUpgradeCosts[nextSpeedLevel - 1] : currentGems >= GemsUpgradeCosts[nextSpeedLevel - 1]);
            bool canUpgradeHP = canLevelUp && nextHPLevel <= 10 &&
                (new[] { 2, 3, 4, 5, 7, 9 }.Contains(nextHPLevel) ? currentPeso >= PesoUpgradeCosts[nextHPLevel - 1] : currentGems >= GemsUpgradeCosts[nextHPLevel - 1]);
            bool canUpgradeSuperkick = canLevelUp && nextSuperkickLevel <= 3 &&
                                       currentGems >= SuperkickUpgradeCosts[nextSuperkickLevel - 1];
            AddDamage_btn.interactable = canUpgradeDamage;
            AddSpeed_btn.interactable = canUpgradeSpeed;
            AddHP_btn.interactable = canUpgradeHP;
            Superkick_btn.interactable = canUpgradeSuperkick;
            ResetButton.interactable = currentGems >= 300 && currentChicken.LevelUp_count > 0;
            AddDamage_btn.image.sprite = (new[] { 2, 3, 5, 7, 9 }.Contains(nextDamageLevel)) ? spritePeso : spriteGems;
            AddSpeed_btn.image.sprite = (nextSpeedLevel <= 3) ? spritePeso : spriteGems;
            AddHP_btn.image.sprite = (new[] { 2, 3, 4, 5, 7, 9 }.Contains(nextHPLevel)) ? spritePeso : spriteGems;
        }
        else
        {
            AddDamage_btn.interactable = false;
            AddSpeed_btn.interactable = false;
            AddHP_btn.interactable = false;
            Superkick_btn.interactable = false;
            ResetButton.interactable = false;
        }
    }

    void ClearContent()
    {
        foreach (Transform child in scrollView.content.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void IncreaseAttribute(ref int attribute, Slider slider, string key)
    {
        if (currentChicken != null && attribute < 10 && currentChicken.LevelUp_count < 5)
        {
            attribute++;
            slider.value = attribute;
            PlayerPrefs.SetInt($"ChickenData_{currentChicken.Name}_{key}", attribute);
            currentChicken.LevelUp_count++;
            PlayerPrefs.SetInt($"ChickenData_{currentChicken.Name}_{LevelUpCountKey}", currentChicken.LevelUp_count);
            if (currentChicken.LevelUp_count == 5)
            {
                DisableUpgradeButtons();
            }
            UpdateButtonsInteractability(currentChicken);
        }
    }

    private void UpdatePlayMakerGlobalVariables()
    {
        var fsm = FsmVariables.GlobalVariables;
        fsm.FindFsmInt("Gems").Value = PlayerPrefs.GetInt("gems", 0);
        fsm.FindFsmInt("Peso").Value = PlayerPrefs.GetInt("peso", 0);
    }

    public void IncreaseDamage()
    {
        int nextDamageLevel = currentChicken.DamageLevel + 1;
        if (nextDamageLevel > 10)
        {
            AddDamage_btn.interactable = false;
            return;
        }
        int currentPeso = PlayerPrefs.GetInt("peso", 0);
        int currentGems = PlayerPrefs.GetInt("gems", 0);
        bool useGems = nextDamageLevel == 4 || nextDamageLevel == 6 || nextDamageLevel == 8 || nextDamageLevel == 10;
        int cost = useGems ? GemsUpgradeCosts[nextDamageLevel - 1] : PesoUpgradeCosts[nextDamageLevel - 1];
        bool canUpgradeDamage = useGems ? currentGems >= cost : currentPeso >= cost;
        if (canUpgradeDamage)
        {
            IncreaseAttribute(ref currentChicken.DamageLevel, DamageSlider, DamageLevelKey);
            PlayerPrefs.SetInt(useGems ? "gems" : "peso", (useGems ? currentGems : currentPeso) - cost);
            PlayerPrefs.Save();
            UpdatePlayMakerGlobalVariables();
            UpdateButtonsInteractability(currentChicken);
            levelsChangedSinceLastReset = true;
            UpdateUpgradeCostTexts();
            UpdateChickenLevel(currentChicken);
        }
    }

    public void IncreaseSpeed()
    {
        int nextSpeedLevel = currentChicken.SpeedLevel + 1;
        if (nextSpeedLevel > 10)
        {
            AddSpeed_btn.interactable = false;
            return;
        }
        int currentPeso = PlayerPrefs.GetInt("peso", 0);
        int currentGems = PlayerPrefs.GetInt("gems", 0);
        int cost = nextSpeedLevel <= 3 ? PesoUpgradeCosts[nextSpeedLevel - 1] : GemsUpgradeCosts[nextSpeedLevel - 1];
        bool canUpgradeSpeed = nextSpeedLevel <= 3 ? currentPeso >= cost : currentGems >= cost;
        if (canUpgradeSpeed)
        {
            IncreaseAttribute(ref currentChicken.SpeedLevel, SpeedSlider, SpeedLevelKey);
            AddSpeed_btn.image.sprite = nextSpeedLevel <= 3 ? spritePeso : spriteGems;
            if (nextSpeedLevel <= 3)
            {
                PlayerPrefs.SetInt("peso", currentPeso - cost);
            }
            else
            {
                PlayerPrefs.SetInt("gems", currentGems - cost);
            }
            PlayerPrefs.Save();
            UpdatePlayMakerGlobalVariables();
            UpdateButtonsInteractability(currentChicken);
            levelsChangedSinceLastReset = true;
            UpdateUpgradeCostTexts();
            UpdateChickenLevel(currentChicken);
        }
    }

    public void IncreaseHP()
    {
        int nextHPLevel = currentChicken.HPLevel + 1;
        if (nextHPLevel > 10)
        {
            AddHP_btn.interactable = false;
            return;
        }
        int currentPeso = PlayerPrefs.GetInt("peso", 0);
        int currentGems = PlayerPrefs.GetInt("gems", 0);
        bool usePeso = new[] { 2, 3, 4, 5, 7, 9 }.Contains(nextHPLevel);
        int cost = usePeso ? PesoUpgradeCosts[nextHPLevel - 1] : GemsUpgradeCosts[nextHPLevel - 1];
        bool canUpgradeHP = usePeso ? currentPeso >= cost : currentGems >= cost;
        if (canUpgradeHP)
        {
            IncreaseAttribute(ref currentChicken.HPLevel, HPSlider, HPLevelKey);
            PlayerPrefs.SetInt(usePeso ? "peso" : "gems", (usePeso ? currentPeso : currentGems) - cost);
            PlayerPrefs.Save();
            UpdatePlayMakerGlobalVariables();
            UpdateButtonsInteractability(currentChicken);
            levelsChangedSinceLastReset = true;
            UpdateUpgradeCostTexts();
            UpdateChickenLevel(currentChicken);
        }
    }

    public void IncreaseSuperkickLevel()
    {
        if (currentChicken != null && currentChicken.SuperkickLevel < 3)
        {
            int currentGems = PlayerPrefs.GetInt("gems", 0);
            int superkickUpgradeCost = currentChicken.SuperkickLevel == 1 ? 50 : 150;
            if (currentGems >= superkickUpgradeCost)
            {
                currentChicken.SuperkickLevel++;
                UpdateSuperkickSlider();
                PlayerPrefs.SetInt("gems", currentGems - superkickUpgradeCost);
                PlayerPrefs.SetInt($"ChickenData_{currentChicken.Name}_{SuperkickLevelKey}", currentChicken.SuperkickLevel);
                PlayerPrefs.Save();
                UpdatePlayMakerGlobalVariables();
                UpdateButtonsInteractability(currentChicken);
                UpdateUpgradeCostTexts();
            }
        }
    }

    private void UpdateSuperkickSlider()
    {
        if (currentChicken != null)
        {
            superkickSlider.UpdateSlider(currentChicken);
        }
    }


    private void InitializeTimers()
    {
        foreach (var chicken in chickenStats)
        {
            string chickenName = chicken.Name;
            TimerButtonIDs buttonID = chickenTimers[chickenName];
            var timerScript = timerScripts[chicken];
            if (chicken.Rest)
            {
                TimerButtonManager.Instance.Initialize(buttonID, (remainingTime, interactable, completeText) =>
                {
                    var timerScript = timerScripts[chicken];
                    if (timerScript.buttonText == null || timerScript.buttonScript == null)
                    timerScript.buttonScript.interactable = interactable;
                    timerScript.completeText = completeText;
                    timerScript.RewardButtonClicked();
                });
            }


            else
            {
                timerScript.buttonScript.interactable = true;
            }
        }
    }

    private void UpdateSliders(ChickenStats stats)
    {
        DamageSlider.value = stats.DamageLevel;
        SpeedSlider.value = stats.SpeedLevel;
        HPSlider.value = stats.HPLevel;
    }

    public void ResetChickenLevel()
    {
        if (currentChicken != null && PlayerPrefs.GetInt("gems", 0) >= 300)
        {
            int currentGems = PlayerPrefs.GetInt("gems", 0) - 300;
            PlayerPrefs.SetInt("gems", currentGems);
            PlayerPrefs.Save();
            var fsmGems = FsmVariables.GlobalVariables.FindFsmInt("Gems");
            fsmGems.Value = currentGems;
            currentChicken.ResetChickenLevel();
            currentChicken.LevelUp_count = 0;
            PlayerPrefs.SetInt($"ChickenData_{currentChicken.Name}_{LevelUpCountKey}", currentChicken.LevelUp_count);
            PlayerPrefs.SetInt($"ChickenData_{currentChicken.Name}_{DamageLevelKey}", currentChicken.DamageLevel);
            PlayerPrefs.SetInt($"ChickenData_{currentChicken.Name}_{SpeedLevelKey}", currentChicken.SpeedLevel);
            PlayerPrefs.SetInt($"ChickenData_{currentChicken.Name}_{HPLevelKey}", currentChicken.HPLevel);
            UpdateSliders(currentChicken);
            UpdateButtonsInteractability(currentChicken);
            UpdateChickenLevel(currentChicken);
            UpdateUpgradeCostTexts();
            ResetButton.interactable = false;
            levelsChangedSinceLastReset = false;
        }
        else
        {
            ResetButton.interactable = false;
        }
    }

    private void EnableResetButton()
    {
        if (!ResetButton.interactable && levelsChangedSinceLastReset)
            ResetButton.interactable = true;
    }
    private void DisableUpgradeButtons()
    {
        AddDamage_btn.interactable = false;
        AddSpeed_btn.interactable = false;
        AddHP_btn.interactable = false;
    }

    private void EnableUpgradeButtons()
    {
        AddDamage_btn.interactable = true;
        AddSpeed_btn.interactable = true;
        AddHP_btn.interactable = true;
    }
}