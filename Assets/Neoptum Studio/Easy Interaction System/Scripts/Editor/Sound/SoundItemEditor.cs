using EIS.Runtime.Sound;
using UnityEditor;
using UnityEngine;

namespace EIS.Editor.Sound
{
    [CustomPropertyDrawer(typeof(SoundItem))]
    public class SoundItemDrawer : PropertyDrawer
    {
        private bool _isFoldoutExpanded = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _isFoldoutExpanded = EditorGUILayout.Foldout(_isFoldoutExpanded, label, true);

            if (_isFoldoutExpanded)
            {
                SerializedProperty soundEffectIndexProp = property.FindPropertyRelative("soundEffectIndex");
                SerializedProperty soundIndexProp = property.FindPropertyRelative("soundIndex");
                SerializedProperty audioClipProp = property.FindPropertyRelative("audioClip");

                EditorGUILayout.PropertyField(soundEffectIndexProp);

                // Conditionally hide soundIndex
                using (new EditorGUI.DisabledScope(soundEffectIndexProp.intValue !=
                                                   (int)SoundItem.SoundEffectIndex.DYNAMIC))
                {
                    EditorGUILayout.PropertyField(soundIndexProp);
                }

                EditorGUILayout.PropertyField(audioClipProp);
            }
        }
    }
}