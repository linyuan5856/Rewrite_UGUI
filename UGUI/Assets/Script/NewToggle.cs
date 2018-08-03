using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine;

namespace ReWriteUGUI
{
    public class NewToggle : UIBehaviour, IPointerClickHandler, ISubmitHandler
    {
        [Serializable]
        public class LYToggleEvent : UnityEngine.Events.UnityEvent<bool>
        {
        }

        public LYToggleEvent onValueChanged = new LYToggleEvent();

        public Graphic graphic;

        [FormerlySerializedAs("m_IsActive")] [Tooltip("Is the toggle currently on or off?")] [SerializeField]
        private bool m_IsOn;

        public bool isOn
        {
            get { return m_IsOn; }
            set { Set(value); }
        }

        [SerializeField] private NewToggleGroup m_Group;

        public NewToggleGroup group
        {
            get { return m_Group; }
            set
            {
                m_Group = value;
                PlayEffect(true);
            }
        }

        private void SetToggleGroup(NewToggleGroup newGroup, bool setMemberValue)
        {
            NewToggleGroup old = this.m_Group;

            if (m_Group != null)
                m_Group.UnregisterToggle(this);

            if (setMemberValue)
                m_Group = newGroup;

            if (newGroup != null && IsActive())
                newGroup.RegisterToggle(this);

            if (newGroup != null && newGroup != old && isOn && IsActive())
                newGroup.NotifyToggleOn(this);
        }

        void Set(bool value)
        {
            Set(value, true);
        }


        void Set(bool value, bool sendCallBack)
        {
            if (m_IsOn == value)
                return;

            m_IsOn = value;

            if (m_Group != null && IsActive())
            {
                if (m_IsOn || (!m_Group.AnyToggleIsOn() && !m_Group.allowSwitchOff))
                {
                    m_IsOn = true;
                    m_Group.NotifyToggleOn(this);
                }
            }

            PlayEffect(true);
            if (sendCallBack)
                onValueChanged.Invoke(m_IsOn);
        }

        void PlayEffect(bool instant)
        {
            if (graphic == null)
                return;
            graphic.CrossFadeAlpha(m_IsOn ? 1.0f : 0.0f, instant ? 0.0f : 0.1f, true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetToggleGroup(m_Group, false);
            PlayEffect(true);
        }

        protected override void OnDisable()
        {
            SetToggleGroup(null, false);
            base.OnDisable();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            InternalToggle();
        }


        public virtual void OnSubmit(BaseEventData eventData)
        {
            InternalToggle();
        }

        private void InternalToggle()
        {
            if (!IsActive())
                return;

            isOn = !isOn;
        }
    }
}