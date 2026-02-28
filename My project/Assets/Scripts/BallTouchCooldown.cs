using System.Collections;
using UnityEngine;

// Attach to any paddle (player or AI) to prevent it from touching the ball
// more than once within `cooldown` seconds.  After a touch the BoxCollider is
// disabled so the ball passes straight through; it re-enables after the delay.
public class BallTouchCooldown : MonoBehaviour
{
    [SerializeField] public float cooldown = 2f;

    private BoxCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_collider == null) return;
        if (collision.gameObject.name == SceneHelper.BallName)
            StartCoroutine(DisableForCooldown());
    }

    private IEnumerator DisableForCooldown()
    {
        _collider.enabled = false;
        yield return new WaitForSeconds(cooldown);
        _collider.enabled = true;
    }
}
