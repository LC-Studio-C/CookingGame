using System;

public interface IProgress
{
    /// <summary>
    /// ���ȸı��¼�
    /// </summary>
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    /// <summary>
    /// ���ȸı��¼���Ϣ������
    /// </summary>
    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progressArgs;
    }
}
