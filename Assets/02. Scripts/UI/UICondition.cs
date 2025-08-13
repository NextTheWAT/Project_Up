using UnityEngine;

// ���� Condition ���� �������� �̷���� UICondition
public class UICondition : MonoBehaviour
{
    public Condition health;
    public Condition stamina;
    public Condition SpeedUp;

    private void Start()
    {
        CharacterManager.Instance.Player.condition.uiCondition = this;
    }
}