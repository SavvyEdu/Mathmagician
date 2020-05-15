using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public InputField inputFieldRef;

    //Variables to hold the text for each part of the problem
    public Text operatorText;
    public Text numText;
    public Text sumText;

    [SerializeField]
    int minNum;
    [SerializeField]
    int maxNum;

    //Variable for our problem generator object
    ProblemGenerator generator;

    [SerializeField]
    bool clear;

    //vectors to hold the base positions of the inputfield and the num text
    Vector2 numPos;
    Vector2 inputPos;
    bool swapped = false;

    void Start()
    {
        clear = true;

        //Get the positions of the input and num text
        inputPos = inputFieldRef.transform.position;
        numPos = numText.transform.position;

        //Create a new problem generator and set the number range
        generator = new ProblemGenerator(minNum, maxNum);

        //Create a new problem and update it's text
        newProblem();
    }

    //Create a new problem, and update the text values
    private void newProblem()
    {
        //Generate a new problem
        generator.newProblem();

        //Update our text
        operatorText.text = generator.sign + "";
        numText.text = generator.num.ToString();
        sumText.text = generator.sum.ToString();

        //Set the positions of the input and the num text
        SetPosition(generator.pos);
    }

    private void OnInput(string userInput)
    {
        if(userInput != "")
        {
            if (int.Parse(userInput) == generator.answer)
            {
                Debug.Log("You solved the problem!");

                newProblem();

               if(clear) inputFieldRef.text = " ";
            }
        }
    }

    private void SetPosition(int pos)
    {
        if(pos == 1)
        {
            if(!swapped)
            {
                Vector2 tmp = inputFieldRef.transform.position;
                inputFieldRef.transform.position = numText.transform.position;
                numText.transform.position = tmp;
                swapped = true;
            }
        }
        else
        {
            inputFieldRef.transform.position = inputPos;
            numText.transform.position = numPos;
            swapped = false;
        }
    }
}
