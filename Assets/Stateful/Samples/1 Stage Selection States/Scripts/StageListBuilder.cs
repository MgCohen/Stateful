using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scaffold.Stateful.Samples
{
    public class StageListBuilder : MonoBehaviour
    {
        public Action<StageData> StageClicked = delegate { };

        [SerializeField] private StageSelector selectorPrefab;
        [SerializeField] private Transform holder;

        private StageSelector currentStage;

        public void BuildList(List<StageData> stages, StageData selectedStage)
        {
            foreach (var stage in stages)
            {
                bool selected = stage == selectedStage;
                StageSelector stageSelector = Instantiate(selectorPrefab, holder);
                stageSelector.Setup(selected, stage, OnStageSelected);

                if (selected)
                {
                    currentStage = stageSelector;
                }
            }
        }

        private void OnStageSelected(StageSelector stage)
        {
            if (currentStage != null)
            {
                currentStage.ToggleSelect(false);
            }
            currentStage = stage;
            currentStage.ToggleSelect(true);

            StageClicked?.Invoke(stage.Stage);
        }
    }
}