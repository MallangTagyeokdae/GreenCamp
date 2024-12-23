using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy._Examples.E23___Popup___with_One_Button
{
    public class ShowOneButtonPopup : MonoBehaviour
    {
        [Header("Prefab Name")]
        public string PopupName = "OneButtonPopup";

        [Header("Labels")]
        public string Title = "My Title";
        public string Message = "My Message";

        [Space(5)]
        public string ButtonLabel = "Ok";
        public UnityEvent OnClick = new UnityEvent();

        public void Show()
        {
            UIPopup
                .Get(PopupName)                        //get the popup with the given name
                .SetTexts(Title, Message, ButtonLabel) //set the title and message texts
                .SetEvents(OnClick)                    //set the button event
                .Show();                               //show the popup
        }
    }
}
