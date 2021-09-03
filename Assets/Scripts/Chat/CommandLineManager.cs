using System;
using System.Collections.Generic;

namespace Assets.Scripts.Chat
{
    public class CommandLineManager
    {
        private Dictionary<string, Action<string[]>> _commandActions;

        public CommandLineManager()
        {
            _commandActions = new Dictionary<string, Action<string[]>>();
        }

        public void AddCommand(string command, Action<string[]> action)
        {
            _commandActions.Add(command, action);
        }

        public void RemoveCommand(string command)
        {
            _commandActions.Remove(command);
        }

        public bool DoesCommandExists(string command)
        {
            return _commandActions.ContainsKey(command);
        }

        public void ExecuteCommand(string command, string[] args)
        {
            _commandActions[command](args);
        }
    }
}
