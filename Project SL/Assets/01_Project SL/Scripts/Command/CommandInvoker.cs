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
        public bool CanQueueCommand { get; set; } = true; // ť�� ��� �߰� ���� ����
        public bool CanExecuteCommand { get; set; } = true; // ��� ���� ���� ����

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
                    if (commandQueue.Count == 0)    // ť�� ���� ����� ����, �ִϸ��̼��� ����ƴٸ� �޺� �ʱ�ȭ 
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
            CanQueueCommand = false; // ��� �߰� �Ŀ��� �ٽ� false�� �����Ͽ� ���� ����� �ִϸ��̼� �̺�Ʈ�� ���� �߰��ǵ��� �� (���Է� ����)
            Debug.Log("Command Added to Queue");
        }

        public void ExecuteNext()
        {
            if (commandQueue.Count > 0 && CanExecuteCommand)
            {
                commandQueue.Dequeue().Execute(ComboStep);
                ComboStep++;

                CanExecuteCommand = false; // ��� ���� �Ŀ��� �ٽ� false�� �����Ͽ� ���� ����� �ִϸ��̼� �̺�Ʈ�� ���� ����ǵ��� ��
            }
        }
    }
}
