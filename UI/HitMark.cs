using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HitMark : MonoBehaviour {
  [SerializeField] GameObject bar; // HitMark Single Prefab
  [SerializeField] GameObject block; // HitMark Block Prefab
  [SerializeField] float duration = 0.8f;

  [Header("Color")]
  [SerializeField] Color backgroundColor = Color.grey;
  [SerializeField] Color markerColor = Color.red;

  [Header("Debug")]
  [SerializeField] float length = 140f;
  [SerializeField] float thickness = 6.5f;
  [SerializeField] int offset = 15;
  [SerializeField] float space = 3.5f;
  [SerializeField] bool canChange = false;

  GameObject[] bars;
  float timer;

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
    gameObject.SetActive(false);
  }

  void Update() {
    timer += Time.deltaTime;
    if (timer > duration) {
      Debug.Log($"timer: {timer}, duration: {duration}");
      gameObject.SetActive(false);
    }
    if (canChange) {
      ChangeHitMaker();
      ChangeBlockColor(0.8f);
    }
  }

  public void Show(float ratio) {
    timer = 0;
    gameObject.SetActive(true);
    ChangeBlockColor(ratio);
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

      if (trans.sizeDelta != new Vector2(length, thickness)) {
        trans.sizeDelta = new Vector2(length, thickness);
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

  void ChangeBlockColor(float ratio) {
    int index = Mathf.Clamp(10 - Mathf.CeilToInt(ratio * 10), 0, 10);
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