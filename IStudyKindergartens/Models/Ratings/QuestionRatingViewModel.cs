﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Models.Ratings
{
    public class QuestionRatingViewModel
    {
        public string Id { get; set; }
        public List<Question> Questions { get; set; }
        public List<int> Ratings { get; set; }
        public string Comment { get; set; }
    }
}