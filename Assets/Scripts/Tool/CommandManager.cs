using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGameFrameWork { 
public enum CommandType
{

}
    public class CommandManager : Singleton<CommandManager>
    {
        public Dictionary<CommandType, List<CommandBase>> commandDic = new Dictionary<CommandType, List<CommandBase>>();

        /// <summary>
        /// 添加命令
        /// </summary>
        /// <param name="command"></param>
        public void AddCommands(CommandType ct, CommandBase command)
        {
            if (commandDic.ContainsKey(ct))
            {
                commandDic[ct].Add(command);
            }
            else
            {
                commandDic.Add(ct, new List<CommandBase>());
                commandDic[ct].Add(command);
            }
        }
        /// <summary>
        /// 移除制定Command
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="command"></param>
        public void RemoveCommand(CommandType ct, CommandBase command)
        {
            if (commandDic.ContainsKey(ct))
            {
                commandDic[ct].Remove(command);
            }
            if (commandDic[ct].Count == 0)
            {
                commandDic.Remove(ct);
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public IEnumerator DoStart(CommandType ct)
        {
            if (commandDic.ContainsKey(ct))
            {
                foreach (CommandBase command in commandDic[ct])
                {
                    yield return new WaitForSeconds(.2f);
                    command.Excute();
                }
            }
        }
    }
}
