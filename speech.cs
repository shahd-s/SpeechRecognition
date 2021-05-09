using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using System.Linq;
using Fungus;
public class speech : MonoBehaviour
{   

    private DictationRecognizer m_DictationRecognizer;
    public Fungus.Flowchart wordsaidoutFlow;
    public List<string> phrasesPossible;
    private Dictionary<int, List<string>> dic = new Dictionary<int, List<string>>();

    [Range(0,1)]
    public float minConfidence;
    public bool Done = false;
    public void changePhrasesPossible(int x) {

        phrasesPossible = dic[x];
    }
    void Start()
    {
        dic.Add(0, new List<string>() { "You don't have a job How will you pay me back", "Yeah sure you can" });
        m_DictationRecognizer = new DictationRecognizer();

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            Done = false;

            wordsaidoutFlow.SetBooleanVariable("Done", false);
            wordsaidoutFlow.SetStringVariable("SpokenPhrase", " ");
            Debug.LogFormat("Dictation result: {0}", text);
            string[] wordsSpoken = text.Split(null);
            List<string> saidWords = wordsSpoken.OfType<string>().ToList();

           
            List<int> pointsPerString = new List<int>();

            for(int i =0; i<phrasesPossible.Count; i++)
            {
                pointsPerString.Add( 0);
            }

            for (int i =0; i <phrasesPossible.Count; i++)
            { 
                string x = phrasesPossible[i];
                string[] xx = x.Split(null);
                List<string> Current = xx.OfType<string>().ToList();
                 

                for(int j =0; j<saidWords.Count; j++)
                if (Current.Contains(saidWords[j]))
                    pointsPerString[i]++;
            }

            int max = -999;
            int index = -1;
            for(int i =0; i<pointsPerString.Count; i++)
            {
                if (pointsPerString[i] > max)
                {
                    max = pointsPerString[i];
                    index = i;
                }

            }
            string x1 = phrasesPossible[index];
            string[] xx1 = x1.Split(null);
            List<string> Current1 = xx1.OfType<string>().ToList();

            if (max >= (int)Current1.Count * minConfidence)
            { Debug.Log("looks like what you said was " + phrasesPossible[index]);

                wordsaidoutFlow.SetStringVariable("SpokenPhrase", index.ToString());
            }
            else
            { Debug.Log("unrecognized");
                wordsaidoutFlow.SetStringVariable("SpokenPhrase", "-1");
            }
            m_DictationRecognizer.Stop();
            Done = true;
        };

       /* m_DictationRecognizer.DictationHypothesis += (text) =>
        {
            Debug.LogFormat("Dictation hypothesis: {0}", text);
            m_Hypotheses.text += text;
        };
       */
        m_DictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
            else Debug.Log("I'm done listening to you now");
           
        };

        m_DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };

       // m_DictationRecognizer.Start();
    }

    public void StartListening() {
        Done = false;

        wordsaidoutFlow.SetBooleanVariable("Done", false);
        Debug.Log("YOU WANT ME TO START LISTENING");
        m_DictationRecognizer.Start();
    }
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //    m_DictationRecognizer.Start();
        wordsaidoutFlow.SetBooleanVariable("Done", Done);
         
    }



}
