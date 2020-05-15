using UnityEngine;

public class ProblemGenerator
{
    //Variables used to generate the two random numbers used to create the problem
    protected int termA;
    protected int termB;

    //Determines where position of the input field and the number text
    public int pos;

    //Variables used for holding the problem's sum and the other product of the problem
    public int sum;
    public int num;

    //Variable used to hold the number the user will be solving for
    public int answer;

    //Variable used for setting the text of the operator
    public char sign;

    //Variables used for holding the lowest and highest numbers to be generated (ex: between 1-3)
    protected int minNum;
    protected int maxNum;

    //Create the class's constructor; it needs the min and max nums
    public ProblemGenerator(int min, int max)
    {
        minNum = min;
        maxNum = max;
    }

    //Generator two random numbers using the min and max num ranges
    protected void generateTerms()
    {
        //Generate our two terms using the min and max numbers for the range
        termA = Random.Range(minNum, maxNum + 1);
        termB = Random.Range(minNum, maxNum + 1);
    }

    protected void addProblem()
    {
        //Get the sum of the two numbers
        sum = termA + termB;

        //The pos will be a 50/50 value to determine which term the player will be solving for
        pos = Random.Range(0, 2);

        //Set the answer and the num's values
        setAnswer();
    }

    protected void subProblem()
    {
        //If we are not, this will literally handle like the addition problem generator above
        //If we are dealig with no negative numbers, this is necessary.

        //Set the sum  to termA - termB
        sum = termA - termB;

        //Check if the sum's negative.
        if (sum < 0)
        {
            //It's negative so make it positive.
            sum *= -1;

            //termB > termA
            //termB - termA = sum
            //Determine which term will be the given number for the problem, and which is the answer and set the pos variable
            if (Random.Range(0, 2) == 1)
            {
                num = termB;
                answer = termA;
                pos = 1;
            }
            else
            {
                num = termA;
                answer = termB;
                pos = 0;
            }
        }
        else
        {
            //termA >= termB
            //termA - termB = sum

            //Determine which term will be the given number for the problem, and which is the answer and set the pos variable
            if (Random.Range(0, 2) == 1)
            {
                num = termB;
                answer = termA;
                pos = 0;
            }
            else
            {
                num = termA;
                answer = termB;
                pos = 1;
            }
        }
    }

    protected void multProblem()
    {
        //Set our sum to the product of the two terms
        sum = termA * termB;

        //The pos will be a 50/50 value to determine which term the player will be solving for
        pos = Random.Range(0, 2);

        //Set the answer and the num's values
        setAnswer();
    }

    protected void divProblem()
    {
        //This may not be necessary / wanted. For example: if we wanted to do 0/1, we'd have to fix this

        //Safety Checks: Make sure neither of the terms are 0
        //If they are, set them to 1.
        if (termA == 0) termA++;
        if (termB == 0) termB++;

        //Set our number to the product of the two terms
        num = termA * termB;

        //Set the answer and the num's values
        if (Random.Range(0, 2) <= 1)
        {
            sum = termA;
            answer = termB;
        }
        else
        {
            sum = termB;
            answer = termA;
        }

        //The position for dividing currently will always be 1.
        //We are only looking at the divisor, not the dividend for problems right now
        pos = 1;
    }

    //Determine which term will be the given number for the problem, and which is the answer using the pos variable
    protected void setAnswer()
    {
        if (pos == 0)
        {
            num = termA;
            answer = termB;
        }
        else
        {
            num = termB;
            answer = termA;
        }
    }

    //Set the ranges for the min and max numbers
    public void setNumRange(int min, int max)
    {
        minNum = min;
        maxNum = max;
    }

    //Used for randomly generating a new problem
    public void newProblem()
    {
        //Generate two new terms
        generateTerms();

        //NOTE: this will need a difficulty variable of some sort to determine what type of problems we'll be generating
        //Randomly choose our next problem type
        switch(Random.Range(0,4))
        {
            case 0:
                addProblem();
                sign = '+';
                break;
            case 1:
                subProblem();
                sign = '-';
                break;
            case 2:
                multProblem();
                sign = '*';
                break;
            default:
                divProblem();
                sign = '/';
                break;
        }
    }
}
