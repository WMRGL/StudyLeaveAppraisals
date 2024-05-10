using StudyLeaveAppraisals.Data;

namespace StudyLeaveAppraisals.Meta
{
    public class NotificationData
    { 
        private readonly DataContext _context;        
        
        public NotificationData(DataContext context) 
        {
            _context = context;            
        }

        public string GetMessage()
        {
            string message = "";

            var messageNotifications = _context.Notifications.Where(n => n.MessageCode == "SLAOutage" && n.IsActive == true);

            if (messageNotifications.Count() > 0)
            {
                message = messageNotifications.First().Message;
            }

            return message;
        }


    }
}
