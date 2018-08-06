using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ReWriteUGUI
{
    public class NewButton : UIBehaviour, ISubmitHandler, IPointerClickHandler
    {
        protected NewButton()
        {
        }

        [Serializable]
        public class BtnOnClickEvent : UnityEvent
        {
        }


        [SerializeField] private BtnOnClickEvent m_onClick = new BtnOnClickEvent();


        public BtnOnClickEvent onClick
        {
            get { return m_onClick; }
            set { m_onClick = value; }
        }


        private void OnPress()
        {
            if (!IsActive())
                return;

            m_onClick.Invoke();
        }


        public void OnSubmit(BaseEventData eventData)
        {
            OnPress();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                return;

            OnPress();
        }
    }
}