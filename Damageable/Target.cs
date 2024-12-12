using UnityEngine;
using System.Collections;

namespace Qiekn {
    /// <summary>
    /// This class represents a target that can be hit and recovered automatically.
    /// Main functions: TakeDamage() and Recover()
    /// </summary>
    public class Target : MonoBehaviour, IDamageable {

        #region Field

        public bool IsHit = false;
        private bool isCoroutined = false; // Prevents multiple coroutines

        enum State { Up, Down };// Just for Debugging
        [SerializeField] State state;

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

        public void TakeDamage() {
            IsHit = true;
            TryDown();
            if (isAutoRecover && !isCoroutined) {
                float delay = Random.Range(minTime, maxTime);
                StartCoroutine(AutoDeleyedRecover(delay));
                isCoroutined = true;
            }
        }

        public void Recover() {
            IsHit = false;
            TryUp();
        }

        private void TryDown() {
            if (state == State.Down) {
                return;
            }
            state = State.Down;
            // Animate the target "down"
            gameObject.GetComponent<Animation>().clip = targetDown;
            gameObject.GetComponent<Animation>().Play();

            // Set the downSound as current sound, and play it
            audioSource.GetComponent<AudioSource>().clip = downSound;
            audioSource.Play();
        }

        private void TryUp() {
            if (state == State.Up) {
                return;
            }
            state = State.Up;
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

        private void Update() {
            // Just for debugging
            if (IsHit) {
                TakeDamage();
            }
        }

        #endregion
    }
}