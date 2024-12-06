using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core
{
    public class ButtonEx : Button
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            AppController.Instance.ButtonEx_OnClick();
        }
    }
}
