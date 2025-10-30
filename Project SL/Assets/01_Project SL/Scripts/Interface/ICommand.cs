using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public interface ICommand
    {
        void Execute(int comboStep); // 명령 실행 메서드
    }
}
