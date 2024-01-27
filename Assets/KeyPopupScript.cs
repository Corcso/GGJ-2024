using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyPopupScript : MonoBehaviour
{
    [SerializeField] private float baseFontSize;
    [SerializeField] private float pulseFontSize;

    public KeyCode myKey;
    RectTransform myRectTransform;
    TextMeshProUGUI myTextMeshPro;
    void Start()
    {
        myRectTransform = GetComponent<RectTransform>();
        myTextMeshPro = GetComponent<TextMeshProUGUI>();

        myRectTransform.localPosition = new Vector3(Random.Range(-200.0f, 200.0f), Random.Range(-200.0f, 200.0f), 0);
        myRectTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-30.0f, 30.0f));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(myKey))
        {
            myTextMeshPro.fontSize = pulseFontSize;
        }

        if (myTextMeshPro.fontSize > baseFontSize) {
            myTextMeshPro.fontSize -= 40 * Time.deltaTime;
        }
    }
}
