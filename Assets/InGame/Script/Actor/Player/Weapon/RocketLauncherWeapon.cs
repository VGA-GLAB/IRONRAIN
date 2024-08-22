namespace IronRain.Player
{
    public class RocketLauncherWeapon : PlayerWeaponBase
    {
        public override void OnShot()
        {
            base.OnShot();
            _smokeEffect.Play(_effectOwnerTime);
        }
    }
}
