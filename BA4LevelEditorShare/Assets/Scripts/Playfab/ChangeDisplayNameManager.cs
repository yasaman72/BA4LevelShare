using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeDisplayNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_inputName;

    public void ChangeDisplayName()
    {
        if (!String.IsNullOrEmpty(m_inputName.text))
            PlayFabController.instance.UpdateDisplayName(m_inputName.text);
    }
}
