namespace IronRain.Player
{
    public class AssaultRifleWeapon : PlayerWeaponBase
    {
        public override void OnShot()
        {
            base.OnShot();
            _muzzleFlashEffect.Play(_effectOwnerTime);
            _smokeEffect.Play(_effectOwnerTime);
        }

    }
}
