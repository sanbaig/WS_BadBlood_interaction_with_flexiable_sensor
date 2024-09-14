using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoConection : MonoBehaviour
{
    private SerialPort serialPort = new SerialPort("COM8", 9600); // ��߲�����

    GameObject HumanObject;
    Fighter HumanFighter;
    public float HumanHealth;

    public FighterStates HumanCurrentState;

    GameObject AIObject;
    Fighter AIFighter;
    public float AIHealth;

    public FighterStates AiCurrentState;

    public float outputDuration = 3.0f; // ���ֵ����ʱ�䣨�룩

    private float previousHumanHealth; // ��¼��һ�ε����ཡ��ֵ
    private float previousAIHealth; // ��¼��һ�ε�AI����ֵ
    private float timeSinceLastCheck; // ��¼���ϴμ��������ʱ��

    void Start()
    {
        serialPort.Open();

        HumanObject = GameObject.Find("HumanFighter");
        HumanFighter = HumanObject.GetComponent<Fighter>();

        AIObject = GameObject.Find("AIFighter");
        AIFighter = AIObject.GetComponent<Fighter>();

        // ��ʼ����һ�εĽ���ֵ
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
            // ������������
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

            // ����Э�̴���Arduinoͨ��
            StartCoroutine(HandleArduinoCommunication());
        }
    }

    private IEnumerator HandleArduinoCommunication()
    {
        // �������ϴμ��������ʱ��
        timeSinceLastCheck += Time.deltaTime;

        // ���ڹ�ȥ0.5�����м��
        if (timeSinceLastCheck >= 0.2f)
        {
            // ���浱ǰ����ֵ
            float tempHumanHealth = HumanHealth;
            float tempAIHealth = AIHealth;

            // ����ʱ���ʱ��
            timeSinceLastCheck = 0f;

            // ���AI��״̬�������
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

            // ������һ�εĽ���ֵ
            previousHumanHealth = tempHumanHealth;
            previousAIHealth = tempAIHealth;
        }
    }

    void OnApplicationQuit()
    {
        serialPort.Close();
    }
}
