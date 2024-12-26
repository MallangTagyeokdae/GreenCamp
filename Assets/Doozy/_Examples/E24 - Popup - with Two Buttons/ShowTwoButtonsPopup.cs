using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.Events;
namespace Doozy._Examples.E24___Popup___with_Two_Buttons
{
    public class ShowTwoButtonsPopup : MonoBehaviour
    {
        [Header("Prefab Name")]
        public string PopupName = "TwoButtonsPopup";

        [Header("Labels")]
        public string Title = "My Title";
        public string Message = "My Message";

        [Space(5)]
        public string LeftButtonLabel = "Ok";
        public UnityEvent OnClickLeftButton = new UnityEvent();

        [Space(5)]
        public string RightButtonLabel = "Cancel";
        public UnityEvent OnClickRightButton = new UnityEvent();

        public void Show()
        {
            UIPopup
                .Get(PopupName)                                              //get the popup with the given name
                .SetTexts(Title, Message, LeftButtonLabel, RightButtonLabel) //set the texts for the popup
                .SetEvents(OnClickLeftButton, OnClickRightButton)            //set the events for the popup
                .Show();                                                     //show the popup
        }
    }
}
