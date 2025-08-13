using System;
using System.Collections;
using UnityEngine;



public interface IDamageable
{
    void TakePhysicalDamage(int amount);
}

// UI�� ������ �� �ִ� PlayerCondition
// �ܺο��� �ɷ�ġ ���� ����� �̰��� ���ؼ� ȣ��. ���������� UI ������Ʈ ����.
public class PlayerCondition : MonoBehaviour, IDamageable
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition stamina { get { return uiCondition.stamina; } }

    Condition SpeedUp { get { return uiCondition.SpeedUp; } }

    //public float noHungerHealthDecay;   // hunger�� 0�϶� ����� �� (value > 0)

    public event Action onTakeDamage;   // Damage ���� �� ȣ���� Action


    private void Update()
    {
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (health.curValue <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Die()
    {
        Debug.Log("�÷��̾ �׾���.");
    }
    public void TakePhysicalDamage(int amount)
    {
        health.Subtract(amount);
        onTakeDamage?.Invoke();
    }
    public void StaminaUp(float amount)
    {
        stamina.Add(amount);
    }
    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }
        stamina.Subtract(amount);
        return true;
    }
    public void UseSpeedUp(float amount)
    {
        StartCoroutine(SpeedUpTime(amount));
    }

    IEnumerator SpeedUpTime(float amount)
    {
        if(CharacterManager.Instance.Player.controller.walkSpeed == 6f || 
           CharacterManager.Instance.Player.controller.runSpeed == 10.5f)
        {
            Debug.Log("�̹� ���ǵ� �� �����Դϴ�.");
            yield break;
        }
        CharacterManager.Instance.Player.controller.walkSpeed *= 1.5f;
        CharacterManager.Instance.Player.controller.runSpeed *= 1.5f;
        Debug.Log(amount + "�� ��ŭ ���ǵ� ��!");
        yield return new WaitForSeconds(amount);
        CharacterManager.Instance.Player.controller.walkSpeed /= 1.5f;
        CharacterManager.Instance.Player.controller.runSpeed /= 1.5f;
    }
}