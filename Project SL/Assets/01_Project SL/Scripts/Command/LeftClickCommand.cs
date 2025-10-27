using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class LeftClickCommand : ICommand
    {
        private CharacterBase character;

        public LeftClickCommand(CharacterBase character)
        {
            this.character = character;
        }

        public void Execute()
        {
        }
    }
}
