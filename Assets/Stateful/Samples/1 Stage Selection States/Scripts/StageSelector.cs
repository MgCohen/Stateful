using DG.Tweening;
using Scaffold.Stateful;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scaffold.Stateful.Samples
{

    public class StageSelector : StatefulBehaviour<StageSelector.StageState>, IPointerClickHandler
    {
        public StageData Stage { get; private set; } = new StageData(0, 1, true, true);

        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Image background;
        [SerializeField] private Transform starHolder;
        [SerializeField] private List<Image> starInstances = new List<Image>();

        private bool selected = false;
        private Action<StageSelector> StageClicked = delegate { };

        public void Setup(bool selected, StageData stage, Action<StageSelector> onStageClicked)
        {
            this.selected = selected;
            this.Stage = stage;
            StageClicked += onStageClicked;
            EvaluateCurrentState();
        }

        public void ToggleSelect(bool state)
        {
            this.selected = state;
            EvaluateCurrentState();
        }

        public override void EvaluateCurrentState()
        {
            base.EvaluateCurrentState();
            if (!Stage.Unlocked)
            {
                ChangeState<LockedStage>();
                return;
            }

            if (selected)
            {
                ChangeState<SelectedStage>();
                return;
            }

            if (Stage.Completed)
            {
                ChangeState<CompletedStage>();
                return;
            }

            ChangeState<UnlockedStage>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CurrentState.OnClick();
        }

        private void DrawStars(Color color)
        {
            if (CurrentState is not SelectedStage)
            {
                starHolder.gameObject.SetActive(false);
                return;
            }


            starHolder.gameObject.SetActive(Stage.Completed);
            for (int i = 0; i < starInstances.Count; i++)
            {
                var star = starInstances[i];
                star.gameObject.SetActive(i < Stage.Stars);
                star.color = color;
            }
        }

        #region States
        [Serializable]
        public abstract class StageState : State<StageSelector>
        {
            [SerializeField] protected Sprite backgroundSprite;
            [SerializeField] protected Color textColor;

            public override void In()
            {
                component.levelText.text = component.Stage.Index.ToString();
                component.background.sprite = backgroundSprite;
                component.levelText.color = textColor;
                component.DrawStars(textColor);
            }

            public virtual void OnClick()
            {
                component.StageClicked?.Invoke(component);
            }
        }

        public class LockedStage : StageState
        {
            [SerializeField] private Transform lockedIcon;
            [SerializeField] private float shakeAmount;

            public override void In()
            {
                base.In();
                lockedIcon.gameObject.SetActive(true);
                component.levelText.gameObject.SetActive(false);
            }

            public override void Out()
            {
                base.Out();
                lockedIcon.gameObject.SetActive(false);
                component.levelText.gameObject.SetActive(true);
            }

            public override void OnClick()
            {
                lockedIcon.transform.DOPunchRotation(Vector3.one * shakeAmount, 0.3f);
            }
        }

        public class UnlockedStage : StageState
        {

        }

        public class SelectedStage : UnlockedStage
        {

        }

        public class CompletedStage : SelectedStage
        {

        }
        #endregion
    }
}
