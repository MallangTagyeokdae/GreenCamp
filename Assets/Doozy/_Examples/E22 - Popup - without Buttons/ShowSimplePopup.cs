using Doozy.Runtime.UIManager.Containers;
using UnityEngine;

namespace Doozy._Examples.E22___Popup___without_Buttons
{
    public class ShowSimplePopup : MonoBehaviour
    {
        [Header("Prefab Name")]
        public string PopupName = "SimplePopup";

        [Header("Labels")]
        public string Title = "My Title";
        public string Message = "My Message";

        public void Show()
        {
            UIPopup
                .Get(PopupName)           //get the popup with the given name
                .SetTexts(Title, Message) //set the title and message texts
                .Show();                  //show the popup
        }
    }
}
