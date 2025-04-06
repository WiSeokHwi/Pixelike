using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip attackSound;
    public AudioClip stepSound;
    public AudioClip hitSound;
    public AudioClip deadSound;
    public AudioClip dashSound;
    public AudioSource audioSource;


    public void AttackSound()
    {
        audioSource.PlayOneShot(attackSound, 0.6f);
    }
    public void StepSound()
    {
        audioSource.PlayOneShot(stepSound);
    }
    public void HitSound()
    {
        audioSource.PlayOneShot(hitSound, 0.4f);
    }
    public void DeadSound()
    {
        audioSource.PlayOneShot(deadSound);
    }    
    public void DashSound()
    {
        audioSource.PlayOneShot(dashSound, 0.4f);
    }
}
