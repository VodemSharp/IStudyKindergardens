using IStudyKindergardens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Repositories
{
    public interface IRatingManager
    {
        List<Question> GetAllQuestions();
        Question GetQuestionById(int id);
        void AddQuestion(Question question);
        void EditQuestion(Question question);
        void RemoveQuestion(int id);

        void Rate(string kindergardenId, List<int> questionIds, List<int> rating, string comment, string userId);
        string CalculateRating(string kindergardenId);
        List<QuestionRating> GetListOfQuestionRatingById(string kindergardenId, string userId);
        string GetCommentById(string kindergardenId, string userId);
    }

    public class RatingManager : IDisposable, IRatingManager
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<Question> GetAllQuestions()
        {
            return db.Questions.ToList();
        }

        public Question GetQuestionById(int id)
        {
            try
            {
                return db.Questions.Where(q => q.Id == id).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void AddQuestion(Question question)
        {
            db.Questions.Add(question);
            db.SaveChanges();
        }

        public void EditQuestion(Question question)
        {
            Question editedQuestion = GetQuestionById(question.Id);
            editedQuestion.Value = question.Value;
            db.SaveChanges();
        }

        public void RemoveQuestion(int id)
        {
            db.Questions.Remove(db.Questions.Where(q => q.Id == id).First());
            db.QuestionRatings.RemoveRange(db.QuestionRatings.Where(qr => qr.QuestionId == id));
            db.SaveChanges();
        }

        public void Rate(string kindergardenId, List<int> questionIds, List<int> rating, string comment, string userId)
        {
            if (!db.Ratings.Any(r => r.SiteUserId == userId))
            {
                List<QuestionRating> questionRatings = new List<QuestionRating> { };
                if (questionIds.Count == rating.Count)
                {
                    for (int i = 0; i < rating.Count; i++)
                    {
                        questionRatings.Add(new QuestionRating { QuestionId = questionIds[i], Rating = rating[i] });
                    }
                }
                db.QuestionRatings.AddRange(questionRatings);
                db.Ratings.Add(new Rating { KindergardenId = kindergardenId, SiteUserId = userId, Comment = comment, QuestionRatings = questionRatings });
            }
            else
            {
                Rating editedRating = db.Ratings.Where(r => r.KindergardenId == kindergardenId && r.SiteUserId == userId).First();
                List<QuestionRating> questionRating = editedRating.QuestionRatings.ToList();
                bool isExist;

                for (int i = 0; i < questionIds.Count; i++)
                {
                    isExist = false;
                    for (int j = 0; j < questionRating.Count; j++)
                    {
                        if (questionIds[i] == questionRating[j].QuestionId)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        questionRating.Add(new QuestionRating { QuestionId = questionIds[i], Rating = rating[i] });
                    }
                }
                for (int i = 0; i < questionRating.Count; i++)
                {
                    for (int j = 0; j < questionIds.Count; j++)
                    {
                        if (questionRating[i].QuestionId == questionIds[j])
                        {
                            questionRating[i].Rating = rating[j];
                            questionIds.RemoveAt(j);
                            rating.RemoveAt(j);
                            break;
                        }
                    }
                }
                for (int i = 0; i < questionIds.Count; i++)
                {
                    questionRating.Add(new QuestionRating { QuestionId = questionIds[i], Rating = rating[i] });
                }
                editedRating.Comment = comment;
                editedRating.QuestionRatings = questionRating;
            }
            db.SaveChanges();
        }

        public string CalculateRating(string kindergardenId)
        {
            double result = 0;
            int count = 0;
            List<Rating> ratings = db.Ratings.Where(r => r.KindergardenId == kindergardenId).ToList();
            if (ratings.Count == 0)
            {
                return "-";
            }
            for (int i = 0; i < ratings.Count; i++)
            {
                for (int j = 0; j < ratings[i].QuestionRatings.Count; j++)
                {
                    result += ratings[i].QuestionRatings.ToList()[j].Rating;
                    count++;
                }
            }
            return Math.Round(result / count, 1).ToString();
        }

        public List<QuestionRating> GetListOfQuestionRatingById(string kindergardenId, string userId)
        {
            return db.Ratings.Where(r => r.KindergardenId == kindergardenId && r.SiteUserId == userId).First().QuestionRatings.ToList();
        }

        public string GetCommentById(string kindergardenId, string userId)
        {
            return db.Ratings.Where(r => r.KindergardenId == kindergardenId && r.SiteUserId == userId).First().Comment;
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}