namespace stockManager.Data
{
    public class DatabaseController
    {
        private readonly StockTrackerContext _context;

        public DatabaseController()
        {
            _context = new StockTrackerContext();
        }

    }
}
