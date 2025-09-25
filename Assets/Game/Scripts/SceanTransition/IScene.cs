using System;

public interface IScene
{
    public void SetVisible(bool isShowed, float duration = 0);
    event Action<IScene> OnShown;
}
