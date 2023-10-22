using System;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using static Scaffold.Stateful.Samples.StatefulAnimatorController;

namespace Scaffold.Stateful.Samples
{

    [CustomEditor(typeof(StatefulAnimatorController))]
    public class StatefulAnimatorEditor : StatefulBehaviourEditor
    {
        private AnimFloat progress;
        private float samplingTime;
        private bool isAnimating;

        private float previewProgress = 0;
        private AnimationClip currentClip = null;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            previewProgress = progress.isAnimating ? (float)EditorApplication.timeSinceStartup - samplingTime : 0;
            currentClip = GetCurrentAnimationClip();

            if (currentClip == null) return;
            TryLoopAnimation(currentClip, previewProgress);
            TryPreviewAnimation(currentClip, previewProgress);
        }

        protected override void DrawStateControls()
        {
            base.DrawStateControls();
            EditorGUILayout.BeginHorizontal();
            if (StatefulLayout.ToggleButton("Preview", AnimationMode.InAnimationMode()))
            {
                ToggleAnimationMode();
            }

            if (StatefulLayout.IconButton("Refresh", 16))
            {
                PopulateStateInfo();
            }
            EditorGUILayout.EndHorizontal();
            TryDrawPreviewProgressBar(currentClip, previewProgress);
        }

        private void TryDrawPreviewProgressBar(AnimationClip clip, float sampleTime)
        {
            if (!progress.isAnimating) return;
            if (currentClip == null) return;

            float progressPercent = Mathf.Clamp01(sampleTime / clip.length);
            Color backColor = new Color32(14, 60, 105, 255);
            Color fillColor = Color.white;

            Rect barRect = EditorGUILayout.GetControlRect(false, 4);
            EditorGUI.DrawRect(barRect, backColor);
            barRect.width *= progressPercent;
            EditorGUI.DrawRect(barRect, fillColor);
        }

        #region Animation Preview
        protected override void OnEnable()
        {
            base.OnEnable();
            ResetPreviewProgress();
            stateful.StateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            AnimationMode.StopAnimationMode();
            stateful.StateChanged -= OnStateChanged;
        }

        public void OnStateChanged(IState state)
        {
            if (AnimationMode.InAnimationMode())
            {
                StartAnimation();
            }
        }

        private void TryLoopAnimation(AnimationClip clip, float currentProgress)
        {
            if (clip != null && clip.isLooping && currentProgress >= clip.length)
            {
                progress.value = 0;
                progress.target = clip.length;
                samplingTime = (float)EditorApplication.timeSinceStartup;
            }
        }

        private void TryPreviewAnimation(AnimationClip clip, float currentProgress)
        {
            if (!Application.isPlaying && AnimationMode.InAnimationMode() && clip)
            {
                AnimationMode.BeginSampling();
                GameObject source = (target as MonoBehaviour).gameObject;
                AnimationMode.SampleAnimationClip(source, clip, currentProgress);
                AnimationMode.EndSampling();
                SceneView.RepaintAll();
            }
        }

        private void ToggleAnimationMode()
        {
            if (AnimationMode.InAnimationMode())
            {
                StopAnimation();
            }
            else
            {
                StartAnimation();
            }
        }

        private void StartAnimation()
        {
            isAnimating = true;
            Debug.Log("Starting Animation Sample");
            AnimationMode.StartAnimationMode();
            ResetPreviewProgress();
            try
            {
                var clip = GetCurrentAnimationClip();
                float length = clip.length * 1.1f; //we add a small extra-time to account for the animFloat lerping stopping before the total clip length
                progress.speed = 1 / length;
                progress.target = length;
                samplingTime = (float)EditorApplication.timeSinceStartup;
            }
            catch
            {
                Debug.Log("Failed to preview animation, maybe you are missing a clip?");
            }
        }

        private void StopAnimation()
        {
            if (!isAnimating) return;

            isAnimating = false;
            Debug.Log("Stopping Animation Sample");
            AnimationMode.StopAnimationMode();
            ResetPreviewProgress();
        }

        private void ResetPreviewProgress()
        {
            progress?.valueChanged?.RemoveAllListeners();
            progress = new AnimFloat(0);
            progress.valueChanged.AddListener(new UnityAction(base.Repaint));
        }

        private AnimationClip GetCurrentAnimationClip()
        {
            AnimatorState state = stateful.GetCurrentState() as AnimatorState;
            return state?.Motion as AnimationClip;
        }
        #endregion
    }
}
