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

        public void Execute(int comboStep)
        {
            switch (comboStep)
            {
                case 0:
                    character.Attack1();
                    break;
                case 1:
                    character.Attack2();
                    break;
                case 2:
                    character.Attack3();
                    break;
            }
        }
    }
}
