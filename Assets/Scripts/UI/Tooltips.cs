using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Tooltips : MonoBehaviour
{
    //tooltip of each upgrade
    [SerializeField] private GameObject cdTip;
    [SerializeField] private GameObject backTip;
    [SerializeField] private GameObject firstTip;
    [SerializeField] private GameObject roomTip;
    [SerializeField] private GameObject darkTip;
    [SerializeField] private GameObject killTip;


    //trigger of each tooltip
    [SerializeField] private GameObject cdTrigger;
    [SerializeField] private GameObject backTrigger;
    [SerializeField] private GameObject firstTrigger;
    [SerializeField] private GameObject roomTrigger;
    [SerializeField] private GameObject darkTrigger;
    [SerializeField] private GameObject killTrigger;


    private void Update()
    {
        //gets pointer
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        //raycasts
        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);


        //if not on anything
        if (raycastResultList.Count == 0)
        {
            cdTip.SetActive(false);
            backTip.SetActive(false);
            firstTip.SetActive(false);
            roomTip.SetActive(false);
            darkTip.SetActive(false);
            killTip.SetActive(false);
        }


        //otherwise show the tip of the respective upgrade
        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject == cdTrigger)
            {
                cdTip.SetActive(true);
            }
            else if (raycastResultList[i].gameObject == backTrigger)
            {
                backTip.SetActive(true);
            }
            else if (raycastResultList[i].gameObject == firstTrigger)
            {
                firstTip.SetActive(true);
            }
            else if (raycastResultList[i].gameObject == roomTrigger)
            {
                roomTip.SetActive(true);
            }
            else if (raycastResultList[i].gameObject == darkTrigger)
            {
                darkTip.SetActive(true);
            }
            else if (raycastResultList[i].gameObject == killTrigger)
            {
                killTip.SetActive(true);
            }
        }
    }
}