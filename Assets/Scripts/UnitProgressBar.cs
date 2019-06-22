using PushingBoxStudios;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceRts
{
    public class UnitProgressBar : MonoBehaviour
    {
        private bool _isBarVisible;
        private Image _barFill;
        private GameObject _unit; //interface
        private float _topBound;

        public Color LowestValueColor { get; set; }

        public Color HighestValueColor { get; set; }

        public float FloatingHeight { get; set; }

        public bool IsVisible
        {
            get { return _isBarVisible; }
            set { SetVisibility(value); }
        }

        private void Awake()
        {
            _barFill = transform.Find("HpBar/HpBarBackground/HpBarFill").GetComponent<Image>();

            LowestValueColor = Color.red;
            HighestValueColor = Color.green;

            SetVisibility(false);
        }

        public void Initialize(GameObject owner, Gauge gauge)
        {
            _unit = owner;

            gauge.CurrentValueChanged += OnValueChanged;
            //owner.Destructed += OnDestruction;

            var meshs = owner.gameObject.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < meshs.Length; i++)
            {
                if (_topBound < meshs[i].bounds.size.y)
                {
                    _topBound = meshs[i].bounds.size.y;
                }
            }

            _barFill.color = HighestValueColor;

            UpdatePosition();
        }

        private void OnDestroy()
        {
            if (_unit != null)
            {
                //_unit.HpChanged -= OnValueChanged;
                //_unit.Destructed -= OnDestruction;
            }
        }

        private void OnDestruction(object sender, EventArgs e)
        {
            Destroy(gameObject);
        }

        private void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (_unit != null)
            {
                var pos = _unit.transform.position;
                pos.y += _topBound + FloatingHeight;
                transform.position = Camera.main.WorldToScreenPoint(pos);
            }
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            var unitGauge = (Gauge)sender;

            _barFill.fillAmount = unitGauge.NormalizedValue;
            _barFill.color = Color.Lerp(HighestValueColor, LowestValueColor, 1 - _barFill.fillAmount);

            if (_barFill.fillAmount < 1 && !_isBarVisible)
            {
                SetVisibility(true);
            }
        }

        private void SetVisibility(bool isVisible)
        {
            var images = GetComponentsInChildren<Image>();

            for (int i = 0; i < images.Length; i++)
            {
                images[i].enabled = isVisible;
            }

            _isBarVisible = isVisible;
        }
    }
}
