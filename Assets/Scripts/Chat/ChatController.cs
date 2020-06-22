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

        private void Start()
        {
            _chatInput.SetActive(false);
            _chatBox.SetActive(false);
        }

        private void Update()
        {
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
                return;
            }

            NewChatMessage(inputField.text);
            inputField.text = "";
            _chatInput.SetActive(false);
        }

        private void NewChatMessage(string message)
        {
            message = message.Trim();

            if (string.IsNullOrWhiteSpace(message))
            {
                _chatBox.SetActive(false);
                return;
            }

            var chatMessage = Instantiate(_chatTextPrefab, _chatContent.transform);
            var chatText = chatMessage.GetComponent<Text>();
            chatText.text = "[You]: " + message;

            _chatBox.SetActive(true);

            Invoke("HideChatBox", 5);
        }

        private void HideChatBox()
        {
            _chatBox.SetActive(false);
        }
    }
}