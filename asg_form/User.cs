
using asg_form.Controllers;
using Microsoft.AspNetCore.Identity;

namespace asg_form
{
    public class User : IdentityUser<long>
    {
        public DateTime CreationTime { get; set; }

        public form? haveform { get; set; }

        public bool? isbooking { get; set; }


    }

    public class Role : IdentityRole<long>
    {
        public string msg { get; set; }
    }






}
