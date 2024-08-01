using System;
using System.Collections.Generic;

public class Answer : ICloneable, IComparable<Answer>
{
    public int AnswerId { get; set; }
    public string AnswerText { get; set; }

    public Answer(int answerId, string answerText)
    {
        AnswerId = answerId;
        AnswerText = answerText;
    }

    public object Clone()
    {
        return new Answer(AnswerId, AnswerText);
    }

    public int CompareTo(Answer other)
    {
        return AnswerId.CompareTo(other.AnswerId);
    }

    public override string ToString()
    {
        return $"{AnswerId}: {AnswerText}";
    }
}

public abstract class Question : ICloneable
{
    public string QuestionHead { get; set; }
    public string QuestionText { get; set; }
    public int Mark { get; set; }
    public Answer[] Answers { get; set; }
    public int CorrectAnswerIndex { get; set; }

    protected Question(string questionHead, string questionText, int mark, Answer[] answers, int correctAnswerIndex)
    {
        QuestionHead = questionHead;
        QuestionText = questionText;
        Mark = mark;
        Answers = answers;
        CorrectAnswerIndex = correctAnswerIndex;
    }

    public abstract void ShowQuestion();

    public abstract object Clone();
}

public class TrueFalseQuestion : Question
{
    public TrueFalseQuestion(string questionHead, string questionText, int mark, bool correctAnswer)
        : base(questionHead, questionText, mark, new Answer[] { new Answer(1, "True"), new Answer(2, "False") }, correctAnswer ? 0 : 1)
    {
    }

    public override void ShowQuestion()
    {
        Console.WriteLine(QuestionHead);
        Console.WriteLine(QuestionText);
        Console.WriteLine("1. True");
        Console.WriteLine("2. False");
    }

    public override object Clone()
    {
        return new TrueFalseQuestion(QuestionHead, QuestionText, Mark, CorrectAnswerIndex == 0);
    }
}

public class MCQQuestion : Question
{
    public MCQQuestion(string questionHead, string questionText, int mark, Answer[] answers, int correctAnswerIndex)
        : base(questionHead, questionText, mark, answers, correctAnswerIndex)
    {
    }

    public override void ShowQuestion()
    {
        Console.WriteLine(QuestionHead);
        Console.WriteLine(QuestionText);
        for (int i = 0; i < Answers.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {Answers[i].AnswerText}");
        }
    }

    public override object Clone()
    {
        Answer[] clonedAnswers = new Answer[Answers.Length];
        for (int i = 0; i < Answers.Length; i++)
        {
            clonedAnswers[i] = (Answer)Answers[i].Clone();
        }
        return new MCQQuestion(QuestionHead, QuestionText, Mark, clonedAnswers, CorrectAnswerIndex);
    }
}

public abstract class Exam : ICloneable
{
    public DateTime ExamTime { get; set; }
    public int NumOfQuestions { get; set; }
    public Subject Subject { get; set; }
    public List<Question> Questions { get; set; }
    public Dictionary<Question, int> UserAnswers { get; set; }

    protected Exam(DateTime examTime, int numOfQuestions, Subject subject)
    {
        ExamTime = examTime;
        NumOfQuestions = numOfQuestions;
        Subject = subject;
        Questions = new List<Question>();
        UserAnswers = new Dictionary<Question, int>();
    }

    public abstract void ShowExam();

    public abstract void DisplayResults();

    public object Clone()
    {
        var clonedExam = (Exam)MemberwiseClone();
        clonedExam.Questions = new List<Question>();
        foreach (var question in Questions)
        {
            clonedExam.Questions.Add((Question)question.Clone());
        }
        return clonedExam;
    }
}

public class FinalExam : Exam
{
    public FinalExam(DateTime examTime, int numOfQuestions, Subject subject)
        : base(examTime, numOfQuestions, subject)
    {
    }

    public override void ShowExam()
    {
        Console.WriteLine("Final Exam");
        foreach (var question in Questions)
        {
            question.ShowQuestion();
        }
    }

    public override void DisplayResults()
    {
        Console.WriteLine("Final Exam Results:");
        int totalMarks = 0;
        int obtainedMarks = 0;

        foreach (var question in Questions)
        {
            question.ShowQuestion();
            Console.WriteLine($"Your Answer: {UserAnswers[question] + 1}");
            Console.WriteLine($"Correct Answer: {question.Answers[question.CorrectAnswerIndex].AnswerText}");
            Console.WriteLine();

            if (UserAnswers[question] == question.CorrectAnswerIndex)
            {
                obtainedMarks += question.Mark;
            }
            totalMarks += question.Mark;
        }

        Console.WriteLine($"Total Marks: {totalMarks}");
        Console.WriteLine($"Obtained Marks: {obtainedMarks}");
        Console.WriteLine($"Grade: {(obtainedMarks * 100) / totalMarks}%");
    }
}

public class PracticalExam : Exam
{
    public PracticalExam(DateTime examTime, int numOfQuestions, Subject subject)
        : base(examTime, numOfQuestions, subject)
    {
    }

    public override void ShowExam()
    {
        Console.WriteLine("Practical Exam");
        foreach (var question in Questions)
        {
            question.ShowQuestion();
        }
    }

    public override void DisplayResults()
    {
        Console.WriteLine("Practical Exam Results:");
        foreach (var question in Questions)
        {
            question.ShowQuestion();
            Console.WriteLine($"Correct Answer: {question.Answers[question.CorrectAnswerIndex].AnswerText}");
            Console.WriteLine();
        }
    }
}

public class Subject
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
    public Exam Exam { get; set; }

    public Subject(int subjectId, string subjectName)
    {
        SubjectId = subjectId;
        SubjectName = subjectName;
    }

    public void CreateExam(ExamType examType, DateTime examTime, int numOfQuestions)
    {
        switch (examType)
        {
            case ExamType.Final:
                Exam = new FinalExam(examTime, numOfQuestions, this);
                break;
            case ExamType.Practical:
                Exam = new PracticalExam(examTime, numOfQuestions, this);
                break;
        }
    }

    public override string ToString()
    {
        return $"Subject: {SubjectName} (ID: {SubjectId})";
    }
}

public enum ExamType
{
    Final,
    Practical
}

public class Program
{
    public static void Main()
    {
        var subject = new Subject(1, "Mathematics");

        Console.WriteLine("Select Exam Type (1: Final, 2: Practical): ");
        int examTypeInput = int.Parse(Console.ReadLine());

        ExamType examType = examTypeInput == 1 ? ExamType.Final : ExamType.Practical;
        subject.CreateExam(examType, DateTime.Now.AddHours(2), 2);

        while (true)
        {
            Console.WriteLine("Enter question type (1: True/False, 2: Multiple Choice): ");
            int questionType = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter question head: ");
            string questionHead = Console.ReadLine();

            Console.WriteLine("Enter question text: ");
            string questionText = Console.ReadLine();

            Console.WriteLine("Enter mark: ");
            int mark = int.Parse(Console.ReadLine());

            if (questionType == 1)
            {
                Console.WriteLine("Enter correct answer (true/false): ");
                bool correctAnswer = bool.Parse(Console.ReadLine());
                var tfQuestion = new TrueFalseQuestion(questionHead, questionText, mark, correctAnswer);
                subject.Exam.Questions.Add(tfQuestion);
            }
            else if (questionType == 2)
            {
                Console.WriteLine("Enter number of options: ");
                int numOptions = int.Parse(Console.ReadLine());
                var answers = new Answer[numOptions];

                for (int i = 0; i < numOptions; i++)
                {
                    Console.WriteLine($"Enter option {i + 1} text: ");
                    string answerText = Console.ReadLine();
                    answers[i] = new Answer(i + 1, answerText);
                }

                Console.WriteLine("Enter correct answer index (starting from 1): ");
                int correctAnswerIndex = int.Parse(Console.ReadLine()) - 1;

                var mcqQuestion = new MCQQuestion(questionHead, questionText, mark, answers, correctAnswerIndex);
                subject.Exam.Questions.Add(mcqQuestion);
            }

            Console.WriteLine("Do you want to add another question? (yes/no)");
            if (Console.ReadLine().ToLower() != "yes")
            {
                break;
            }
        }

        Console.WriteLine(subject);

        Console.WriteLine("Do you want to start the exam? (yes/no)");
        string userResponse = Console.ReadLine();

        if (userResponse.ToLower() == "yes")
        {
            DateTime startTime = DateTime.Now;
            foreach (var question in subject.Exam.Questions)
            {
                question.ShowQuestion();
                Console.WriteLine("Your answer: ");
                int userAnswer = int.Parse(Console.ReadLine());
                subject.Exam.UserAnswers[question] = userAnswer - 1;
            }

            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - startTime;

            subject.Exam.DisplayResults();
            Console.WriteLine($"Time taken: {duration.TotalMinutes} minutes");
        }

    }
}