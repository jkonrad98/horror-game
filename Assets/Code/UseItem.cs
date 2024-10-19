using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    [SerializeField] GameObject flashlight, binoculars;

    GameObject itemHeld;
    // Start is called before the first frame update
    private void Start()
    {
        itemHeld = binoculars;
    }

    void OnChangedItem()
    {
        //add functionality so when the slot changes, it changes the UseTheItem obj. 
        //Also create Item parent class so then flashlight can derive from it so the code could look clean here:
        //itemHeld.UseTheItem();
        //And then flashlight would derive from UsableItem class, enabling virtual class and overriding the method
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            itemHeld.GetComponent<UsableItem>().UseTheItem();
        }
    }
}
