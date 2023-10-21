using Microsoft.AspNetCore.Mvc;

namespace asg_form.Controllers
{
    public class match_records:ControllerBase
    {
        public class T_match
        {
            public int Id { get; set; }
            public schedule.team_game game { get; set; }
            public string ban_qsz { get; set; }
            public string ban_jgz { get; set; }
        }
    }
}
