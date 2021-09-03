using Assets.Scripts.FogOfWar;
using SpaceRts;
using SpaceRts.Planets;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

namespace Assets.Scripts.Chat
{
    public class ChatController : MonoBehaviour
    {
        private readonly char CommandChar = '/';

        [SerializeField]
        private GameObject _chatTextPrefab;

        [SerializeField]
        private GameObject _chatInput;

        [SerializeField]
        private GameObject _chatContent;

        [SerializeField]
        private GameObject _chatBox;

        private CommandLineManager _cliManager;
        private bool _isTyping;
        private DateTime _lastMessageTimeStamp;

        private void Start()
        {
            _isTyping = false;
            _lastMessageTimeStamp = DateTime.Now;

            _chatInput.SetActive(false);
            _chatBox.SetActive(false);

            SetupCommands();
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

        private void SetupCommands()
        {
            _cliManager = new CommandLineManager();
            _cliManager.AddCommand("rev", FogRevealCommand);
            _cliManager.AddCommand("units", CreateUnitsCommand);
        }

        private void NewChatMessage(ChatMessage message)
        {
            if (!message.IsValid())
            {
                _chatBox.SetActive(false);
                return;
            }

            if (message.Text[0] == CommandChar)
            {
                HandleCommand(message.Text);
                return;
            }

            var chatMessage = Instantiate(_chatTextPrefab, _chatContent.transform);
            var chatText = chatMessage.GetComponent<Text>();
            chatText.text = message.ToString();
            chatText.color = message.Color;

            _chatBox.SetActive(true);
        }

        private void HandleCommand(string messageText)
        {
            var words = messageText
                .Substring(1)
                .Split(' ');

            var commandText = words[0];

            if (string.IsNullOrWhiteSpace(commandText)
                || !_cliManager.DoesCommandExists(commandText))
            {
                SystemMessage("Command unkown.");
                return;
            }

            var args = words.Skip(1)
                .ToArray();

            _cliManager.ExecuteCommand(commandText, args);
        }

        private void SystemMessage(string text)
        {
            var invalidCommandMessage = new ChatMessage
            {
                Sender = "System",
                Color = Color.gray,
                Text = text
            };

            NewChatMessage(invalidCommandMessage);
        }

        private void FogRevealCommand(string[] args)
        {
            GameObject.Find("FogOfWar")
                .GetComponent<IFogOfWar>()
                .RevealAll();
        }

        private void CreateUnitsCommand(string[] args)
        {
            var objectSelector = GameObject.Find("ObjectSelector")
                .GetComponent<ObjectSelector>();

            var selectedObject = objectSelector.SelectedObjects
                .FirstOrDefault();

            if (selectedObject == null)
            {
                SystemMessage("You must select a planet first.");
                return;
            }

            var planet = selectedObject.Transform.gameObject.GetComponent<Planet>();
         
            if (planet == null)
            {
                SystemMessage("You must select a planet first.");
                return;
            }

            var quantity = 1;
            
            if (args.Length > 0)
            {
                quantity = Convert.ToInt32(args[0]);
            }

            for (int i = 0; i < quantity; i++)
            {
                planet.CreateShip();
            }
        }
    }
}