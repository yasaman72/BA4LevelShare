using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetLogText : MonoBehaviour
{
    [SerializeField] private float m_timeToClear = 10;

    private TextMeshProUGUI m_text;

    void Start()
    {
        m_text = GetComponent<TextMeshProUGUI>();
    }

    public void ShowMessage(string message)
    {
        m_text.text = message;
        Invoke(nameof(ClearTextMessage), m_timeToClear);
    }

    private void ClearTextMessage()
    {
        m_text.text = "";
    }
}
