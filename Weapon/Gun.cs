using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour {
    public float damage = 10f;
    public float range = 100f;

    public Camera fpsCam;
    InputAction fireAction;
    LayerMask enemyLayer;

    private void Start() {
        fireAction = InputSystem.actions.FindAction("Fire");
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    void Update() {
        if (fireAction.IsPressed()) {
            Shoot();
        }
    }

    public void Shoot() {
        RaycastHit hit;
        Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, enemyLayer);
        if (hit.transform != null) {
            Debug.Log("Hit: " + hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null) {
                target.TakeDamage(damage);
            }
        }
    }
}
