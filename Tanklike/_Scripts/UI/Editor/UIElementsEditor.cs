using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TankLike.UI.Styles;
using UnityEngine.UI;
using TMPro;

namespace TankLike.UI.Styles
{
    public class UIElementsEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/UI/Image_TankLike")]
        public static void CreateCustomUIImage()
        {
            GameObject newImage = new GameObject("Image");

            newImage.AddComponent<Image>();
            newImage.AddComponent<StylesImage>();
            newImage.GetComponent<StylesImage>().SetUp();

            // parent the new GameObject to the selected GameObject
            GameObject selectedGameObject = Selection.activeGameObject;

            if (selectedGameObject != null)
            {
                newImage.transform.SetParent(selectedGameObject.transform, false);
            }

            // Select and focus on the newly created GameObject
            Selection.activeGameObject = newImage;
            EditorGUIUtility.PingObject(newImage);
        }

        [MenuItem("GameObject/UI/Text_TankLike")]
        public static void CreateCustomUIText()
        {
            GameObject newImage = new GameObject("Text");

            newImage.AddComponent<TextMeshProUGUI>();
            newImage.AddComponent<StylesText>();
            newImage.GetComponent<StylesText>().SetUp();

            // parent the new GameObject to the selected GameObject
            GameObject selectedGameObject = Selection.activeGameObject;

            if (selectedGameObject != null)
            {
                newImage.transform.SetParent(selectedGameObject.transform, false);
            }

            // Select and focus on the newly created GameObject
            Selection.activeGameObject = newImage;
            EditorGUIUtility.PingObject(newImage);
        }
    }
}
