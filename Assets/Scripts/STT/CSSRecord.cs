using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;



public class CSSRecord : MonoBehaviour
{
    private string _microphoneID = null;
    private AudioClip _recording = null;
    private int _recordingLengthSec = 15;
    private int _recordingHZ = 22050;
    // Force save as 16-bit .wav
    const int BlockSize_16Bit = 2;
    public bool Flag = false;
    public string sentence;
    public bool audioclick = false;



    GameObject _renderder;
    public GameObject _Play;
    public GameObject _Stop;

    //  public Image recordingrenderer;
    public Sprite Recording_Icon;
    public Sprite StopRecording_Icon;


    // ����� ���(Kor)�� �� �ڿ� ����
    string url = "https://naveropenapi.apigw.ntruss.com/recog/v1/stt?lang=Kor";

    // �޾ƿ� ���� �����ϰ� �����ϱ� ���� JSON ����
    [Serializable]
    public class VoiceRecognize
    {
        public string text;
    }

    private void Start()
    {
        _microphoneID = Microphone.devices[0];
    }


    // ��ư�� OnPointerDown �� �� ȣ��
    public void startRecording()
    {
        Debug.Log("start recording");
        _recording = Microphone.Start(_microphoneID, false, _recordingLengthSec, _recordingHZ);
        //	recordingrenderer.sprite = Recording_Icon; // Recording Icon (Red)
        //	audioclick = true;
        /*
        if(!audioclick){
             Debug.Log("start recording");
             _recording = Microphone.Start(_microphoneID, false, _recordingLengthSec, _recordingHZ);
             recordingrenderer.sprite = Recording_Icon; // Recording Icon (Red)
             audioclick = true;

        }else{

        // Recording audio
            if (Microphone.IsRecording(_microphoneID))
        {
            Microphone.End(_microphoneID);

            recordingrenderer.sprite = StopRecording_Icon; // Stop Recording Icon (Blue)
            Debug.Log("stop recording");
            if (_recording == null)
            {
                Debug.LogError("nothing recorded");
                return;
            }
            // audio clip to byte array
            byte[] byteData = getByteFromAudioClip(_recording);


            // ������ audioclip api ������ ����
            StartCoroutine(PostVoice(url, byteData));
        }
        // ���� ����
        audioclick = false;
        return;

        }
        */

        _Play.SetActive(false);
        _Stop.SetActive(true);
    }
    public void StopRecording()
    {
        // Recording audio
        if (Microphone.IsRecording(_microphoneID))
        {
            Microphone.End(_microphoneID);

            //  recordingrenderer.sprite = StopRecording_Icon; // Stop Recording Icon (Blue)
            Debug.Log("stop recording");
            if (_recording == null)
            {
                Debug.LogError("nothing recorded");
                return;
            }
            // audio clip to byte array
            byte[] byteData = getByteFromAudioClip(_recording);


            // ������ audioclip api ������ ����
            StartCoroutine(PostVoice(url, byteData));
        }
        // ���� ����
        //   audioclick = false;
        _Play.SetActive(true);
        _Stop.SetActive(false);
        return;

    }



    private byte[] getByteFromAudioClip(AudioClip audioClip)
    {
        MemoryStream stream = new MemoryStream();
        const int headerSize = 44;
        ushort bitDepth = 16;


        int fileSize = audioClip.samples * BlockSize_16Bit + headerSize;


        // audio clip�� �������� file stream�� �߰�(��ũ ���� �Լ� ����)
        WriteFileHeader(ref stream, fileSize);
        WriteFileFormat(ref stream, audioClip.channels, audioClip.frequency, bitDepth);
        WriteFileData(ref stream, audioClip, bitDepth);

        // stream�� array���·� �ٲ�
        byte[] bytes = stream.ToArray();
        return bytes;
    }

    #region write .wav file functions
    private static int WriteFileHeader(ref MemoryStream stream, int fileSize)
    {
        int count = 0;
        int total = 12;

        // riff chunk id
        byte[] riff = Encoding.ASCII.GetBytes("RIFF");
        count += WriteBytesToMemoryStream(ref stream, riff, "ID");

        // riff chunk size
        int chunkSize = fileSize - 8; // total size - 8 for the other two fields in the header
        count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(chunkSize), "CHUNK_SIZE");

        byte[] wave = Encoding.ASCII.GetBytes("WAVE");
        count += WriteBytesToMemoryStream(ref stream, wave, "FORMAT");

        // Validate header
        Debug.AssertFormat(count == total, "Unexpected wav descriptor byte count: {0} == {1}", count, total);

        return count;
    }

    private static int WriteFileFormat(ref MemoryStream stream, int channels, int sampleRate, UInt16 bitDepth)
    {
        int count = 0;
        int total = 24;

        byte[] id = Encoding.ASCII.GetBytes("fmt ");
        count += WriteBytesToMemoryStream(ref stream, id, "FMT_ID");

        int subchunk1Size = 16; // 24 - 8
        count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(subchunk1Size), "SUBCHUNK_SIZE");

        UInt16 audioFormat = 1;
        count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(audioFormat), "AUDIO_FORMAT");

        UInt16 numChannels = Convert.ToUInt16(channels);
        count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(numChannels), "CHANNELS");

        count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(sampleRate), "SAMPLE_RATE");

        int byteRate = sampleRate * channels * BytesPerSample(bitDepth);
        count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(byteRate), "BYTE_RATE");

        UInt16 blockAlign = Convert.ToUInt16(channels * BytesPerSample(bitDepth));
        count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(blockAlign), "BLOCK_ALIGN");

        count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(bitDepth), "BITS_PER_SAMPLE");

        // Validate format
        Debug.AssertFormat(count == total, "Unexpected wav fmt byte count: {0} == {1}", count, total);

        return count;
    }

    private static int WriteFileData(ref MemoryStream stream, AudioClip audioClip, UInt16 bitDepth)
    {
        int count = 0;
        int total = 8;

        // Copy float[] data from AudioClip
        float[] data = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(data, 0);

        byte[] bytes = ConvertAudioClipDataToInt16ByteArray(data);

        byte[] id = Encoding.ASCII.GetBytes("data");
        count += WriteBytesToMemoryStream(ref stream, id, "DATA_ID");

        int subchunk2Size = Convert.ToInt32(audioClip.samples * BlockSize_16Bit); // BlockSize (bitDepth)
        count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(subchunk2Size), "SAMPLES");

        // Validate header
        Debug.AssertFormat(count == total, "Unexpected wav data id byte count: {0} == {1}", count, total);

        // Write bytes to stream
        count += WriteBytesToMemoryStream(ref stream, bytes, "DATA");

        // Validate audio data
        Debug.AssertFormat(bytes.Length == subchunk2Size, "Unexpected AudioClip to wav subchunk2 size: {0} == {1}", bytes.Length, subchunk2Size);

        return count;
    }

    private static byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
    {
        MemoryStream dataStream = new MemoryStream();

        int x = sizeof(Int16);

        Int16 maxValue = Int16.MaxValue;

        int i = 0;
        while (i < data.Length)
        {
            dataStream.Write(BitConverter.GetBytes(Convert.ToInt16(data[i] * maxValue)), 0, x);
            ++i;
        }
        byte[] bytes = dataStream.ToArray();

        // Validate converted bytes
        Debug.AssertFormat(data.Length * x == bytes.Length, "Unexpected float[] to Int16 to byte[] size: {0} == {1}", data.Length * x, bytes.Length);

        dataStream.Dispose();

        return bytes;
    }

    private static int WriteBytesToMemoryStream(ref MemoryStream stream, byte[] bytes, string tag = "")
    {
        int count = bytes.Length;
        stream.Write(bytes, 0, count);
        //Debug.LogFormat ("WAV:{0} wrote {1} bytes.", tag, count);
        return count;
    }

    #endregion

    private static int BytesPerSample(UInt16 bitDepth)
    {
        return bitDepth / 8;
    }


    private IEnumerator PostVoice(string url, byte[] data)
    {
        // request ����
        WWWForm form = new WWWForm();
        UnityWebRequest request = UnityWebRequest.Post(url, form);


        // ��û ��� ����
        request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", "wmd38nv1fh");
        request.SetRequestHeader("X-NCP-APIGW-API-KEY", "Stbjc6SGyVef8HXFtmlbDso4kARnsOcibEf1qTGi");
        request.SetRequestHeader("Content-Type", "application/octet-stream");


        // �ٵ� ó�������� ��ģ Audio Clip data�� �Ǿ���
        request.uploadHandler = new UploadHandlerRaw(data);


        // ��û�� ���� �� response�� ���� ������ ���
        yield return request.SendWebRequest();


        // ���� response�� ����ִٸ� error
        if (request == null)
        {
            Debug.LogError(request.error);
        }
        else
        {
            // json ���·� ���� {"text":"�νİ��"}
            string message = request.downloadHandler.text;
            VoiceRecognize voiceRecognize = JsonUtility.FromJson<VoiceRecognize>(message);


            Debug.Log("Voice Server responded: " + voiceRecognize.text);
            // Voice Server responded: �νİ��
            sentence = voiceRecognize.text;

        }
    }

}
