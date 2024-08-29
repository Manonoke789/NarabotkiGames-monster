using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayMaker;
using HutongGames.PlayMaker;

public class SuperkickSlider : MonoBehaviour
{
    public Slider existingSlider;
    public Sprite[] segmentSprites;
    public float segmentSpacing = 10f;

    private List<Image> segments = new List<Image>();

    private void Start()
    {
        RectTransform sliderRectTransform = existingSlider.GetComponent<RectTransform>();

        int maxValue = (int)existingSlider.maxValue;
        float segmentWidth = (sliderRectTransform.rect.width - (maxValue - 1) * segmentSpacing) / maxValue;

        for (int i = 0; i < maxValue; i++)
        {
            GameObject segmentObject = new GameObject($"Segment {i}");
            segmentObject.transform.SetParent(sliderRectTransform, false);

            Image segmentImage = segmentObject.AddComponent<Image>();
            segmentImage.sprite = segmentSprites[i];

            RectTransform segmentRectTransform = segmentImage.rectTransform;
            segmentRectTransform.anchorMin = new Vector2((float)i / maxValue, 0f);
            segmentRectTransform.anchorMax = new Vector2((float)(i + 1) / maxValue, 1f);
            segmentRectTransform.offsetMin = new Vector2(i == 0 ? 0f : segmentSpacing / 2f, 0f);
            segmentRectTransform.offsetMax = new Vector2(i == maxValue - 1 ? 0f : -segmentSpacing / 2f, 0f);

            segments.Add(segmentImage);
        }

        ChickenStats currentChicken = GetCurrentChicken();
        if (currentChicken != null)
        {
            UpdateSlider(currentChicken);
        }
    }

    private ChickenStats GetCurrentChicken()
    {
        FsmObject barracksCurrentChicken = FsmVariables.GlobalVariables.FindFsmObject("barracks_current_chicken");
        if (barracksCurrentChicken != null && barracksCurrentChicken.Value is ChickenStats)
        {
            return (ChickenStats)barracksCurrentChicken.Value;
        }
        return null;
    }

    public void UpdateSlider(ChickenStats stats)
    {
        existingSlider.value = stats.SuperkickLevel;
        UpdateSegmentSprites(stats.SuperkickLevel);
    }

    private void UpdateSegmentSprites(int selectedSegment)
    {
        for (int i = 0; i < segments.Count; i++)
        {
            Image segmentImage = segments[i];
            if (i < selectedSegment)
            {
                segmentImage.sprite = segmentSprites[i];
                segmentImage.enabled = true;
            }
            else
            {
                segmentImage.enabled = false;
            }
        }
    }
}