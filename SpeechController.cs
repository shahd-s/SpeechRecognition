using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
public class SpeechController : MonoBehaviour
{
    const string LANG_CODE = "en-US";

    [SerializeField]
    Text t;
    //The phrases that are valid for a user to say go here and can be initially set in the inspector
    //pre-fungus
    public List<string> phrasesPossible;

    //The fungus flowchart
    public Fungus.Flowchart wordsaidoutFlow;
    //This dictionary is for ease of integration with fungus. 
    private Dictionary<int, List<string>> dic = new Dictionary<int, List<string>>();

    //The confidence that I made that helps the code determine if what they said matches
    //any phrases in phrasesPossible.
    //For now, this is a percentage. So if you choose 0.5 (which 0.5-0.7 is best for longer sentances)
    //I check if it's correct by ensuring the words in the m_DictationRecognizer are >= minConfidence*the words
    //in the highest scoring phrasesPossible string.
    [Range(0, 1)]
    public float minConfidence;

    //For fungus
    public bool Done = false;

    //For fungus
    public void changePhrasesPossible(int x)
    {

        phrasesPossible = dic[x];
    }
    void setUp(string c)
    {
        SpeechToText.instance.Setting(c);
        TextToSpeech.instance.Setting(c, 1, 1);
    }
    // Start is called before the first frame update
    void Start()
    {

        dic.Add(0, new List<string>() { "You don't have a job How will you pay me back", "Yeah sure you can" });
        setUp(LANG_CODE);

        //register callbacks
        SpeechToText.instance.onResultCallback = onFinalSpeechResult;
        TextToSpeech.instance.onStartCallBack = onSpeakStart;
        TextToSpeech.instance.onDoneCallback = onSpeakStop;
    }

    public void StartSpeaking(string message)
    {
        TextToSpeech.instance.StartSpeak(message);
    }

    public void StopSpeaking()
    {
        TextToSpeech.instance.StopSpeak();
    }

    void onSpeakStart()
    {

        Debug.Log("speech started");
    }

    void onSpeakStop()
    {
        Debug.Log("speech stop");

    }



    //speech to text now

    public void StartListening()
    {
        //This is the method fungus calls
        Done = false;
        Debug.Log("Start Listening");
        wordsaidoutFlow.SetBooleanVariable("Done", false);
        SpeechToText.instance.StartRecording();
    }
    public void StopListening()
    {
        Done = false;
        Debug.Log("Stop Listening");
        wordsaidoutFlow.SetBooleanVariable("Done", false);
        SpeechToText.instance.StopRecording();
    }

    //this is the call back when word is ready
    void onFinalSpeechResult(string text)
    {
        t.text = text;
        #region For fungus
        Done = false;
        wordsaidoutFlow.SetBooleanVariable("Done", false);
        wordsaidoutFlow.SetStringVariable("SpokenPhrase", " ");
        #endregion

        //Logging the dictation result.
        Debug.LogFormat("Dictation result: {0}", text);

        //Splitting the words into a list of string so we can begin
        //deciphering if what was said matches any of phrasesPossible
        string[] wordsSpoken = text.Split(null);
        List<string> saidWords = wordsSpoken.OfType<string>().ToList();


        //creating a list of ints that will store the scores of each phrasePossible
        //the index of the highest scoring one is the phrasesPossible[i] that was
        //most likely to have been said.
        //This is validated further by minConfidence
        List<int> pointsPerString = new List<int>();

        //init counters to 0
        for (int i = 0; i < phrasesPossible.Count; i++)
        {
            pointsPerString.Add(0);
        }

        //begin analysis of which phrase against saidWords
        for (int i = 0; i < phrasesPossible.Count; i++)
        {
            //Put phrasesPossible[i] that we're checking
            //into its own list of strings (to get individual word)
            string x = phrasesPossible[i];
            string[] xx = x.Split(null);
            List<string> Current = xx.OfType<string>().ToList();

            //Count how many words in the user's spoken phrase match the current phrasesPossible
            //entry we're checking
            for (int j = 0; j < saidWords.Count; j++)
                if (Current.Contains(saidWords[j]))
                    pointsPerString[i]++;
        }

        //get the maximum score and the index it corresponds to
        int max = -999;
        int index = -1;
        for (int i = 0; i < pointsPerString.Count; i++)
        {
            if (pointsPerString[i] > max)
            {
                max = pointsPerString[i];
                index = i;
            }

        }

        //Split the winning phrasePossible into list of string
        string x1 = phrasesPossible[index];
        string[] xx1 = x1.Split(null);
        List<string> Current1 = xx1.OfType<string>().ToList();


        //Check if the winning phrasePossible was even valid to bein with
        //using minConfidence
        if (max >= (int)Current1.Count * minConfidence)
        {
            Debug.Log("looks like what you said was " + phrasesPossible[index]);

            wordsaidoutFlow.SetStringVariable("SpokenPhrase", index.ToString());
        }
        else
        {
            Debug.Log("unrecognized");
            wordsaidoutFlow.SetStringVariable("SpokenPhrase", "-1");
        }

        StopListening();
        //Alert fungus too
        Done = true;

    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //    m_DictationRecognizer.Start();
        wordsaidoutFlow.SetBooleanVariable("Done", Done);

    }
}
