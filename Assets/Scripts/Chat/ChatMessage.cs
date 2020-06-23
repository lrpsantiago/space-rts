using UnityEngine;

namespace Assets.Scripts.Chat
{
    public class ChatMessage
    {
        private string _sender;
        private string _text;

        public string Sender
        {
            get => _sender;

            set
            {
                _sender = value.Trim();
            }
        }

        public string Text
        {
            get => _text;

            set
            {
                _text = value.Trim();
            }
        }

        public Color Color { get; set; } = Color.white;

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Sender)
                && !string.IsNullOrWhiteSpace(Text);
        }

        public override string ToString()
        {
            return $"[{Sender}]: {Text}";
        }
    }
}
