using System;
using System.Collections;
using UnityEngine;



public interface IDamageable
{
    void TakePhysicalDamage(int amount);
}

// UI를 참조할 수 있는 PlayerCondition
// 외부에서 능력치 변경 기능은 이곳을 통해서 호출. 내부적으로 UI 업데이트 수행.
public class PlayerCondition : MonoBehaviour, IDamageable
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition stamina { get { return uiCondition.stamina; } }

    Condition SpeedUp { get { return uiCondition.SpeedUp; } }

    //public float noHungerHealthDecay;   // hunger가 0일때 사용할 값 (value > 0)

    public event Action onTakeDamage;   // Damage 받을 때 호출할 Action


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
        Debug.Log("플레이어가 죽었다.");
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
            Debug.Log("이미 스피드 업 상태입니다.");
            yield break;
        }
        CharacterManager.Instance.Player.controller.walkSpeed *= 1.5f;
        CharacterManager.Instance.Player.controller.runSpeed *= 1.5f;
        Debug.Log(amount + "초 만큼 스피드 업!");
        yield return new WaitForSeconds(amount);
        CharacterManager.Instance.Player.controller.walkSpeed /= 1.5f;
        CharacterManager.Instance.Player.controller.runSpeed /= 1.5f;
    }
}