using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    #region Field

    [SerializeField] Transform container;
    [SerializeField] GameObject prefabWhite;
    [SerializeField] GameObject prefabBlue;
    [SerializeField] GameObject prefabYellow;
    [SerializeField] GameObject prefabGreen;
    private Image[] fillers;

    [SerializeField] TextMeshProUGUI currentText;
    [SerializeField] TextMeshProUGUI totalText;

    float current; // current sum points
    float total; // maximum sum points

    // Current Points
    [Header("Basic")]
    [SerializeField] float health;      // 普通血量
    [SerializeField] float shield;      // 护盾血量
    [SerializeField] float armor;       // 护甲血量
    [SerializeField] float overhealth;  // 过量治疗

    // Maximum Points
    float maxHealth;
    float maxShield;
    float maxArmor;
    float maxOverHealth;

    [Header("Config")]
    [SerializeField] float leapSpeed = 5f;

    #endregion

    void Start() {
        InitializeHealthInfo();
        InitializeHealthBar();
    }

    // Just for debugging
    void Update() {
        UpdateHealthBar();
        if (current < 0) current = 0;
        if (current > total) current = total;
    }

    void InitializeHealthInfo() {
        maxHealth = health;
        maxShield = shield;
        maxArmor = armor;
        maxOverHealth = overhealth;
    }

    void InitializeHealthBar() {
        int white = Mathf.CeilToInt(maxHealth / 25f);
        int blue = white + Mathf.CeilToInt(maxShield / 25f);
        int yellow = blue + Mathf.CeilToInt(maxArmor / 25f);
        int green = yellow + Mathf.CeilToInt(maxOverHealth / 25f);
        int n = green;
        fillers = new Image[n];
        for (int i = 0; i < n; i++) {
            GameObject block;
            if (i >= 0 && i < white) block = Instantiate(prefabWhite, container);
            else if (i < blue) block = Instantiate(prefabBlue, container);
            else if (i < yellow) block = Instantiate(prefabYellow, container);
            else block = Instantiate(prefabGreen, container);
            fillers[i] = block.transform.GetChild(0).GetComponent<Image>();
        }
    }

    public void UpdateHealthBar() {
        current = health + shield + armor + overhealth;
        total = maxHealth + maxShield + maxArmor + maxOverHealth;

        var index = Mathf.FloorToInt(current / 25);
        var res = current % 25 / 25f; // 不完全方块的填充比例

        for (int i = 0; i < fillers.Length; i++) {
            if (i < index) {
                fillers[i].fillAmount = 1;   // 完全填充
            } else if (i == index) {
                fillers[i].fillAmount = res; // 部分填充
            } else {
                fillers[i].fillAmount = 0;   // 空状态
            }
        }
        currentText.SetText(current.ToString("f0"));
        totalText.SetText(" / " + total.ToString("f0"));
    }

    public void TakeDamage(float amount) {
        // Green
        if (overhealth > 0) {
            float damgeToApply = Mathf.Min(amount, overhealth);
            overhealth -= damgeToApply;
            amount -= damgeToApply;
        }
        // Yellow
        if (amount > 0 && armor > 0) {
            float mitigatedDamage = amount * 0.5f; // 假设护甲减免50%
            float remainingDamage = Mathf.Max(0, mitigatedDamage - armor);
            armor = Mathf.Max(0, armor - mitigatedDamage);
            amount = remainingDamage;

        }
        // Blue
        if (amount > 0 && shield > 0) {
            float damgeToApply = Mathf.Min(amount, shield);
            shield -= damgeToApply;
            amount -= damgeToApply;
        }
        // White
        if (amount > 0 && health > 0) {
            health -= amount;
            if (health < 0) {
                health = 0;
                Debug.Log("Player Death");
            }
        }
        UpdateHealthBar();
    }

    public void Heal(float amount) {
        // White
        if (health < maxHealth) {
            float healToApply = Mathf.Min(amount, maxHealth - health);
            health += healToApply;
            amount -= healToApply;
        }
        // Blue
        if (amount > 0 && shield < maxShield) {
            float healToApply = Mathf.Min(amount, maxShield - shield);
            shield += healToApply;
            amount -= healToApply;
        }
        // Yellow
        if (amount > 0 && armor < maxArmor) {
            float healToApply = Mathf.Min(amount, maxArmor - armor);
            armor += healToApply;
            amount -= healToApply;
        }
        UpdateHealthBar();
    }

    public float GetCurrentHealthRatio() {
        return health / maxHealth;
    }
}
