using PushingBoxStudios;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceRts
{
    public class UnitHpBar : MonoBehaviour
    {
        private bool _isHpBarVisible;
        private Image _hpBarFill;
        private Ship _unit;
        private float _height;
        private Color _lowestValueColor = Utils.ToFloatRGBA(0, 255, 170);
        private Color _highestValueColor = Utils.ToFloatRGBA(0, 255, 170);

        private void Awake()
        {
            _hpBarFill = transform.Find("HpBar/HpBarBackground/HpBarFill").GetComponent<Image>();
            SetVisibility(false);
        }

        public void Initialize(Ship owner)
        {
            _unit = owner;
            _unit.HpChanged += OnHpChanged;
            _unit.Destructed += OnDestruction;

            var meshs = owner.gameObject.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < meshs.Length; i++)
            {
                if (_height < meshs[i].bounds.size.y)
                {
                    _height = meshs[i].bounds.size.y;
                }
            }

            _hpBarFill.color = _highestValueColor;

            SetVisibility(true);
            UpdatePosition();
        }

        private void OnDestroy()
        {
            if (_unit != null)
            {
                _unit.HpChanged -= OnHpChanged;
                _unit.Destructed -= OnDestruction;
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
                pos.y += _height + 0.25f;
                transform.position = Camera.main.WorldToScreenPoint(pos);
            }
        }

        private void OnHpChanged(object sender, EventArgs e)
        {
            var unitGauge = (Gauge)sender;

            _hpBarFill.fillAmount = unitGauge.NormalizedValue;
            _hpBarFill.color = Color.Lerp(_highestValueColor, _lowestValueColor, 1 - _hpBarFill.fillAmount);

            if (_hpBarFill.fillAmount < 1 && !_isHpBarVisible)
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

            _isHpBarVisible = isVisible;
        }
    }
}
