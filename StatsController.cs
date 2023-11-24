using UnityEngine;
using System.IO;
using System.Net.NetworkInformation;
using System;
using System.Text.RegularExpressions;

public class StatsController : MonoBehaviour
{
    private string filePath = "stats.csv";
    private int score;
    private int iScore;
    private int rank;
    private int reward;
    private int nextThreshold;
    private string macAddress;
    public int LevelIndex;

    private void Start()
    {
        WriteToFile(DateTime.Now, "S");
    }

    private void OnDestroy()
    {
        WriteToFile(DateTime.Now, "E");
    }

    private void GetMacAddress()
    {
        macAddress = "";
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adapter in nics)
        {
            PhysicalAddress address = adapter.GetPhysicalAddress();
            byte[] bytes = address.GetAddressBytes();
            for (int i = 0; i < bytes.Length; i++)
            {
                macAddress += bytes[i].ToString("X2");
                if (i != bytes.Length - 1)
                {
                    macAddress += ":";
                }
            }
            if (macAddress.Length > 0)
            {
                break;
            }
        }
    }

    private void WriteToFile(DateTime date, string inputOutput)
    {
        string id = PlayerPrefs.GetString("Player");
        string playerNom = PlayerPrefs.GetString("name:" + id);

        string reversedPlayerNom = playerNom;
        if (IsArabic(playerNom))
        {
            reversedPlayerNom = ReverseString(playerNom);
        }

        GetMacAddress();

        score = PlayerPrefs.GetInt("Player" + PlayerPrefs.GetString("Player") + "scoreTotal");
        iScore = PlayerPrefs.GetInt("Player" + PlayerPrefs.GetString("Player") + "RewardScore");
        rank = PlayerPrefs.GetInt("Player" + PlayerPrefs.GetString("Player") + "Rank");
        nextThreshold = PlayerPrefs.GetInt("Player" + PlayerPrefs.GetString("Player") + "NextThreshold");
        reward = PlayerPrefs.GetInt("Player" + PlayerPrefs.GetString("Player") + "RewardCounter");

        TimeSpan daysSince1900 = date - new DateTime(1900, 1, 1);
        int days = (int)daysSince1900.TotalDays + 2;
        TimeSpan timeSinceMidnight = date.TimeOfDay;
        int seconds = (int)timeSinceMidnight.TotalSeconds;

        StreamWriter fileWriter = new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));

        string csvData = string.Format("{0}; <seconds ;{1};>; {2}; {3}; {4}.{5}.{6}; {7}; <level ;{8};>; <score ;{9};>; <rank ;{10};>; <iScore ;{11};> ; <nextThreshold ;{12};>; <reward ;{13};>; <positives ;{14};>; <negatives ;{15};>",
            days, seconds, inputOutput, Application.version, macAddress, id, reversedPlayerNom, Main.controller._currentStrategy.name(), LevelIndex,
            score, rank, iScore, nextThreshold, reward, Main.model.Positives, Main.model.Negatives);
        fileWriter.WriteLine(csvData);

        fileWriter.Close();

    }

    bool IsArabic(string input)
    {
        const string arabicPattern = @"\p{IsArabic}";
        return Regex.IsMatch(input, arabicPattern);
    }

    string ReverseString(string input)
    {
        char[] charArray = input.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}