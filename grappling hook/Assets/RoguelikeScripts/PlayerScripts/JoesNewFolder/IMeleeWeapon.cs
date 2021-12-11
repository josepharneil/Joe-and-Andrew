using System;

public interface IMeleeWeapon
{
    public void StartLightAttack(bool isFacingLeft, Action onFinish);

    public void StartHeavyAttack(bool isFacingLeft, Action onFinish);
}
