using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JaugeSegment : MonoBehaviour
{
    private LayoutElement layout;
    public RectTransform rect;
    public Image iconPrefab;
    public Transform iconsParent;
    // public Image icon;
    public Text title;
    public Image jauge;
    public int heightFactor = 1000;

    public void InitializeSegment(float _height, Sprite _icon, int iconsAmount, string _title, Color _color, float _iconSizeModifier)
    {
        layout = GetComponent<LayoutElement>();
        layout.preferredHeight = _height * heightFactor;
        for (var i = 0; i < iconsAmount; i++)
        {
            Image spawnedImage = Instantiate(iconPrefab, iconsParent);
            spawnedImage.sprite = _icon;
            spawnedImage.transform.localScale = Vector3.one * _iconSizeModifier;
        }
        title.text = _title;
        jauge.color = _color;
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
