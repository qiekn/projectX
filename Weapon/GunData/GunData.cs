using System;
using UnityEngine;

namespace Qiekn {
  [CreateAssetMenu(fileName = "GunData", menuName = "Gun/GunData", order = 0)]
  public class GunData : ScriptableObject {

    public enum GunType {
      AssultRifle,
      Shotgun,
      SniperRifle,
      SMG,
      Machinegun,
      Pistol,
    }

    public enum FireMode {
      Semi,
      Auto,
      Burst,
    }

    public string gunName;
    public GunType gunType;
    public FireMode fireMode;
    public int burstCount = 2;
    public int burstRateOfFire = 900;

    [Header("Fire")]
    public float damage;
    public float damageMinimum;
    public float range;
    public float rangeMaximum = 100;
    public int rateOfFire;
    public int ammunition = 800; //子弹速度

    [Header("Reload")]
    public float magazineSize;
    public float reloadTime;
    public bool canReloadEmpty = true;

    [Header("Sound")]
    public AudioClip fire;
    public AudioClip fireDist;
    public AudioClip fireSilencer;
    public AudioClip fireTailOutdoor;
    public AudioClip reload;

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