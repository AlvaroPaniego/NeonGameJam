using UnityEngine;

public class Boost : MonoBehaviour
{
    [SerializeField]
    private float boostSpeed;
    [SerializeField]
    private float countdown;

    Player player;
    float beginCountDown;
    bool boostActivated=false;

    private void OnTriggerEnter(Collider other)
    {
        player=other.GetComponent<Player>();
        player.SpeedUp(boostSpeed);
        boostActivated = true;
        GetComponent<Renderer>().enabled = false;
    }

    private void Update()
    {
        if (boostActivated)
        {
            if (beginCountDown > countdown)
            {

                player.ResetSpeed();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"CountDown" + beginCountDown);
                beginCountDown += Time.deltaTime;
            }
        }
    }
}
