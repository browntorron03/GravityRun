using UnityEngine;

public class HurtPlayer : MonoBehaviour
{

    public int damageToGive = 1;

     private GameManager gm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FindObjectOfType<HealthManager>().HurtPlayer(damageToGive);
        }
    }
}
