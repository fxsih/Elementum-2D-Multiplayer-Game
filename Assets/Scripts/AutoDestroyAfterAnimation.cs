using UnityEngine;

public class AutoDestroyAfterAnimation : MonoBehaviour
{
    public float lifetime = 0.25f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
