using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnyPost.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string CommentContent { get; set; }
        public DateTime CommentDate { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
