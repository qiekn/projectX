using UnityEngine;
using System.Collections;

namespace Qiekn {
    /// <summary>
    /// This class represents a target that can be hit and recovered automatically.
    /// </summary>
    public class Target : MonoBehaviour, IDamageable {

        #region Field

        [SerializeField] float maxHealth = 1000;
        float curHealth;

        private bool isCoroutined = false; // Prevents multiple coroutines

        [Header("Customizable Options")]
        [SerializeField] bool isAutoRecover = true;
        [SerializeField] float minTime; // Minimum time before the target goes back up
        [SerializeField] float maxTime; // Maximum time before the target goes back up

        [Header("Audio")]
        [SerializeField] AudioClip upSound;
        [SerializeField] AudioClip downSound;

        [Header("Animations")]
        [SerializeField] AnimationClip targetUp;
        [SerializeField] AnimationClip targetDown;
        [SerializeField] AudioSource audioSource;

        #endregion

        #region Methods

        void Start() {
            curHealth = maxHealth;
        }

        public void TakeDamage(float amount) {
            curHealth -= amount;
            if (curHealth <= 0) {
                DownImpl();
            }
        }

        public float GetCurrentRatio() {
            return curHealth / maxHealth;
        }

        public void Recover() {
            curHealth = maxHealth;
            UpImpl();
        }

        private void DownImpl() {
            // Animate the target "down"
            gameObject.GetComponent<Animation>().clip = targetDown;
            gameObject.GetComponent<Animation>().Play();
            // Set the downSound as current sound, and play it
            audioSource.GetComponent<AudioSource>().clip = downSound;
            audioSource.Play();

            // If Auto recover
            if (isAutoRecover && !isCoroutined) {
                float delay = Random.Range(minTime, maxTime);
                StartCoroutine(AutoDeleyedRecover(delay));
                isCoroutined = true;
            }
        }

        private void UpImpl() {
            // Animate the target "up" 
            gameObject.GetComponent<Animation>().clip = targetUp;
            gameObject.GetComponent<Animation>().Play();
            // Set the upSound as current sound, and play it
            audioSource.GetComponent<AudioSource>().clip = upSound;
            audioSource.Play();
        }

        private IEnumerator AutoDeleyedRecover(float delay) {
            yield return new WaitForSeconds(delay);
            isCoroutined = false;
            Recover();
        }

        #endregion
    }
}