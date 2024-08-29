using UnityEngine;
using UnityEngine.UI;

public class SliderImageSwapper : MonoBehaviour
{
    public Slider targetSlider;
    public Sprite atlasSprite;

    private Image fillImage;
    private Texture2D atlasTexture;

    void Start()
    {
        fillImage = GetComponent<Image>();
        fillImage.sprite = atlasSprite;
        atlasTexture = atlasSprite.texture;
        targetSlider.onValueChanged.AddListener(OnSliderValueChanged);
        UpdateFillImage();
    }

    private void OnSliderValueChanged(float value)
    {
        UpdateFillImage();
    }

    public void UpdateFillImage()
    {
        int imageIndex = Mathf.FloorToInt(targetSlider.value * 9);
        imageIndex = Mathf.Clamp(imageIndex, 0, 9);

        float uvWidth = 1.0f / 10.0f;
        float uvX = imageIndex * uvWidth;

        Rect uvRect = new Rect(uvX, 0, uvWidth, 1);
        Vector4 uvData = new Vector4(uvRect.x, uvRect.y, uvRect.width, uvRect.height);
        fillImage.overrideSprite = Sprite.Create(atlasTexture, new Rect(0, 0, atlasTexture.width, atlasTexture.height), Vector2.zero, 100, 1, SpriteMeshType.FullRect, uvData);
    }
}