using System;

namespace PushingBoxStudios
{
    public class ModifiableAttribute
    {
        private float _baseValue;
        private float _multiplier;

        public event EventHandler ValueChanged;

        public float BaseValue 
        {
            get { return _baseValue; }
            
            set
            {
                _baseValue = value;
                OnValueChanged();
            }
        }

        public float Multiplier 
        {
            get { return _multiplier; }
            
            set
            {
                _multiplier = value;
                OnValueChanged();
            }
        }

        public float FinalValue
        {
            get { return BaseValue * Multiplier; }
        }

        public ModifiableAttribute(float baseValue)
        {
            BaseValue = baseValue;
            Multiplier = 1;
        }

        public ModifiableAttribute(float baseValue, float multiplier)
        {
            BaseValue = baseValue;
            Multiplier = multiplier;
        }

        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }
    }
}
