using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElement : MonoBehaviour
{
    public List<UIButton> uiElements;
    public List<UIButton> uiLockElements;
    public List<Sprite> uiImages;
    public List<Image> groupImages;
    public TMP_Text name;
    public TMP_Text level;
    public TMP_Text currentHealth; 
    public TMP_Text maxHealth; 
    public TMP_Text elementInfo;
    public TMP_Text armor;
    public TMP_Text levelUpCost;
    public TMP_Text spare1;
    public TMP_Text spare2;
    public TMP_Text spare3;
    public Slider progressBar;
    public Image image;
}
