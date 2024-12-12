using UnityEngine;

namespace Qiekn {
  [CreateAssetMenu(fileName = "GunData", menuName = "GunData", order = 0)]
  public class GunData : ScriptableObject {
    public string gunName;

    [Header("Fire")]
    public float damage;
    public float damageMinimum;
    public float range;
    public int rpm;
    public int ammunition; //子弹速度

    [Header("Reload")]
    public float magazineSize;
    public float reloadTime;

    [Header("Spread")]
    public float spreadMinimum;
    public float spreadMaximum;
    public float spreadAttack;
    public float spreadDecay;
    public float spreadDecayDelay;
    public float spreadResetDelay;

    [Header("Recoil")]
    public float recoilAttack;
    public float recoilMaximum;
    public float recoilDecay;
    public float recoilDecayDelay;
    public float recoilResetDelay;
    public float recoilSmooth;
    public float recoilRandomness;
  }
}