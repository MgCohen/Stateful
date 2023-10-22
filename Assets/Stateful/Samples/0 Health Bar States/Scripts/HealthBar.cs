using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scaffold.Stateful.Samples
{
    public class HealthBar : StatefulBehaviour
    {
        public override StateStrategy Strategy => StateStrategy.Variable;

        [SerializeField] private int health = 100;
        [SerializeField] private int maxHealth = 100;

        [SerializeField] private List<Graphic> imagesToColor = new List<Graphic>();
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Slider healthSlider;

        private Tween healthTextTween;

        private void Start()
        {
            healthSlider.maxValue = maxHealth;
            SetHealth(maxHealth);
        }

        public void AddHealth(int amount)
        {
            transform.DOKill(true);
            transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
            SetHealth(health + amount);
        }

        public void RemoveHealth(int amount)
        {
            transform.DOKill(true);
            transform.DOPunchRotation(Vector3.one * 3, 0.1f);
            SetHealth(health - amount);
        }

        private void SetHealth(int amount)
        {
            if (healthTextTween.IsActive())
            {
                healthTextTween.Kill();
                healthSlider.DOKill();
            }

            int targetValue = Mathf.Clamp(amount, 0, maxHealth);
            healthTextTween = DOTween.To(() => health, x => health = x, targetValue, 0.2f).OnUpdate(() =>
            {
                healthText.text = health.ToString();
                EvaluateCurrentState();
            });

            healthSlider.DOValue(targetValue, 0.2f);
        }

        public class HealthBarState : State<HealthBar>
        {
            [SerializeField] private Color color;
            [SerializeField, Range(0, 1)] public float HealthPercentage;

            public override bool Evaluate()
            {
                float percentage = (float)component.health / (float)component.maxHealth;
                return percentage >= HealthPercentage;
            }

            public override void In()
            {
                foreach (var image in component.imagesToColor)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        image.color = color;
                        continue;
                    }
#endif
                    image.DOKill();
                    image.DOColor(color, 0.2f);
                }
            }

            public override string StateName => $"{Mathf.RoundToInt(HealthPercentage * 100)}%";
        }
    }
}
