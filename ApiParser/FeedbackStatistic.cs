using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiParser
{
    public class FeedbackStatistic
    {
        public int storyId { get; set; }
        public int answerId { get; set; }

        public FeedbackStatistic(int storyId, int answerId)
        {
            this.storyId = storyId;
            this.answerId = answerId;
        }
    }
}
