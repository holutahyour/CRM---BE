
namespace CRM.Base.Common.Domain.Common
{
    public class Result<T>
    {
        public T Content { get; set; }
        public bool HasError => ErrorMessage != "";
        public string ErrorMessage { get; set; } = "";
        public string Message { get; set; } = "";
        public string RequestId { get; set; } = "";
        public int DataCount { get; set; }
        public bool IsSuccess { get; set; } = true;
        public MetaData MetaData { get; set; } = new MetaData();
        public DateTime RequestTime { get; set; } = DateTime.UtcNow;
        public DateTime ResponseTime { get; set; } = DateTime.UtcNow;

        public Result()
        {

        }

        public Result(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public void SetError(string errorMessage, string messsage)
        {
            ErrorMessage = errorMessage;
            Message = messsage;
            IsSuccess = false;
        }

        public Result<T> SetSuccess(T content, string messsage)
        {
            Content = content;
            IsSuccess = true;
            Message = messsage;

            //if (content is IList list)
            //{
            //    DataCount = list.Count;
            //}
            //else DataCount = 1;

            return this;
        }

        public Result<T> SetMeta(int total, int from, int to, int perPage, int lastPage, string path, string firstPageUrl, string prevPageUrl, string nextPageUrl, string lastPageUrl)
        {
            MetaData.Total = total;
            MetaData.From = from;
            MetaData.To = to;
            MetaData.PerPage = perPage;
            MetaData.LastPage = lastPage;
            MetaData.Path = path;
            MetaData.FirstPageUrl = firstPageUrl;
            MetaData.PrevPageUrl = prevPageUrl;
            MetaData.NextPageUrl = nextPageUrl;
            MetaData.LastPageUrl = lastPageUrl;
            return this;
        }
    }
}



public class MetaData
{
    public int Total { get; set; }
    public int From { get; set; }
    public int To { get; set; }
    public int PerPage { get; set; }
    public int LastPage { get; set; }
    public string Path { get; set; }
    public string FirstPageUrl { get; set; }
    public string PrevPageUrl { get; set; }
    public string NextPageUrl { get; set; }
    public string LastPageUrl { get; set; }
}
