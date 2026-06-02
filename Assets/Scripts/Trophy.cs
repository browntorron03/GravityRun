using UnityEngine;

public class Trophy : MonoBehaviour
{
    private GameManager gm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.gm.canBeatLevel = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ("Player"))
        {
            GameManager.gm.CollectTrophy();
            Destroy(gameObject);
            Debug.Log("This Works");
        }
    }
}