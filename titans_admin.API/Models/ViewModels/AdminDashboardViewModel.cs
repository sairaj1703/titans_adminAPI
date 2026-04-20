namespace titans_admin.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalTradeLicenses { get; set; }
        public int ActivePrograms { get; set; }
        public int TotalTransactions { get; set; }
        public int ViolationCounter { get; set; }
        public List<UserListItemViewModel> RecentUsers { get; set; } = new List<UserListItemViewModel>();
    }
}
