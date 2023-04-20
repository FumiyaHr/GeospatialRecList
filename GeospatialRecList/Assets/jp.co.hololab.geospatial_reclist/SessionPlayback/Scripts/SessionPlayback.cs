using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;

public class SessionPlayback : MonoBehaviour
{
    [SerializeField] private Button startPlaybackButton;
    [SerializeField] private Button stopPlaybackButton;
    [SerializeField] private Button startRecordingButton;
    [SerializeField] private Button stopRecordingButton;
    [SerializeField] private Button deleteCheckPanel_Yes;
    [SerializeField] private Dropdown dropdown;
    [SerializeField] private Text m_Log;

    [SerializeField] private ARSession m_Session;

    void Awake()
    {
        GetFileAll();
    }

    private void GetFileAll()
    {
        // �f�[�^�p�X����Recording���݂邩�m�F
        var folderPath = Application.persistentDataPath;
        var filePaths = Directory.GetFiles(folderPath, "*.mp4");
        var fileNames = Array.ConvertAll(filePaths, path => Path.GetFileName(path));
        // �h���b�v�_�E����������
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(fileNames));
    }

    static int GetRotation() => Screen.orientation switch
    {
        ScreenOrientation.Portrait => 0,
        ScreenOrientation.LandscapeLeft => 90,
        ScreenOrientation.PortraitUpsideDown => 180,
        ScreenOrientation.LandscapeRight => 270,
        _ => 0
    };


    private void Start()
    {
        startPlaybackButton.onClick.AddListener(() => StartPlayback());
        stopPlaybackButton.onClick.AddListener(() => StopPlayback());
        startRecordingButton.onClick.AddListener(() => StartRecording());
        stopRecordingButton.onClick.AddListener(() => StopRecording());
        deleteCheckPanel_Yes.onClick.AddListener(() => DeleteRecording());
    }

    private void StartPlayback()
    {
        var folderPath = Application.persistentDataPath;
        var m_Mp4Path = Path.Combine(folderPath, dropdown.options[dropdown.value].text);

        if (File.Exists(m_Mp4Path))
        {
            if (m_Session.subsystem is ARCoreSessionSubsystem subsystem)
            {
                var status = subsystem.StartPlayback(m_Mp4Path);
                Log($"playback�̏�ԁF{status}");
            }
            else
            {
                Log("not fount subsystem : StartPlayback");
            }
        }
    }

    private void StopPlayback()
    {
        if (m_Session.subsystem is ARCoreSessionSubsystem subsystem)
        {
            var status = subsystem.StopPlayback();
            Log($"playback�̏�ԁF{status}");
        }
        else
        {
            Log("not fount subsystem : StopPlayback");
        }
    }

    private void StartRecording()
    {
        Debug.Log(Application.persistentDataPath);
        Console.Write(Application.persistentDataPath);
        if (m_Session.subsystem is ARCoreSessionSubsystem subsystem)
        {
            using (var config = new ArRecordingConfig(subsystem.session))
            {
                // �^�C���X�^���v��t���ĐV����Session��Recording
                var m_NewMp4Path = Path.Combine(Application.persistentDataPath, $"arcore-session-{DateTime.Now:yyyyMMddHHHmmss}.mp4");
                config.SetMp4DatasetFilePath(subsystem.session, m_NewMp4Path);
                config.SetRecordingRotation(subsystem.session, GetRotation());
                var status = subsystem.StartRecording(config);
                Log($"StartRecording to {config.GetMp4DatasetFilePath(subsystem.session)} => {status}");
            }
        }
    }

    private void StopRecording()
    {
        if (m_Session.subsystem is ARCoreSessionSubsystem subsystem)
        {
            var status = subsystem.StopRecording();
            Log($"StopRecording() => {status}");
        }
        GetFileAll();
    }

    private void DeleteRecording()
    {
        var folderPath = Application.persistentDataPath;
        var m_Mp4Path = Path.Combine(folderPath, dropdown.options[dropdown.value].text);

        if (File.Exists(m_Mp4Path))
        {
            // �t�@�C���폜
            File.Delete(m_Mp4Path);
            // ���X�g�X�V
            GetFileAll();
        }
    }

    void Log(string msg)
    {
        Debug.Log(msg);
        m_Log.text = msg;
    }
}
