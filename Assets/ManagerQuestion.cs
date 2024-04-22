using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class ManagerQuestion : MonoBehaviour
{
    DatabaseReference reference;
    List<Question> listQuestion = new List<Question>();

    public TextMeshProUGUI textQuestion;
    public TextMeshProUGUI textOptionA;
    public TextMeshProUGUI textOptionB;

    public GameObject QuestionManager;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;

            UploadQuestions();
        });

        QuestionManager.SetActive(false);

        

    }

    void UploadQuestions()
    {
        reference.Child("questions").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsFaulted)
            {
                Debug.LogError("Eroare la preluarea intrebarilor: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                foreach (DataSnapshot questionSanpshot in snapshot.Children) 
                {
                    Question question = new Question();
                    question.text = questionSanpshot.Child("Text").Value.ToString();
                    question.OptionA = questionSanpshot.Child("OptionA").Value.ToString();
                    question.OptionB = questionSanpshot.Child("OptionB").Value.ToString();
                    question.CorrectOption = questionSanpshot.Child("CorrectOption").Value.ToString();

                    listQuestion.Add(question);

                }

                QuestionRandom();
            }

        });

      
    }

    void QuestionRandom()
    {
        if(listQuestion.Count > 0)
        {
            int index = Random.Range(0, listQuestion.Count);
            Question randomQuestion = listQuestion[index];

            textQuestion.text = randomQuestion.text;
            textOptionA.text = randomQuestion.OptionA;
            textOptionB.text = randomQuestion.OptionB;

            listQuestion.RemoveAt(index);

        }
        else
        {
            Debug.LogWarning("Lista de intrebari este goala");
        }
    }


    public void CheckOption(string selectedOption, string correctOption)
    {
        if (selectedOption == correctOption)
        {
            textQuestion.text = "Raspuns corect";

           
        }
        else
        {
            textQuestion.text = "Raspuns gresit";


        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            QuestionRandom();

            QuestionManager.SetActive(true);
        }
    }

}
public class Question
{
    public string text;
    public string OptionA;
    public string OptionB;
    public string CorrectOption;
}
