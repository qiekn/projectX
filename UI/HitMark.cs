using UnityEngine;
using UnityEngine.UI;

public class HitMark : MonoBehaviour {
  [SerializeField] GameObject bar; // HitMark Single Prefab
  [SerializeField] GameObject block; // HitMark Block Prefab

  [Header("Color")]
  [SerializeField] Color backgroundColor = Color.grey;
  [SerializeField] Color markerColor = Color.red;

  [Header("Debug")]
  [SerializeField] float width = 140f;
  [SerializeField] float height = 6.5f;
  [SerializeField] int offset = 15;
  [SerializeField] float space = 3.5f;

  [Header("Linker")]
  [SerializeField] GameObject targetHealthBar;
  [SerializeField] bool toggleLink = true;
  [Range(0, 1)]
  [SerializeField] float targetCurrentHealthRatio = 1f;

  GameObject[] bars;
  void Start() {
    bars = new GameObject[4];
    for (int i = 0; i < 4; i++) {
      bars[i] = Instantiate(bar, gameObject.transform);
      var trans = bars[i].GetComponent<RectTransform>();
      trans.rotation = Quaternion.Euler(0, 0, 45 + i * 90);
      for (int j = 0; j < 10; j++) {
        Instantiate(block, bars[i].transform);
      }
    }
  }

  void Update() {
    ChangeHitMaker();
    ChangeBlockColor(targetCurrentHealthRatio);
    if (toggleLink && targetHealthBar != null) {
      targetCurrentHealthRatio = targetHealthBar.GetComponent<HealthBar>().GetCurrentHealthRatio();
    }
  }

  void ChangeHitMaker() {
    for (int i = 0; i < 4; i++) {
      var layout = bars[i].GetComponent<HorizontalLayoutGroup>();
      var trans = bars[i].GetComponent<RectTransform>();
      bool layoutChanged = false;

      // Check if the layout properties have changed
      if (layout.padding.left != offset) {
        layout.padding.left = offset;
        layoutChanged = true;
      }

      if (layout.spacing != space) {
        layout.spacing = space;
        layoutChanged = true;
      }

      if (trans.sizeDelta != new Vector2(width, height)) {
        trans.sizeDelta = new Vector2(width, height);
        layoutChanged = true;
      }

      // If any layout property has changed, trigger a rebuild
      if (layoutChanged) {
        LayoutRebuilder.MarkLayoutForRebuild(trans);
      }
    }
    /* You can also call this in OnValidate to ensure layout rebuild in the editor
    void OnValidate() {
      for (int i = 0; i < 4; i++) {
        var trans = bars[i].GetComponent<RectTransform>();
        LayoutRebuilder.MarkLayoutForRebuild(trans);
      }
    }
    */
  }

  void ChangeBlockColor(float x) {
    int index = Mathf.Clamp(10 - Mathf.CeilToInt(x * 10), 0, 10);
    for (int i = 0; i < 4; i++) {
      var blocks = bars[i].GetComponentsInChildren<Image>();
      for (int j = 0; j < blocks.Length; j++) {
        if (j < index) {
          // ColorUtility.TryParseHtmlString("#FF4545", out color);
          blocks[j].color = markerColor;
        } else {
          blocks[j].color = backgroundColor;
        }
      } // inner for
    } // outer for
  }
}