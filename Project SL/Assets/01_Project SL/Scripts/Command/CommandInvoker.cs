using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class CommandInvoker 
    {
        public Queue<ICommand> CommandQueue => commandQueue;
        private Queue<ICommand> commandQueue = new Queue<ICommand>();
        public bool CanQueueCommand { get; set; } = true; // 큐에 명령 추가 가능 여부
        public bool CanExecuteCommand { get; set; } = true; // 명령 실행 가능 여부

        public int ComboStep { get; private set; } = 0;

        private AnimationEventListener animationEventListener;

        public CommandInvoker(AnimationEventListener animationEventListener)
        {
            this.animationEventListener = animationEventListener;
            this.animationEventListener.OnReceiveAnimationEvent += OnCallbackReceiveAnimationEvent;
        }

        private void OnCallbackReceiveAnimationEvent(string eventName)
        {
            switch (eventName) 
            {
                case "EnableCommandQueue":
                    CanQueueCommand = true;
                    break;
                case "DisableCommandQueue":
                    CanQueueCommand = false;
                    break;
                case "EndCombo":
                    CanExecuteCommand = true;
                    if (commandQueue.Count == 0)    // 큐에 다음 명령이 없고, 애니메이션이 종료됐다면 콤보 초기화 
                    { 
                        ComboStep = 0; 
                        Debug.Log("Combo Reset");
                    }
                    break;
            }
        }

        public void TryAddCommand(ICommand command)
        {
            if (!CanQueueCommand) return;

            commandQueue.Enqueue(command);
            CanQueueCommand = false; // 명령 추가 후에는 다시 false로 설정하여 다음 명령이 애니메이션 이벤트를 통해 추가되도록 함 (선입력 방지)
            Debug.Log("Command Added to Queue");
        }

        public void ExecuteNext()
        {
            if (commandQueue.Count > 0 && CanExecuteCommand)
            {
                commandQueue.Dequeue().Execute(ComboStep);
                ComboStep++;

                CanExecuteCommand = false; // 명령 실행 후에는 다시 false로 설정하여 다음 명령이 애니메이션 이벤트를 통해 실행되도록 함
            }
        }
    }
}
