using Cinemachine;
using UnityEngine;

public class Cinematic : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Detective")
        {
            Debug.Log("Temp Cinematic");
            animator.SetBool("Cutscene", true);
            Invoke("SwitchBackToNormal", 5);
        }
    }

    void SwitchBackToNormal()
    {
        animator.SetBool("Cutscene", false);
        Destroy(gameObject);
    }
}
