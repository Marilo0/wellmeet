using System.ComponentModel.DataAnnotations;
using Wellmeet.Data;

namespace Wellmeet.DTO
{
    public class ActivityParticipantDTO
    {
        [Required]
        public int ActivityId { get; set; }
    }
}
