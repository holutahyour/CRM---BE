namespace CRM.Domain.Datas
{
    public class CommunicationData
    {
        public class EmailData
        {
            public string To { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public string Attachment { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string FullName { get; set; }
        }

        public class SmsData
        {
            public string Title { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public string Message { get; set; }
        }

        public class TestData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string PatientCode { get; set; }
            public string Reference { get; set; }

        }
    }
}
