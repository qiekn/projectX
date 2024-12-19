using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Qiekn {
  /// <summary>
  /// base class for all guns
  /// </summary>
  public class GunBehaviour : MonoBehaviour, IGun {
    #region Declaration

    enum FireSound {
      Normal,
      Dist,
      Silencer,
    }

    #endregion

    #region SerializeField

    [SerializeField] GunData gunData;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform fpsCamTransform;
    [SerializeField] HitMark hitMark;
    [SerializeField] LayerMask targetLayerMask;

    // Animation (to-do)
    // Audio
    AudioSource audioSource;
    [SerializeField] FireSound fireSound = FireSound.Normal;
    [SerializeField] GunData.FireMode fireMode;
    [SerializeField] int burstCount;

    #endregion

    #region Field

    // Actions
    InputAction fireAction;
    InputAction reloadAction;
    // Flags
    bool isReloading;
    [SerializeField] bool isOutDoor;
    bool canShootInSemiFireMode;

    [SerializeField] float currentAmmo; // SerializeField Just for Debugging
    float nextTimeToFire;

    #endregion

    #region Debug
    [SerializeField] TextMeshProUGUI fireModeText;
    [SerializeField] TextMeshProUGUI currentAmmoText;
    #endregion

    public void Fire() {
      if (!isReloading && currentAmmo > 0 && Time.time >= nextTimeToFire) {
        // Auto
        if (fireMode == GunData.FireMode.Auto) {
          // nextTimeToFire = Time.time + 60f / gunData.rateOfFire;
          nextTimeToFire = Time.time + 40f / gunData.rateOfFire;
          FireImpl();
        }
        // Semi
        else if (fireMode == GunData.FireMode.Semi && canShootInSemiFireMode) {
          nextTimeToFire = Time.time + 60f / gunData.rateOfFire;
          FireImpl();
          canShootInSemiFireMode = false;
          Debug.Log("Disable Fire");
        }
        // Burst
        else if (fireMode == GunData.FireMode.Burst) {
          if (currentAmmo > 0) {
            nextTimeToFire = Time.time + 60f / gunData.rateOfFire * gunData.burstCount;
            StartCoroutine(BurstFire());
          }
        }
      }
    }

    private IEnumerator BurstFire() {
      for (int i = 0; i < burstCount; i++) {
        yield return new WaitForSeconds(60f / gunData.burstRateOfFire);
        if (currentAmmo > 0) {
          FireImpl();
          currentAmmo--;
        }
      }
    }

    private void FireImpl() {
      --currentAmmo;
      ShowAmmo();
      // Different Fire Sound
      if (fireSound == FireSound.Normal) {
        audioSource.PlayOneShot(gunData.fire);
        if (isOutDoor && gunData.fireTailOutdoor != null) {
          audioSource.PlayOneShot(gunData.fireTailOutdoor);
        }
      } else if (fireSound == FireSound.Dist) {
        audioSource.PlayOneShot(gunData.fireDist);
      } else if (fireSound == FireSound.Silencer) {
        audioSource.PlayOneShot(gunData.fireSilencer);
      }
      RaycastHit hit;
      Physics.Raycast(fpsCamTransform.position, fpsCamTransform.forward, out hit, gunData.rangeMaximum, targetLayerMask);
      if (hit.transform != null) {
        var target = hit.transform.GetComponent<IDamageable>();
        if (target != null) {
          target.TakeDamage(gunData.damage);
          hitMark.Show(target.GetCurrentRatio());
        }
      }
    }

    public void Reload() {
      if (!isReloading && (currentAmmo < gunData.magazineSize || gunData.canReloadEmpty)) {
        StartCoroutine(ReloadImpl());
      }
    }

    IEnumerator ReloadImpl() {
      audioSource.PlayOneShot(gunData.reload);
      // Off Magazine
      currentAmmo = currentAmmo > 0 ? 1 : 0;
      ShowAmmo();
      // Reloading
      isReloading = true;
      yield return new WaitForSeconds(gunData.reloadTime);
      // Insert Magazine
      currentAmmo += gunData.magazineSize;
      ShowAmmo();
      isReloading = false;
    }

    void ShowAmmo() {
      currentAmmoText.SetText($"Ammo: {currentAmmo} / {gunData.magazineSize}");
    }

    #region Unity

    void Start() {
      // Init Gun
      currentAmmo = gunData.magazineSize;
      // Input Actions
      fireAction = InputSystem.actions.FindAction("Attack");
      reloadAction = InputSystem.actions.FindAction("Reload");
      // Audio Component
      audioSource = GetComponent<AudioSource>();
      if (audioSource == null) {
        throw new System.Exception("AudioSource component is missing on the " + gunData.gunName + " Gun.");
      }
      // Read config from gun data;
      fireMode = gunData.fireMode;
      fireModeText.text = "Fire Mode: " + fireMode.ToString();
      burstCount = gunData.burstCount;
    }

    void Update() {
      if (fireAction.IsPressed()) { Fire(); }
      if (fireMode == GunData.FireMode.Semi && canShootInSemiFireMode == false && !fireAction.IsPressed()) {
        canShootInSemiFireMode = true;
        Debug.Log("Enable Fire");
      }
      if (reloadAction.IsPressed()) { Reload(); }

      // Perform actions when B key is pressed
      if (Input.GetKeyDown(KeyCode.B)) {
        fireMode = (GunData.FireMode)(((int)fireMode + 1) % Enum.GetValues(typeof(GunData.FireMode)).Length);
        fireModeText.text = "Fire Mode: " + fireMode.ToString();
      }
    }

    #endregion
  }
}