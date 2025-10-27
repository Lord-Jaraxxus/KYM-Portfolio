using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class CommandInvoker 
    {
        private Queue<ICommand> commandQueue = new Queue<ICommand>();

        public void AddCommand(ICommand command)
        {
            commandQueue.Enqueue(command);
        }

        public void ExecuteNext()
        {
            if (commandQueue.Count > 0)
                commandQueue.Dequeue().Execute();
        }
    }
}
