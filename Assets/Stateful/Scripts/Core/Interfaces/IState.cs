using System;

public interface IState: IEquatable<IState>
{
    public string StateName { get; }
    public bool Evaluate();
    public void In();
    public void Tick();
    public void Out();
}