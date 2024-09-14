using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoConection : MonoBehaviour
{
    private SerialPort serialPort = new SerialPort("COM8", 9600); // 提高波特率

    GameObject HumanObject;
    Fighter HumanFighter;
    public float HumanHealth;

    public FighterStates HumanCurrentState;

    GameObject AIObject;
    Fighter AIFighter;
    public float AIHealth;

    public FighterStates AiCurrentState;

    public float outputDuration = 3.0f; // 输出值持续时间（秒）

    private float previousHumanHealth; // 记录上一次的人类健康值
    private float previousAIHealth; // 记录上一次的AI健康值
    private float timeSinceLastCheck; // 记录自上次检查以来的时间

    void Start()
    {
        serialPort.Open();

        HumanObject = GameObject.Find("HumanFighter");
        HumanFighter = HumanObject.GetComponent<Fighter>();

        AIObject = GameObject.Find("AIFighter");
        AIFighter = AIObject.GetComponent<Fighter>();

        // 初始化上一次的健康值
        previousHumanHealth = HumanFighter.health;
        previousAIHealth = AIFighter.health;
    }

    void Update()
    {
        HumanHealth = HumanFighter.health;
        HumanCurrentState = HumanFighter.currentState;

        AIHealth = AIFighter.health;
        AiCurrentState = AIFighter.currentState;

        if (serialPort.IsOpen)
        {
            // 监听键盘输入
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                serialPort.WriteLine("1");
                Debug.Log("Sent: 1");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                serialPort.WriteLine("0");
                Debug.Log("Sent: 0");
            }

            // 启动协程处理Arduino通信
            StartCoroutine(HandleArduinoCommunication());
        }
    }

    private IEnumerator HandleArduinoCommunication()
    {
        // 更新自上次检查以来的时间
        timeSinceLastCheck += Time.deltaTime;

        // 仅在过去0.5秒后进行检查
        if (timeSinceLastCheck >= 0.2f)
        {
            // 保存当前健康值
            float tempHumanHealth = HumanHealth;
            float tempAIHealth = AIHealth;

            // 重置时间计时器
            timeSinceLastCheck = 0f;

            // 检查AI的状态和人类的
            if (AiCurrentState == FighterStates.KICK && tempHumanHealth < previousHumanHealth)
            {
                serialPort.WriteLine("1");
                Debug.Log("Sent: 1");
                yield return new WaitForSeconds(outputDuration);
                serialPort.WriteLine("0");
                Debug.Log("Sent: 0");
            }
            else if (AiCurrentState == FighterStates.PUNCH && tempHumanHealth < previousHumanHealth)
            {
                serialPort.WriteLine("2");
                Debug.Log("Sent: 2");
                yield return new WaitForSeconds(outputDuration);
                serialPort.WriteLine("0");
                Debug.Log("Sent: 0");
            }
            else if (HumanCurrentState == FighterStates.KICK && tempAIHealth < previousAIHealth)
            {
                serialPort.WriteLine("3");
                Debug.Log("Sent: 3");
                yield return new WaitForSeconds(outputDuration);
                serialPort.WriteLine("0");
                Debug.Log("Sent: 0");
            }
            else if (HumanCurrentState == FighterStates.PUNCH && tempAIHealth < previousAIHealth)
            {
                serialPort.WriteLine("4");
                Debug.Log("Sent: 4");
                yield return new WaitForSeconds(outputDuration);
                serialPort.WriteLine("0");
                Debug.Log("Sent: 0");
            }

            // 更新上一次的健康值
            previousHumanHealth = tempHumanHealth;
            previousAIHealth = tempAIHealth;
        }
    }

    void OnApplicationQuit()
    {
        serialPort.Close();
    }
}
