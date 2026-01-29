using System.ComponentModel.DataAnnotations;

namespace Wellmeet.DTO
{
    public class ActivityParticipantDTO
    {
        [Required]
        public int ActivityId { get; set; }
    }
}
