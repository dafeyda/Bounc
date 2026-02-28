using UnityEngine;

public class BallSound : MonoBehaviour
{
    private AudioSource _audioSource;
    private static AudioClip _bonkClip;

    public static void Setup(GameObject ball)
    {
        if (_bonkClip == null)
            _bonkClip = Resources.Load<AudioClip>("Bonk");

        var bs = ball.AddComponent<BallSound>();
        bs._audioSource = ball.AddComponent<AudioSource>();
        bs._audioSource.clip = _bonkClip;
        bs._audioSource.playOnAwake = false;
        bs._audioSource.spatialBlend = 0f; // 2D global sound
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_audioSource != null && _bonkClip != null)
        {
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.Play();
        }

        if (collision.contactCount > 0)
            ImpactRing.Spawn(collision.GetContact(0));
    }
}
