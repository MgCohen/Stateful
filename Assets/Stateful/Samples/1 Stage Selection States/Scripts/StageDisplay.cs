using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Scaffold.Stateful.Samples
{

    public class StageDisplay : MonoBehaviour
    {
        [SerializeField] private StageListBuilder stageList;
        [SerializeField] private TextMeshProUGUI selectedStageText;
        [SerializeField] private int stageCount = 15;


        private void Start()
        {
            List<StageData> stages = MockStageData();
            StageData selected = MockSelectedStage(stages);

            stageList.BuildList(stages, selected);
            stageList.StageClicked += SelectStage;

            SelectStage(selected);
        }

        private void SelectStage(StageData stage)
        {
            selectedStageText.text = $"Selected Stage: {stage.Index}";
        }


        private StageData MockSelectedStage(List<StageData> stages)
        {
            return stages.Last(s => s.Unlocked == true);
        }

        private List<StageData> MockStageData()
        {
            List<StageData> mockedData = new List<StageData>();

            int completedStages = Random.Range(0, stageCount);
            int unlockedOffset = Random.Range(1, 3);
            int unlockedStages = Mathf.Min(stageCount, completedStages + unlockedOffset);
            for (int i = 0; i < stageCount; i++)
            {
                bool unlocked = unlockedStages >= i;
                bool completed = completedStages >= i;
                int stars = completed ? Random.Range(1, 4) : 0;
                StageData stageData = new StageData(i, stars, unlocked, completed);
                mockedData.Add(stageData);
            }

            return mockedData;
        }
    }
}
