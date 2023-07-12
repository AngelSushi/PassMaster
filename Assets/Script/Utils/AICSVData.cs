using System;
using System.Text;
using UnityEngine;
using System.IO;

public class AICSVData : MonoBehaviour
{

    [SerializeField] private string filePath;
    [SerializeField] private string columns;
    
    private StringBuilder _stringBuilder;

    private void Start()
    {
        _stringBuilder = new StringBuilder(columns);
      //  Time.timeScale = 10f;
    } 

    private void OnDestroy() => Save();
    public void AddLine(string name, GameController.Difficulty difficulty, float timer, bool isBurned) => _stringBuilder.AppendFormat($"{name};{difficulty.ToString()};{timer.ToString("F6")};{isBurned.ToString()};");
    public void Save()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.Write(_stringBuilder.ToString());
        }
    } 
    
}
