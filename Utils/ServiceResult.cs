namespace TravelSBE.Utils
{
    public class ServiceResult<T>
    {
        public bool IsSuccessful { get; set; }

        public T Result { get; set; }

        public string ValidationMessage { get; set; }
        public ServiceResult(T data)
        {
            IsSuccessful = true;
            Result = data;
            ValidationMessage = null;
        }
        public ServiceResult(string validationMessage)
        {
            IsSuccessful = false;
            Result = default(T);
            ValidationMessage = validationMessage;
        }
        public ServiceResult() { }
    }

}
