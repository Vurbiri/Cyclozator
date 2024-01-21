using System;

public class CatchingEventArgs : EventArgs
{
    public bool isAptly { get; private set; }
    public bool isCoin { get; private set; }

    public CatchingEventArgs(bool isAptly, bool isCoin)
    {
        this.isAptly = isAptly;
        this.isCoin = isCoin;
    }

}
