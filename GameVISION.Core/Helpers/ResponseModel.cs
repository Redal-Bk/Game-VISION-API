namespace GameVISION.Core.Helpers
{
    public class ResponseModel<T>
    {
        public bool IsSuccess { get; set; }
        public T? Result { get; set; }
        public string? Message { get; set; }
        public int? TotalCount {  get; set; }

    }
    public class ResponseModel
    {
        public bool IsSuccess { get; set; }       
        public string? Message { get; set; }
       
    }
}
