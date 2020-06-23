using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Chat
{
    public class ChatController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _chatTextPrefab;

        [SerializeField]
        private GameObject _chatInput;

        [SerializeField]
        private GameObject _chatContent;

        [SerializeField]
        private GameObject _chatBox;

        private bool _isTyping;
        private DateTime _lastMessageTimeStamp;

        private void Start()
        {
            _isTyping = false;
            _lastMessageTimeStamp = DateTime.Now;

            _chatInput.SetActive(false);
            _chatBox.SetActive(false);
        }

        private void Update()
        {
            var diff = DateTime.Now - _lastMessageTimeStamp;

            if (diff.Seconds >= 5 && !_isTyping)
            {
                _chatBox.SetActive(false);
            }

            if (!Input.GetKeyDown(KeyCode.Return))
            {
                return;
            }

            var inputField = _chatInput.GetComponent<InputField>();

            if (!_chatInput.activeSelf)
            {
                _chatBox.SetActive(true);
                _chatInput.SetActive(true);
                inputField.Select();
                inputField.ActivateInputField();
                _isTyping = true;
                return;
            }

            var chatMessage = new ChatMessage
            {
                Sender = "You",
                Text = inputField.text,
                Color = Color.white
            };

            NewChatMessage(chatMessage);
            inputField.text = "";
            _chatInput.SetActive(false);
            _isTyping = false;
            _lastMessageTimeStamp = DateTime.Now;
        }

        private void NewChatMessage(ChatMessage message)
        {
            if (!message.IsValid())
            {
                _chatBox.SetActive(false);
                return;
            }

            var chatMessage = Instantiate(_chatTextPrefab, _chatContent.transform);
            var chatText = chatMessage.GetComponent<Text>();
            chatText.text = message.ToString();
            chatText.color = message.Color;

            _chatBox.SetActive(true);
        }
    }
}