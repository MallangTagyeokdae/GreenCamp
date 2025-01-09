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
    public List<Image> uiImages;
    public TMP_Text name;
    public TMP_Text level;
    public TMP_Text health; 
    public Slider progressBar;
}
