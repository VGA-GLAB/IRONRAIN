namespace IronRain.Player
{
    public class AssaultRifleWeapon : PlayerWeaponBase
    {
        public override void OnShot()
        {
            base.OnShot();
            _muzzleFlashEffect.Play();
            _smokeEffect.Play();
        }

    }
}
