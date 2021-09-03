using System;
using UnityEngine;

namespace PushingBoxStudios
{
    /// <summary>
    /// Class that represents a gauge. 
    /// </summary>
    public class Gauge
    {
        private float _currentValue;
        private float _maxValue;

        public event EventHandler CurrentValueChanged;
        public event EventHandler MaxValueChanged;

        /// <summary>
        /// The maximum value of the gauge. If the maximum value is set to a lower value than the current value, the
        /// current value will be set to the new maximum value.
        /// </summary>
        public float MaxValue
        {
            get { return _maxValue; }

            set
            {
                _maxValue = value;
                OnMaxValueChanged();

                if (CurrentValue > _maxValue)
                {
                    CurrentValue = _maxValue;
                }
            }
        }

        /// <summary>
        /// The current value of the gauge, a number between 0 and MaxValue.
        /// </summary>
        public float CurrentValue
        {
            get { return _currentValue; }

            set
            {
                _currentValue = Mathf.Clamp(value, 0, MaxValue);
                OnCurrentValueChanged();
            }
        }

        /// <summary>
        /// Return a number between 0 and 1 which is the percentage of the current value over the maximum value.
        /// </summary>
        public float NormalizedValue
        {
            get { return CurrentValue / MaxValue; }
        }

        /// <summary>
        /// Constructor of Gauge. The initial value is equals to the maximum value.
        /// </summary>
        /// <param name="maxValue">The maximum value supported by the gauge.</param>
        public Gauge(float maxValue)
        {
            MaxValue = maxValue;
            CurrentValue = MaxValue;
        }

        /// <summary>
        /// Constructor of Gauge.
        /// </summary>
        /// <param name="maxValue">The maximum value supported by the gauge.</param>
        /// <param name="initialValue">A custom initial value fo the gauge.</param>
        public Gauge(float maxValue, float initialValue)
        {
            MaxValue = maxValue;
            CurrentValue = initialValue;
        }

        private void OnMaxValueChanged()
        {
            if (MaxValueChanged != null)
            {
                MaxValueChanged(this, EventArgs.Empty);
            }
        }

        private void OnCurrentValueChanged()
        {
            if (CurrentValueChanged != null)
            {
                CurrentValueChanged(this, EventArgs.Empty);
            }
        }
    }
}
