using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class CommandInvoker 
    {
        private Queue<ICommand> commandQueue = new Queue<ICommand>();
        public bool CanQueueCommand { get; set; } = true; // ť�� ��� �߰� ���� ����
                
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
                    if (commandQueue.Count == 0)    // ť�� ���� ����� ����, �ִϸ��̼��� ����ƴٸ� �޺� �ʱ�ȭ 
                    { 
                        ComboStep = 0; 
                    }
                    break;
            }
        }

        public void TryAddCommand(ICommand command)
        {
            if (!CanQueueCommand) return;

            commandQueue.Enqueue(command);
        }

        public void ExecuteNext()
        {
            if (commandQueue.Count > 0)
            {
                commandQueue.Dequeue().Execute();

            }
        }
    }
}
