namespace IronRain.Player
{
    public class RocketLauncherWeapon : PlayerWeaponBase
    {
        public override void OnShot()
        {
            base.OnShot();
            _muzzleFlashEffect.Play(_effectOwnerTime);
            _smokeEffect.Play(_effectOwnerTime);
        }
    }
}
