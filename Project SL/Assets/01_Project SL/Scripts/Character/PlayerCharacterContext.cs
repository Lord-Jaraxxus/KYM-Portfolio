using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class PlayerCharacterContext : SingletonBase<PlayerCharacterContext>
    {
        public CharacterBase CurrentPlayerCharacter { get; private set; }

        public void Register(CharacterBase playerCharacter) 
        {
            CurrentPlayerCharacter = playerCharacter;
        }
    }
}
