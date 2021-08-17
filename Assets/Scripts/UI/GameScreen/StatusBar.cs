using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.GameScreen
{
    public class StatusBar : MonoBehaviour
    {
        [SerializeField]
        private Image _background;

        [SerializeField]
        private Scrollbar _scrollbar;

        [SerializeField]
        private Image _fill;

        [SerializeField]
        private TextMeshProUGUI _labelText;

        public string LabelText
        {
            get => _labelText.text;
            set => _labelText.text = value;
        }

        [SerializeField]
        private TextMeshProUGUI _valueText;

        private Color _textColor;

        private int _val;
        private int _max;
        private int _boost;
        private int _maxMax;

        private void Awake()
        {
            if (string.IsNullOrEmpty(_labelText.text))
                _labelText.gameObject.SetActive(false);
        }

        public void Draw(int val, int max, int boost, int maxMax = -1)
        {
            if (max > 0)
                val = Math.Min(max, Math.Max(0, val));
            if (val == _val && max == _max && boost == _boost && maxMax == _maxMax)
                return;

            _val = val;
            _max = max;
            _boost = boost;
            _maxMax = maxMax;
            InternalDraw();
        }

        private void InternalDraw()
        {
            var textColorInt = 16777215;
            if (_maxMax > 0 && _max - _boost == _maxMax)
            {
                textColorInt = 16572160;
            }
            else if (_boost > 0)
            {
                textColorInt = 6206769;
            }

            var textColor = ParseUtils.ColorFromUInt((uint) textColorInt);
            if (_textColor != textColor)
            {
                SetTextColor(textColor);
            }

            if (_max > 0)
            {
                _scrollbar.size = (float) _val / _max;
                _valueText.text = _val + "/" + _max;
            }
            else
            {
                _scrollbar.size = 1;
                _valueText.text = _val.ToString();
            }

            if (!_valueText.gameObject.activeSelf)
                _valueText.gameObject.SetActive(true);

            if (_boost != 0)
            {
                _valueText.text += " (" + (_boost > 0 ? "+" : "") + _boost + ")";
            }
        }

        private void SetTextColor(Color color)
        {
            _textColor = color;
            _valueText.color = color;
        }
    }
}