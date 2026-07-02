using UnityEngine;

public class pause_in_fase : MonoBehaviour
{
    public GameObject UI_Manager_prefab;
    
    void Start()
    {
        Instantiate(UI_Manager_prefab);
    }      
}
