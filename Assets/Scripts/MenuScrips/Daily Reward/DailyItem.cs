using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Item_State { not_available, available, claimed };

public class DailyItem : MonoBehaviour
{
    public int item_id;
    public int coins;
    public int xp;
    public Item_State item_state;
    [SerializeField] GameObject claimed;
    [SerializeField] GameObject available;
    public static DailyItem Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }


    // Update is called once per frame
    void Update()
    {
       if(item_state == Item_State.claimed)
        {
            claimed.SetActive(true);
            available.SetActive(false);
        }

        if (item_state == Item_State.available)
        {
            available.SetActive(true);
            claimed.SetActive(false);
        }
    }
}
