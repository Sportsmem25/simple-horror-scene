using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    public TextMeshProUGUI interactionText;
    public Image crosshair;
    public Image panelOrder;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowInteraction(string messageText)
    {
        if (interactionText != null)
        {
            interactionText.text = messageText;
            interactionText.enabled = true;
        }
    }

    public void HideInteractionHint()
    {
        if (interactionText != null)
            interactionText.enabled = false;
    }

    public IEnumerator ShowOrder()
    {
        panelOrder.enabled = true;
        yield return new WaitForSeconds(3f);
        panelOrder.enabled = false;
    }
}