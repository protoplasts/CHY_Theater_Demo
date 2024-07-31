
using static FUEN104_2_FinalProject.Models.ViewModels.ConfirmSelectionViewModel;
using static FUEN104_2_FinalProject.Models.ViewModels.SeatViewModel;

namespace FUEN104_2_FinalProject.Models.ViewModels
{
    public class ConfirmationViewModel
    {
        public List<SeatInfo> SelectedSeats { get; set; }
        public string MovieName { get; set; }
        public string MovieEnglishName { get; set; }
        public decimal MovieTotalPrice { get; set; }
        public string MovieImg { get; set; }
        public string Level { get; set; }
        public DateTime ShowDateTime { get; set; }
        public string AuditoriumName { get; set; }
        public int AuditoriumId { get; set; }
        public int ShowId { get; set; }
        public int SelectedTicketTypeId { get; set; }
        public List<SelectedSnack> SelectedSnacks { get; set; }
       

        
    }
}
