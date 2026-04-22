using SABI.SOA;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SABI.Example
{
    public class HealthBarExample : MonoBehaviour
    {
        [SerializeField] private Image imageFileToManage;
        [SerializeField] private SageFloat health;
        [SerializeField] private SageFloat maxHealth;
        [SerializeField] private TextMeshProUGUI healthText;

        private void Awake() { OnValueChange(health.GetValue(), 0); }

        private void OnEnable() { health.Subscribe(this, OnValueChange); }

        private void OnDisable() { health.UnSubscribe(this, OnValueChange); }

        private void OnValueChange(float newValue, float oldValue)
        {
            float fillAmount = newValue / maxHealth.GetValue();
            imageFileToManage.fillAmount = fillAmount;
            healthText.text = $"Health: {newValue}";
        }
    }
}