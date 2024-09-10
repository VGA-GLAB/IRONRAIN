namespace IronRain.Player
{
    public class RocketLauncherWeapon : PlayerWeaponBase
    {
        public override void OnShot()
        {
            base.OnShot();
            _muzzleFlashEffect.Play();
            _smokeEffect.Play();
        }
    }
}
