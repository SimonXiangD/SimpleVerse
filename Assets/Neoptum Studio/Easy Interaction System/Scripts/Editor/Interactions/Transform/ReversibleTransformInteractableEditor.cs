using EIS.Runtime.Interactions.Transforms;
using UnityEditor;
using UnityEngine;

namespace EIS.Editor.Interactions.Transforms
{
    [CustomEditor(typeof(ReversibleTransformInteractable))]
    public class ReversibleTransformInteractableEditor : UnityEditor.Editor
    {
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 initialScale;


        private SerializedProperty changePositionProperty;
        private SerializedProperty changeRotationProperty;
        private SerializedProperty changeScaleProperty;
        private SerializedProperty newPositionProperty;
        private SerializedProperty newRotationProperty;
        private SerializedProperty newScaleProperty;

        private SerializedProperty thresholdPositionProperty;
        private SerializedProperty thresholdRotationProperty;
        private SerializedProperty thresholdScaleProperty;

        private void OnEnable()
        {
            Transform mTransform = ((ReversibleTransformInteractable)target).transform;
            initialPosition = mTransform.position;
            initialRotation = mTransform.rotation;
            initialScale = mTransform.localScale;

            // Cache serialized properties for efficient access
            changePositionProperty = serializedObject.FindProperty("requireChangePosition");
            changeRotationProperty = serializedObject.FindProperty("requireChangeRotation");
            changeScaleProperty = serializedObject.FindProperty("requireChangeScale");

            newPositionProperty = serializedObject.FindProperty("newPosition");
            newRotationProperty = serializedObject.FindProperty("newRotation");
            newScaleProperty = serializedObject.FindProperty("newScale");

            thresholdPositionProperty = serializedObject.FindProperty("thresholdPosition");
            thresholdRotationProperty = serializedObject.FindProperty("thresholdRotation");
            thresholdScaleProperty = serializedObject.FindProperty("thresholdScale");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            using (new EditorGUI.IndentLevelScope())
            {
                if (changePositionProperty.boolValue)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(newPositionProperty, new GUIContent("New Position"));
                    EditorGUILayout.PropertyField(thresholdPositionProperty, new GUIContent("Position Stop threshold"));
                }

                if (changeRotationProperty.boolValue)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(newRotationProperty, new GUIContent("New Rotation"));
                    EditorGUILayout.PropertyField(thresholdRotationProperty, new GUIContent("Rotation Stop threshold"));
                }

                if (changeScaleProperty.boolValue)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(newScaleProperty, new GUIContent("New Scale"));
                    EditorGUILayout.PropertyField(thresholdScaleProperty, new GUIContent("Scale Stop threshold"));
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Move to initial"))
            {
                MoveToInitial();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Register New Position, Rotation, Scale"))
            {
                RegisterNewValues();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void MoveToInitial()
        {
            Transform mTransform = ((ReversibleTransformInteractable)target).transform;

            if (changePositionProperty.boolValue)
                mTransform.position = initialPosition;

            if (changeRotationProperty.boolValue)
                mTransform.rotation = initialRotation;

            if (changeScaleProperty.boolValue)
                mTransform.localScale = initialScale;
        }


        private void RegisterNewValues()
        {
            var targetComponent = (ReversibleTransformInteractable)target;
            var transform = targetComponent.transform;
            newPositionProperty.vector3Value = transform.position;
            newRotationProperty.quaternionValue = transform.rotation;
            newScaleProperty.vector3Value = transform.localScale;
            // serializedObject.ApplyModifiedProperties();
        }
    }
}