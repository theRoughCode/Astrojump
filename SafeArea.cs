using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
      RectTransform Panel;
      Rect LastSafeArea = new Rect (0, 0, 0, 0);

      void Awake ()
      {
          Panel = GetComponent<RectTransform> ();
          Refresh ();
      }

      void Update ()
      {
          Refresh ();
      }

      void Refresh ()
      {
          Rect safeArea = Screen.safeArea;

          if (safeArea != LastSafeArea) ApplySafeArea(safeArea);
      }

      void ApplySafeArea (Rect r)
      {
          LastSafeArea = r;

          // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
          Vector2 anchorMin = r.position;
          Vector2 anchorMax = r.position + r.size;
          anchorMin.x /= Screen.width;
          anchorMin.y /= Screen.height;
          anchorMax.x /= Screen.width;
          anchorMax.y /= Screen.height;
          Panel.anchorMin = anchorMin;
          Panel.anchorMax = anchorMax;
      }
}
