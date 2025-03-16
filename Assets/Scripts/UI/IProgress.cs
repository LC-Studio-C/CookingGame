using System;

public interface IProgress
{
    /// <summary>
    /// 进度改变事件
    /// </summary>
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    /// <summary>
    /// 进度改变事件消息传递器
    /// </summary>
    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progressArgs;
    }
}
