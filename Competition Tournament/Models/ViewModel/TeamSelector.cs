using Microsoft.EntityFrameworkCore;

namespace Competition_Tournament.Models.ViewModel
{
    [Keyless]
    public class TeamSelector
    {
        public Competition Competition { get; set; }
        public int Team { get; set; }
    }
}
