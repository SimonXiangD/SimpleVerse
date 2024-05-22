using EIS.Runtime.Interactions.Sound;
using UnityEditor;

namespace EIS.Editor.Interactions.Sound
{
    [CustomEditor(typeof(TriggerSoundEffectInteraction))]
    public class TriggerSoundEffectInteractionEditor : UnityEditor.Editor
    {
        private SerializedProperty playSoundByProp;
        private SerializedProperty soundEffectIndexProp;
        private SerializedProperty soundIndexProp;
        private SerializedProperty audioClipProp;
        private SerializedProperty useHintsProp;
        private SerializedProperty primaryHintTextProp;

        private void OnEnable()
        {
            playSoundByProp = serializedObject.FindProperty("playSoundBy");
            soundEffectIndexProp = serializedObject.FindProperty("soundEffectIndex");
            soundIndexProp = serializedObject.FindProperty("soundIndex");
            audioClipProp = serializedObject.FindProperty("audioClip");
            useHintsProp = serializedObject.FindProperty("useHints");
            primaryHintTextProp = serializedObject.FindProperty("primaryHintText");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(playSoundByProp);

            using (new EditorGUI.IndentLevelScope())
            {
                switch (playSoundByProp.enumValueIndex)
                {
                    case (int)TriggerSoundEffectInteraction.PlaySoundBy.EffectIndex:
                        EditorGUILayout.PropertyField(soundEffectIndexProp);
                        break;
                    case (int)TriggerSoundEffectInteraction.PlaySoundBy.DynamicIndex:
                        EditorGUILayout.PropertyField(soundIndexProp);
                        break;
                    default:
                        EditorGUILayout.PropertyField(audioClipProp);
                        break;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(useHintsProp);

            using (new EditorGUI.DisabledGroupScope(!useHintsProp.boolValue))
            {
                EditorGUILayout.PropertyField(primaryHintTextProp);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}