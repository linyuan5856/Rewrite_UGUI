using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

namespace ReWriteUGUI
{
    public class NewToggleGroup : UIBehaviour
    {
        [SerializeField] private bool m_AllowSwitchOff = false;

        public bool allowSwitchOff
        {
            get { return m_AllowSwitchOff; }
            set { m_AllowSwitchOff = value; }
        }

        private List<NewToggle> m_toggles = new List<NewToggle>();

        protected NewToggleGroup()
        {
        }

        private void ValidateToggles(NewToggle t)
        {
            if (t == null || !m_toggles.Contains(t))
                throw new ArgumentException(string.Format("{0} is not the part of toggleGroup{1}",
                    new object[] {t, this}));
        }

        public void NotifyToggleOn(NewToggle toggle)
        {
            ValidateToggles(toggle);

            for (int i = 0; i < m_toggles.Count; i++)
            {
                if (m_toggles[i] == toggle)
                    continue;
                m_toggles[i].isOn = false;
            }
        }

        public void UnregisterToggle(NewToggle toggle)
        {
            if (m_toggles.Contains(toggle))
                m_toggles.Remove(toggle);
        }

        public void RegisterToggle(NewToggle toggle)
        {
            if (!m_toggles.Contains(toggle))
                m_toggles.Add(toggle);
        }

        public bool AnyToggleIsOn()
        {
            return m_toggles.Find(x => x.isOn);
        }

        public void SetAllTogglesOff()
        {
            bool oldAllowSwitchOff = m_AllowSwitchOff;
            m_AllowSwitchOff = true;

            for (int i = 0; i < m_toggles.Count; i++)
                m_toggles[i].isOn = false;

            m_AllowSwitchOff = oldAllowSwitchOff;
        }
    }
}