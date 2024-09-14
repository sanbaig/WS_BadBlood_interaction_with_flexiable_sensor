using System.IO.Ports;
using UnityEngine;

public class TestConnection : MonoBehaviour
{
    private SerialPort serialPort = new SerialPort("COM8", 9600); // 提高波特率

    void Start()
    {
        serialPort.Open();
    }

    void Update()
    {
        if (serialPort.IsOpen)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                serialPort.WriteLine("1");
                Debug.Log("Sent: 1");
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                serialPort.WriteLine("0");
                Debug.Log("Sent: 0");
            }
        }
    }

    void OnApplicationQuit()
    {
        serialPort.Close();
    }
}
